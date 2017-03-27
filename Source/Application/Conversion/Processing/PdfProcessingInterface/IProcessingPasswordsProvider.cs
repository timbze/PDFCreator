using pdfforge.PDFCreator.Conversion.Jobs.Jobs;

namespace pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface
{
    public interface IProcessingPasswordsProvider
    {
        void SetSignaturePassword(Job job);
        void SetEncryptionPasswords(Job job);
    }
}
