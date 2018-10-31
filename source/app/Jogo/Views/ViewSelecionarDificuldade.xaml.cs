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
    /// Interaction logic for ViewSelecionarDificuldade.xaml
    /// </summary>
    public partial class ViewSelecionarDificuldade : UserControl, IViewBase
    {
        public Cenario Cenario { get { return Model.Enum.Cenario.SelecionarDificuldade; } }
        public Dificuldade Facil { get { return Dificuldade.Facil; } }
        public Dificuldade Media { get { return Dificuldade.Media; } }
        public Dificuldade Dificil { get { return Dificuldade.Dificil; } }

        public ViewSelecionarDificuldade()
        {
            InitializeComponent();
        }
    }
}
