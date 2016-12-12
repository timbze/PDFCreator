using pdfforge.DataStorage.Storage;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.DesignTimeViewModels.Helper;
using pdfforge.PDFCreator.UI.DesignTimeViewModels.UserControls.ApplicationSettings;
using pdfforge.PDFCreator.UI.ViewModels;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.Dialogs
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DesignTimeMainWindowViewModel : MainWindowViewModel
    {
        private static readonly TranslationHelper TranslationHelper = new TranslationHelper(new TranslationProxy(), new DefaultSettingsProvider(), new AssemblyHelper());

        public DesignTimeMainWindowViewModel() : base(new DesignTimeSettingsManager(), new DesignTimeInteractionInvoker(), new DesignTimeUserGuideHelper(), new VersionHelper(new AssemblyHelper()), new DesignTimeDragAndDropHandler(), new DisabledWelcomeWindowCommand(), new ApplicationNameProvider("PDFCreator"))
        {
            TranslationHelper.InitTranslator("English");
        }
    }

    public class DesignTimeSettingsLoader : ISettingsLoader
    {
        public PdfCreatorSettings LoadPdfCreatorSettings()
        {
            return new PdfCreatorSettings(new IniStorage());
        }

        public void SaveSettings(PdfCreatorSettings settings)
        {
        }
    }
}