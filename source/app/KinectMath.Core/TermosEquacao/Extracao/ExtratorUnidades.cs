using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace KinectMath.Core.TermosEquacao.Extracao
{
    public class ExtratorUnidades : IExtratorTermosEquacao
    {
        private const string PadraoUnidades = @"(-?\+?\d*x?)";

        public IEnumerable<TermoEquacao> Extrair(string ladoEquacao)
        {
            var matches = Regex.Matches(ladoEquacao, PadraoUnidades);
            foreach (Match match in matches)
            {
                if (match.Value == string.Empty) continue;
                if (match.Value.Contains("x")) continue;

                //var valor = int.Parse(match.Value.Replace("x", ""));
                var valor = int.Parse(match.Value);
                yield return new Unidade(valor);
            }
        }
    }
}
