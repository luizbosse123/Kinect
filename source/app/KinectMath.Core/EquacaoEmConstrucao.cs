using KinectMath.Core.TermosEquacao;
using KinectMath.Core.TermosEquacao.Extracao;
using System.Collections.Generic;
using System.Linq;

namespace KinectMath.Core
{
    public class EquacaoEmConstrucao
    {
        private readonly int valorDeX;

        private readonly IExtratorTermosEquacao extratorUnidades = new ExtratorUnidades();
        private readonly IExtratorTermosEquacao extratorIncognitas = new ExtratorIncognitas();
        private readonly List<TermoEquacao> unidadesEsquerda = new List<TermoEquacao>();
        private readonly List<TermoEquacao> incognitasEsquerda = new List<TermoEquacao>();
        private readonly List<TermoEquacao> unidadesDireita = new List<TermoEquacao>();
        private readonly List<TermoEquacao> incognitasDireita = new List<TermoEquacao>();

        public EquacaoEmConstrucao(int prValorDeX)
        {
            this.valorDeX = prValorDeX;
        }

        public void Adicionar(string valorString, Lado lado)
        {
            if (lado == Lado.Esquerdo)
            {
                unidadesEsquerda.AddRange(extratorUnidades.Extrair(valorString));
                incognitasEsquerda.AddRange(extratorIncognitas.Extrair(valorString));
            }
            else
            {
                unidadesDireita.AddRange(extratorUnidades.Extrair(valorString));
                incognitasDireita.AddRange(extratorIncognitas.Extrair(valorString));
            }
        }

        public Equilibrio AvaliarEquilibrio()
        {
            var totalEsquerda = Soma(unidadesEsquerda) + Soma(incognitasEsquerda);
            var totalDireita = Soma(unidadesDireita) + Soma(incognitasDireita);

            if (totalDireita > totalEsquerda)
                return Equilibrio.PendendoParaDireita;
            if (totalDireita < totalEsquerda)
                return Equilibrio.PendendoParaEsquerda;
            return Equilibrio.Equilibrado;                    
        }

        public bool EhQuivalente(Equacao equacao)
        {
            var unidadesEsquerdaEquacao = equacao.LadoEsquerdo.Where(t => t.Tipo == TipoTermo.Unidade).ToArray();
            var incognitasEsquerdaEquacao = equacao.LadoEsquerdo.Where(t => t.Tipo == TipoTermo.Incognita).ToArray();
            var unidadesDireitaEquacao = equacao.LadoDireito.Where(t => t.Tipo == TipoTermo.Unidade).ToArray();
            var incognitasDireitaEquacao = equacao.LadoDireito.Where(t => t.Tipo == TipoTermo.Incognita).ToArray();

            return Soma(unidadesEsquerda) == Soma(unidadesEsquerdaEquacao) &&
                   Soma(incognitasEsquerda) == Soma(incognitasEsquerdaEquacao) &&
                   Soma(unidadesDireita) == Soma(unidadesDireitaEquacao) &&
                   Soma(incognitasDireita) == Soma(incognitasDireitaEquacao);
        }

        private int Soma(IEnumerable<TermoEquacao> listaTermos)
        {
            return listaTermos.Sum(termo =>
            {
                if (termo.Tipo == TipoTermo.Incognita)
                    return termo.Valor * valorDeX;
                return termo.Valor;
            });
        }

        private bool SaoEquivalentes(IList<TermoEquacao> lista1, IList<TermoEquacao> lista2)
        {
            if (lista1.Count != lista2.Count)
                return false;

            for (int i = 0; i < lista1.Count; i++)
            {
                if (lista1[i].Valor != lista2[i].Valor)
                    return false;
            }

            return true;
        }

        public SnapshotEquacao ObterRepresentacao()
        {
            return new SnapshotEquacao(
                unidadesEsquerda: unidadesEsquerda.Sum(t => t.Valor),
                incognitasEsquerda: incognitasEsquerda.Sum(t => t.Valor),
                unidadesDireita: unidadesDireita.Sum(t => t.Valor),
                incognitasDireita: incognitasDireita.Sum(t => t.Valor)
            );
        }
    }
}
