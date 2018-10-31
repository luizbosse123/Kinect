using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Jogo.Controles
{
    public class BalancaViewModel : FrameworkElement
    {
        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            drawingContext.DrawEllipse(Brushes.Yellow, new Pen(Brushes.Teal, 3), new Point(20,20), 100, 10);
        }
    }
}
