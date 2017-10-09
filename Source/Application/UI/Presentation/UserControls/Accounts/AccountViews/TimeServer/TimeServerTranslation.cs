using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews
{
    public class TimeServerTranslation : AccountsTranslation
    {
        private IPluralBuilder PluralBuilder { get; set; } = new DefaultPluralBuilder();

        public string EditTimeServerAccount { get; private set; } = "Edit Time Server Account";

        public string RemoveTimeServerAccount { get; private set; } = "Remove Time Server Account";

        private string[] TimeServerGetsDisabled { get; set; } = { "Signing will be disabled for this profile.", "Signing will be disabled for this profiles." };

        public string GetTimeServerGetsDisabledMessage(int numberOfProfiles)
        {
            return PluralBuilder.GetFormattedPlural(numberOfProfiles, TimeServerGetsDisabled);
        }

        public string UrlText { get; set; } = "URL:";
        public string IsSecured { get; private set; } = "Secured Time Server";
    }
}
