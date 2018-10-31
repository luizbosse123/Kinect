using Jogo.Model.Enum;
using KinectMath.Core.Operacoes;
using KinectMath.Core.TermosEquacao;
using System.Windows.Controls;

namespace Jogo.Views
{
    /// <summary>
    /// Interaction logic for ViewResolverEquacao.xaml
    /// </summary>
    public partial class ViewResolverEquacao : UserControl, IViewBase
    {
        public Cenario Cenario { get { return Cenario.ResolverEquacao; } }

        public TipoTermo Unidade { get { return TipoTermo.Unidade; } }
        public TipoTermo Incognita { get { return TipoTermo.Incognita; } }

        public TipoOperacao Adicao { get {return TipoOperacao.Adicao;} }
        public TipoOperacao Subtracao { get { return TipoOperacao.Subtracao; } }
        public TipoOperacao Multiplicacao { get { return TipoOperacao.Multiplicacao; } }
        public TipoOperacao Divisao { get { return TipoOperacao.Divisao; } }

        public ViewResolverEquacao()
        {
            InitializeComponent();
        }
    }
}
