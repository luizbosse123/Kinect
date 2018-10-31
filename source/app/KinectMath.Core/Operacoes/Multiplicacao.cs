using KinectMath.Core.TermosEquacao;

namespace KinectMath.Core.Operacoes
{
    public class Multiplicacao : Operacao
    {
        public Multiplicacao(TermoEquacao termo) 
            : base(TipoOperacao.Multiplicacao, termo)
        {
        }
    }
}