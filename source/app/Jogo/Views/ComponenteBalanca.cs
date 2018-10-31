using KinectMath.Core;
using System.Windows;
using System.Windows.Media;

namespace Jogo.Views
{
    public class ComponenteBalanca : FrameworkElement
    {
        private const double Margem = 10.0;
        private Point ponto1;
        private Point ponto2;

        //public bool PendeDireita
        //{
        //    get
        //    {
        //        //var viewModel = (JogoViewModel)DataContext;
        //        //return viewModel.EquacaoEmConstrucao.AvaliarEquilibrio() == Equilibrio.PendendoParaDireita;
        //        return false;
        //    }
        //}

        //public bool PendeEsquerda
        //{
        //    get
        //    {
        //        //var viewModel = (JogoViewModel)DataContext;
        //        //return viewModel.EquacaoEmConstrucao.AvaliarEquilibrio() == Equilibrio.PendendoParaEsquerda;
        //        return false;
        //    }
        //}

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            DesenharBaseBalanca(drawingContext);
            DesenharLinha(drawingContext);
        }

        private void DesenharLinha(DrawingContext drawingContext)
        {
            ponto1 = new Point(Margem, 10);
            ponto2 = new Point((ActualWidth - Margem), 10);
            drawingContext.DrawLine(new Pen(Brushes.Black, 10.0), ponto1, ponto2);
        }

        private void DesenharBaseBalanca(DrawingContext dc)
        {
            var centroHorizontal = ActualWidth / 2;
            const int larguraBase = 300;
            var inicio = new Point((centroHorizontal - larguraBase/2), Margem);
            var segments = new PathSegmentCollection
            {
                new QuadraticBezierSegment
                {
                    Point1 = new Point(centroHorizontal, larguraBase/2 - Margem),
                    Point2 = new Point((centroHorizontal + larguraBase/2), Margem)
                }
            };

            var geo = new PathGeometry(new[] { new PathFigure(inicio, segments, false) });
            dc.DrawGeometry(Brushes.LightGray, new Pen(Brushes.Black, 2), geo);
        }

        public Equilibrio CursorSobreLado(Point cursor)
        {
            var meioComponente = ActualWidth / 2;
            return Equilibrio.PendendoParaEsquerda;

        }
    }
}
