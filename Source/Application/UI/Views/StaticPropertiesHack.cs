using pdfforge.LicenseValidator.Interface;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.StartupInterface;
using pdfforge.PDFCreator.UI.ViewModels.Converter;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.Translations;
using pdfforge.PDFCreator.UI.Views.UserControls;
using Translatable;

namespace pdfforge.PDFCreator.UI.Views
{
    public class StaticPropertiesHack : IStaticPropertiesHack
    {
        private readonly IUserGuideHelper _userGuideHelper;
        private readonly ILicenseChecker _licenseChecker;
        private readonly ITranslationFactory _translationFactory;

        public StaticPropertiesHack(IUserGuideHelper userGuideHelper, ILicenseChecker licenseChecker, ITranslationFactory translationFactory)
        {
            _userGuideHelper = userGuideHelper;
            _licenseChecker = licenseChecker;
            _translationFactory = translationFactory;
        }

        public void SetStaticProperties()
        {
            // THIS SHOULD USUALLY NOT BE DONE!
            TokenHintPanel.TranslationFactory = _translationFactory;
            TokenHintPanel.UserGuideHelper = _userGuideHelper;
            ErrorReportHelper.LicenseChecker = _licenseChecker;
            TranslatorConverter.Translation = _translationFactory.CreateTranslation<ApplicationTranslation>();
            _translationFactory.TranslationChanged += (sender, args) =>
            {
                TranslatorConverter.Translation = _translationFactory.CreateTranslation<ApplicationTranslation>();
            };
            // TODO find a better way using WPF resources
        }


    }
}