using pdfforge.Obsidian;
using pdfforge.PDFCreator.UI.ViewModels.ActionViewModels.Translations;
using pdfforge.PDFCreator.UI.ViewModels.Helper;

namespace pdfforge.PDFCreator.UI.ViewModels.ActionViewModels
{
    public class BackgroundActionViewModel : ActionViewModel
    {
        private readonly IOpenFileInteractionHelper _openFileInteractionHelper;

        public BackgroundActionViewModel(BackgroundSettingsAndActionTranslation translation, IOpenFileInteractionHelper openFileInteractionHelper)
        {
            Translation = translation;
            _openFileInteractionHelper = openFileInteractionHelper;
            
            SelectBackgroundCommand = new DelegateCommand(SelectBackgroundExecute);
            DisplayName = Translation.DisplayName;
            Description = Translation.Description;
        }
        public BackgroundSettingsAndActionTranslation Translation { get; private set; }

        public DelegateCommand SelectBackgroundCommand { get; private set; }

        public override bool IsEnabled
        {
            get { return CurrentProfile != null && CurrentProfile.BackgroundPage.Enabled; }
            set
            {
                CurrentProfile.BackgroundPage.Enabled = value;
                RaisePropertyChanged(nameof(IsEnabled));
            }
        }

        private void SelectBackgroundExecute(object obj)
        {
            var titel = Translation.SelectBackgroundFile;
            var filter = Translation.PDFFiles
                         + @" (*.pdf)|*.pdf|"
                         + Translation.AllFiles
                         + @" (*.*)|*.*";

            CurrentProfile.BackgroundPage.File = _openFileInteractionHelper.StartOpenFileInteraction(CurrentProfile.BackgroundPage.File, titel, filter);
            RaisePropertyChanged(nameof(CurrentProfile));
        }

        public override HelpTopic GetContextBasedHelpTopic()
        {
            return HelpTopic.Background;
        }
    }
}