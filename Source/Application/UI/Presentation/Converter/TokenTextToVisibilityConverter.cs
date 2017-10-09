using pdfforge.PDFCreator.UI.Presentation.Helper;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace pdfforge.PDFCreator.UI.Presentation.Converter
{
    public class TokenTextToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2)
                throw new NotImplementedException("This converter needs at least two values");

            var stringvalue = values[0] as string;
            if (stringvalue == null)
                return DependencyProperty.UnsetValue;
            //throw new NotImplementedException($"This converter needs a string. current is:{values[0].GetType()}");

            var tokenHelper = values[1] as TokenHelper;
            if (tokenHelper == null)
                throw new NotImplementedException("This converter needs a TokenHelper");

            if (tokenHelper.ContainsInsecureTokens(stringvalue))
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("convert back not implemented");
        }
    }
}
