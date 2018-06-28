using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System;

namespace pdfforge.PDFCreator.UI.Presentation.Commands
{
    // TODO: find better name or make behaviour more obvious
    public class EvaluateSettingsAndNotifyUserCommandExceptWhenSettingsChanged : EvaluateSettingsAndNotifyUserCommand
    {
        public EvaluateSettingsAndNotifyUserCommandExceptWhenSettingsChanged(IInteractionRequest interactionRequest, ICurrentSettingsProvider currentSettingsProvider, ITranslationUpdater translationUpdater, IRegionHelper regionHelper, IProfileChecker profileChecker, IAppSettingsChecker appSettingsChecker, ISettingsChanged settingsChanged) : base(interactionRequest, currentSettingsProvider, translationUpdater, regionHelper, profileChecker, appSettingsChecker, settingsChanged)
        {
            QueryOnProfileSettingsChanged = false;
        }
    }

    public class EvaluateSettingsAndNotifyUserCommand : TranslatableCommandBase<EvaluateSettingsAndNotifyUserTranslation>, IWaitableCommand
    {
        private readonly IInteractionRequest _interactionRequest;
        private readonly ICurrentSettingsProvider _currentSettingsProvider;
        private readonly IRegionHelper _regionHelper;
        private readonly IProfileChecker _profileChecker;
        private readonly IAppSettingsChecker _appSettingsChecker;
        private readonly ISettingsChanged _settingsChanged;

        protected bool QueryOnProfileSettingsChanged = true;

        public EvaluateSettingsAndNotifyUserCommand(IInteractionRequest interactionRequest,
            ICurrentSettingsProvider currentSettingsProvider,
            ITranslationUpdater translationUpdater, IRegionHelper regionHelper,
            IProfileChecker profileChecker, IAppSettingsChecker appSettingsChecker,
            ISettingsChanged settingsChanged)
            : base(translationUpdater)
        {
            _interactionRequest = interactionRequest;
            _currentSettingsProvider = currentSettingsProvider;
            _regionHelper = regionHelper;
            _profileChecker = profileChecker;
            _appSettingsChecker = appSettingsChecker;
            _settingsChanged = settingsChanged;
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object obj)
        {
            var currentRegion = _regionHelper.CurrentRegionName;
            MessageInteraction interaction = null;

            if (currentRegion == MainRegionViewNames.SettingsView)
            {
                var currentSettings = _currentSettingsProvider.Settings;
                var result = _appSettingsChecker.CheckDefaultViewers(currentSettings.ApplicationSettings);
                var resultDict = new ActionResultDict();
                resultDict.Add(Translation.DefaultViewer, result);
                interaction = DetermineInteraction(resultDict, settingsChanged: false);
            }
            else if (currentRegion == MainRegionViewNames.ProfilesView)
            {
                var currentSettings = _currentSettingsProvider.Settings;
                var resultDict = _profileChecker.CheckProfileList(currentSettings.ConversionProfiles, currentSettings.ApplicationSettings.Accounts);
                var settingsChanged = _settingsChanged.HaveChanged();
                interaction = DetermineInteraction(resultDict, settingsChanged);
            }

            if (interaction != null)
                _interactionRequest.Raise(interaction, ResolveInteractionResult);
            else
                IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Success));
        }

        private MessageInteraction DetermineInteraction(ActionResultDict actionResultDict, bool settingsChanged)
        {
            if (!actionResultDict)
            {
                var text = Translation.InvalidSettings;
                var title = Translation.PDFCreatorSettings;
                var userQuestion = settingsChanged ? Translation.SaveAnyway : Translation.ProceedAnyway;
                var buttons = settingsChanged ? MessageOptions.YesNoCancel : MessageOptions.YesCancel;
                return new MessageInteraction(text, title, buttons, MessageIcon.Warning, actionResultDict, userQuestion);
            }
            if (settingsChanged && QueryOnProfileSettingsChanged)
            {
                var text = Translation.UnsavedChanges;
                var title = Translation.PDFCreatorSettings;
                return new MessageInteraction(text, title, MessageOptions.YesNoCancel, MessageIcon.Question);
            }

            return null;
        }

        private void ResolveInteractionResult(MessageInteraction interactionResult)
        {
            var macroResult = ResponseStatus.Success;
            switch (interactionResult.Response)
            {
                case MessageResponse.Yes:
                    macroResult = ResponseStatus.Success;
                    break;

                case MessageResponse.No:
                    _currentSettingsProvider.Reset();
                    macroResult = ResponseStatus.Skip;
                    break;

                case MessageResponse.Cancel:
                    macroResult = ResponseStatus.Cancel;
                    break;
            }
            IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(macroResult));
        }

        public event EventHandler<MacroCommandIsDoneEventArgs> IsDone;
    }
}
