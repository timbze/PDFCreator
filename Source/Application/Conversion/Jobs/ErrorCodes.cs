using Translatable;

// ReSharper disable InconsistentNaming

namespace pdfforge.PDFCreator.Conversion.Jobs
{
    [Translatable]
    public enum ErrorCode
    {
        [Translation("PDF Architect could not open output file.")]
        Viewer_ArchitectCouldNotOpenOutput = 10100,

        [Translation("System could not open output file.")]
        Viewer_CouldNotOpenOutputFile = 10101,

        [Translation("No compatible e-mail client installed.")]
        MailClient_NoCompatibleEmailClientInstalled = 11101,

        [Translation("Unknown error in e-mail client action.")]
        MailClient_GenericError = 11999,

        [Translation("No certification file is specified.")]
        ProfileCheck_NoCertificationFileSpecified = 12100,

        [Translation("Certificate file does not exist.")]
        ProfileCheck_CertificateFileDoesNotExist = 12101,

        [Translation("Automatic saving without certificate password.")]
        ProfileCheck_AutoSaveWithoutCertificatePassword = 12102,

        [Translation("Secured Time Server without user name.")]
        ProfileCheck_SecureTimeServerWithoutUsername = 12103,

        [Translation("Secured Time Server without password.")]
        ProfileCheck_SecureTimeServerWithoutPassword = 12104,

        [Translation("The certificate password is wrong.")]
        Signature_WrongCertificatePassword = 12200,

        [Translation("The certificate has no private key.")]
        Signature_NoPrivateKey = 12201,

        [Translation("Not enough space for signature.")]
        Signature_NotEnoughSpaceForSignature = 12202,

        [Translation("Not enough space for signature.")]
        Signature_ProfileCheck_NotEnoughSpaceForSignature = 12203,

        [Translation("Launched signing without certification password.")]
        Signature_LaunchedSigningWithoutPassword = 12204,

        [Translation("Error while signing. Can not connect to time server.")]
        Signature_NoTimeServerConnection = 12205,

        [Translation("Error while signing. The certificate is invalid or has expired.")]
        Signature_Invalid = 12206,

        [Translation("The specified time server account for signing is not configured.")]
        Signature_NoTimeServerAccount = 12207,

        [Translation("Error while signing the document.")]
        Signature_GenericError = 12999,

        [Translation("The default printer is invalid.")]
        Printing_InvalidDefaultPrinter = 13100,

        [Translation("The selected printer is invalid.")]
        Printing_InvalidSelectedPrinter = 13101,

        [Translation("Error while printing the file.")]
        Printing_GenericError = 13999,

        [Translation("No script file is specified.")]
        Script_NoScriptFileSpecified = 14100,

        [Translation("Script file does not exist.")]
        Script_FileDoesNotExist = 14101,

        [Translation("Script file contains illegal characters.")]
        Script_IllegalCharacters = 14102,

        [Translation("Error while running the script action.")]
        Script_GenericError = 14999,

        [Translation("No SMTP e-mail address is specified.")]
        Smtp_NoEmailAddress = 15100,

        [Translation("No SMTP e-mail recipients are specified.")]
        Smtp_NoRecipients = 15101,

        [Translation("No SMTP e-mail server is specified.")]
        Smtp_NoServerSpecified = 15102,

        [Translation("Invalid SMTP port.")]
        Smtp_InvalidPort = 15103,

        [Translation("No SMTP e-mail user name is specified.")]
        Smtp_NoUserSpecified = 15104,

        [Translation("The specified SMTP account is not configured.")]
        Smtp_NoAccount = 15105,

        [Translation("No password provided for E-mail over SMTP.")]
        Smtp_NoPasswordSpecified = 15111,

        [Translation("E-mail over SMTP could not be delivered to one or more recipients.")]
        Smtp_EmailNotDelivered = 15106,

        [Translation("Could not authorize to SMTP server.")]
        Smtp_AuthenticationDenied = 15107,

        [Translation("User cancelled retyping SMTP e-mail password.")]
        Smtp_UserCancelled = 15108,

        [Translation("Invalid SMTP e-mail recipients.")]
        Smtp_InvalidRecipients = 15109,

        [Translation("Error while sending e-mail over SMTP.")]
        Smtp_GenericError = 15999,

        [Translation("No background file is specified.")]
        Background_NoFileSpecified = 17100,

        [Translation("Background file does not exist.")]
        Background_FileDoesNotExist = 17101,

        [Translation("Background file is no PDF file.")]
        Background_NoPdf = 17102,

        [Translation("Could not open cover file.")]
        Cover_CouldNotOpenFile = 17200,

        [Translation("Could not open attachment file.")]
        Attachment_CouldNotOpenFile = 17201,

        [Translation("Error while adding background to the document.")]
        Background_GenericError = 17999,

        [Translation("FTP server is not specified.")]
        Ftp_NoServer = 18100,

        [Translation("FTP server user name is not specified.")]
        Ftp_NoUser = 18101,

