using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Jogo.Model.Enum;
using Jogo.Views;

namespace Jogo.Model
{
    public class CenarioParaVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2)
                throw new ArgumentException("O array de objetos está fora do padrão");

            var cenarioAtual = (Cenario)values[0];
            var view = (IViewBase)values[1];

            return cenarioAtual == view.Cenario ? Visibility.Visible : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
