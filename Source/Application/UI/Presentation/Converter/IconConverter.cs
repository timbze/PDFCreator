using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.TabHelper;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace pdfforge.PDFCreator.UI.Presentation.Converter
{
    [ValueConversion(typeof(IconList), typeof(FrameworkElement))]
    public class IconConverter : IValueConverter
    {
        public IconConverter()
        {
            Icons = new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/PDFCreator.Presentation;component/Styles/PdfCreatorIcons.xaml",
                    UriKind.RelativeOrAbsolute)
            };
        }

        public ResourceDictionary Icons { get; set; }

        //TODO size via parameter?
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is IconList))
                return null;

            var icon = (IconList)value;

            var frameworkElement = GetIconForEnumVal(icon);

            frameworkElement.Width = 28;
            frameworkElement.Height = 28;

            return frameworkElement;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        private FrameworkElement GetIconForEnumVal(IconList item)
        {
            return Icons[item.ToString()] as FrameworkElement;
        }
    }
}
