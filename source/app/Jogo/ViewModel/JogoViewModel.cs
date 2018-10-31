using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Jogo.Model;
using Jogo.Model.Enum;
using Jogo.RegrasJogo;
using Jogo.Views;
using KinectMath.Core;
using KinectMath.Core.Operacoes;
using KinectMath.Core.TermosEquacao;

namespace Jogo.ViewModel
{
    public class JogoViewModel : ObservableObject
    {
        #region Propriedades

        private Cenario cenarioAtual = Cenario.Inicio;
        public Cenario CenarioAtual
        {
            get { return cenarioAtual; }
            set
            {
                cenarioAtual = value;
                RaisePropertyChangedEvent("CenarioAtual");
            }
        }

        private Dificuldade dificuldadeSelecionada;
        public Dificuldade DificuldadeSelecionada
        {
            get { return dificuldadeSelecionada; }
            set
            {
                dificuldadeSelecionada = value;
                RaisePropertyChangedEvent("DificuldadeSelecionada");
            }
        }

        private bool habilitaKinectViewer;
        public bool HabilitaKinectViewer
        {
            get { return habilitaKinectViewer; }
            set
            {
                habilitaKinectViewer = value;
                RaisePropertyChangedEvent("HabilitaKinectViewer");
            }
        }

        private string textoEquacaoAtual;
        public string TextoEquacaoAtual
        {
            get { return textoEquacaoAtual; }
            set
            {
                textoEquacaoAtual = value;
                RaisePropertyChangedEvent("TextoEquacaoAtual");
            }
        }

        private Visibility visibilidadeValorAjustavel = Visibility.Collapsed;
        public Visibility VisibilidadeValorAjustavel
        {
            get { return visibilidadeValorAjustavel; }
            set
            {
                visibilidadeValorAjustavel = value;
                RaisePropertyChangedEvent("VisibilidadeValorAjustavel");
            }
        }

        private int valorNovoElemento;
        public int ValorNovoElemento
        {
            get { return valorNovoElemento; }
            set
            {
                valorNovoElemento = value;
                TrianguloArrastavel.Valor = value;
                OperacaoArrastavel.Valor = value;
            }
        }

        private Visibility visibilidadeTipoTermoQuandoMultiplicacaoOuDivisao;

        public Visibility VisibilidadeTipoTermoQuandoMultiplicacaoOuDivisao
        {
            get { return visibilidadeTipoTermoQuandoMultiplicacaoOuDivisao; }
            set
            {
                visibilidadeTipoTermoQuandoMultiplicacaoOuDivisao = value;
                RaisePropertyChangedEvent("VisibilidadeTipoTermoQuandoMultiplicacaoOuDivisao");
            }
        }

        public TrianguloArrastavel TrianguloArrastavel { get; set; }

        public CirculoArrastavel CirculoArrastavel { get; set; }

        public OperacaoArrastavel OperacaoArrastavel { get; set; }

        public EquacaoEmConstrucao EquacaoEmConstrucao { get; set; }
        public Equacao EquacaoEmFaseDeResolucao { get; set; }

        private SolidColorBrush _backgroundArrastandoElemento = Brushes.Transparent;
        private string mensagemEtapaCompletada;

        public SolidColorBrush BackgroundArrastandoElemento
        {
            get { return _backgroundArrastandoElemento; }
            set
            {
                _backgroundArrastandoElemento = value;
                RaisePropertyChangedEvent("BackgroundArrastandoElemento");
            }
        }

        #endregion

        public JogoViewModel()
        {
            HabilitaKinectViewer = true;
        }

        #region Metodos

        #region Tela Jogar

        public ICommand JogarCommand
        {
            get { return new DelegateCommand(Jogar); }
        }

        private void Jogar(object parameters)
        {
            // Proximo cenário

            CenarioAtual = Cenario.SelecionarDificuldade;

            HabilitaKinectViewer = false;

            SelecionaDificuldade(Dificuldade.Facil);

          //  var tipoTermo = (TipoTermo)parameters;
            VisibilidadeValorAjustavel = Visibility.Visible;

            TrianguloArrastavel.Visibility = Visibility.Visible;
           // TrianguloArrastavel.TipoTermo = tipoTermo;
           // TrianguloArrastavel.Valor = 1;

        }

        #endregion

        #region Tela Selecionar Dificuldade

        public ICommand SelecionaDificuldadeCommand
        {
            get { return new DelegateCommand(SelecionaDificuldade); }
        }

        private void SelecionaDificuldade(object parameters)
        {
            DificuldadeSelecionada = (Dificuldade)parameters;
            IniciarNovoJogo();
            //IniciarFaseDeResolucao();
        }

