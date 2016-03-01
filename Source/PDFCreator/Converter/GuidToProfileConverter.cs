using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using pdfforge.PDFCreator.Core.Settings;
using pdfforge.PDFCreator.Helper;

namespace pdfforge.PDFCreator.Converter
{
    [ValueConversion(typeof(string), typeof(string))]
    internal class GuidToProfileConverter : IValueConverter
    {
        private readonly IList<ConversionProfile> _profiles;

        public GuidToProfileConverter()
        {
            _profiles = SettingsHelper.Settings.ConversionProfiles;
        }

        public GuidToProfileConverter(IList<ConversionProfile> profiles)
        {
            _profiles = profiles;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (var conversionProfile in _profiles)
            {
                if (conversionProfile.Guid.Equals(value))
                    return conversionProfile.Name;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
