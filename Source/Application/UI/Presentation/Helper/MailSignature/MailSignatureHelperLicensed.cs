using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using System;

namespace pdfforge.PDFCreator.UI.Presentation.Helper
{
    public class MailSignatureHelperLicensed : MailSignatureHelperFreeVersion
    {
        public MailSignatureHelperLicensed(ITranslationUpdater translationUpdater) : base(translationUpdater)
        { }

        public override string ComposeMailSignature()
        {
            return Environment.NewLine
                   + "______________________________"
                   + Environment.NewLine
                   + Translation.MailSignatureLicensed
                   + Environment.NewLine
                   + "www.pdfforge.org";
        }
    }
}
