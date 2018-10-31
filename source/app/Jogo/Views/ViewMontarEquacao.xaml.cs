using System.Windows;
using System.Windows.Controls;
using Jogo.Model.Enum;
using KinectMath.Core.TermosEquacao;

namespace Jogo.Views
{
    /// <summary>
    /// Interaction logic for ViewMontarEquacao.xaml
    /// </summary>
    public partial class ViewMontarEquacao : UserControl, IViewBase
    {
        public Cenario Cenario { get { return Cenario.MontarEquacao; } }

        public TipoTermo Unidade { get {return TipoTermo.Unidade;} }
        public TipoTermo Incognita { get { return TipoTermo.Incognita; } }

        public ViewMontarEquacao()
        {
            InitializeComponent();
        }

        private void viewMontarEquacao_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                viewBalanca.NovaEquacao();
            }
        }
    }
}
