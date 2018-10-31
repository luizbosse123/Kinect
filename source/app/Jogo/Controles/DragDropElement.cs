using Jogo.Views;
using Microsoft.Kinect.Toolkit.Controls;
using Microsoft.Kinect.Toolkit.Controls.Enum;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Jogo.ViewModel;

namespace Jogo.Controles
{
    public class DragDropElement : Decorator
    {
        public bool Manipulavel { get { return true; } }
        public bool Pressionavel { get { return false; } }

        public int XInicial { get; set; }
        public int YInicial { get; set; }

        private readonly KinectRegionBinder kinectRegionBinder;
        private GripState lastGripState;
        private HandPointer capturedHandPointer;
        private HandPointer grippedHandpointer;
        private Point gripPoint;

        private static readonly DependencyProperty IsPrimaryHandPointerOverProperty = DependencyProperty.Register(
                    "IsPrimaryHandPointerOver", typeof(bool), typeof(DragDropElement), new PropertyMetadata(false, (o, args) => ((DragDropElement)o).OnIsPrimaryHandPointerOverChanged((bool)args.NewValue)));

        public DragDropElement()
        {
            lastGripState = GripState.Released;

            kinectRegionBinder = new KinectRegionBinder(this);
            kinectRegionBinder.OnKinectRegionChanged += OnKinectRegionChanged;

            KinectRegion.AddHandPointerEnterHandler(this, OnHandPointerEnter);
            KinectRegion.AddHandPointerMoveHandler(this, OnHandPointerMove);
            KinectRegion.AddHandPointerGotCaptureHandler(this, OnHandPointerCaptured);
            KinectRegion.AddHandPointerLostCaptureHandler(this, OnHandPointerLostCapture);
            KinectRegion.AddHandPointerGripHandler(this, OnHandPointerGrip);
            KinectRegion.AddHandPointerGripReleaseHandler(this, OnHandPointerGripRelease);
            KinectRegion.AddQueryInteractionStatusHandler(this, OnQueryInteractionStatus);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isOver"></param>
        public void OnIsPrimaryHandPointerOverChanged(bool isOver)
        {
            VisualStateManager.GoToState(this, isOver ? "HandPointerOver" : "Normal", true);
        }

        private void OnKinectRegionChanged(object sender, KinectRegion oldKinectRegion, KinectRegion newKinectRegion)
        {
            if (oldKinectRegion != null)
                oldKinectRegion.HandPointersUpdated -= this.KinectRegionHandPointersUpdated;

            if (newKinectRegion != null)
            {
                newKinectRegion.HandPointersUpdated += this.KinectRegionHandPointersUpdated;

                // Bind our IsPrimaryHandpointerOver dependency property
                var binding = new Binding { Source = this, Path = new PropertyPath(KinectRegion.IsPrimaryHandPointerOverProperty) };
                BindingOperations.SetBinding(this, IsPrimaryHandPointerOverProperty, binding);
            }
        }

        private void KinectRegionHandPointersUpdated(object sender, EventArgs e)
        {
            var kinectRegion = (KinectRegion)sender;
            var primaryHandPointer = kinectRegion.HandPointers.FirstOrDefault(hp => hp.IsPrimaryHandOfPrimaryUser);

            if (primaryHandPointer == null)
                return;

            if (primaryHandPointer.HandEventType == HandEventType.Grip)
            {
                if (this.capturedHandPointer != primaryHandPointer)
                    // Grip ocorre aqui
                    this.grippedHandpointer = primaryHandPointer;

                return;
            }

            if (this.grippedHandpointer == primaryHandPointer && primaryHandPointer.HandEventType == HandEventType.GripRelease)
                this.grippedHandpointer = null;
        }

        private void OnHandPointerCaptured(object sender, HandPointerEventArgs handPointerEventArgs)
        {
            if (this.capturedHandPointer != null)
                // Release capture on any previous captured handpointer
                this.capturedHandPointer.Capture(null);

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

        private void OnHandPointerMove(object sender, HandPointerEventArgs kinectHandPointerEventArgs)
        {
            var capturado = kinectHandPointerEventArgs.HandPointer.Captured;
            if (capturado == null)
                return;

            if (this.Equals(kinectHandPointerEventArgs.HandPointer.Captured))
            {
                kinectHandPointerEventArgs.Handled = true;

                var currentPosition = kinectHandPointerEventArgs.HandPointer.GetPosition(this);

                // move objeto
                if (this.Parent is Canvas)
                {
                    var y = Canvas.GetTop(this);
                    var x = Canvas.GetLeft(this);

                    if (double.IsNaN(y)) y = 0;
                    if (double.IsNaN(x)) x = 0;

                    y += (currentPosition.Y - gripPoint.Y);
                    x += (currentPosition.X - gripPoint.X);

                    Canvas.SetTop(this, y);
                    Canvas.SetLeft(this, x);

                    ((JogoViewModel)DataContext).ArrastandoElemento(true);
                }

                if (this.lastGripState == GripState.Released)
                    return;

                if (!kinectHandPointerEventArgs.HandPointer.IsInteractive)
                    ReleaseGrip();
            }
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
                return;

            if (this.capturedHandPointer != handPointer)
            {
                if (handPointer.Captured == null)
                    // Only capture hand pointer if it isn't already captured
                    handPointer.Capture(this);
                else
                    // Some other control has capture, ignore grip
                    return;
            }

            Grip();
            this.gripPoint = handPointer.GetPosition(this);
        }

        private void Grip()
        {
            this.lastGripState = GripState.Gripped;
        }

        private void OnHandPointerGripRelease(object sender, HandPointerEventArgs kinectHandPointerEventArgs)
        {
            if (this.Equals(kinectHandPointerEventArgs.HandPointer.Captured))
            {
                if (Tag != null)
                {
                    ((IViewBalanca)Tag).AdicionarElementoQuadro(this);
                }

                ((JogoViewModel)DataContext).ArrastandoElemento(false);

                Canvas.SetTop(this, YInicial);
                Canvas.SetLeft(this, XInicial);

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
    }
}
