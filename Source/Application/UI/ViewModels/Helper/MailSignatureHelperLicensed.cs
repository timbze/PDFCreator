using System;
using pdfforge.PDFCreator.UI.ViewModels.Translations;

namespace pdfforge.PDFCreator.UI.ViewModels.Helper
{
    public class MailSignatureHelperLicensed : MailSignatureHelperFreeVersion
    {
        public MailSignatureHelperLicensed(MailSignatureHelperTranslation translation) : base(translation)
        {   }

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