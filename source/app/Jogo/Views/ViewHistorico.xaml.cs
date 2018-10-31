using System.Linq;
using KinectMath.Core;
using System.Windows.Controls;
using KinectMath.Core.Operacoes;

namespace Jogo.Views
{
    /// <summary>
    /// Interaction logic for ViewHistorico.xaml
    /// </summary>
    public partial class ViewHistorico : UserControl
    {
        public ViewHistorico()
        {
            InitializeComponent();
        }

        public void Atualizar(DescricaoPassoHistoricoOperacoes[] passoAPasso)
        {
            ListaOperacoesEsquerda.Children.Clear();
            ListaEstadosEquacao.Children.Clear();
            ListaOperacoesDireita.Children.Clear();

            foreach (var passo in passoAPasso.TakeLast(10))
            {
                var stringOperacaoEsquerda = string.Empty;
                var stringOperacaoDireita = string.Empty;

                if (passo.OperacaoEfetuada != null)
                {
                    if (passo.OperacaoEfetuada.Lado == Lado.Esquerdo)
                        stringOperacaoEsquerda = DescricaoPara(passo.OperacaoEfetuada.Operacao);

                    if (passo.OperacaoEfetuada.Lado == Lado.Direito)
                        stringOperacaoDireita = DescricaoPara(passo.OperacaoEfetuada.Operacao);
                }

                ListaOperacoesEsquerda.Children.Add(TextoEstilizado(stringOperacaoEsquerda.PadRight(5)));
                ListaEstadosEquacao.Children.Add(TextoEstilizado(passo.Equacao));
                ListaOperacoesDireita.Children.Add(TextoEstilizado(stringOperacaoDireita.PadLeft(5)));
            }
        }

        private static string DescricaoPara(Operacao operacao)
        {
            return string.Format("({0})", operacao);
        }

        private static Label TextoEstilizado(string texto)
        {
            return new Label
            {
                Content = texto,
                FontSize = 28
            };
        }
    }
}
