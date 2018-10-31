using System.Collections.Generic;

namespace KinectMath.Core.TermosEquacao.Extracao
{
    public interface IExtratorTermosEquacao
    {
        IEnumerable<TermoEquacao> Extrair(string ladoEquacao);
    }
}
