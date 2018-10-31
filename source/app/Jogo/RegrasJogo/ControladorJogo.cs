using Jogo.Model.Enum;
using KinectMath.Core;
using System;
using System.Collections.Generic;

namespace Jogo.RegrasJogo
{
    public static class ControladorJogo
    {
        private static readonly BancoEquacoes BancoEquacoes;
        private static Dificuldade DificuldadeSelecionada { get; set; }
        private static JogoEmAndamento JogoEmAndamento { get; set; }

        static ControladorJogo()
        {
            BancoEquacoes = new BancoEquacoes(InterpretadorEquacoesPrimeiroGrauFactory.ObterInterpretador());
        }

        public static void Inicializar(Dificuldade dificuldadeInicial)
        {
            DificuldadeSelecionada = dificuldadeInicial;

            var equacao = BancoEquacoes.ObterEquacao(dificuldadeInicial);
            JogoEmAndamento = new JogoEmAndamento(equacao);
        }

        public static InformacoesEquacao ProximaEquacao()
        {
            var novaEquacao = BancoEquacoes.ObterEquacao(DificuldadeSelecionada);
            JogoEmAndamento.IniciarNovaEquacao(novaEquacao);

            ObserversNovaEquacao.ForEach(acao => acao.Invoke(novaEquacao));

            return novaEquacao;
        }

        public static InformacoesEquacao ObterEquacaoAtual()
        {
            return JogoEmAndamento.EquacaoAtual;
        }

        private static readonly List<Action<InformacoesEquacao>> ObserversNovaEquacao = new List<Action<InformacoesEquacao>>();
        public static void RegistrarParaNovaEquacao(Action<InformacoesEquacao> acao)
        {
            ObserversNovaEquacao.Add(acao);
        }
    }
}