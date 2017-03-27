using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using Pdftools.Pdf;
using Pdftools.PdfSecure;

namespace pdfforge.PDFCreator.Conversion.Processing.PdfToolsProcessing
{
    internal class Encrypter
    {
        internal bool SaveEncryptedFileAs(Secure pdf, ConversionProfile profile, JobPasswords jobPasswords, string outputFilename)
        {
            var encryptionFilter = GetEncryptionFilter(profile);
            var keyLength = GetEncryptionStrength(profile);
            var permissions = GetPdfPermissions(profile);

            var userPassword = "";
            var ownerPassword = "";

            if (IsEncryptionEnabled(profile))
            {
                userPassword = profile.PdfSettings.Security.RequireUserPassword ? jobPasswords.PdfUserPassword : "";
                ownerPassword = jobPasswords.PdfOwnerPassword;
            }

            return pdf.SaveAs(outputFilename, userPassword, ownerPassword, permissions, keyLength, encryptionFilter, encryptionFilter);
        }

        private bool IsEncryptionEnabled(ConversionProfile profile)
        {
            if (!profile.PdfSettings.Security.Enabled)
                return false;

            //Disable encryption for PDF/A and PDF/X
            if (profile.OutputFormat != OutputFormat.Pdf)
                return false;

            return true;
        }

        private string GetEncryptionFilter(ConversionProfile profile)
        {
            switch (profile.PdfSettings.Security.EncryptionLevel)
            {
                case EncryptionLevel.Rc40Bit:
                case EncryptionLevel.Rc128Bit:
                    return "RC4";
                case EncryptionLevel.Aes128Bit:
                    return "AESV2";
                case EncryptionLevel.Aes256Bit:
                    return "AESV3";
                default:
                    return "None";
            }
        }

        private int GetEncryptionStrength(ConversionProfile profile)
        {
            if (!IsEncryptionEnabled(profile))
                return 0;

            switch (profile.PdfSettings.Security.EncryptionLevel)
            {
                case EncryptionLevel.Rc40Bit:
                    return 40;
                case EncryptionLevel.Rc128Bit:
                case EncryptionLevel.Aes128Bit:
                    return 128;
                case EncryptionLevel.Aes256Bit:
                    return 256;
                default:
                    return 0;
            }
        }

        private PDFPermission GetPdfPermissions(ConversionProfile profile)
        {
            if (!IsEncryptionEnabled(profile))
                return PDFPermission.ePermNoEncryption;

            var securitySettings = profile.PdfSettings.Security;

            PDFPermission permissionValue = (PDFPermission) (-3904);

            if (securitySettings.AllowPrinting)
                permissionValue = permissionValue | PDFPermission.ePermDigitalPrint ^ PDFPermission.ePermPrint;

            if (securitySettings.AllowToEditTheDocument)
                permissionValue = permissionValue | PDFPermission.ePermModify;

            if (securitySettings.AllowToCopyContent)
                permissionValue = permissionValue | PDFPermission.ePermCopy;

            if (securitySettings.AllowToEditComments)
                permissionValue = permissionValue | PDFPermission.ePermAnnotate;

            if ((securitySettings.EncryptionLevel == EncryptionLevel.Rc128Bit)
                || (securitySettings.EncryptionLevel == EncryptionLevel.Aes128Bit)
                || (securitySettings.EncryptionLevel == EncryptionLevel.Aes256Bit))
            {
                if (securitySettings.AllowPrinting)
                    if (securitySettings.RestrictPrintingToLowQuality)
                        permissionValue = permissionValue ^ PDFPermission.ePermDigitalPrint;
                //Remove higher bit of AllowPrinting
                if (securitySettings.AllowToFillForms)
                    permissionValue = permissionValue | PDFPermission.ePermFillForms; //Set automatically for 40Bit
                if (securitySettings.AllowScreenReader)
                    permissionValue = permissionValue | PDFPermission.ePermSupportDisabilities;
                //Set automatically for 40Bit
                if (securitySettings.AllowToEditAssembly)
                    permissionValue = permissionValue | PDFPermission.ePermAssemble; //Set automatically for 40Bit
            }
            else
            {
                permissionValue =
                    permissionValue
                    | PDFPermission.ePermDigitalPrint
                    | PDFPermission.ePermFillForms
                    | PDFPermission.ePermSupportDisabilities
                    | PDFPermission.ePermAssemble;
            }
            return permissionValue;
        }
    }
}
