using System.Collections.Generic;
using pdfforge.DynamicTranslator;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction.DialogInteractions;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.Utilities.Tokens;

namespace pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ProfileSettings
{
    public class SaveTabViewModel : CurrentProfileViewModel
    {
        private readonly IInteractionInvoker _interactionInvoker;

        public SaveTabViewModel(ITranslator translator, IInteractionInvoker interactionInvoker)
        {
            Translator = translator;
            _interactionInvoker = interactionInvoker;

            var tokenHelper = new TokenHelper(Translator);
            TokenReplacer = tokenHelper.TokenReplacerWithPlaceHolders;

            FileNameViewModel = new TokenViewModel(x => CurrentProfile.FileNameTemplate = x, () => CurrentProfile?.FileNameTemplate, tokenHelper.GetTokenListForFilename());
            FolderViewModel = new TokenViewModel(x => CurrentProfile.SaveDialog.Folder = x, () => CurrentProfile?.SaveDialog.Folder, tokenHelper.GetTokenListForDirectory());
        }

        public DelegateCommand BrowseSaveFolderCommand => new DelegateCommand(BrowseSaveFolderExecute);

        public TokenViewModel FileNameViewModel { get; set; }
        public TokenViewModel FolderViewModel { get; set; }

        public TokenReplacer TokenReplacer { get; set; }

        public ITranslator Translator { get; set; }

        public IEnumerable<EnumValue<OutputFormat>> DefaultFileFormalValues => Translator.GetEnumTranslation<OutputFormat>();

        private void BrowseSaveFolderExecute(object o)
        {
            var interaction = new FolderBrowserInteraction();
            interaction.Description = Translator.GetTranslation("ProfileSettingsWindow", "SelectSaveDialogFolder");
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