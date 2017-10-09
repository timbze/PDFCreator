using System.Security.Cryptography.X509Certificates;

namespace pdfforge.PDFCreator.Utilities
{
    public interface ISignaturePasswordCheck
    {
        bool IsValidPassword(string certificateFile, string certificatePassword);
    }

    public class SignaturePasswordCheck : ISignaturePasswordCheck
    {
        /// <summary>
        ///     This method returns true if a certificatePassword for a given certificate file is valid.
        /// </summary>
        /// <param name="certificateFile">Name of p12 or pfx certificate file.</param>
        /// <param name="certificatePassword">A certificatePassword string.</param>
        /// <returns>
        ///     True if the certificatePassword is valid.
        /// </returns>
        public bool IsValidPassword(string certificateFile, string certificatePassword)
        {
            try
            {
                var cert = new X509Certificate2(certificateFile, certificatePassword);
                return true;
            }
            catch //(CryptographicException)
            {
                return false;
            }
        }
    }
}
