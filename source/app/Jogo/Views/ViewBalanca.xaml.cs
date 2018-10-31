using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Jogo.Controles;
using Jogo.Model.Enum;
using Jogo.RegrasJogo;
using Jogo.ViewModel;
using KinectMath.Core;
using KinectMath.Core.TermosEquacao;

namespace Jogo.Views
{
    /// <summary>
    /// Interaction logic for ViewBalanca.xaml
    /// </summary>
    //TODO diferenciar entre a fase de construção e resolução da equação
    public partial class ViewBalanca : UserControl, INotifyPropertyChanged, IViewBalanca
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChangedEvent(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private int anguloPlataforma;
        public int AnguloPlataforma
        {
            get { return anguloPlataforma; }
            set
            {
                anguloPlataforma = value;
                RaisePropertyChangedEvent("AnguloPlataforma");
            }
        }

        public ViewBalanca()
        {
            InitializeComponent();

            Loaded += (sender, e) =>
            {
                if (!DesignerProperties.GetIsInDesignMode(this))
                {
                    this.ObterViewModel().TrianguloArrastavel = TrianguloParaArrastar;
                    this.ObterViewModel().CirculoArrastavel = CirculoParaArrastar;
                }
            };
        }

        public void NovaEquacao()
        {
            AreaDropEsquerda.Children.Clear();
           // AreaDropDireita.Children.Clear();
        }

        public bool AdicionarElementoQuadro(DragDropElement elementoDropado)
        {
            var peso = elementoDropado.Child as TrianguloArrastavel;

            var equacaoEmConstrucao = this.ObterViewModel().EquacaoEmConstrucao;
            if (FoiExecutadoDrop(elementoDropado, AreaDropEsquerda))
            {
                AdicionarPesoEReavaliarObjetivo(peso, Lado.Esquerdo, equacaoEmConstrucao);
                return true;
            }
            /*
            if (FoiExecutadoDrop(elementoDropado, AreaDropDireita))
            {
                AdicionarPesoEReavaliarObjetivo(peso, Lado.Direito, equacaoEmConstrucao);
                return true;
            }
            */
            return false;
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

        private void AdicionarPesoEReavaliarObjetivo(TrianguloArrastavel trianguloArrastavel, Lado lado, EquacaoEmConstrucao equacaoEmConstrucao)
        {
             //equacaoEmConstrucao.Adicionar(trianguloArrastavel.Texto, lado);
            AtualizarEquilibrioBalanca(equacaoEmConstrucao);
            AtualizarRepresentacaoBalanca(equacaoEmConstrucao);

            var equacaoAtual = ControladorJogo.ObterEquacaoAtual().EquacaoProcessada;
            if (equacaoEmConstrucao.EhQuivalente(equacaoAtual))
            {
                this.ObterViewModel().AvancarParaEtapaDeResolucao();
                //Dispatcher.BeginInvoke(new Action(delegate
                //{
                //    this.ObterViewModel().AvancarParaEtapaDeResolucao();
                //}), DispatcherPriority.Send, TimeSpan.FromMilliseconds(2000));
            }
        }

        //TODO é possível animar | "Não temos tempo pra isso - deixa assim"
        private void AtualizarEquilibrioBalanca(EquacaoEmConstrucao equacaoEmConstrucao)
        {
            const int anguloQuandoDesequilibrada = 10;

            var equilibrio = equacaoEmConstrucao.AvaliarEquilibrio();
            switch (equilibrio)
            {
                case Equilibrio.PendendoParaDireita:
                    AnguloPlataforma = anguloQuandoDesequilibrada;
                    break;
                case Equilibrio.PendendoParaEsquerda:
                    AnguloPlataforma = -anguloQuandoDesequilibrada;
                    break;
                default:
                    AnguloPlataforma = 0;
                    break;
            }
        }

        private void AtualizarRepresentacaoBalanca(EquacaoEmConstrucao equacaoEmConstrucao)
        {
            var representacao = equacaoEmConstrucao.ObterRepresentacao();

            AreaDropEsquerda.Children.Clear();
          //  AreaDropDireita.Children.Clear();

            if (representacao.IncognitasEsquerda != 0)
            {
                var peso = new TrianguloArrastavel
                {
                    TipoTermo = TipoTermo.Incognita,
                    Valor = representacao.IncognitasEsquerda
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

            /*if (representacao.IncognitasDireita != 0)
            {
                var peso = new TrianguloArrastavel
                {
                    TipoTermo = TipoTermo.Incognita,
                    Valor = representacao.IncognitasDireita
                };
                AreaDropDireita.Children.Add(peso);
            }

            if (representacao.UnidadesDireita != 0)
            {
                var peso = new TrianguloArrastavel
                {
                    TipoTermo = TipoTermo.Unidade,
                    Valor = representacao.UnidadesDireita
                };
                AreaDropDireita.Children.Add(peso);
            }
            */
            AreaDropEsquerda.UpdateLayout();
          //  AreaDropDireita.UpdateLayout();
        }
    }
}
