using pdfforge.Obsidian;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Interactions;

namespace pdfforge.PDFCreator.UI.Presentation.Windows.Startup
{
    public class ShowWelcomeWindowCommand : WelcomeCommand
    {
        private readonly IWelcomeSettingsHelper _welcomeSettingsHelper;
        private readonly IInteractionInvoker _interactionInvoker;

        public ShowWelcomeWindowCommand(IWelcomeSettingsHelper welcomeSettingsHelper, IInteractionInvoker interactionInvoker)
        {
            _welcomeSettingsHelper = welcomeSettingsHelper;
            _interactionInvoker = interactionInvoker;
        }

        protected override void ExecuteWelcomeAction()
        {
            if (_welcomeSettingsHelper.IsFirstRun())
            {
                _welcomeSettingsHelper.SetCurrentApplicationVersionAsWelcomeVersionInRegistry();
                _interactionInvoker.Invoke(new WelcomeInteraction());
            }
        }
    }
}
