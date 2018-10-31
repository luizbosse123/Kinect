using KinectMath.Core.Operacoes;
using KinectMath.Core.TermosEquacao;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace Jogo.Views
{
    public class OperacaoArrastavel : FrameworkElement
    {
        private int valor;
        private TipoTermo tipoTermo;
        private TipoOperacao tipoOperacao;
        private FormattedText TextoFormatado { get; set; }

        public TipoOperacao TipoOperacao
        {
            get { return tipoOperacao; }
            set
            {
                tipoOperacao = value; 
                AtualizarTexto();
            }
        }

        public TipoTermo TipoTermo
        {
            get { return tipoTermo; }
            set
            {
                tipoTermo = value;
                AtualizarTexto();
            }
        }

        public int Valor
        {
            get { return valor; }
            set
            {
                valor = value;
                AtualizarTexto();
            }
        }

        public OperacaoArrastavel()
        {
            Resetar();
            AtualizarTexto();
        }

        private void AtualizarTexto()
        {
            var novoTexto = string.Format("{0} {1}{2}", Prefixo(), Valor, Sufixo());
            TextoFormatado = new FormattedText(novoTexto, new CultureInfo("pt-BR"), FlowDirection.LeftToRight, new Typeface("Arial"), 48, Brushes.Black);

            InvalidateVisual();
        }

        private string Sufixo()
        {
            return (TipoTermo == TipoTermo.Incognita)
                ? "x"
                : string.Empty;
        }

        private string Prefixo()
        {
            switch (TipoOperacao)
            {
                case TipoOperacao.Adicao: return "+";
                case TipoOperacao.Subtracao: return "-";
                case TipoOperacao.Multiplicacao: return "x";
                default: return "÷";
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            const int tamanho = 80;
            drawingContext.DrawEllipse(Brushes.White, new Pen(Brushes.Black, 1), new Point(tamanho / 2, tamanho / 2), tamanho, tamanho);

            var posicaoTextoCentralizado = PosicaoTextoCentralizado(new Rect(0, 0, tamanho, tamanho));
            drawingContext.DrawText(TextoFormatado, posicaoTextoCentralizado);
        }

        private Point PosicaoTextoCentralizado(Rect tamanhoImagem)
        {
            return new Point(tamanhoImagem.Width / 2 - TextoFormatado.Width / 2,
                tamanhoImagem.Height / 2 - TextoFormatado.Height / 2);
        }

        public void Resetar()
        {
            tipoOperacao = TipoOperacao.Adicao;
            tipoTermo = TipoTermo.Unidade;
            valor = 1;
        }
    }
}