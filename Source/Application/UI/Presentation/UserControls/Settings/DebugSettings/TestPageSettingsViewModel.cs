using System.Windows.Input;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DebugSettings
{
    public class TestPageSettingsViewModel : ADebugSettingsItemControlModel
    {
        private readonly IInteractionInvoker _invoker;
        private readonly IPrinterHelper _printerHelper;
        private readonly ITestPageHelper _testPageHelper;

        public TestPageSettingsViewModel(ITestPageHelper testPageHelper, IPrinterHelper printerHelper, ISettingsManager settingsManager, ITranslationUpdater translationUpdater, IInteractionInvoker invoker, ICurrentSettingsProvider settingsProvider, IGpoSettings gpoSettings) :
            base(settingsManager, translationUpdater, settingsProvider, gpoSettings)
        {
            PrintPdfCreatorTestpageCommand = new DelegateCommand(PdfCreatorTestpageExecute);
            PrintWindowsTestpageCommand = new DelegateCommand(WindowsTestpageExecute);
            _printerHelper = printerHelper;
            _testPageHelper = testPageHelper;
            _invoker = invoker;
        }

        public ICommand PrintPdfCreatorTestpageCommand { get; }
        public ICommand PrintWindowsTestpageCommand { get; }

        private void PdfCreatorTestpageExecute(object o)
        {
            if (!QuerySaveModifiedSettings())
                return;

            _testPageHelper.CreateTestPage();
        }

        private void WindowsTestpageExecute(object o)
        {
            if (!QuerySaveModifiedSettings())
                return;

            _printerHelper.PrintWindowsTestPage(ApplicationSettings.PrimaryPrinter);
        }


        private bool AppSettingsAreModified()
        {
            return !ApplicationSettings.Equals(SettingsProvider.Settings.ApplicationSettings);
        }

        private void SaveAppSettings()
        {
            SettingsProvider.Settings.ApplicationSettings = ApplicationSettings.Copy();
            SettingsManager.ApplyAndSaveSettings(SettingsProvider.Settings); // call apply to trigger LanguageChanged event
        }
        
        private bool QuerySaveModifiedSettings()
        {
            if (!AppSettingsAreModified())
                return true; //No changes -> proceed

            var message = Translation.AskSaveModifiedSettings;
            var caption = Translation.AppSettings;

            var interaction = new MessageInteraction(message, caption, MessageOptions.YesNo, MessageIcon.Question);

            _invoker.Invoke(interaction);

            var response = interaction.Response;

            if (response == MessageResponse.Yes) //Proceed with saved settings
            {
                SaveAppSettings();
                return true;
            }
            if (response == MessageResponse.No) //Proceed with old settings
            {
                return true;
            }
            return false; //Cancel Testprinting
        }
    }
}
