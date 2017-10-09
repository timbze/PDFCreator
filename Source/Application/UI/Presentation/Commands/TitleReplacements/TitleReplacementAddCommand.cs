using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.TitleReplacementSettings;
using System;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.Commands.TitleReplacements
{
    public class TitleReplacementAddCommand : ICommand
    {
        private readonly IInteractionRequest _interactionRequest;
        private readonly ICurrentSettingsProvider _settingsProvider;

        public TitleReplacementAddCommand(IInteractionRequest interactionRequest, ICurrentSettingsProvider settingsProvider)
        {
            _interactionRequest = interactionRequest;
            _settingsProvider = settingsProvider;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var titleReplacement = new TitleReplacement();
            var editTitleReplacementInteraction = new TitleReplacementEditInteraction(titleReplacement);
            _interactionRequest.Raise(editTitleReplacementInteraction, AddInteractionCallback);
        }

        private void AddInteractionCallback(TitleReplacementEditInteraction titleReplacementEditInteraction)
        {
            if (titleReplacementEditInteraction.Success)
            {
                var applicationSettingsTitleReplacement = _settingsProvider.Settings.ApplicationSettings.TitleReplacement;
                applicationSettingsTitleReplacement.Add(titleReplacementEditInteraction.Replacement);
            }
        }

#pragma warning disable CS0067

        public event EventHandler CanExecuteChanged;

#pragma warning restore CS0067
    }
}
