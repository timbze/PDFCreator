using System;
using pdfforge.PDFCreator.Core.StartupInterface;
using Pdftools.Pdf2Pdf;
using Pdftools.PdfSecure;
using PdfTools.Pdf;

namespace pdfforge.PDFCreator.Conversion.Processing.PdfToolsProcessing
{
    public interface IPdfToolsLicensing
    {
        ExitCode ExitCode { get; set; }
        bool Apply();
    }

    public class PdfToolsLicensing : IPdfToolsLicensing
    {
        protected string PdfToolboxKey { private get; set; }
        protected string PdfAConverterKey { private get; set; }
        protected string PdfSecureKey { private get; set; }

        protected PdfToolsLicensing(string pdfToolboxKey, string pdfAConverterKey, string pdfSecureKey)
        {
            PdfToolboxKey = pdfToolboxKey;
            PdfAConverterKey = pdfAConverterKey;
            PdfSecureKey = pdfSecureKey;
        }

        public PdfToolsLicensing(Func<string, string> decryptKeyFunc)
        {
            PdfToolboxKey = decryptKeyFunc(LicenseKeys.Encrypted_PdfToolboxKey);
            PdfAConverterKey = decryptKeyFunc(LicenseKeys.Encrypted_PdfAConverterKey);
            PdfSecureKey = decryptKeyFunc(LicenseKeys.Encrypted_PdfSecureKey);
        }

        public ExitCode ExitCode { get; set; } = ExitCode.Ok;

        public bool Apply()
        {
            SetLicenses();

            if (!Pdf2Pdf.LicenseIsValid)
            {
                ExitCode = ExitCode.InvalidPdfToolsPdf2PdfLicense;
                return false;
            }

            if (!Secure.LicenseIsValid)
            {
                ExitCode = ExitCode.InvalidPdfToolsSecureLicense;
                return false;
            }

            try
            {
                Document.CheckLicense();
            }
            catch
            {
                ExitCode = ExitCode.InvalidPdfToolsDocumentLicense;
                return false;
            }

            return true;
        }

        private void SetLicenses()
        {
            if (!string.IsNullOrWhiteSpace(PdfAConverterKey))
                Pdf2Pdf.SetLicenseKey(PdfAConverterKey);

            if (!string.IsNullOrWhiteSpace(PdfSecureKey))
                Secure.SetLicenseKey(PdfSecureKey);

            if (!string.IsNullOrWhiteSpace(PdfToolboxKey))
                Document.LicenseKey = PdfToolboxKey;
        }
    }
}
