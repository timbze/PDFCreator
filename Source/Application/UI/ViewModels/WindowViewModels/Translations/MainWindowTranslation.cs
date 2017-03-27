using Translatable;

namespace pdfforge.PDFCreator.UI.ViewModels.WindowViewModels.Translations
{
    public class MainWindowTranslation : ITranslatable
    {
        public string AppSettingsButtonText { get; private set; } = "Application Settings";
        public string IntroductionGetHelpText { get; private set; } = "To get help, hit F1 or the question mark.";
        public string IntroductionStep1Text { get; private set; } = "1. Set up your profiles and settings here";
        public string IntroductionStep2Text { get; private set; } = "2. Print any document to the printer 'PDFCreator'.";
        public string IntroductionStep3Text { get; private set; } = "3. Save your PDF or send it as mail.";
        public string IntroductionText { get; private set; } = "PDFCreator allows to create PDF documents from any application that is able to print. Creating a PDF document is easy:";
        public string ProfileSettingsButtonText { get; private set; } = "Profile Settings";
    }
}
