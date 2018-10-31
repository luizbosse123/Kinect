using KinectMath.Core.Operacoes;
using KinectMath.Core.TermosEquacao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinectMath.Core
{
    public class Equacao
    {
        private readonly List<TermoEquacao> termosEsquerda = new List<TermoEquacao>();
        private readonly List<TermoEquacao> termosDireita = new List<TermoEquacao>();
        private readonly HistoricoOperacoesEquacao historicoOperacoes;
        private readonly Calculadora calculadora;

        public Equacao(IEnumerable<TermoEquacao> termosLadoEsquerdo, IEnumerable<TermoEquacao> termosLadoDireito)
        {
            termosEsquerda.AddRange(termosLadoEsquerdo);
            termosDireita.AddRange(termosLadoDireito);

            ValorDeX = new ResolvedorEquacaoPrimeiroGrau(this).ResolverEquacao();
            historicoOperacoes = new HistoricoOperacoesEquacao(this);
            calculadora = new Calculadora();
        }

        public Equacao ObterCopia()
        {
            return new Equacao(CopiarTermos(termosEsquerda), CopiarTermos(termosDireita));
        }

        public static List<TermoEquacao> CopiarTermos(List<TermoEquacao> listaOriginalTermos)
        {
            var listaTermos = new List<TermoEquacao>();
            listaTermos.AddRange(listaOriginalTermos.Where(termo => termo.Tipo == TipoTermo.Unidade).Select(t => new Unidade(t.Valor)));
            listaTermos.AddRange(listaOriginalTermos.Where(termo => termo.Tipo == TipoTermo.Incognita).Select(t => new Incognita(t.Valor) { Potencia = t.Potencia }));
            listaTermos.AddRange(listaOriginalTermos.Where(termo => termo.Tipo == TipoTermo.Divisor).Select(t => new Divisor(t.Valor)));

            return listaTermos;
        }

        public IReadOnlyCollection<TermoEquacao> LadoDireito
        {
            get { return termosDireita.AsReadOnly(); }
        }

        public IReadOnlyCollection<TermoEquacao> LadoEsquerdo
        {
            get { return termosEsquerda; }
        }

        public IHistoricoOperacoes HistoricoOperacoes
        {
            get { return historicoOperacoes; }
        }

        public void AplicarOperacao(Lado ladoParaAplicar, Operacao operacao)
        {
            switch (operacao.Tipo)
            {
                case TipoOperacao.Adicao:
                    Somar(ladoParaAplicar, operacao.Termo);
                    break;
                
                case TipoOperacao.Subtracao:
                    Subtrair(ladoParaAplicar, operacao.Termo);
                    break;

                case TipoOperacao.Multiplicacao:
                    Multiplicar(ladoParaAplicar, operacao.Termo);
                    break;
                
                case TipoOperacao.Divisao:
                    Dividir(ladoParaAplicar, operacao.Termo);
                    break;
            }
        }

        public void Somar(Lado ladoParaAplicar, TermoEquacao termo)
        {
            var listaTermos = ObterTermosDoLado(ladoParaAplicar);
            calculadora.Adicionar(termo, listaTermos);

            historicoOperacoes.RegistrarOperacao(new Adicao(termo), ladoParaAplicar);
        }

        public void Subtrair(Lado ladoParaAplicar, TermoEquacao termo)
        {
            var listaTermos = ObterTermosDoLado(ladoParaAplicar);
            calculadora.Subtrair(termo, listaTermos);

            historicoOperacoes.RegistrarOperacao(new Subtracao(termo), ladoParaAplicar);
        }

        public void Multiplicar(Lado ladoParaAplicar, TermoEquacao multiplicador)
        {
            var listaTermos = ObterTermosDoLado(ladoParaAplicar);
            calculadora.Multiplicar(multiplicador, listaTermos);

            historicoOperacoes.RegistrarOperacao(new Multiplicacao(multiplicador), ladoParaAplicar);
        }

        public void Dividir(Lado ladoParaAplicar, TermoEquacao divisor)
        {
            var listaTermos = ObterTermosDoLado(ladoParaAplicar);
            calculadora.Dividir(divisor, listaTermos);

            historicoOperacoes.RegistrarOperacao(new Divisao(divisor), ladoParaAplicar);
        }

        private List<TermoEquacao> ObterTermosDoLado(Lado lado)
        {
            return lado == Lado.Direito ? termosDireita : termosEsquerda;
        }

        public Equilibrio Equilibrio {
            get { return historicoOperacoes.AvaliarEquilibrio(); }
        }

        public int ValorDeX { get; private set; }

        //controlar histórico das operações
        internal void AdicionarTermoLadoEsquerdo(TermoEquacao termo)
        {
            termosEsquerda.Add(termo);
        }

        //controlar histórico das operações
        internal void AdicionarTermoLadoDireito(TermoEquacao termo)
        {
            termosDireita.Add(termo);
        }

        private class Calculadora
        {
            public void Adicionar(TermoEquacao termo, ICollection<TermoEquacao> listaTermos)
            {
                var termoDoMesmoTipo = listaTermos.FirstOrDefault(t => t.Tipo == termo.Tipo && t.Potencia == termo.Potencia);

                if (termoDoMesmoTipo != null)
                    termoDoMesmoTipo.Valor += termo.Valor;
                else
                    listaTermos.Add(termo);
            }

            public void Subtrair(TermoEquacao termo, ICollection<TermoEquacao> listaTermos)
            {
                var termoDoMesmoTipo = listaTermos.FirstOrDefault(t => t.Tipo == termo.Tipo && t.Potencia == termo.Potencia);

                if (termoDoMesmoTipo != null)
                    termoDoMesmoTipo.Valor -= termo.Valor;
                else
                {
                    termo.Valor *= -1;
                    listaTermos.Add(termo);
                }
            }

            public void Multiplicar(TermoEquacao multiplicador, IEnumerable<TermoEquacao> listaTermos)
            {
                var termos = listaTermos.ToArray();
                
                foreach (var termo in termos.Where(t => t.Tipo != TipoTermo.Divisor))
                {
                    termo.Valor *= multiplicador.Valor;

                    if (termo.Tipo == multiplicador.Tipo)
                    {
                        if (termo.Tipo == TipoTermo.Incognita)
                        {
                            termo.Potencia++;
                        }
                    }
                    else
                    {
                        termo.Tipo = TipoTermo.Incognita;
                    }
                }

                //TODO testar simplificação
                //if (termos.Any(t => t.Tipo == TipoTermo.Divisor))
                //{
                //    var divisorExistente = termos.First(t => t.Tipo == TipoTermo.Divisor);

                //    //todos eles são divisíveis
                //    if (termos.All(t => t.Valor % divisorExistente.Valor == 0)) //se são divisíveis, simplificar
                //    {
                //        Dividir(divisorExistente, termos);
                                                                        
                //    }
                //}
            }

            public void Dividir(TermoEquacao divisor, IEnumerable<TermoEquacao> listaTermos)
            {
                if (divisor.Valor == 0)
                    throw new DivideByZeroException("Tentativa de dividir por zero");

                var termos = listaTermos.ToArray();

                if(termos.Any(t => t.Tipo == TipoTermo.Divisor))
                {
                    var divisorExistente = termos.First(t => t.Tipo == TipoTermo.Divisor);
                    divisorExistente.Valor *= divisor.Valor;
                    return;
                }

                foreach (var termo in termos)
                {
                    termo.Valor /= divisor.Valor;
                    if (termo.Tipo == divisor.Tipo)
                    {
                        if (termo.Tipo == TipoTermo.Incognita)
                        {
                            termo.Potencia--;
                        }
                    }
                    else
                    {
                        termo.Tipo = TipoTermo.Incognita;
                    }
                }
            }
        }

        private class HistoricoOperacoesEquacao : IHistoricoOperacoes
        {
            private readonly List<TermoEquacao> termosOriginaisEsquerda;
            private readonly List<TermoEquacao> termosOriginaisDireita;
            private readonly int valorDeX;
            private readonly Stack<OperacaoEfetuada> operacoes = new Stack<OperacaoEfetuada>();

            public HistoricoOperacoesEquacao(Equacao equacaoBase)
            {
                termosOriginaisEsquerda = CopiarTermos(equacaoBase.termosEsquerda);
                termosOriginaisDireita = CopiarTermos(equacaoBase.termosDireita);
                valorDeX = equacaoBase.ValorDeX;
            }

            public void RegistrarOperacao(Operacao operacao, Lado lado)
            {
                operacoes.Push(new OperacaoEfetuada(operacao, lado));
            }

            internal Equilibrio AvaliarEquilibrio()
            {
                var esquerda = ProcessarOperacoes(operacoes.ToArray().Where(o => o.Lado == Lado.Esquerdo).Select(o => o.Operacao).ToArray());
                var direita = ProcessarOperacoes(operacoes.ToArray().Where(o => o.Lado == Lado.Direito).Select(o => o.Operacao).ToArray());

                if (esquerda < direita)
                    return Equilibrio.PendendoParaDireita;
                if (esquerda > direita)
                    return Equilibrio.PendendoParaEsquerda;
                return Equilibrio.Equilibrado;
            }

            private int ProcessarOperacoes(IEnumerable<Operacao> operacoes)
            {
                var resultado = 1d;

                foreach (var operacao in operacoes.OrderBy(o => o.Tipo))
                {
                    var termo = operacao.Termo;
                    var valorRealTermo = (int) Math.Pow(termo.Valor, termo.Potencia);

                    if(termo.Tipo == TipoTermo.Incognita)
                        valorRealTermo *= valorDeX;

                    switch (operacao.Tipo)
                    {
                        case TipoOperacao.Adicao:
                            resultado += valorRealTermo;
                            break;
                        case TipoOperacao.Subtracao:
                            resultado -= valorRealTermo;
                            break;
                        case TipoOperacao.Multiplicacao:
                            resultado *= valorRealTermo;
                            break;
                        case TipoOperacao.Divisao:
                            resultado /= valorRealTermo;
                            break;
                    }
                }

                return (int)resultado;
            }

            public IOperacao[] ObterOperacoes(Lado ladoEquacao)
            {
                return operacoes
                    .ToArray()
                    .Where(o => o.Lado == Lado.Esquerdo)
                    .Cast<IOperacao>()
                    .Reverse()
                    .ToArray();
            }

            public DescricaoPassoHistoricoOperacoes[] PassoAPasso()
            {
                var equacao = ObterCopiaEquacao();
                var passoAPasso = new List<DescricaoPassoHistoricoOperacoes>
                {
                    new DescricaoPassoHistoricoOperacoes(equacao.ToString(), null)
                };

                foreach (var operacaoEfetuada in operacoes.ToArray().Reverse())
                {
                    equacao.AplicarOperacao(operacaoEfetuada.Lado, operacaoEfetuada.Operacao);
                    passoAPasso.Add(new DescricaoPassoHistoricoOperacoes(equacao.ToString(), operacaoEfetuada));
                }

                return passoAPasso.ToArray();
            }

            private Equacao ObterCopiaEquacao()
            {
                var equacao = new Equacao(CopiarTermos(termosOriginaisEsquerda), CopiarTermos(termosOriginaisDireita));
                return equacao;
            }
        }

        private class ResolvedorEquacaoPrimeiroGrau
        {
            private readonly List<TermoEquacao> termosEsquerda;
            private readonly List<TermoEquacao> termosDireita;

            public ResolvedorEquacaoPrimeiroGrau(Equacao equacaoSimplificada)
            {
                termosEsquerda = CopiarTermos(equacaoSimplificada.termosEsquerda);
                termosDireita = CopiarTermos(equacaoSimplificada.termosDireita);
            }

            public int ResolverEquacao()
            {
                EliminarDivisores();
                EliminarIncognitasNaDireita();
                EliminarUnidadesNaEsquerda();
                IsolarIncognita();
                return ValorIncognita();
            }

            private void EliminarDivisores()
            {
                if (ExisteUmDivisor(termosEsquerda))
                {
                    var divisorEsquerda = termosEsquerda.First(t => t.Tipo == TipoTermo.Divisor);
                    termosEsquerda.Remove(divisorEsquerda);

                    foreach (var termo in termosDireita.Where(t => t.Tipo != TipoTermo.Divisor))
                    {
                        termo.Valor *= divisorEsquerda.Valor;
                    }
                }

                if (ExisteUmDivisor(termosDireita))
                {
                    var divisorDireita = termosDireita.First(t => t.Tipo == TipoTermo.Divisor);
                    termosDireita.Remove(divisorDireita);

                    foreach (var termo in termosEsquerda)
                    {
                        termo.Valor *= divisorDireita.Valor;
                    }
                }
            }

            private bool ExisteUmDivisor(IEnumerable<TermoEquacao> listaTermos)
            {
                return listaTermos.Any(t => t.Tipo == TipoTermo.Divisor);
            }

            private void EliminarIncognitasNaDireita()
            {
                if (!ExistemIncognitas(termosDireita))
                    return;

                if (!ExistemIncognitas(termosEsquerda))
                    termosEsquerda.Add(new Incognita(0));

                var incognitaLadoEsquerdo = termosEsquerda.First(t => t.Tipo == TipoTermo.Incognita);
                var incognitaLadoDireito = termosDireita.First(t => t.Tipo == TipoTermo.Incognita);

                incognitaLadoEsquerdo.Valor += -incognitaLadoDireito.Valor;
                incognitaLadoDireito.Valor += -incognitaLadoDireito.Valor;
            }

            private bool ExistemIncognitas(IEnumerable<TermoEquacao> listaTermos)
            {
                return listaTermos.Any(t => t.Tipo == TipoTermo.Incognita);
            }

            private void EliminarUnidadesNaEsquerda()
            {
                if (!ExistemUnidades(termosEsquerda))
                    return;

                if (!ExistemUnidades(termosDireita))
                    termosDireita.Add(new Unidade(0));
                
                var unidadeLadoEsquerdo = termosEsquerda.First(t => t.Tipo == TipoTermo.Unidade);
                var unidadeLadoDireito = termosDireita.First(t => t.Tipo == TipoTermo.Unidade);

                unidadeLadoDireito.Valor += -unidadeLadoEsquerdo.Valor;
                unidadeLadoEsquerdo.Valor += -unidadeLadoEsquerdo.Valor;
            }

            private bool ExistemUnidades(IEnumerable<TermoEquacao> listaTermos)
            {
                return listaTermos.Any(t => t.Tipo == TipoTermo.Unidade);
            }

            private void IsolarIncognita()
            {
                var incognitaLadoEsquerdo = termosEsquerda.First(t => t.Tipo == TipoTermo.Incognita);
                var unidadeLadoDireito = termosDireita.First(t => t.Tipo == TipoTermo.Unidade);

                if (incognitaLadoEsquerdo.Valor < 0)
                {
                    incognitaLadoEsquerdo.Valor *= -1;
                    unidadeLadoDireito.Valor *= -1;
                }

                if (incognitaLadoEsquerdo.Valor != 1)
                {
                    if (incognitaLadoEsquerdo.Valor == 0)
                        throw new DivideByZeroException("Os dois lados da equação possuem incógnitas mesmo número de incógnitas (ex: 2x = 2x)");
                    unidadeLadoDireito.Valor /= incognitaLadoEsquerdo.Valor;
                    incognitaLadoEsquerdo.Valor /= incognitaLadoEsquerdo.Valor;
                }
            }

            private int ValorIncognita()
            {
                return termosDireita.First(t => t.Tipo == TipoTermo.Unidade).Valor;
            }
        }

        public SnapshotEquacao ObterSnapshot()
        {
            return new SnapshotEquacao(
                unidadesEsquerda: Unidades(termosEsquerda).Sum(t => t.Valor),
                incognitasEsquerda: Incognitas(termosEsquerda).Sum(t => t.Valor),
                unidadesDireita: Unidades(termosDireita).Sum(t => t.Valor),
                incognitasDireita: Incognitas(termosDireita).Sum(t => t.Valor)
            );
        }

        private IEnumerable<TermoEquacao> Unidades(IEnumerable<TermoEquacao> termos)
        {
            return termos.Where(t => t.Tipo == TipoTermo.Unidade);
        }

        private IEnumerable<TermoEquacao> Incognitas(IEnumerable<TermoEquacao> termos)
        {
            return termos.Where(t => t.Tipo == TipoTermo.Incognita);
        }

        public bool FoiResolvida()
        {
            var representacao = ObterSnapshot();
            return representacao.IncognitasEsquerda == 1 &&
                   representacao.UnidadesEsquerda == 0 &&
                   representacao.IncognitasDireita == 0 &&
                   representacao.UnidadesDireita == ValorDeX
                   ||
                   representacao.IncognitasDireita == 1 &&
                   representacao.UnidadesDireita == 0 &&
                   representacao.IncognitasEsquerda == 0 &&
                   representacao.UnidadesEsquerda == ValorDeX;
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            var snapshot = ObterSnapshot();

            if (snapshot.IncognitasEsquerda != 0)
            {
                stringBuilder.AppendFormat("{0}x ", snapshot.IncognitasEsquerda);
            }
            
            if (snapshot.UnidadesEsquerda != 0)
            {
                var sinal = (snapshot.IncognitasEsquerda != 0)
                    ? snapshot.UnidadesEsquerda > 0 ? "+" : ""
                    : "";
                stringBuilder.AppendFormat("{0}{1} ", sinal, snapshot.UnidadesEsquerda);
            }

            stringBuilder.Append("= ");

            if (snapshot.IncognitasDireita != 0)
            {
                stringBuilder.AppendFormat("{0}x ", snapshot.IncognitasDireita);
            }

            if (snapshot.UnidadesDireita != 0)
            {
                var sinal = (snapshot.IncognitasDireita != 0)
                   ? snapshot.UnidadesDireita > 0 ? "+" : ""
                   : "";

                stringBuilder.AppendFormat("{0}{1}", sinal, snapshot.UnidadesDireita);
            }

            return stringBuilder.ToString();
        }
    }
}
