using KinectMath.Core.Operacoes;

namespace KinectMath.Core
{
    public interface IHistoricoOperacoes
    {
        IOperacao[] ObterOperacoes(Lado ladoEquacao);
        DescricaoPassoHistoricoOperacoes[] PassoAPasso();
    }
}