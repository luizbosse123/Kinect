
using KinectMath.Core.TermosEquacao;

namespace KinectMath.Core.Operacoes
{
    public enum TipoOperacao
    {
        Adicao = 1,
        Subtracao = 2,
        Multiplicacao = 3,
        Divisao = 4
    }

    public interface IOperacao
    {
        TipoOperacao Tipo { get; }
        TermoEquacao Termo { get; }
    }

    public class Operacao : IOperacao
    {
        public TipoOperacao Tipo { get; private set; }
        public TermoEquacao Termo { get; private set; }

        public Operacao(TipoOperacao tipo, TermoEquacao termo)
        {
            Tipo = tipo;
            Termo = termo;
        }

        public override string ToString()
        {
            var simboloOperacao = SimboloOperacao();
            var sufixo = Termo.Tipo == TipoTermo.Incognita ? "x" : string.Empty;

            if(Termo.Valor < 0)
                return string.Format("{0} ({1}{2})", simboloOperacao, Termo.Valor, sufixo);
            return string.Format("{0} {1}{2}", simboloOperacao, Termo.Valor, sufixo);
        }

        private string SimboloOperacao()
        {
            switch (Tipo)
            {
                case TipoOperacao.Adicao: return "+";
                case TipoOperacao.Subtracao: return "-";
                case TipoOperacao.Multiplicacao: return "x";
                default: return "÷";
            }
        }
    }
}
