using pdfforge.PDFCreator.Utilities;
// ReSharper disable InconsistentNaming

namespace pdfforge.PDFCreator.Conversion.Jobs
{
    public enum ErrorCode
    {
        [StringValue("PDF Architect could not open output file.")]
        Viewer_ArchitectCouldNotOpenOutput = 10100,

        [StringValue("System could not open output file.")]
        Viewer_CouldNotOpenOutputFile = 10101,

        [StringValue("No compatible e-mail client installed.")]
        MailClient_NoCompatibleEmailClientInstalled = 11101,

        [StringValue("Unknown error in e-mail client action.")]
        MailClient_GenericError = 11999,

        [StringValue("No certification file is specified.")]
        ProfileCheck_NoCertificationFileSpecified = 12100,

        [StringValue("Certificate file does not exist.")]
        ProfileCheck_CertificateFileDoesNotExist = 12101,

        [StringValue("Automatic saving without certificate password.")]
        ProfileCheck_AutoSaveWithoutCertificatePassword = 12102,

        [StringValue("Secured Time Server without user name.")]
        ProfileCheck_SecureTimeServerWithoutUsername = 12103,

        [StringValue("Secured Time Server without password.")]
        ProfileCheck_SecureTimeServerWithoutPassword = 12104,

        [StringValue("The certificate password is wrong.")]
        Signature_WrongCertificatePassword = 12200,

        [StringValue("The certificate has no private key.")]
        Signature_NoPrivateKey = 12201,

        [StringValue("Not enough space for signature.")]
        Signature_NotEnoughSpaceForSignature = 12202,

        [StringValue("Not enough space for signature.")]
        Signature_ProfileCheck_NotEnoughSpaceForSignature = 12203,

        [StringValue("Launched signing without certification password.")]
        Signature_LaunchedSigningWithoutPassword = 12204,

        [StringValue("Error while signing. Can not connect to time server.")]
        Signature_NoTimeServerConnection = 12205,

        [StringValue("Error while signing the document.")]
        Signature_GenericError = 12999,

        [StringValue("The default printer is invalid.")]
        Printing_InvalidDefaultPrinter = 13100,

        [StringValue("The selected printer is invalid.")]
        Printing_InvalidSelectedPrinter = 13101,

        [StringValue("Error while printing the file.")]
        Printing_GenericError = 13999,

        [StringValue("No script file is specified.")]
        Script_NoScriptFileSpecified = 14100,

        [StringValue("Script file does not exist.")]
        Script_FileDoesNotExist = 14101,

        [StringValue("Script file contains illegal characters.")]
        Script_IllegalCharacters = 14102,

        [StringValue("Error while running the script action.")]
        Script_GenericError = 14999,

        [StringValue("No SMTP e-mail address is specified.")]
        Smtp_NoEmailAddress = 15100,

        [StringValue("No SMTP e-mail recipients are specified.")]
        Smtp_NoRecipients = 15101,

        [StringValue("No SMTP e-mail server is specified.")]
        Smtp_NoServerSpecified = 15102,
        
        [StringValue("Invalid SMTP e-mail port.")]
        Smtp_InvalidPort = 15103,
        
        [StringValue("No SMTP e-mail user name is specified.")]
        Smtp_NoUserSpecified = 15104,
        
        [StringValue("No password provided for E-mail over SMTP.")]
        Smtp_NoPasswordSpecified = 15111,
        
        [StringValue("E-mail over SMTP could not be delivered to one or more recipients.")]
        Smtp_EmailNotDelivered = 15106,
        
        [StringValue("Could not authorize to SMTP server.")]
        Smtp_AuthenticationDenied = 15107,
        
        [StringValue("User cancelled retyping SMTP e-mail password.")]
        Smtp_UserCancelled = 15108,
               
        [StringValue("Error while sending e-mail over SMTP.")]
        Smtp_GenericError = 15999,
        
        [StringValue("No background file is specified.")]
        Background_NoFileSpecified = 17100,

        [StringValue("Background file does not exist.")]
        Background_FileDoesNotExist = 17101,

