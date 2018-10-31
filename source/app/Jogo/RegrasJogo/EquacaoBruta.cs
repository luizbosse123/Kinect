using Jogo.Model.Enum;

namespace Jogo.RegrasJogo
{
    public class EquacaoBruta
    {
        public string RepresentacaoTextual { get; private set; }
        public Dificuldade Dificuldade { get; private set; }

        public EquacaoBruta(string equacao, Dificuldade dificuldade)
        {
            RepresentacaoTextual = equacao;
            Dificuldade = dificuldade;
        }

        public override string ToString()
        {
            return RepresentacaoTextual;
        }
    }
}