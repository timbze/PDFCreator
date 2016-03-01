using System;
using System.Globalization;
using System.Windows.Data;

namespace pdfforge.PDFCreator.Converter
{
    internal class EnumToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string parameterString = parameter.ToString();   
            return Enum.Parse(targetType, parameterString);
        }
    }
}
