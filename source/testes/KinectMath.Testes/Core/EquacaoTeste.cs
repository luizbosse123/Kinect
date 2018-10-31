using FluentAssertions;
using KinectMath.Core;
using KinectMath.Core.Operacoes;
using KinectMath.Core.TermosEquacao;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace KinectMath.Testes.Core
{
    public class EquacaoTeste
    {
        private InterpretadorEquacoesPrimeiroGrau interpretador;

        [SetUp]
        public void SetUp()
        {
            interpretador = InterpretadorEquacoesPrimeiroGrauFactory.ObterInterpretador();
        }
        
        [Test]
        public void somando_unidades_no_direito_da_equacao()
        {
            var equacao = interpretador.Interpretar("x + 2 = 2");

            equacao.Somar(Lado.Direito, 2.Unidades());

            DeveTerSubstituido(2.Unidades(), por:4.Unidades(), no:equacao.LadoDireito);
        }

        private static void DeveTerSubstituido(TermoEquacao termoOriginal, TermoEquacao por, IEnumerable<TermoEquacao> no)
        {
            no.Should().NotContain(t => t == termoOriginal);
            no.Should().Contain(t => t == por);
        }

        [Test]
        public void somando_unidades_no_lado_esquerdo_da_equacao()
        {
            var equacao = interpretador.Interpretar("x + 2 = 2");

            equacao.Somar(Lado.Esquerdo, 2.X());

            DeveTerSubstituido(1.X(), por: 3.X(), no: equacao.LadoEsquerdo);
        }

        [Test]
        public void subtraindo_unidades_num_lado_da_equacao()
        {
            var equacao = interpretador.Interpretar("x + 2 = 2");

            equacao.Subtrair(Lado.Esquerdo, 2.X());
            equacao.Subtrair(Lado.Esquerdo, 4.Unidades());

            DeveTerSubstituido(1.X(), por: (-1).X(), no: equacao.LadoEsquerdo); 
            DeveTerSubstituido(2.Unidades(), por: (-2).Unidades(), no: equacao.LadoEsquerdo); 
        }

        [Test]
        public void subtraindo_incognitas_no_lado_direito_da_equacao()
        {
            var equacao = interpretador.Interpretar("x + 2 = 2");

            equacao.Subtrair(Lado.Direito, 2.X());
            equacao.Subtrair(Lado.Direito, 4.Unidades());

            DeveTerSubstituido(0.X(), por: (-2).X(), no: equacao.LadoDireito);
            DeveTerSubstituido(2.Unidades(), por: (-2).Unidades(), no: equacao.LadoDireito);
        }

        [Test]
        public void multiplicando_unidades_do_lado_esquerdo_da_equacao()
        {
            var equacao = interpretador.Interpretar("x + 2 = 2");

            equacao.Multiplicar(Lado.Esquerdo, 2.Unidades());

            DeveTerSubstituido(1.X(), por: 2.X(), no: equacao.LadoEsquerdo);
            DeveTerSubstituido(2.Unidades(), por: 4.Unidades(), no: equacao.LadoEsquerdo);
        }

        [Test]
        public void multiplicando_unidades_do_lado_direito_da_equacao()
        {
            var equacao = interpretador.Interpretar("x + 2 = 2");

            equacao.Multiplicar(Lado.Direito, 2.Unidades());

            DeveTerSubstituido(2.Unidades(), por: 4.Unidades(), no: equacao.LadoDireito);
        }

        [Test]
        public void multiplicando_os_dois_lados_da_equacao_por_uma_incognita()
        {
            var equacao = interpretador.Interpretar("x + 2 = 3");

            equacao.Multiplicar(Lado.Esquerdo, 2.X());
            DeveTerSubstituido(1.X(), por: 2.X().ElevadoA(2), no: equacao.LadoEsquerdo);
            DeveTerSubstituido(2.Unidades(), por: 4.X().ElevadoA(1), no: equacao.LadoEsquerdo);

            equacao.Multiplicar(Lado.Direito, 2.X());
            DeveTerSubstituido(3.Unidades(), por: 6.X().ElevadoA(1), no: equacao.LadoDireito);
        }

        [TestCase(TipoOperacao.Adicao)]
        [TestCase(TipoOperacao.Multiplicacao)]
        [TestCase(TipoOperacao.Subtracao)]
        [TestCase(TipoOperacao.Divisao)]
        public void registrando_operacoes_de_multiplicacao_no_historico_de_operacoes(TipoOperacao tipoOperacao)
        {
            var equacao = interpretador.Interpretar("x + 2 = 2");

            equacao.RealizarOperacao(tipoOperacao, 3.Unidades(), Lado.Esquerdo);
            equacao.RealizarOperacao(tipoOperacao, 3.X(), Lado.Esquerdo);
            DeveTerRegistradoUmaOperacaoDe(tipoOperacao, Lado.Esquerdo, equacao, 3.Unidades(), noIndice: 0);
            DeveTerRegistradoUmaOperacaoDe(tipoOperacao, Lado.Esquerdo, equacao, 3.X(), noIndice: 1);

            equacao.RealizarOperacao(tipoOperacao, 2.Unidades(), Lado.Direito);
            equacao.RealizarOperacao(tipoOperacao, 1.X(), Lado.Direito);
            DeveTerRegistradoUmaOperacaoDe(tipoOperacao, Lado.Direito, equacao, 2.Unidades(), noIndice: 0);
            DeveTerRegistradoUmaOperacaoDe(tipoOperacao, Lado.Direito, equacao, 1.X(), noIndice: 1);
        }

        private void DeveTerRegistradoUmaOperacaoDe(TipoOperacao tipoEsperado, Lado lado, Equacao equacao, TermoEquacao termo, int noIndice)
        {
            var operacoes = equacao.HistoricoOperacoes.ObterOperacoes(lado);
            var operacao = operacoes[noIndice];
            operacao.Tipo.Should().Be(tipoEsperado);
            operacao.Termo.Should().Be(termo);
        }

        [Test]
        public void verificando_que_ao_efetuar_uma_operacao_apenas_de_um_lado_a_equacao_fica_desequilibrada()
        {
            var equacao = interpretador.Interpretar("x + 2 = 4"); //x == 2

            equacao.Somar(Lado.Direito, 2.Unidades());
            equacao.Equilibrio.Should().Be(Equilibrio.PendendoParaDireita);

            equacao.Somar(Lado.Esquerdo, 2.Unidades());
            equacao.Equilibrio.Should().Be(Equilibrio.Equilibrado);

            equacao.Somar(Lado.Esquerdo, 2.Unidades());
            equacao.Equilibrio.Should().Be(Equilibrio.PendendoParaEsquerda);

            equacao.Somar(Lado.Direito, 1.Unidades());
            equacao.Somar(Lado.Direito, 1.Unidades());
            equacao.Equilibrio.Should().Be(Equilibrio.Equilibrado);
            

            equacao.Dividir(Lado.Direito, 2.Unidades());
            equacao.Equilibrio.Should().Be(Equilibrio.PendendoParaEsquerda);

            equacao.Dividir(Lado.Esquerdo, 2.Unidades());
            equacao.Equilibrio.Should().Be(Equilibrio.Equilibrado);


            equacao.Somar(Lado.Direito, 2.X());
            equacao.Equilibrio.Should().Be(Equilibrio.PendendoParaDireita);

            equacao.Somar(Lado.Esquerdo, 4.Unidades());
            equacao.Equilibrio.Should().Be(Equilibrio.Equilibrado);

            equacao.Multiplicar(Lado.Esquerdo, 2.X());
            equacao.Equilibrio.Should().Be(Equilibrio.PendendoParaEsquerda);

            equacao.Multiplicar(Lado.Direito, 2.X());
            equacao.Equilibrio.Should().Be(Equilibrio.Equilibrado);

            equacao.Subtrair(Lado.Direito, 2.X());
            equacao.Equilibrio.Should().Be(Equilibrio.PendendoParaEsquerda);

            equacao.Subtrair(Lado.Esquerdo, 2.X());
            equacao.Equilibrio.Should().Be(Equilibrio.Equilibrado);

            equacao.Subtrair(Lado.Esquerdo, 2.X());
            equacao.Equilibrio.Should().Be(Equilibrio.PendendoParaDireita);

            equacao.Somar(Lado.Esquerdo, 1.X());
            equacao.Equilibrio.Should().Be(Equilibrio.PendendoParaDireita);

            equacao.Somar(Lado.Esquerdo, 1.X());
            equacao.Equilibrio.Should().Be(Equilibrio.Equilibrado);
            
            equacao.Dividir(Lado.Esquerdo, 2.X());
            equacao.Equilibrio.Should().Be(Equilibrio.PendendoParaDireita);

            equacao.Dividir(Lado.Direito, 2.X());
            equacao.Equilibrio.Should().Be(Equilibrio.Equilibrado);

            equacao.Dividir(Lado.Esquerdo, 2.X());
            equacao.Equilibrio.Should().Be(Equilibrio.PendendoParaDireita);

            equacao.Multiplicar(Lado.Esquerdo, 2.X());
            equacao.Equilibrio.Should().Be(Equilibrio.Equilibrado);
        }

        [TestCase("x + 2 = 2", 0)]
        [TestCase("x + 2 = 4", 2)]
        [TestCase("x + 2 = 2x + 8", -6)]
        [TestCase("-x + 5 = 2", 3)]
        [TestCase("-5x + 5 = 20", -3)]
        [TestCase("x + 2 = (2x + 10)/4", 1)]
        [TestCase("(x + 2)/2 = (2x + 10)/8", 1)]
        public void calculando_o_valor_de_x(string textoEquacao, int valorEsperadoDeX)
        {
            var equacao = interpretador.Interpretar(textoEquacao);

            equacao.ValorDeX.Should().Be(valorEsperadoDeX);
        }
    }

    public static class EquacaoTesteExtensions
    {
        public static void RealizarOperacao(this Equacao equacao, TipoOperacao tipoOperacao, TermoEquacao termo, Lado lado)
        {
            switch (tipoOperacao)
            {
                case TipoOperacao.Adicao:
                    equacao.Somar(lado, termo);
                    break;
                case TipoOperacao.Subtracao:
                    equacao.Subtrair(lado, termo);
                    break;
                case TipoOperacao.Multiplicacao:
                    equacao.Multiplicar(lado, termo);
                    break;
                case TipoOperacao.Divisao:
                    equacao.Dividir(lado, termo);
                    break;
                default: throw new Exception(string.Format("Operação não suportada: {0}", tipoOperacao));
            }
        }

        public static TermoEquacao ElevadoA(this TermoEquacao termo, int potencia)
        {
            termo.Potencia = potencia;
            return termo;
        }
    }
}
