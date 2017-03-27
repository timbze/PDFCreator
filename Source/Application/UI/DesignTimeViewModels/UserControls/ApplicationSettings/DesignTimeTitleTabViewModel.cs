using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ApplicationSettings;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ApplicationSettings.Translations;
using Translatable;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.UserControls.ApplicationSettings
{
    public class DesignTimeTitleTabViewModel : TitleTabViewModel
    {
        public DesignTimeTitleTabViewModel() : base(new TitleTabTranslation(), new TranslationFactory())
        {
        }
    }
}