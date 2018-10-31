using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace KinectMath.Core.TermosEquacao.Extracao
{
    public class ExtratorIncognitas : IExtratorTermosEquacao
    {
        private const string PadraoIncognitas = @"(-?\d*x)";

        public IEnumerable<TermoEquacao> Extrair(string ladoEquacao)
        {
            var matches = Regex.Matches(ladoEquacao, PadraoIncognitas);
            foreach (Match match in matches)
            {
                var valorString = match.Value.Replace("x", "");

                switch (valorString)
                {
                    case "":
                        valorString = "1";
                        break;
                    case "-":
                        valorString = "-1";
                        break;
                }

                var valor = int.Parse(valorString);
                yield return new Incognita(valor);
            }
        }
    }
}
