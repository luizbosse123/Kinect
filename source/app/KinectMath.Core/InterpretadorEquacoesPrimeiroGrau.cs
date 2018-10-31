using KinectMath.Core.TermosEquacao;
using KinectMath.Core.TermosEquacao.Extracao;
using System.Collections.Generic;
using System.Linq;

namespace KinectMath.Core
{
    public class InterpretadorEquacoesPrimeiroGrau
    {
        private readonly IExtratorTermosEquacao extratorUnidades;
        private readonly IExtratorTermosEquacao extratorIncognitas;
        private readonly ExtratorDivisores extratorDivisores;

        public InterpretadorEquacoesPrimeiroGrau(ExtratorUnidades extratorUnidades, ExtratorIncognitas extratorIncognitas, ExtratorDivisores extratorDivisores)
        {
            this.extratorUnidades = extratorUnidades;
            this.extratorIncognitas = extratorIncognitas;
            this.extratorDivisores = extratorDivisores;
        }

        public Equacao Interpretar(string equacaoString)
        {
            var ladosEquacaoString = equacaoString.Replace(" ", "").Split('=');
            var ladoEsquerdo = ladosEquacaoString[0];
            var ladoDireito = ladosEquacaoString[1];

            var termosLadoEsquerdo = ExtrairTermosSimplificando(ladoEsquerdo);
            var termosLadoDireito = ExtrairTermosSimplificando(ladoDireito);
                        
            return new Equacao(termosLadoEsquerdo, termosLadoDireito);
        }

        private IEnumerable<TermoEquacao> ExtrairTermosSimplificando(string ladoEquacao)
        {
            var unidades = extratorUnidades.Extrair(SanitizarParaExtracaoUnidades(ladoEquacao));
            var incognitas = extratorIncognitas.Extrair(ladoEquacao);
            var divisores = extratorDivisores.Extrair(ladoEquacao);

            unidades = SimplificarUnidades(unidades);
            incognitas = SimplificarIncognitas(incognitas);

            return unidades.Union(incognitas).Union(divisores);
        }

        private static string SanitizarParaExtracaoUnidades(string ladoEquacao)
        {
            return ladoEquacao.Contains('/') 
                ? ladoEquacao.Split(new[] {'/'})[0].Replace("(", "").Replace(")", "") 
                : ladoEquacao;
        }

        private static IEnumerable<TermoEquacao> SimplificarUnidades(IEnumerable<TermoEquacao> unidades)
        {
            return new TermoEquacao[] { new Unidade(unidades.Sum(unidade => unidade.Valor)) };
        }

        private static IEnumerable<TermoEquacao> SimplificarIncognitas(IEnumerable<TermoEquacao> incognitas)
        {
            return new TermoEquacao[] { new Incognita(incognitas.Sum(incognita => incognita.Valor)) };
        }
    }
}
