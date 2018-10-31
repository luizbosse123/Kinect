using FluentAssertions;
using KinectMath.Core.TermosEquacao;
using KinectMath.Core.TermosEquacao.Extracao;
using NUnit.Framework;
using System.Collections;
using System.Linq;

namespace KinectMath.Testes.Core.TermosEquacao.Extracao
{
    public class ExtratorIncognitasTeste
    {
        [TestCaseSource("Incognitas")]
        public void extraindo_as_incognitas_da_equacao(string ladoEquacao, int[] incognitasEsperadas)
        {
            IExtratorTermosEquacao extrator = new ExtratorIncognitas();
            var termosEncontrados = extrator.Extrair(ladoEquacao).ToArray();

            var termosEsperados = incognitasEsperadas.Select(valor => new Incognita(valor)).ToArray();
            termosEncontrados.Should().HaveSameCount(termosEsperados);
            termosEncontrados.Should().ContainInOrder(termosEsperados);
        }

        public static IEnumerable Incognitas
        {
            get
            {
                yield return new TestCaseData("x+5", new[] { 1 });
                yield return new TestCaseData("-x-5", new[] { -1 });
                yield return new TestCaseData("+5+x-5", new[] { 1 });
                yield return new TestCaseData("5-x-5", new[] { -1 });
                yield return new TestCaseData("-200+2x+5-3", new[] { 2 });
                yield return new TestCaseData("-200+100+2x+5-4x-3", new[] { 2, -4 });
            }
        }
    }
}
