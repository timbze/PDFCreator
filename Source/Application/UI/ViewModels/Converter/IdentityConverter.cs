using System;
using System.Globalization;
using System.Windows.Data;

namespace pdfforge.PDFCreator.UI.ViewModels.Converter
{
    public class IdentityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}