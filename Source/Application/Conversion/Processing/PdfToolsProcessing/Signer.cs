using NLog;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using Pdftools.Pdf;
using Pdftools.PdfSecure;
using System;
using System.Security.Cryptography.X509Certificates;
using Signature = pdfforge.PDFCreator.Conversion.Settings.Signature;

namespace pdfforge.PDFCreator.Conversion.Processing.PdfToolsProcessing
{
    internal class Signer
    {
        private readonly ICertificateManager _certificateManager;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public Signer(ICertificateManager certificateManager)
        {
            _certificateManager = certificateManager;
        }

        internal bool SignPdfFile(Secure secure, ConversionProfile profile, JobPasswords jobPasswords, Accounts accounts)
        {
            if (!profile.PdfSettings.Signature.Enabled)
                return true;

            if (string.IsNullOrEmpty(jobPasswords.PdfSignaturePassword))
            {
                _logger.Error("Launched signing without certificate password.");
                throw new ProcessingException("Launched signing without certificate password.", ErrorCode.Signature_LaunchedSigningWithoutPassword);
            }

            var certificate = _certificateManager.GetOrRegisterCertificate(profile.PdfSettings.Signature.CertificateFile, jobPasswords.PdfSignaturePassword);

            var timeServerAccount = accounts.GetTimeServerAccount(profile);
            if (timeServerAccount == null)
            {
                _logger.Error("Launched signing without available timeserver account.");
                throw new ProcessingException("Launched signing without available timeserver account.", ErrorCode.Signature_NoTimeServerAccount);
            }

            try
            {
                return DoSignPdfFile(secure, profile, certificate, timeServerAccount);
            }
            catch (ProcessingException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ProcessingException(ex.GetType() + " while signing:" + Environment.NewLine + ex.Message, ErrorCode.Signature_GenericError, ex);
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
            if (!signatureSettings.DisplaySignatureInDocument)
                return new PDFRect(0, 0, 0, 0);

            return new PDFRect(signatureSettings.LeftX, signatureSettings.LeftY, signatureSettings.RightX, signatureSettings.RightY);
        }

        private bool DoSignPdfFile(Secure pdf, ConversionProfile profile, X509Certificate2 certificate, TimeServerAccount timeServerAccount)
        {
            var signatureSettings = profile.PdfSettings.Signature;

            if (!certificate.HasPrivateKey)
            {
                _logger.Error("Canceled signing. The certificate '" + signatureSettings.CertificateFile + "' has no private key.");
                throw new ProcessingException(
                    "Canceled signing. The certificate '" + signatureSettings.CertificateFile + "' has no private key.", ErrorCode.Signature_NoPrivateKey);
            }

            using (var signature = new Pdftools.PdfSecure.Signature())
            {
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

                var timeServerUri = new UriBuilder(timeServerAccount.Url);

                if (timeServerAccount.IsSecured)
                {
                    timeServerUri.UserName = Uri.EscapeDataString(timeServerAccount.UserName);
                    timeServerUri.Password = Uri.EscapeDataString(timeServerAccount.Password);
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
}
