using System;
using System.Globalization;
using System.Windows.Data;
using pdfforge.PDFCreator.UI.ViewModels.Translations;

namespace pdfforge.PDFCreator.UI.ViewModels.Converter
{
    public class TranslatorConverter : IValueConverter
    {
        public static ApplicationTranslation Translation { get; set; } = new ApplicationTranslation();
        public object Convert(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            return Translation.SetByAdministrator;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}