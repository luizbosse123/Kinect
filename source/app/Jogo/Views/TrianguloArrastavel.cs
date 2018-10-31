using KinectMath.Core.TermosEquacao;
using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Jogo.Views
{
    public class TrianguloArrastavel : FrameworkElement
    {
        private BitmapImage imagemTriangulo;
        //private FormattedText textoFormatado;

        private int valor;
        private TipoTermo tipoTermo;
        private string imagem = "triangulo.png";

        public int Valor
        {
            get { return valor; }
            set
            {
                valor = value;
               // AtualizarTexto();
            }
        }

        public TipoTermo TipoTermo
        {
            get { return tipoTermo; }
            set
            {
                tipoTermo = value;
             //   AtualizarTexto();
            }
        }

        public TrianguloArrastavel()
        {
            Resetar();
           // AtualizarTexto();
            CarregarImagem();

            Height = imagemTriangulo.Height;
            Width = imagemTriangulo.Width;
        }

        //private BitmapSource fontePeso;
        //public BitmapSource FontePeso
        /*{
            get { return fontePeso; }
            set
            {
                fontePeso = value;
                imagemTriangulo = CarregarImagem(value);
                
                InvalidateVisual();
            }
        }*/

        private BitmapImage CarregarImagem(BitmapSource source)
        {
            var encoder = new PngBitmapEncoder();
            var memoryStream = new MemoryStream();
            var imagem = new BitmapImage();

            encoder.Frames.Add(BitmapFrame.Create(source));
            encoder.Save(memoryStream);

            imagem.BeginInit();
            imagem.StreamSource = new MemoryStream(memoryStream.ToArray());
            imagem.EndInit();

            return imagem;
        }

       /* private void AtualizarTexto()
        {
            textoFormatado = new FormattedText(Texto, new CultureInfo("pt-BR"), FlowDirection.LeftToRight, new Typeface("Arial"), 48, Brushes.Black);
            InvalidateVisual();
        }

        public string Texto
        {
            get { return string.Format("{0}{1}", valor, Sufixo()); }
        }*/

        private string Sufixo()
        {
            return (TipoTermo == TipoTermo.Incognita)
                ? "x"
                : string.Empty;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

           if (valor > 0 && imagem != "triangulo.png")
            {
                imagem = "triangulo.png";
                CarregarImagem();
            }

            const int margem = 10;

            var tamanhoImagem = new Rect(new Size(imagemTriangulo.PixelWidth + margem, imagemTriangulo.PixelHeight + margem));
            drawingContext.DrawImage(imagemTriangulo, tamanhoImagem);
          //  var posicaoTextoCentralizado = PosicaoTextoCentralizado(tamanhoImagem);
         //   drawingContext.DrawText(textoFormatado, posicaoTextoCentralizado);
        }
        /*
        private Point PosicaoTextoCentralizado(Rect tamanhoImagem)
        {
            return new Point(tamanhoImagem.Width * .9 / 2 - textoFormatado.Width / 2,
                tamanhoImagem.Height * 1.1 / 2 - textoFormatado.Height / 2);
        }*/

        public void Resetar()
        {
            tipoTermo = TipoTermo.Unidade;
            valor = 1;
        }

        private void CarregarImagem()
        {
            imagemTriangulo = new BitmapImage();
            imagemTriangulo.BeginInit();
            imagemTriangulo.UriSource = new Uri(string.Concat(@"pack://application:,,,/Jogo;component/Resources/Images/", imagem), UriKind.RelativeOrAbsolute);
            imagemTriangulo.EndInit();
        }
    }
}