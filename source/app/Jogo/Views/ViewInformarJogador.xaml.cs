using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Jogo.Model.Enum;

namespace Jogo.Views
{
    /// <summary>
    /// Interaction logic for ViewInformarJogador.xaml
    /// </summary>
    public partial class ViewInformarJogador : UserControl, IViewBase
    {
        public Cenario Cenario { get { return Cenario.InformarJogador; } }

        public ViewInformarJogador()
        {
            InitializeComponent();
        }
    }
}