        private void IniciarNovoJogo()
        {
            ControladorJogo.Inicializar(DificuldadeSelecionada);
            var equacaoPorResolver = ControladorJogo.ProximaEquacao();
            TextoEquacaoAtual = equacaoPorResolver.ToString();
            EquacaoEmConstrucao = new EquacaoEmConstrucao(equacaoPorResolver.EquacaoProcessada.ValorDeX);

            // Proximo cenário
            CenarioAtual = Cenario.MontarEquacao;
            
            SelecionarTipoTermoParaMontagem(TipoTermo.Unidade);
            ValorNovoElemento = 1;

            //TODO montar string de acordo com o que foi arrastado
            VisibilidadeValorAjustavel = Visibility.Collapsed;
            TrianguloArrastavel.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region Tela Montar Equação

        public ICommand SelecionarTipoTermoParaMontagemCommand
        {
            get { return new DelegateCommand(SelecionarTipoTermoParaMontagem); }
        }

        private void SelecionarTipoTermoParaMontagem(object parameters)
        {
            var tipoTermo = (TipoTermo)parameters;
            VisibilidadeValorAjustavel = Visibility.Visible;

            TrianguloArrastavel.Visibility = Visibility.Visible;
            TrianguloArrastavel.TipoTermo = tipoTermo;
            TrianguloArrastavel.Valor = 1;
        }

        #endregion

        #region Tela Informar Jogador Proximo Etapa

        public ICommand ContinuacaoCommand
        {
            get { return new DelegateCommand(ContinuacaoProximaEtapa); }
        }

        private void ContinuacaoProximaEtapa(object parameters)
        {
            IniciarFaseDeResolucao();
        }

        private void IniciarFaseDeResolucao()
        {
            CenarioAtual = Cenario.ResolverEquacao;

            ValorNovoElemento = 1;
            EquacaoEmFaseDeResolucao = ControladorJogo.ObterEquacaoAtual().EquacaoProcessada.ObterCopia();
            visibilidadeValorAjustavel = Visibility.Collapsed;
            SelecionarTipoOperacaoParaResolucao(TipoOperacao.Adicao);
            OperacaoArrastavel.Resetar();
        }

        #endregion

        #region Tela Resolver Equação

        public ICommand SelecionarTipoTermoParaResolucaoCommand
        {
            get { return new DelegateCommand(SelecionarTipoTermoParaResolucao); }
        }

        private void SelecionarTipoTermoParaResolucao(object parameters)
        {
            var tipoTermo = (TipoTermo)parameters;
            ValorNovoElemento = 1;
            VisibilidadeValorAjustavel = Visibility.Visible;
            OperacaoArrastavel.Visibility = Visibility.Visible;

            OperacaoArrastavel.TipoTermo = tipoTermo;
        }

        public ICommand SelecionarTipoOperacaoParaResolucaoCommand
        {
            get { return new DelegateCommand(SelecionarTipoOperacaoParaResolucao); }
        }

        private void SelecionarTipoOperacaoParaResolucao(object parametro)
        {
            var tipoOperacao = (TipoOperacao)parametro;
            OperacaoArrastavel.TipoOperacao = tipoOperacao;

            if ((tipoOperacao == TipoOperacao.Multiplicacao || tipoOperacao == TipoOperacao.Divisao))
            {
                VisibilidadeTipoTermoQuandoMultiplicacaoOuDivisao = Visibility.Collapsed;
                VisibilidadeValorAjustavel = Visibility.Visible;
            }
            else
            {
                VisibilidadeTipoTermoQuandoMultiplicacaoOuDivisao = Visibility.Visible;
            }
        }

        #endregion

        #region Tela Proxima Equação

        public ICommand ProximaEquacaoCommand
        {
            get { return new DelegateCommand(ProximaEquacao); }
        }

        private void ProximaEquacao(object parameters)
        {
            SelecionaDificuldade(Dificuldade.Facil);
        }

        #endregion

        public ICommand ProximaEtapaCommand
        {
            get { return new DelegateCommand(ProximaEtapa); }
        }

        private void ProximaEtapa(object parameters)
        {
            VisibilidadeValorAjustavel = Visibility.Collapsed;
            var cenario = (Cenario)parameters;
            
            CenarioAtual = cenario;
        }

        public string MensagemEtapaCompletada
        {
            get { return mensagemEtapaCompletada; }
            set
            {
                mensagemEtapaCompletada = value;
                RaisePropertyChangedEvent("MensagemEtapaCompletada");
            }
        }

        public void ArrastandoElemento(bool arrastando)
        {
            if (arrastando)
            {
                BackgroundArrastandoElemento = new SolidColorBrush(Color.FromArgb(20, 0, 0, 255));
            }
            else
            {
                BackgroundArrastandoElemento = Brushes.Transparent;
            }
        }

        #endregion

        public void AvancarParaEtapaDeResolucao()
        {
            MensagemEtapaCompletada = "Você montou corretamente a equação, agora vamos resolvê-la!";
            ProximaEtapaCommand.Execute(Cenario.Continuacao);
        }

        public void AvancarParaMontagemDeUmaNovaEquacao()
        {
            MensagemEtapaCompletada = "Parabéns, você resolveu a equação corretamente!";
            ProximaEtapaCommand.Execute(Cenario.ProximaEquacao);
        }
    }
}
