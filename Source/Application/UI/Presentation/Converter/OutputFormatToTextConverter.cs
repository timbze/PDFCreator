using pdfforge.PDFCreator.Conversion.Settings.Enums;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace pdfforge.PDFCreator.UI.Presentation.Converter
{
    public class OutputFormatToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var outputFormat = value as OutputFormat?;
            if (outputFormat == null)
                return DependencyProperty.UnsetValue;

            var type = typeof(OutputFormat);
            var memInfo = type.GetMember(outputFormat.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
            var description = ((DescriptionAttribute)attributes[0]).Description;
            return description;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