        [Translation("FTP server password is not specified.")]
        Ftp_NoPassword = 18103,

        [Translation("Could not login to FTP server.")]
        Ftp_LoginError = 18104,

        [Translation("Failure in directory on FTP server.")]
        Ftp_DirectoryError = 18105,

        [Translation("Could not read from FTP directory to ensure unique filenames.")]
        Ftp_DirectoryReadError = 18106,

        [Translation("Could not upload file to FTP.")]
        Ftp_UploadError = 18107,

        [Translation("Could not login to FTP server. Please check your Internet connection.")]
        Ftp_ConnectionError = 18108,

        [Translation("Automatic saving without FTP server password.")]
        Ftp_AutoSaveWithoutPassword = 18109,

        [Translation("User cancelled retyping FTP password.")]
        Ftp_UserCancelled = 18110,

        [Translation("The specified FTP account is not configured.")]
        Ftp_NoAccount = 18111,

        [Translation("Error while uploading file to FTP server.")]
        Ftp_GenericError = 18999,

        [Translation("The specified Dropbox account is not configured.")]
        Dropbox_AccountNotSpecified = 19001,

        [Translation("The specified Dropbox access token is not configured.")]
        Dropbox_AccessTokenNotSpecified = 19002,

        [Translation("These characters are not allowed as Dropbox folder name: < > : ? * \" |")]
        Dropbox_InvalidFolderName = 19003,

        [Translation("Automatic saving without target directory.")]
        AutoSave_NoTargetDirectory = 21100,

        [Translation("Automatic saving without filename template.")]
        AutoSave_NoFilenameTemplate = 21101,

        [Translation("No cover file is specified.")]
        Cover_NoFileSpecified = 22100,

        [Translation("Cover file does not exist.")]
        Cover_FileDoesNotExist = 22101,

        [Translation("The cover file is no PDF file.")]
        Cover_NoPdf = 22102,

        [Translation("No attachment file is specified.")]
        Attachment_NoFileSpecified = 23100,

        [Translation("Attachment file does not exist.")]
        Attachment_FileDoesNotExist = 23101,

        [Translation("Attachment file is no PDF file.")]
        Attachment_NoPdf = 23102,

        [Translation("No stamp text is specified.")]
        Stamp_NoText = 24100,

        [Translation("No stamp font is specified.")]
        Stamp_NoFont = 24101,

        [Translation("Automatic saving without owner password.")]
        AutoSave_NoOwnerPassword = 25100,

        [Translation("Automatic saving without user password.")]
        AutoSave_NoUserPassword = 25101,

        [Translation("Error while encrypting the document.")]
        Encryption_Error = 25200,

        [Translation("Launched encryption without owner password.")]
        Encryption_NoOwnerPassword = 25201,

        [Translation("Launched encryption without user password.")]
        Encryption_NoUserPassword = 25202,

        [Translation("Error while encrypting the document.")]
        Encryption_GenericError = 25999,

        [Translation("Missing output file for PDF processing.")]
        Processing_OutputFileMissing = 26100,

        [Translation("Error while processing the document.")]
        Processing_GenericError = 26999,

        [Translation("Preselected folder for save dialog is empty.")]
        SaveDialog_NoPreselectedFolder = 28100,

        [Translation("Error while copying the output file.")]
        Conversion_ErrorWhileCopyingOutputFile = 28200,

        [Translation("File path too long")]
        Conversion_PathTooLong = 28201,

        [Translation("Internal Ghostscript error.")]
        Conversion_GhostscriptError = 29100,

        [Translation("Unknown internal error.")]
        Conversion_UnknownError = 29200,

        [Translation("Error during PDF/A conversion.")]
        Conversion_PdfAError = 30999,

        [Translation("Error while uploading the document to Dropbox.")]
        Dropbox_Upload_Error = 31201,

        [Translation("Error while uploading and sharing the document to Dropbox.")]
        Dropbox_Upload_And_Share_Error = 31202,

        [Translation("Missing URL for HTTP upload.")]
        HTTP_NoUrl = 32101,

        [Translation("Missing user name for HTTP authentication.")]
        HTTP_NoUserNameForAuth = 32102,

        [Translation("Automatic saving without password for HTTP authentication.")]
        HTTP_NoPasswordForAuthWithAutoSave = 32103,

        [Translation("The specified HTTP account is not configured.")]
        HTTP_NoAccount = 32104,

        [Translation("User cancelled retyping HTTP autentication password.")]
        HTTP_UserCancelled = 32105,

        [Translation("HTTP URL must start with 'http://' or 'https://'.")]
        HTTP_MustStartWithHttp = 32106,

        [Translation("Error while uploading to HTTP server. 401 interaction is not authorized.")]
        HTTP_UnAuthorized_Request_Error = 32107,

        [Translation("Error while uploading to HTTP server.")]
        HTTP_Generic_Error = 32999,

        [Translation("Error while trying to log-in.")]
        PasswordAction_Login_Error = 33001,
    }
}
