using pdfforge.PDFCreator.Conversion.Settings.Enums;
using Translatable;

namespace pdfforge.PDFCreator.UI.ViewModels.Translations
{
    public class PdfTabTranslation : ITranslatable
    {
        //General Tab
        public string GeneralTabHeader { get; private set; } = "General";
        public string PdfVersionHeader { get; private set; } = "PDF Version";
        public string CurrentPdfVersionLabel { get; private set; } = "Current PDF version:";
        public string PdfVersionNoteTextBlock { get; private set; } = "Note: For best compatibility PDFCreator uses the lowest version that is possible with the current settings.";
        public string PageOrientationHeader { get; private set; } = "Page Orientation";
        public string ColorModelHeader { get; private set; } = "Color Model";
        public string ViewerSettingsHeader { get; private set; } = "Viewer Settings";
        public string PageViewLabel { get; private set; } = "Page view:";
        public string DocumentViewLabel { get; private set; } = "Document view:";
        public string ViewerStartsOnPageLabel { get; private set; } = "Viewer opens on page:";

        //Compression Tab
        public string CompressionTabHeader { get; private set; } = "Compression";
        public string ColorAndGrayscaleImagesHeader { get; private set; } = "Color and Grayscale Images";
        public string ColorAndGrayCompressionCheckBox { get; private set; } = "Enable compression";
        public string ColorAndGrayJpegFactor { get; private set; } = "Factor:";
        public string ColorAndGrayResampleCheckBox { get; private set; } = "Resample images to";
        public string ColorAndGrayDpi { get; private set; } = "DPI";

        public string MonochromeImagesHeader { get; private set; } = "Monochrome Images";
        public string MonochromeCompressionCheckBox { get; private set; } = "Enable compression";
        public string MonochromeResampleCheckBox { get; private set; } = "Resample images to";
        public string MonochromeDpi { get; private set; } = "DPI";

        //Security Tab
        public string PdfSecurityTabHeader { get; private set; } = "Security";
        public string SecurityCheckBox { get; private set; } = "Encrypt PDF documents to protect its contents";
        public string UserPasswordCheckBox { get; private set; } = "Require a password to open the PDF document";
        public string SecurityPasswordsButton { get; private set; } = "Set Passwords";

        public string EncryptionLevelHeader { get; private set; } = "Encryption Level";
        public string Rc128BitEncryptionText { get; private set; } = "Low (128 Bit)";
        public string Rc128BitEncryptionHint { get; private set; } = "- Unsecure. Use only for compatibility.";
        public string Aes128BitEncryptionText { get; private set; } = "Medium (128 Bit AES)";
        public string Aes256BitEncryptionText { get; private set; } = "High (256 Bit AES)";
        public string Aes256BitOnlyForPlusAndBusiness { get; private set; } = "- Only available in PDFCreator Plus and Business";
        public string MoreInfo { get; private set; } = "More info";

        public string AllowTheUser { get; private set; } = "Allow the user";
        public string CopyContentPermissionCheckBox { get; private set; } = "to copy content from the document";
        public string PrintDocumentPermissionCheckbox { get; private set; } = "to print the document";
        public string LowQualityPrintPermissionCheckBox { get; private set; } = "restrict to low quality";
        public string ScreenReaderPermissionCheckBox { get; private set; } = "to use a screen reader";
        public string EditDocumentPermissionCheckBox { get; private set; } = "to edit the document";
        public string EditCommentsPermissionCheckBox { get; private set; } = "to edit comments";
        public string FillFormsPermissionCheckBox { get; private set; } = "to fill forms";
        public string EditAssemblyPermissionCheckBox { get; private set; } = "to edit the assembly";

        //Signature Tab
        public string SignatureTabHeader { get; private set; } = "Signature";
        public string SignatureCheckBox { get; private set; } = "Sign PDF document with a digital signature";
        public string CertificateFileLabel { get; private set; } = "Certificate File:";
        public string SignaturePasswordButton { get; private set; } = "Set Password";
        public string ReasonLabel { get; private set; } = "Reason:";
        public string ContactLabel { get; private set; } = "Contact:";
        public string LocationLabel { get; private set; } = "Location:";
        public string AllowMultiSigningCheckBox { get; private set; } = "Allow multiple signing";
        public string TimeServerUrlLabel { get; private set; } = "Time Server URL:";
        public string DefaultTimeServerButton { get; private set; } = "Default Time Server";
        public string SecuredTimeserverCheckBox { get; private set; } = "Secured Time Server";
        public string LoginNameLabel { get; private set; } = "User Name:";
        public string TimeServerPasswordButton { get; private set; } = "Set Password";
        public string DisplaySignatureCheckBox { get; private set; } = "Display signature in document";
        public string SignaturePageLabel { get; private set; } = "Page:";
        public string LeftXLabel { get; private set; } = "Left X:";
        public string RightXLabel { get; private set; } = "Right X:";
        public string LeftYLabel { get; private set; } = "Left Y:";
        public string RightYLabel { get; private set; } = "Right Y:";
        public string PDFSignature { get; private set; } = "PDF Signature";
        public string SelectCertFile { get; private set; } = "Select certificate file";
        public string PfxP12Files { get; private set; } = "PFX/P12 files";
        public string AllFiles { get; private set; } = "All files";
        public string CertificateDoesNotExist { get; private set; } = "The certificate file does not exist.";
        public string TimeServerPasswordTitle { get; private set; } = "Password for Time Server";
        public string TimeServerPasswordDescription { get; private set; } = "Time Server Password:";

        public EnumTranslation<PageOrientation>[] PageOrientationValues { get; private set; } = EnumTranslation<PageOrientation>.CreateDefaultEnumTranslation();
        public EnumTranslation<ColorModel>[] ColorModelValues { get; private set; } = EnumTranslation<ColorModel>.CreateDefaultEnumTranslation();
        public EnumTranslation<PageView>[] PageViewValues { get; private set; } = EnumTranslation<PageView>.CreateDefaultEnumTranslation();
        public EnumTranslation<DocumentView>[] DocumentViewValues { get; private set; } = EnumTranslation<DocumentView>.CreateDefaultEnumTranslation();
        public EnumTranslation<CompressionColorAndGray>[] CompressionColorAndGrayValues { get; private set; } = EnumTranslation<CompressionColorAndGray>.CreateDefaultEnumTranslation();
        public EnumTranslation<CompressionMonochrome>[] CompressionMonochromeValues { get; private set; } = EnumTranslation<CompressionMonochrome>.CreateDefaultEnumTranslation();
        public EnumTranslation<SignaturePage>[] SignaturePageValues { get; private set; } = EnumTranslation<SignaturePage>.CreateDefaultEnumTranslation(); 
    }
}