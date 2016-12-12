using System.Linq;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.UI.DesignTimeViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ApplicationSettings;
using pdfforge.PDFCreator.UI.ViewModels.Wrapper;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.UserControls.ApplicationSettings
{
    public class DesignTimePrinterTabViewModel : PrinterTabViewModel
    {
        public DesignTimePrinterTabViewModel() : base(new DesignTimePrinterProvider(), new TranslationProxy(), null, null, null, null)
        {
            var settings = new PdfCreatorSettings(null);
            settings.ConversionProfiles.Add(new ConversionProfile());

            SetSettingsAndRaiseNotifications(settings, new GpoSettingsDefaults());

            PrinterMappings.Add(new PrinterMappingWrapper(new PrinterMapping("PDFCreator", ""), settings.ConversionProfiles));
            PrinterMappings.Add(new PrinterMappingWrapper(new PrinterMapping("PDFCreator2", ""), settings.ConversionProfiles));
            PrimaryPrinter = PdfCreatorPrinters.First();
        }
    }
}