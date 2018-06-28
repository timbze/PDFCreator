using System.Windows.Input;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DebugSettings
{
    public class RestoreSettingsViewModel : ADebugSettingsItemControlModel
    {
        private readonly IInteractionRequest _request;

        public RestoreSettingsViewModel(IInteractionRequest request, ISettingsManager settingsManager, ITranslationUpdater translationUpdater, ICurrentSettingsProvider settingsProvider, IGpoSettings gpoSettings ) :
            base(settingsManager, translationUpdater, settingsProvider, gpoSettings)
        {
            _request = request;
            RestoreDefaultSettingsCommand = new DelegateCommand(RestoreDefaultSettingsExecute);
        }

        public ICommand RestoreDefaultSettingsCommand { get; }

        private void RestoreDefaultSettingsExecute(object obj)
        {
            var title = Translation.RestoreDefaultSettingsTitle;
            var message = Translation.RestoreDefaultSettingsMessage;
            var messageInteraction = new MessageInteraction(message, title, MessageOptions.YesNo, MessageIcon.Question);
            _request.Raise(messageInteraction, interaction =>
            {
                if (messageInteraction.Response == MessageResponse.Yes)
                {
                    var profileBuilder = new DefaultSettingsBuilder();
                    var defaultSettings = profileBuilder.CreateDefaultSettings(SettingsProvider.Settings);
                    ApplySettingsProcedure(defaultSettings);
                }
            } );
        }
    }
}
