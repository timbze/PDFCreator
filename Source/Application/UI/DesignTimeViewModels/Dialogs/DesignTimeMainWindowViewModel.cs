using pdfforge.DataStorage.Storage;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.DesignTimeViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels.Translations;
using pdfforge.PDFCreator.Utilities;
using Translatable;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.Dialogs
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DesignTimeMainWindowViewModel : MainWindowViewModel
    {
        private static readonly TranslationHelper TranslationHelper = new TranslationHelper(new DefaultSettingsProvider(), new AssemblyHelper(), new TranslationFactory());

        public DesignTimeMainWindowViewModel() : base(new DesignTimeSettingsManager(), new DesignTimeInteractionInvoker(), new DesignTimeUserGuideHelper(), new VersionHelper(new AssemblyHelper()), new DesignTimeDragAndDropHandler(), new DisabledWelcomeWindowCommand(), new ApplicationNameProvider("PDFCreator"), new MainWindowTranslation(), new TranslationFactory())
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

        public void SaveSettingsInRegistry(PdfCreatorSettings settings)
        {   }
    }
}