        [StringValue("Background file is no PDF file.")]
        Background_NoPdf = 17102,

        [StringValue("Could not open cover file.")]
        Cover_CouldNotOpenFile = 17200,

        [StringValue("Could not open attachment file.")]
        Attachment_CouldNotOpenFile = 17201,

        [StringValue("Error while adding background to the document.")]
        Background_GenericError = 17999,

        [StringValue("FTP server is not specified.")]
        Ftp_NoServer = 18100,

        [StringValue("FTP server user name is not specified.")]
        Ftp_NoUser = 18101,

        [StringValue("FTP server password is not specified.")]
        Ftp_NoPassword = 18103,

        [StringValue("Could not login to FTP server.")]
        Ftp_LoginError = 18104,

        [StringValue("Failure in directory on FTP server.")]
        Ftp_DirectoryError = 18105,

        [StringValue("Could not read from FTP directory to ensure unique filenames.")]
        Ftp_DirectoryReadError = 18106,

        [StringValue("Could not upload file to FTP.")]
        Ftp_UploadError = 18107,

        [StringValue("Could not login to FTP server. Please check your Internet connection.")]
        Ftp_ConnectionError = 18108,

        [StringValue("Automatic saving without FTP server password.")]
        Ftp_AutoSaveWithoutPassword = 18109,

        [StringValue("User cancelled retyping FTP password.")]
        Ftp_UserCancelled = 18110,

        [StringValue("Error while uploading file to FTP server.")]
        Ftp_GenericError = 18999,

        [StringValue("The specified Dropbox account is not configured.")]
        Dropbox_AccountNotSpecified = 19001,

        [StringValue("The specified Dropbox access token is not configured.")]
        Dropbox_AccessTokenNotSpecified = 19002,

        [StringValue("These characters are not allowed as Dropbox folder name: < > \\ : ? * \" |")]
        Dropbox_InvalidFolderName = 19003,

        [StringValue("Automatic saving without target directory.")]
        AutoSave_NoTargetDirectory = 21100,

        [StringValue("Automatic saving without filename template.")]
        AutoSave_NoFilenameTemplate = 21101,

        [StringValue("No cover file is specified.")]
        Cover_NoFileSpecified = 22100,

        [StringValue("Cover file does not exist.")]
        Cover_FileDoesNotExist = 22101,

        [StringValue("The cover file is no PDF file.")]
        Cover_NoPdf = 22102,

        [StringValue("No attachment file is specified.")]
        Attachment_NoFileSpecified = 23100,

        [StringValue("Attachment file does not exist.")]
        Attachment_FileDoesNotExist = 23101,

        [StringValue("Attachment file is no PDF file.")]
        Attachment_NoPdf = 23102,

        [StringValue("No stamp text is specified.")]
        Stamp_NoText = 24100,

        [StringValue("No stamp font is specified.")]
        Stamp_NoFont = 24101,

        [StringValue("Automatic saving without owner password.")]
        AutoSave_NoOwnerPassword = 25100,

        [StringValue("Automatic saving without user password.")]
        AutoSave_NoUserPassword = 25101,

        [StringValue("Error while encrypting the document.")]
        Encryption_Error = 25200,

        [StringValue("Launched encryption without owner password.")]
        Encryption_NoOwnerPassword = 25201,

        [StringValue("Launched encryption without user password.")]
        Encryption_NoUserPassword = 25202,

        [StringValue("Error while encrypting the document.")]
        Encryption_GenericError = 25999,

        [StringValue("Missing output file for PDF processing.")]
        Processing_OutputFileMissing = 26100,

        [StringValue("Error while processing the document.")]
        Processing_GenericError = 26999,

        [StringValue("Preselected folder for save dialog is empty.")]
        SaveDialog_NoPreselectedFolder = 28100,

        [StringValue("Error while copying the output file.")]
        Conversion_ErrorWhileCopyingOutputFile = 28200,

        [StringValue("Internal Ghostscript error.")]
        Conversion_GhostscriptError = 29100,

        [StringValue("Unknown internal error.")]
        Conversion_UnknownError = 29200,
    }
}
