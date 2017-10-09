using pdfforge.PDFCreator.Conversion.Settings.Enums;
using System;

namespace pdfforge.PDFCreator.Core.SettingsManagement
{
    public static class UpdateIntervalHelper
    {
        public static UpdateInterval ParseUpdateInterval(string updateInterval)
        {
            return (UpdateInterval)Enum.Parse(typeof(UpdateInterval), updateInterval, true);
        }
    }
}
