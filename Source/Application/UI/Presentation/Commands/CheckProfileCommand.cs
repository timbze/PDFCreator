using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using System;

namespace pdfforge.PDFCreator.UI.Presentation.Commands
{
    public class CheckProfileCommand : IWaitableCommand
    {
        private readonly IProfileChecker _profileChecker;
        private readonly ICurrentSettingsProvider _currentSettingsProvider;
        private readonly IInteractionRequest _interactionRequest;
        private readonly ErrorCodeInterpreter _errorCodeInterpreter;

        public CheckProfileCommand(IProfileChecker profileChecker, ICurrentSettingsProvider currentSettingsProvider, IInteractionRequest interactionRequest, ErrorCodeInterpreter errorCodeInterpreter)
        {
            _profileChecker = profileChecker;
            _currentSettingsProvider = currentSettingsProvider;
            _interactionRequest = interactionRequest;
            _errorCodeInterpreter = errorCodeInterpreter;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var profileCheckResult = _profileChecker.ProfileCheckDict(_currentSettingsProvider.Profiles, _currentSettingsProvider.Settings.ApplicationSettings.Accounts);

            if (!profileCheckResult)
            {
                var interaction = new ProfileProblemsInteraction(profileCheckResult);
                _interactionRequest.Raise(interaction, MessageInteractionCompleted);
            }
            else
            {
                IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Success));
            }
        }

        private void MessageInteractionCompleted(ProfileProblemsInteraction messageInteraction)
        {
            var responseStatus = messageInteraction.IgnoreProblems
                ? ResponseStatus.Success
                : ResponseStatus.Cancel;

            IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(responseStatus));
        }

        public event EventHandler CanExecuteChanged;

        public event EventHandler<MacroCommandIsDoneEventArgs> IsDone;
    }
}
