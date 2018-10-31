using Jogo.ViewModel;
using Microsoft.Kinect.Toolkit.Controls;
using Microsoft.Kinect.Toolkit.Controls.Enum;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Jogo.Controles
{
    /// <summary>
    /// Interaction logic for ValorAjustavel.xaml
    /// </summary>
    [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")] //TODO ver se precisa
    [TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Pressed", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Focused", GroupName = "FocusStates")]
    [TemplateVisualState(Name = "Unfocused", GroupName = "FocusStates")]
    public partial class ValorAjustavel : UserControl
    {
        private static readonly bool IsInDesignMode = DesignerProperties.GetIsInDesignMode(new DependencyObject());

        private GripState lastGripState;
        private HandPointer capturedHandPointer;
        private HandPointer grippedHandpointer;

        private const int HandPointerSamplesToGather = 50;
        private const int LimiarParaIncrementarValor = 4;
        private readonly HandPointerSampleTracker sampleTracker = new HandPointerSampleTracker(HandPointerSamplesToGather);
        private KinectRegionBinder kinectRegionBinder;

        private Point gripPoint;

        private Point? lastMousePoint = null;

        private static readonly DependencyProperty IsPrimaryHandPointerOverProperty = DependencyProperty.Register(
            "IsPrimaryHandPointerOver", typeof(bool), typeof(ValorAjustavel), new PropertyMetadata(false, (o, args) => ((ValorAjustavel)o).OnIsPrimaryHandPointerOverChanged((bool)args.NewValue)));

        public ValorAjustavel()
        {
            InitializeComponent();

            this.lastGripState = GripState.Released;

            // Create KinectRegion binding
            this.kinectRegionBinder = new KinectRegionBinder(this);
            this.kinectRegionBinder.OnKinectRegionChanged += this.OnKinectRegionChanged;

            if (!IsInDesignMode)
            {
                this.InitializeKinectButtonBase();
            }
        }

        public void OnIsPrimaryHandPointerOverChanged(bool isOver)
        {
            VisualStateManager.GoToState(this, isOver ? "HandPointerOver" : "Normal", true);
        }

        private void OnKinectRegionChanged(object sender, KinectRegion oldKinectRegion, KinectRegion newKinectRegion)
        {
            if (oldKinectRegion != null)
            {
                oldKinectRegion.HandPointersUpdated -= this.KinectScrollViewerHandPointersUpdated;
            }

            if (newKinectRegion != null)
            {
                newKinectRegion.HandPointersUpdated += this.KinectScrollViewerHandPointersUpdated;

                // Bind our IsPrimaryHandpointerOver dependency property
                var binding = new Binding { Source = this, Path = new PropertyPath(KinectRegion.IsPrimaryHandPointerOverProperty) };
                BindingOperations.SetBinding(this, IsPrimaryHandPointerOverProperty, binding);
            }
        }

        private void KinectScrollViewerHandPointersUpdated(object sender, EventArgs e)
        {
            var kinectRegion = (KinectRegion)sender;
            var primaryHandPointer = kinectRegion.HandPointers.FirstOrDefault(hp => hp.IsPrimaryHandOfPrimaryUser);
            if (primaryHandPointer == null)
            {
                return;
            }

            if (primaryHandPointer.HandEventType == HandEventType.Grip)
            {
                if (this.capturedHandPointer == primaryHandPointer)
                {
                    return;
                }

                //Grip ocorre aqui
                this.grippedHandpointer = primaryHandPointer;
                return;
            }

            if (this.grippedHandpointer == primaryHandPointer && primaryHandPointer.HandEventType == HandEventType.GripRelease)
            {
                this.grippedHandpointer = null;
            }
        }

        private void InitializeKinectButtonBase()
        {
            KinectRegion.AddHandPointerEnterHandler(this, this.OnHandPointerEnter);
            KinectRegion.AddHandPointerMoveHandler(this, this.OnHandPointerMove);

            KinectRegion.AddHandPointerGotCaptureHandler(this, this.OnHandPointerCaptured);
            KinectRegion.AddHandPointerLostCaptureHandler(this, this.OnHandPointerLostCapture);


            KinectRegion.AddHandPointerGripHandler(this, this.OnHandPointerGrip);
            KinectRegion.AddHandPointerGripReleaseHandler(this, this.OnHandPointerGripRelease);
            KinectRegion.AddQueryInteractionStatusHandler(this, this.OnQueryInteractionStatus);
        }

        private void OnHandPointerCaptured(object sender, HandPointerEventArgs handPointerEventArgs)
        {
            if (this.capturedHandPointer != null)
            {
                // Release capture on any previous captured handpointer
                this.capturedHandPointer.Capture(null);
            }

            this.capturedHandPointer = handPointerEventArgs.HandPointer;
            handPointerEventArgs.Handled = true;
        }

        private void OnHandPointerLostCapture(object sender, HandPointerEventArgs handPointerEventArgs)
        {
            if (this.capturedHandPointer == handPointerEventArgs.HandPointer)
            {
                this.capturedHandPointer = null;
                ReleaseGrip();
            }
        }

        private void ReleaseGrip()
        {
            Console.WriteLine("Release");
            this.lastGripState = GripState.Released;
        }

        private void OnHandPointerEnter(object sender, HandPointerEventArgs kinectHandPointerEventArgs)
        {
            if (kinectHandPointerEventArgs.HandPointer.IsPrimaryHandOfPrimaryUser)
            {
                kinectHandPointerEventArgs.Handled = true;
                if (this.grippedHandpointer == kinectHandPointerEventArgs.HandPointer)
                {
                    this.HandleHandPointerGrip(kinectHandPointerEventArgs.HandPointer);
                    this.grippedHandpointer = null;
                }
            }
        }

        private int tendencia = 0;
        private void OnHandPointerMove(object sender, HandPointerEventArgs kinectHandPointerEventArgs)
        {
            if (this.Equals(kinectHandPointerEventArgs.HandPointer.Captured))
            {
                kinectHandPointerEventArgs.Handled = true;

                var currentPosition = kinectHandPointerEventArgs.HandPointer.GetPosition(this);

                this.sampleTracker.AddSample(currentPosition.X, currentPosition.Y, kinectHandPointerEventArgs.HandPointer.TimestampOfLastUpdate);
                if (this.lastGripState == GripState.Released)
                {
                    return;
                }

                if (!kinectHandPointerEventArgs.HandPointer.IsInteractive)
                {
                    ReleaseGrip();
                    return;
                }

                if (!lastMousePoint.HasValue)
                    lastMousePoint = gripPoint;

                var diffVector = lastMousePoint.Value - currentPosition;
                tendencia += (diffVector.X > 0) ? -1 : 1;
                if (tendencia > LimiarParaIncrementarValor)
                {
                    IncrementarValor();
                    tendencia = 0;
                }
                else if (tendencia < -LimiarParaIncrementarValor)
                {
                    DecrementarValor();
                    tendencia = 0;
                }

                lastMousePoint = currentPosition;
            }
        }

        private void DecrementarValor()
        {
            var viewModel = ((JogoViewModel)DataContext);
            viewModel.ValorNovoElemento -= (viewModel.ValorNovoElemento == 1) ? 2 : 1;
        }

        private void IncrementarValor()
        {
            var viewModel = ((JogoViewModel)DataContext);
            viewModel.ValorNovoElemento += (viewModel.ValorNovoElemento == -1) ? 2 : 1; ;
        }

        private void OnHandPointerGrip(object sender, HandPointerEventArgs kinectHandPointerEventArgs)
        {
            if (kinectHandPointerEventArgs.HandPointer.IsPrimaryUser && kinectHandPointerEventArgs.HandPointer.IsPrimaryHandOfUser && kinectHandPointerEventArgs.HandPointer.IsInteractive)
            {
                this.HandleHandPointerGrip(kinectHandPointerEventArgs.HandPointer);
                kinectHandPointerEventArgs.Handled = true;
            }
        }

        private void HandleHandPointerGrip(HandPointer handPointer)
        {
            if (handPointer == null)
            {
                return;
            }

            if (this.capturedHandPointer != handPointer)
            {
                if (handPointer.Captured == null)
                {
                    // Only capture hand pointer if it isn't already captured
                    handPointer.Capture(this);
                }
                else
                {
                    // Some other control has capture, ignore grip
                    return;
                }
            }

            Grip();
            this.gripPoint = handPointer.GetPosition(this);
        }

        private void Grip()
        {
            Console.WriteLine("Grip");
            this.lastGripState = GripState.Gripped;
        }

        private void OnHandPointerGripRelease(object sender, HandPointerEventArgs kinectHandPointerEventArgs)
        {
            if (this.Equals(kinectHandPointerEventArgs.HandPointer.Captured))
            {
                kinectHandPointerEventArgs.Handled = true;
                ReleaseGrip();
                kinectHandPointerEventArgs.HandPointer.Capture(null);
            }
        }

        private void OnQueryInteractionStatus(object sender, QueryInteractionStatusEventArgs queryInteractionStatusEventArgs)
        {
            if (this.Equals(queryInteractionStatusEventArgs.HandPointer.Captured))
            {
                queryInteractionStatusEventArgs.IsInGripInteraction = this.lastGripState == GripState.Gripped;
                queryInteractionStatusEventArgs.Handled = true;
            }
        }

        private void Esquerda_OnClick(object sender, RoutedEventArgs e)
        {
            DecrementarValor();
        }

        private void Direita_OnClick(object sender, RoutedEventArgs e)
        {
            IncrementarValor();
        }
    }
}
