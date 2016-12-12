using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.UI.DesignTimeViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.Assistants.Update;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ApplicationSettings;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.UserControls.ApplicationSettings
{
    public class DesignTimeGeneralTabViewModel : GeneralTabViewModel
    {
        public DesignTimeGeneralTabViewModel()
            : base(new DesignTimeLanguageProvider(), null, new TranslationProxy(),
                new UpdateAssistant(null, null, null, null, null, null, null, null), null, null, null, null)
        {
        }
    }
}