using System.Collections.Generic;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.UI.Presentation.Wrapper;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.General
{
    public class DefaultPrinterSettingsViewModel : AGeneralSettingsItemControlModel
    {
        public DefaultPrinterSettingsViewModel(ITranslationUpdater translationUpdater, ICurrentSettingsProvider settingsProvider, IGpoSettings gpoSettings) : base(translationUpdater, settingsProvider, gpoSettings)
        {
        }

        public IEnumerable<AskSwitchPrinter> AskSwitchPrinterValues
        {
            get
            {
                return new List<AskSwitchPrinter>
                {
                    new AskSwitchPrinter(Translation.Ask, true),
                    new AskSwitchPrinter(Translation.Yes, false)
                };
            }
        }
    }
}
