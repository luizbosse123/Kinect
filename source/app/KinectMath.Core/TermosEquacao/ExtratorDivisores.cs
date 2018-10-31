using KinectMath.Core.TermosEquacao.Extracao;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace KinectMath.Core.TermosEquacao
{
    public class ExtratorDivisores : IExtratorTermosEquacao
    {
        private const string PadraoDivisores = @"\/(-?\d+)";

        public IEnumerable<TermoEquacao> Extrair(string ladoEquacao)
        {
            var matches = Regex.Matches(ladoEquacao, PadraoDivisores);
            foreach (Match match in matches)
            {
                if (match.Value == string.Empty) continue;

                var valor = int.Parse(match.Value.Replace("/", ""));
                yield return new Divisor(valor);
            }
        }
    }
}