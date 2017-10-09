using NLog;
using pdfforge.PDFCreator.Conversion.Jobs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace pdfforge.PDFCreator.Conversion.Processing.PdfToolsProcessing
{
    public interface ICertificateManager : IDisposable
    {
        X509Certificate2 GetOrRegisterCertificate(string filename, string password);
    }

    public class CertificateManager : ICertificateManager
    {
        public StoreName StoreName { get; set; } = StoreName.My;
        public StoreLocation StoreLocation { get; set; } = StoreLocation.CurrentUser;

        private readonly ISet<X509Certificate2> _storedCertificates = new HashSet<X509Certificate2>();
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public X509Certificate2 GetOrRegisterCertificate(string filename, string password)
        {
            lock (this)
            {
                filename = Path.GetFullPath(filename);

                var certificate = CreateCertificate(filename, password);
                RegisterCertificate(certificate, filename);

                return certificate;
            }
        }

        private X509Certificate2 CreateCertificate(string pfxFile, string password)
        {
            try
            {
                return new X509Certificate2(pfxFile, password);
            }
            catch (CryptographicException)
            {
                _logger.Error("Canceled signing. The password for certificate '" + pfxFile + "' is wrong.");
                throw new ProcessingException("Canceled signing. The password for certificate '" + pfxFile + "' is wrong.", ErrorCode.Signature_WrongCertificatePassword);
            }
        }

        private void RegisterCertificate(X509Certificate2 certificate, string filename)
        {
            X509Store store = new X509Store(StoreName, StoreLocation);

            store.Open(OpenFlags.ReadWrite);

            var serialNumber = certificate.GetSerialNumberString() ?? "";

            var existingCerts = store.Certificates.Find(X509FindType.FindBySerialNumber, serialNumber, false);

            if (existingCerts.Count == 0)
            {
                store.Add(certificate);
                _storedCertificates.Add(certificate);
            }

            store.Close();
        }

        private void UnregisterCertificates(IEnumerable<X509Certificate2> certificates)
        {
            X509Store store = new X509Store(StoreName, StoreLocation);
            store.Open(OpenFlags.ReadWrite);

            foreach (var certificate in certificates)
            {
                store.Remove(certificate);
            }

            store.Close();
        }

        public void Dispose()
        {
            UnregisterCertificates(_storedCertificates);
        }
    }
}
