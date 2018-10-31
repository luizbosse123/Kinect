using System;
using Jogo.Controles;
using Jogo.RegrasJogo;
using Jogo.ViewModel;
using KinectMath.Core;
using KinectMath.Core.Operacoes;
using KinectMath.Core.TermosEquacao;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Jogo.Model.Enum;

namespace Jogo.Views
{
    /// <summary>
    /// Interaction logic for ViewBalancaResolucaoEquacao.xaml
    /// </summary>
    public partial class ViewBalancaResolucaoEquacao : UserControl, IViewBalanca, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ViewBalancaResolucaoEquacao()
        {
            InitializeComponent();

            Loaded += (sender, e) =>
            {
                if (!DesignerProperties.GetIsInDesignMode(this))
                {
                    this.ObterViewModel().OperacaoArrastavel = OperacaoParaArrastar;
                    ControladorJogo.RegistrarParaNovaEquacao(novaEquacao =>
                    {
                        var equacaoProcessada = novaEquacao.EquacaoProcessada;
                        AtualizarRepresentacaoBalanca(equacaoProcessada);
                        AtualizarHistoricoOperacoes(equacaoProcessada);
                    });
                }
            };
        }

        protected void RaisePropertyChangedEvent(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private int anguloPlataforma;
        public int AnguloPlataforma2
        {
            get { return anguloPlataforma; }
            set
            {
                anguloPlataforma = value;
                RaisePropertyChangedEvent("AnguloPlataforma2");
            }
        }

        public bool AdicionarElementoQuadro(DragDropElement elementoDropado)
        {
            var operacao = elementoDropado.Child as OperacaoArrastavel;

            var equacaoAtual = this.ObterViewModel().EquacaoEmFaseDeResolucao;
            if (FoiExecutadoDrop(elementoDropado, AreaDropEsquerda))
            {
                AplicarOperacaoEReavaliarObjetivo(operacao, Lado.Esquerdo, equacaoAtual);
                return true;
            }

         //   if (FoiExecutadoDrop(elementoDropado, AreaDropDireita))
        //    {
         //       AplicarOperacaoEReavaliarObjetivo(operacao, Lado.Direito, equacaoAtual);
         //       return true;
        //    }

            return false;
        }

        private void AplicarOperacaoEReavaliarObjetivo(OperacaoArrastavel operacaoArrastavel, Lado lado, Equacao equacaoAtual)
        {
            var termo = ObterTermo(operacaoArrastavel.TipoTermo, operacaoArrastavel.Valor);
            var operacao = new Operacao(operacaoArrastavel.TipoOperacao, termo);
            equacaoAtual.AplicarOperacao(lado, operacao);
            
            AtualizarEquilibrioBalanca(equacaoAtual);
            AtualizarRepresentacaoBalanca(equacaoAtual);
            AtualizarHistoricoOperacoes(equacaoAtual);

            if (equacaoAtual.FoiResolvida())
            {
                this.ObterViewModel().AvancarParaMontagemDeUmaNovaEquacao();
                //Dispatcher.BeginInvoke(new Action(delegate
                //{
                //    this.ObterViewModel().AvancarParaMontagemDeUmaNovaEquacao();
                //}), DispatcherPriority.Send, TimeSpan.FromMilliseconds(2000));
            }
        }

        private TermoEquacao ObterTermo(TipoTermo tipo, int valor)
        {
            switch (tipo)
            {
                case TipoTermo.Unidade: return new Unidade(valor);
                default: return new Incognita(valor);
            }
        }

        private bool FoiExecutadoDrop(DragDropElement elemento, WrapPanel areaDrop)
        {
            var elementoCentroX = elemento.ActualWidth / 2;
            var elementoCentroY = elemento.ActualHeight / 2;
            var pontoOndeFoiSolto = elemento.TransformToAncestor(CanvasBalanca).Transform(new Point(0, 0));

            // centraliza ponto de detecção para centro do elemento
            pontoOndeFoiSolto.X += elementoCentroX;
            pontoOndeFoiSolto.Y += elementoCentroY;

            var posicaoAreaDrop = areaDrop.TransformToAncestor(CanvasBalanca).Transform(new Point(0, 0));

            return pontoOndeFoiSolto.X > posicaoAreaDrop.X &&
                pontoOndeFoiSolto.X < (posicaoAreaDrop.X + areaDrop.ActualWidth) &&
                pontoOndeFoiSolto.Y > posicaoAreaDrop.Y &&
                pontoOndeFoiSolto.Y < (posicaoAreaDrop.Y + areaDrop.ActualHeight);
        }

        private void AtualizarEquilibrioBalanca(Equacao equacao)
        {
            const int anguloQuandoDesequilibrada = 10;

            switch (equacao.Equilibrio)
            {
                case Equilibrio.PendendoParaDireita:
                    AnguloPlataforma2 = anguloQuandoDesequilibrada;
                    break;
                case Equilibrio.PendendoParaEsquerda:
                    AnguloPlataforma2 = -anguloQuandoDesequilibrada;
                    break;
                default:
                    AnguloPlataforma2 = 0;
                    break;
            }
        }

        private void AtualizarRepresentacaoBalanca(Equacao equacao)
        {
            var representacao = equacao.ObterSnapshot();

            AreaDropEsquerda.Children.Clear();
            //AreaDropDireita.Children.Clear();

            if (representacao.IncognitasEsquerda != 0)
            {
                var peso = new TrianguloArrastavel
                {
                    TipoTermo = TipoTermo.Incognita,
                    Valor = representacao.IncognitasEsquerda,
                };
                AreaDropEsquerda.Children.Add(peso);
            }

            if (representacao.UnidadesEsquerda != 0)
            {
                var peso = new TrianguloArrastavel
                {
                    TipoTermo = TipoTermo.Unidade,
                    Valor = representacao.UnidadesEsquerda
                };
                AreaDropEsquerda.Children.Add(peso);
            }

            //if (representacao.IncognitasDireita != 0)
            //{
            //    var peso = new TrianguloArrastavel
            //    {
            //        TipoTermo = TipoTermo.Incognita,
            //        Valor = representacao.IncognitasDireita
            //    };
            //    AreaDropDireita.Children.Add(peso);
            //}

            //if (representacao.UnidadesDireita != 0)
            //{
            //    var peso = new TrianguloArrastavel
            //    {
            //        TipoTermo = TipoTermo.Unidade,
            //        Valor = representacao.UnidadesDireita
            //    };
            //    AreaDropDireita.Children.Add(peso);
            //}

            AreaDropEsquerda.UpdateLayout();
            //AreaDropDireita.UpdateLayout();
        }

        private void AtualizarHistoricoOperacoes(Equacao equacao)
        {
            HistoricoOperacoes.Atualizar(equacao.HistoricoOperacoes.PassoAPasso());
        }
    }
}
