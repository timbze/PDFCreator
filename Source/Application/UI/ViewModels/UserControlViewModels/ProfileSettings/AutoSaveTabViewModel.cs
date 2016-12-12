using pdfforge.DynamicTranslator;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction.DialogInteractions;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.Utilities.Tokens;

namespace pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ProfileSettings
{
    public class AutoSaveTabViewModel : CurrentProfileViewModel
    {
        private readonly IInteractionInvoker _interactionInvoker;

        public AutoSaveTabViewModel(ITranslator translator, IInteractionInvoker interactionInvoker)
        {
            Translator = translator;
            _interactionInvoker = interactionInvoker;
            var tokenHelper = new TokenHelper(Translator);
            TokenReplacer = tokenHelper.TokenReplacerWithPlaceHolders;

            TargetDirectoryViewModel = new TokenViewModel(
                s => CurrentProfile.AutoSave.TargetDirectory = s,
                () => CurrentProfile?.AutoSave.TargetDirectory,
                tokenHelper.GetTokenListForDirectory());
        }

        public TokenViewModel TargetDirectoryViewModel { get; set; }

        public DelegateCommand BrowseAutoSaveFolderCommand => new DelegateCommand(BrowseAutoSaveFolderExecute);

        public ITranslator Translator { get; }

        public TokenReplacer TokenReplacer { get; set; }

        public override HelpTopic GetContextBasedHelpTopic()
        {
            return HelpTopic.ProfileAutoSave;
        }

        protected override void HandleCurrentProfileChanged()
        {
            TargetDirectoryViewModel.RaiseTextChanged();
        }

        private void BrowseAutoSaveFolderExecute(object obj)
        {
            var interaction = new FolderBrowserInteraction();
            interaction.Description = Translator.GetTranslation("ProfileSettingsWindow", "SelectAutoSaveFolder");
            interaction.ShowNewFolderButton = true;

            _interactionInvoker.Invoke(interaction);

            if (!interaction.Success)
                return;

            TargetDirectoryViewModel.Text = interaction.SelectedPath;
            TargetDirectoryViewModel.RaiseTextChanged();
        }
    }
}