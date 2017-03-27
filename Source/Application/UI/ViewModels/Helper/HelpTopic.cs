using pdfforge.PDFCreator.Utilities.UserGuide;

namespace pdfforge.PDFCreator.UI.ViewModels.Helper
{
    public enum HelpTopic
    {
        [HelpTopic("pdfcreator/index")] General,

        [HelpTopic("pdfcreator/introduction/whats-new")] WhatsNew,

        [HelpTopic("pdfcreator/using-pdfcreator/create-a-pdf")] CreatingPdf,

        [HelpTopic("pdfcreator/pdfcreator-settings/application-settings/index")] AppSettings,

        // Application settings
        [HelpTopic("pdfcreator/pdfcreator-settings/application-settings/general")] AppGeneral,

        [HelpTopic("pdfcreator/pdfcreator-settings/application-settings/printers")] AppPrinters,

        [HelpTopic("pdfcreator/pdfcreator-settings/application-settings/title")] AppTitle,

        [HelpTopic("pdfcreator/pdfcreator-settings/application-settings/debug")] AppDebug,

        [HelpTopic("pdfcreator/pdfcreator-settings/application-settings/license")] AppLicense,

        // Profile settings
        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/index")] ProfileSettings,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/document")] ProfileDocument,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/save")] ProfileSave,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/auto-save")] ProfileAutoSave,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/actions/index")] ProfileActions,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/actions/add-cover")] Cover,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/actions/add-background")] Background,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/actions/open-document")] OpenViewer,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/actions/add-attachment")] Attachment,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/actions/print-document")] PrintDocument,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/actions/dropbox")] Dropbox,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/actions/open-email-client")] OpenEmailClient,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/actions/send-email-over-smtp")] SendEmailOverSmtp,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/actions/run-script")] RunScript,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/actions/upload-with-ftp")] UploadWithFtp,
		
        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/actions/user-tokens")] UserTokens,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/image-formats")] ProfileImageFormats,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/pdf/index")] ProfilePdf,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/pdf/general")] PdfGeneral,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/pdf/compression")] PdfCompression,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/pdf/security")] PdfSecurity,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/pdf/signature")] PdfSignature,

        //Using-PdfCreator
        [HelpTopic("pdfcreator/using-pdfcreator/tokens")] Tokens,

        //Faq
        [HelpTopic("pdfcreator/frequently-asked-questions/pdf-tools")] PdfTools,

        //Server
        [HelpTopic("pdfcreator-server/index")] Server,

        [HelpTopic("pdfcreator-server/pdfcreator-server-settings/queue-settings")] ServerQueueSettings,

        [HelpTopic("pdfcreator-server/pdfcreator-server-settings/general-settings/index")] ServerGeneralSettings,

        [HelpTopic("pdfcreator-server/performance-test")] ServerPerformanceTest,

        [HelpTopic("pdfcreator-server/license/index")] ServerLicense,

        [HelpTopic("pdfcreator/license/index")] License
    }
}