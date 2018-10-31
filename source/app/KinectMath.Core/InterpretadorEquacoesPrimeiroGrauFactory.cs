using KinectMath.Core.TermosEquacao;
using KinectMath.Core.TermosEquacao.Extracao;

namespace KinectMath.Core
{
    public static class InterpretadorEquacoesPrimeiroGrauFactory
    {
        public static InterpretadorEquacoesPrimeiroGrau ObterInterpretador()
        {
            return new InterpretadorEquacoesPrimeiroGrau(new ExtratorUnidades(), new ExtratorIncognitas(), new ExtratorDivisores());
        }
    }
}
