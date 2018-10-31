using Jogo.Model.Enum;
using KinectMath.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Jogo.RegrasJogo
{
    public class BancoEquacoes
    {
        private static LinkedListNode<EquacaoBruta> equacaoFacilAtual; 
        private static LinkedListNode<EquacaoBruta> equacaoMediaAtual; 
        private static LinkedListNode<EquacaoBruta> equacaoDificilAtual; 
        
        private static LinkedList<EquacaoBruta> Filtrar(Dificuldade dificuldadeDesejada, IEnumerable<EquacaoBruta> listaCompletaEquacoes)
        {
            var listaFiltrada = new LinkedList<EquacaoBruta>();
            foreach (var equacaoFacil in listaCompletaEquacoes.Where(e => e.Dificuldade == dificuldadeDesejada))
            {
                listaFiltrada.AddLast(equacaoFacil);
            }
            return listaFiltrada;
        } 

        private readonly InterpretadorEquacoesPrimeiroGrau interpretadorEquacoesPrimeiroGrau;

        public BancoEquacoes(InterpretadorEquacoesPrimeiroGrau interpretadorEquacoesPrimeiroGrau)
        {
            this.interpretadorEquacoesPrimeiroGrau = interpretadorEquacoesPrimeiroGrau;

            var caminhoArquivo = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BancoEquacoes.csv");
            var equacoes = File.ReadAllLines(caminhoArquivo)
                .Select(linha =>
                {
                    var dadosBrutos = linha.Split(';');
                    var dificuldade = int.Parse(dadosBrutos[0]);
                    var texto = dadosBrutos[1];

                    return new EquacaoBruta(texto, (Dificuldade)dificuldade);
                })
                .ToArray();

            var faceis = Filtrar(Dificuldade.Facil, equacoes);
            equacaoFacilAtual = faceis.First;

            var medias = Filtrar(Dificuldade.Media, equacoes);
            equacaoMediaAtual = medias.First;

            var dificeis = Filtrar(Dificuldade.Dificil, equacoes);
            equacaoDificilAtual = dificeis.First;
        }

        public InformacoesEquacao ObterEquacao(Dificuldade dificuldade)
        {
            var equacaoAtual = ObterEquacaoAtualPorDificuldade(dificuldade);
            var equacaoBruta = equacaoAtual.Value;
            var equacaoProcessada = interpretadorEquacoesPrimeiroGrau.Interpretar(equacaoBruta.RepresentacaoTextual);

            var proxima = equacaoAtual.Next;
            AtualizarEquacaoAtual(dificuldade, proxima);

            return new InformacoesEquacao(equacaoProcessada, equacaoBruta);
        }

        private void AtualizarEquacaoAtual(Dificuldade dificuldade, LinkedListNode<EquacaoBruta> proxima)
        {
            switch (dificuldade)
            {
                case Dificuldade.Facil:
                    equacaoFacilAtual = proxima;
                    break;
                case Dificuldade.Media: 
                    equacaoMediaAtual = proxima;
                    break;
                default: 
                    equacaoDificilAtual = proxima;
                    break;
            }
        }

        private LinkedListNode<EquacaoBruta> ObterEquacaoAtualPorDificuldade(Dificuldade dificuldade)
        {
            switch (dificuldade)
            {
                case Dificuldade.Facil: return equacaoFacilAtual;
                case Dificuldade.Media: return equacaoMediaAtual;
                default: return equacaoDificilAtual;
            }
        }
    }
}