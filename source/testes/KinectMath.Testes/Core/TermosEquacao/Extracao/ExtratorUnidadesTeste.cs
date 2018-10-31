using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using KinectMath.Core.TermosEquacao;
using KinectMath.Core.TermosEquacao.Extracao;
using NUnit.Framework;

namespace KinectMath.Testes.Core.TermosEquacao.Extracao
{
    public class ExtratorUnidadesTeste
    {
        [TestCaseSource("Unidades")]
        public void extraindo_as_unidades_da_equacao(string ladoEquacao, int[]unidadesEsperadas)
        {
            IExtratorTermosEquacao extrator = new ExtratorUnidades();
            var termosEncontrados = extrator.Extrair(ladoEquacao).ToArray();

            var termosEsperados = unidadesEsperadas.Select(valor => new Unidade(valor)).ToArray();
            termosEncontrados.Should().HaveSameCount(termosEsperados);
            termosEncontrados.Should().ContainInOrder(termosEsperados);
        }

        public static IEnumerable Unidades
        {
            get
            {                
                yield return new TestCaseData("x+5", new[] { 5 });
                yield return new TestCaseData("x-5", new[] { -5 });
                yield return new TestCaseData("4-2x", new[] { 4 });
                yield return new TestCaseData("+5+x-5", new[] { 5, -5 });
                yield return new TestCaseData("5+x-5", new[] { 5, -5 });
                yield return new TestCaseData("-5+x-5", new[] { -5, -5 });
                yield return new TestCaseData("-200+2x+5-3", new[] { -200, 5, -3});
                yield return new TestCaseData("-200+100+2x+5-3", new[] { -200, 100, 5, -3 });
            }
        }
    }
}
