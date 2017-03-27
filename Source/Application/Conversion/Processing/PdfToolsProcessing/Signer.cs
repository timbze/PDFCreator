using System;
using System.Security.Cryptography.X509Certificates;
using NLog;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using Pdftools.Pdf;
using Pdftools.PdfSecure;
using Signature = pdfforge.PDFCreator.Conversion.Settings.Signature;

namespace pdfforge.PDFCreator.Conversion.Processing.PdfToolsProcessing
{
    internal class Signer
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        internal bool SignPdfFile(Secure secure, ConversionProfile profile, X509Certificate2 certificate)
        {
            if (!profile.PdfSettings.Signature.Enabled)
                return true;

            try
            {
                return DoSignPdfFile(secure, profile, certificate);
            }
            catch (ProcessingException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ProcessingException(ex.GetType() + " while signing:" + Environment.NewLine + ex.Message, ErrorCode.Signature_GenericError);
            }
        }

        public int GetSignaturePageNumber(Signature signatureSettings)
        {
            switch (signatureSettings.SignaturePage)
            {
                case SignaturePage.FirstPage:
                    return 1;
                case SignaturePage.LastPage:
                    return -1;
                case SignaturePage.CustomPage:
                    return signatureSettings.SignatureCustomPage;
            }

            return -1;
        }

        public PDFRect GetSignatureRect(Signature signatureSettings)
        {
            if ( !signatureSettings.DisplaySignatureInDocument)
                return new PDFRect(0, 0, 0, 0);

            return new PDFRect(signatureSettings.LeftX, signatureSettings.LeftY, signatureSettings.RightX, signatureSettings.RightY);
        }

        private bool DoSignPdfFile(Secure pdf, ConversionProfile profile, X509Certificate2 certificate)
        {
            var signatureSettings = profile.PdfSettings.Signature;

            if (!certificate.HasPrivateKey)
            {
                _logger.Error("Canceled signing. The certificate '" + signatureSettings.CertificateFile + "' has no private key.");
                throw new ProcessingException(
                    "Canceled signing. The certificate '" + signatureSettings.CertificateFile + "' has no private key.", ErrorCode.Signature_NoPrivateKey);
            }

            var signature = new Pdftools.PdfSecure.Signature();

            signature.Provider = ""; //"Microsoft Base Cryptographic Provider v1.0;123456";
            signature.Name = certificate.GetNameInfo(X509NameType.SimpleName, false); 
            signature.Store = "MY";
            signature.StoreLocation = 1; // 0 = Local Machine; 1 = Current User
            signature.SignerFingerprintStr = certificate.Thumbprint;

            signature.ContactInfo = signatureSettings.SignContact;
            signature.Location = signatureSettings.SignLocation;
            signature.Reason = signatureSettings.SignReason;

            signature.PageNo = GetSignaturePageNumber(signatureSettings);
            signature.Rect = GetSignatureRect(signatureSettings);

            var timeServerUri = new UriBuilder(signatureSettings.TimeServerUrl);

            if (signatureSettings.TimeServerIsSecured)
            {
                timeServerUri.UserName = Uri.EscapeDataString(signatureSettings.TimeServerLoginName);
                timeServerUri.Password = Uri.EscapeDataString(signatureSettings.TimeServerPassword);
            }

            signature.TimeStampURL = timeServerUri.ToString();

            signature.FillColor = 16777215; //White
            signature.StrokeColor = 13158600; //Grey

            if (!signatureSettings.AllowMultiSigning 
                && profile.OutputFormat != OutputFormat.PdfA1B)
                    return pdf.AddDocMDPSignature(signature, 1);

            return pdf.AddSignature(signature);
        }
    }
}