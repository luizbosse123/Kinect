using Jogo.Model.Enum;
using System.Windows.Controls;

namespace Jogo.Views
{
    /// <summary>
    /// Interaction logic for ViewTelaInicial.xaml
    /// </summary>
    public partial class ViewTelaInicial : UserControl, IViewBase
    {
        public Cenario Cenario { get { return Model.Enum.Cenario.Inicio; } }

        public ViewTelaInicial()
        {
            InitializeComponent();
        }
    }
}
