using FluentAssertions;
using KinectMath.Core;
using KinectMath.Core.TermosEquacao;
using KinectMath.Core.TermosEquacao.Extracao;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace KinectMath.Testes.Core
{
    public class InterpretadorEquacoesPrimeiroGrauTestes
    {
        private InterpretadorEquacoesPrimeiroGrau interpretador;

        [SetUp]
        public void SetUp()
        {
            interpretador = new InterpretadorEquacoesPrimeiroGrau(new ExtratorUnidades(), new ExtratorIncognitas(), new ExtratorDivisores());
        }

        [Test]
        public void interpretando_uma_equacao()
        {            
            Equacao equacao = interpretador.Interpretar("x + 2x - 2 + 8 = (3x + 4 - 4x - 5)/2");

            DeveHaver(quantidade: 3, tipo: TipoTermo.Incognita, lado: equacao.LadoEsquerdo);
            DeveHaver(quantidade: 6, tipo: TipoTermo.Unidade, lado: equacao.LadoEsquerdo);

            DeveHaver(quantidade: -1, tipo: TipoTermo.Incognita, lado: equacao.LadoDireito);
            DeveHaver(quantidade: -1, tipo: TipoTermo.Unidade, lado: equacao.LadoDireito);
            DeveHaver(quantidade: 2, tipo: TipoTermo.Divisor, lado: equacao.LadoDireito);
        }

        private void DeveHaver(int quantidade, TipoTermo tipo, IEnumerable<TermoEquacao> lado)
        {
            var soma = lado
                .Where(termo => termo.Tipo == tipo)
                .Sum(termo => termo.Valor);
            soma.Should().Be(quantidade);
        }

        [Test]
        public void verificando_que_a_equacao_foi_simplificada()
        {
            Equacao equacao = interpretador.Interpretar("x + 2x - 2 + 8 = 4 - 2x");

            DeveTerJuntadoAsIncognitasNoLado(equacao.LadoEsquerdo);
            DeveTerJuntadoAsUnidadesNoLado(equacao.LadoEsquerdo);

            DeveTerJuntadoAsIncognitasNoLado(equacao.LadoDireito);
            DeveTerJuntadoAsUnidadesNoLado(equacao.LadoDireito);
        }

        private void DeveTerJuntadoAsIncognitasNoLado(IReadOnlyCollection<TermoEquacao> lado)
        {
            lado.Should().ContainSingle(termo => termo.Tipo == TipoTermo.Incognita);
        }

        private void DeveTerJuntadoAsUnidadesNoLado(IReadOnlyCollection<TermoEquacao> lado)
        {
            lado.Should().ContainSingle(termo => termo.Tipo == TipoTermo.Unidade);
        }

        //[Test]
        //public void verificando_que_uma_incognita_com_multiplicador_eh_adicionada_no_lado_onde_nao_existe_incognita_para_simplificar_os_calculos()
        //{
        //    Equacao equacao = interpretador.Interpretar("x + 2 = 4");

        //    var incognita = equacao.LadoDireito.FirstOrDefault(termo => termo.Tipo == TipoTermo.Incognita);
        //    incognita.Should().NotBeNull("deveria ter criado uma incógnita);
        //    equacao.LadoDireito.Should().ContainSingle(termo => termo.Tipo == TipoTermo.Unidade);
        //}
    }
}