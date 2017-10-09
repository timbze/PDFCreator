using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System;

namespace pdfforge.PDFCreator.UI.Presentation.Commands
{
    internal class SaveAndContinueEvaluationCommand : TranslatableCommandBase<ContiueAndSaveEvaluationTranslation>, IWaitableCommand
    {
        private readonly IInteractionRequest _interactionRequest;
        private readonly ICurrentSettingsProvider _settingsProvider;
        private readonly ISettingsProvider _mainSettingsProvider;

        public SaveAndContinueEvaluationCommand(IInteractionRequest interactionRequest, ICurrentSettingsProvider settingsProvider, ISettingsProvider mainSettingsProvider, ITranslationUpdater translationUpdater)
            : base(translationUpdater)
        {
            _interactionRequest = interactionRequest;
            _settingsProvider = settingsProvider;
            _mainSettingsProvider = mainSettingsProvider;
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            if (!_settingsProvider.Settings.Equals(_mainSettingsProvider.Settings))
            {
                var interaction = new ValidateContinueWithSavingInteraction(Translation.Description, Translation.Title);
                _interactionRequest.Raise(interaction, ResolveInteractionResult);
            }
            else
            {
                IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Success));
            }
        }

        private void ResolveInteractionResult(ValidateContinueWithSavingInteraction interactionResult)
        {
            var responseStatus = interactionResult.Response == MessageResponse.Yes ? ResponseStatus.Success : ResponseStatus.Cancel;
            IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(responseStatus));
        }

        public event EventHandler<MacroCommandIsDoneEventArgs> IsDone;
    }
}
