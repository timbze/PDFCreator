using pdfforge.DynamicTranslator;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Licensing;
using pdfforge.PDFCreator.Core.StartupInterface;
using pdfforge.PDFCreator.UI.ViewModels.Converter;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.UI.Views.UserControls;

namespace pdfforge.PDFCreator.UI.Views
{
    public class StaticPropertiesHack : IStaticPropertiesHack
    {
        private readonly ITranslator _translator;
        private readonly IUserGuideHelper _userGuideHelper;
        private readonly IActivationHelper _activationHelper;

        public StaticPropertiesHack(ITranslator translator, IUserGuideHelper userGuideHelper, IActivationHelper activationHelper)
        {
            _translator = translator;
            _userGuideHelper = userGuideHelper;
            _activationHelper = activationHelper;
        }

        public void SetStaticProperties()
        {
            // THIS SHOULD USUALLY NOT BE DONE!

            TranslatorConverter.Translator = _translator;
            TokenHintPanel.UserGuideHelper = _userGuideHelper;
            ErrorReportHelper.ActivationHelper = _activationHelper;

            // TODO find a better way using WPF resources
        }
    }
}