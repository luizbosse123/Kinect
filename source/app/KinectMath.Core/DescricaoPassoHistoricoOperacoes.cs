namespace KinectMath.Core
{
    public class DescricaoPassoHistoricoOperacoes
    {
        public OperacaoEfetuada OperacaoEfetuada { get; private set; }
        public string Equacao { get; private set; }

        public DescricaoPassoHistoricoOperacoes(string equacao, OperacaoEfetuada operacaoEfetuada)
        {
            OperacaoEfetuada = operacaoEfetuada;
            Equacao = equacao;
        }
    }
}