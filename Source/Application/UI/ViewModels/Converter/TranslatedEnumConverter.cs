using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Translatable;

namespace pdfforge.PDFCreator.UI.ViewModels.Converter
{
    public class TranslatedEnumConverter : DependencyObject, IValueConverter
    {
        private TranslationFactory _translationFactory;

        public ITranslationFactory TranslationFactory
        {
            get { return _translationFactory; }
            set
            {
                if (value is TranslationFactory)
                _translationFactory = (TranslationFactory) value;
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var sourceType = value.GetType();
            if (!sourceType.IsEnum)
                throw new InvalidOperationException("The target must be an enum");

            if (_translationFactory?.TranslationSource == null)
                return null;

            try
            {
                return _translationFactory.TranslationSource.GetTranslation(TranslationAttribute.GetValue(value));
            }
            catch
            {
                return value.ToString();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}