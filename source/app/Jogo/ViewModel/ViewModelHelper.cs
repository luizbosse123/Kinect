using System.Windows.Controls;

namespace Jogo.ViewModel
{
    public static class ViewModelHelper
    {
        public static JogoViewModel ObterViewModel(this UserControl elemento)
        {
            return (JogoViewModel)elemento.DataContext;
        }
    }
}