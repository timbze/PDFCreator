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
            _testPageHelper.CreateTestPage();
        }

        private void WindowsTestpageExecute(object o)
        {
            _printerHelper.PrintWindowsTestPage(SettingsProvider.Settings.ApplicationSettings.PrimaryPrinter);
        }
    }
}
