using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Printer;
using pdfforge.PDFCreator.UI.Presentation.Wrapper;
using System.Linq;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimePrinterViewModel : PrinterViewModel
    {
        private static readonly ISettingsProvider SettingsProvider = new DesignTimePrintJobViewModel.DesignTimeSettingsProvider();

        public DesignTimePrinterViewModel() : base(new DesignTimePrinterProvider(), null, null, null, new DesignTimeTranslationUpdater(), new DesignTimePrinterHelper(), SettingsProvider, new GpoSettingsDefaults())
        {
            var settings = SettingsProvider.Settings;

            PrinterMappings.Add(new PrinterMappingWrapper(new PrinterMapping("PDFCreator", ""), settings.ConversionProfiles));
            PrinterMappings.Add(new PrinterMappingWrapper(new PrinterMapping("PDFCreator2", ""), settings.ConversionProfiles));
            PrimaryPrinter = PdfCreatorPrinters.First();
        }
    }
}
