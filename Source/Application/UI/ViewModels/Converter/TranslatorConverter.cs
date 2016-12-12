using System;
using System.Globalization;
using System.Windows.Data;
using pdfforge.DynamicTranslator;

namespace pdfforge.PDFCreator.UI.ViewModels.Converter
{
    public class TranslatorConverter : IValueConverter
    {
        public static ITranslator Translator { private get; set; }
        public string TranslationSection { get; set; }
        public string TranslationKey { get; set; }

        public object Convert(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            if (targetType != typeof(string))
                throw new InvalidOperationException("The target must be a string");

            var translation = Translator?.GetTranslation(TranslationSection, TranslationKey);

            return translation;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}