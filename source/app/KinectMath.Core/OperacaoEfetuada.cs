using KinectMath.Core.Operacoes;

namespace KinectMath.Core
{
    public class OperacaoEfetuada
    {
        public Operacao Operacao { get; private set; }
        public Lado Lado { get; private set; }

        public OperacaoEfetuada(Operacao operacao, Lado lado)
        {
            Operacao = operacao;
            Lado = lado;
        }
    }
}