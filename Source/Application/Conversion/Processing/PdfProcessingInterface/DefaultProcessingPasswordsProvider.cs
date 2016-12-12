using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;

namespace pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface
{
    public class DefaultProcessingPasswordsProvider : IProcessingPasswordsProvider
    {
        public void SetSignaturePassword(Job job)
        {
            job.Passwords.PdfSignaturePassword = job.Profile.PdfSettings.Signature.SignaturePassword;
        }

        public void SetEncryptionPasswords(Job job)
        {
            job.Passwords.PdfOwnerPassword = job.Profile.PdfSettings.Security.OwnerPassword;
            job.Passwords.PdfUserPassword = job.Profile.PdfSettings.Security.UserPassword;
        }
    }
}
