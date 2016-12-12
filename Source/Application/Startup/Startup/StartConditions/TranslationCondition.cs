using System.Linq;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.StartupInterface;

namespace pdfforge.PDFCreator.Core.Startup.StartConditions
{
    public class TranslationCondition : IStartupCondition
    {
        private readonly ILanguageProvider _languageProvider;
        private readonly ILanguageDetector _languageDetector;
        private readonly TranslationHelper _translationHelper;

        public TranslationCondition(ILanguageProvider languageProvider, ILanguageDetector languageDetector, TranslationHelper translationHelper)
        {
            _languageProvider = languageProvider;
            _languageDetector = languageDetector;
            _translationHelper = translationHelper;
        }

        public StartupConditionResult Check()
        {
            if (!_languageProvider.GetAvailableLanguages().Any())
                return StartupConditionResult.BuildErrorWithMessage((int)ExitCode.NoTranslations, @"Could not find any translation. Please reinstall PDFCreator.");

            var language = _languageDetector.FindDefaultLanguage() ?? "english";

            // Initialize translations for further checks
            _translationHelper.InitTranslator(language);

            return StartupConditionResult.BuildSuccess();
        }
    }
}
