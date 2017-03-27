using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using NLog;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;

namespace pdfforge.PDFCreator.Conversion.Processing.PdfToolsProcessing
{
    class CertificateRegistrar : IDisposable
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public X509Certificate2 Certificate { get; }

        public CertificateRegistrar(Signature signatureSettings , string password)
        {
            if (!signatureSettings.Enabled)
                Certificate = null;
            else if (string.IsNullOrEmpty(signatureSettings.CertificateFile))
                Certificate = null;
            else
                Certificate = RegisterCertificate(signatureSettings.CertificateFile, password);
        }

        public void Dispose()
        {
            if (Certificate != null)
                UnregisterCertificate(Certificate);
        }

        private X509Certificate2 RegisterCertificate(string pfxFile, string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                _logger.Error("Launched signing without certificate password.");
                throw new ProcessingException("Launched signing without certificate password.", ErrorCode.Signature_LaunchedSigningWithoutPassword);
            }

            try
            {
                var cert = new X509Certificate2(pfxFile, password);

                X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                store.Open(OpenFlags.ReadWrite);
                store.Add(cert);
                store.Close();

                return cert;
            }
            catch (CryptographicException)
            {
                _logger.Error("Canceled signing. The password for certificate '" + pfxFile + "' is wrong.");
                throw new ProcessingException("Canceled signing. The password for certificate '" + pfxFile + "' is wrong.", ErrorCode.Signature_WrongCertificatePassword);
            }
        }

        private void UnregisterCertificate(X509Certificate2 certificate)
        {
            X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadWrite);
            store.Remove(certificate);
            store.Close();
        }
    }
}
