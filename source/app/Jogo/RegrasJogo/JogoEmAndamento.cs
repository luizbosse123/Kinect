using Jogo.Model.Enum;

namespace Jogo.RegrasJogo
{
    class JogoEmAndamento
    {
        public Dificuldade Dificuldade { get; private set; }
        public InformacoesEquacao EquacaoAtual { get; set; }

        public JogoEmAndamento(InformacoesEquacao equacaoInicial)
        {
            Dificuldade = equacaoInicial.EquacaoBruta.Dificuldade;
            IniciarNovaEquacao(equacaoInicial);
        }

        public void IniciarNovaEquacao(InformacoesEquacao novaEquacao)
        {
            EquacaoAtual = novaEquacao;
        }
    }
}