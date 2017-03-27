using System.Collections.Generic;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction.DialogInteractions;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.Translations;
using pdfforge.PDFCreator.Utilities.Tokens;

namespace pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ProfileSettings
{
    public class SaveTabViewModel : CurrentProfileViewModel
    {
        private readonly IInteractionInvoker _interactionInvoker;

        public SaveTabViewModel(SaveTabTranslation translation, IInteractionInvoker interactionInvoker, TokenHelper tokenHelper)
        {
            Translation = translation;
            _interactionInvoker = interactionInvoker;

            TokenReplacer = tokenHelper.TokenReplacerWithPlaceHolders;

            FileNameViewModel = new TokenViewModel(x => CurrentProfile.FileNameTemplate = x, () => CurrentProfile?.FileNameTemplate, tokenHelper.GetTokenListForFilename());
            FolderViewModel = new TokenViewModel(x => CurrentProfile.SaveDialog.Folder = x, () => CurrentProfile?.SaveDialog.Folder, tokenHelper.GetTokenListForDirectory());
        }

        public DelegateCommand BrowseSaveFolderCommand => new DelegateCommand(BrowseSaveFolderExecute);

        public TokenViewModel FileNameViewModel { get; set; }
        public TokenViewModel FolderViewModel { get; set; }

        public TokenReplacer TokenReplacer { get; set; }

        public SaveTabTranslation Translation { get; set; }

        public IEnumerable<OutputFormat> DefaultFileFormalValues => System.Enum.GetValues(typeof(OutputFormat)) as OutputFormat[];


        private void BrowseSaveFolderExecute(object o)
        {
            var interaction = new FolderBrowserInteraction();
            interaction.Description = Translation.SelectSaveDialogFolder;
            interaction.ShowNewFolderButton = true;

            _interactionInvoker.Invoke(interaction);

            if (!interaction.Success)
                return;

            FolderViewModel.Text = interaction.SelectedPath;
            FolderViewModel.RaiseTextChanged();
        }

        public override HelpTopic GetContextBasedHelpTopic()
        {
            return HelpTopic.ProfileSave;
        }

        protected override void HandleCurrentProfileChanged()
        {
            FileNameViewModel.RaiseTextChanged();
            FolderViewModel.RaiseTextChanged();
        }
    }
}