using KinectMath.Core;

namespace Jogo.RegrasJogo
{
    public class InformacoesEquacao
    {
        public Equacao EquacaoProcessada { get; private set; }
        public EquacaoBruta EquacaoBruta { get; private set; }

        public InformacoesEquacao(Equacao equacaoProcessada, EquacaoBruta equacaoBruta)
        {
            EquacaoProcessada = equacaoProcessada;
            EquacaoBruta = equacaoBruta;
        }

        public override string ToString()
        {
            return EquacaoBruta.RepresentacaoTextual;
        }
    }
}