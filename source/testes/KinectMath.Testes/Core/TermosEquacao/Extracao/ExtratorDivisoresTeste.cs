using FluentAssertions;
using KinectMath.Core.TermosEquacao;
using KinectMath.Core.TermosEquacao.Extracao;
using NUnit.Framework;
using System.Collections;
using System.Linq;

namespace KinectMath.Testes.Core.TermosEquacao.Extracao
{
    public class ExtratorDivisoresTeste
    {
        [TestCaseSource("Divisores")]
        public void extraindo_as_unidades_da_equacao(string ladoEquacao, int[] unidadesEsperadas)
        {
            IExtratorTermosEquacao extrator = new ExtratorDivisores();
            var termosEncontrados = extrator.Extrair(ladoEquacao).ToArray();

            var termosEsperados = unidadesEsperadas.Select(valor => new Divisor(valor)).ToArray();
            termosEncontrados.Should().HaveSameCount(termosEsperados);
            termosEncontrados.Should().ContainInOrder(termosEsperados);
        }

        public static IEnumerable Divisores
        {
            get
            {
                yield return new TestCaseData("(x+5)/5", new[] { 5 });
                yield return new TestCaseData("(x-5)/-5", new[] { -5 });
                yield return new TestCaseData("4-2x", new int[0]);
            }
        }
    }
}