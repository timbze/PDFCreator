using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.MailSmtp;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeEditEmailTextViewModel : EditEmailTextViewModel
    {
        public DesignTimeEditEmailTextViewModel() :
            base(new DesignTimeTranslationUpdater(), new MailSignatureHelperFreeVersion(new DesignTimeTranslationUpdater()), new TokenHelper(new DesignTimeTranslationUpdater()))
        {
        }
    }
}
