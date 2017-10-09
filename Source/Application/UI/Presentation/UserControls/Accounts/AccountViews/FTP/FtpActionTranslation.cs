using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews
{
    public class FtpActionTranslation : AccountsTranslation
    {
        private IPluralBuilder PluralBuilder { get; set; } = new DefaultPluralBuilder();

        public string SelectFtpAccount { get; private set; } = "Please select a FTP account:";
        public string UploadWithFtp { get; private set; } = "Upload with FTP";
        public string RemoveFtpAccount { get; private set; } = "Remove FTP account";
        public string EditFtpAccount { get; private set; } = "Edit FTP Account";
        public string FtpUploadTitle { get; private set; } = "FTP Upload";
        public string FtpAccountColon { get; private set; } = "FTP Account:";
        public string FtpServerPassword { get; private set; } = "FTP Server Password:";

        private string[] FtpGetsDisabled { get; set; } = { "The FTP action will be disabled for this profile.", "The FTP action will be disabled for this profiles." };

        public string GetFtpGetsDisabledMessage(int numberOfProfiles)
        {
            return PluralBuilder.GetFormattedPlural(numberOfProfiles, FtpGetsDisabled);
        }
    }
}
