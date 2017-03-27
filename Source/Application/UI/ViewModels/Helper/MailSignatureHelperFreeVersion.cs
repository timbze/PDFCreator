using System;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.UI.ViewModels.Translations;

namespace pdfforge.PDFCreator.UI.ViewModels.Helper
{
    public class MailSignatureHelperFreeVersion : IMailSignatureHelper
    {
        protected readonly MailSignatureHelperTranslation Translation;

        public MailSignatureHelperFreeVersion(MailSignatureHelperTranslation translation)
        {
            Translation = translation;
        }

        public virtual string ComposeMailSignature()
        { 
            return Environment.NewLine
                + "______________________________"
                + Environment.NewLine
                + Translation.MailSignatureFreeVersion
                + Environment.NewLine
                + "www.pdfforge.org";
        }
    }
}