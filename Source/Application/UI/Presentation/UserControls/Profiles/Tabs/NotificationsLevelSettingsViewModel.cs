using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ServiceLocator;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Tabs
{
    public class NotificationsLevelSettingsViewModel : ProfileUserControlViewModel<NotificationsLevelSettingsTranslation>, IWhitelisted
    {
        public NotificationsLevelSettingsViewModel(ICurrentSettingsProvider settingsProvider,
        IGpoSettings gpoSettings, ITranslationUpdater translationUpdater, EditionHintOptionProvider editionHintOptionProvider, IDispatcher dispatcher) :
            base(translationUpdater, settingsProvider, dispatcher)
        {
            AllowNotifications = !editionHintOptionProvider.ShowOnlyForPlusAndBusinessHint;
        }

        public bool AllowNotifications { get; }
    }
}
