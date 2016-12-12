using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using pdfforge.DynamicTranslator;

namespace pdfforge.PDFCreator.UI.ViewModels.Converter
{
    public class TranslatedEnumConverter : DependencyObject, IValueConverter
    {
        public static DependencyProperty TranslatorProperty = DependencyProperty.Register("Translator", typeof(ITranslator), typeof(FrameworkElement));

        public ITranslator Translator
        {
            get { return (ITranslator) GetValue(TranslatorProperty); }
            set { SetValue(TranslatorProperty, value); }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var sourceType = value.GetType();
            if (!sourceType.IsEnum)
                throw new InvalidOperationException("The target must be an enum");

            if (Translator == null)
                return value.ToString();

            var translation = Translator.GetTranslation("Enums", sourceType.Name + "." + value);

            if (string.IsNullOrEmpty(translation))
                translation = value.ToString();

            return translation;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}