using pdfforge.Obsidian;
using pdfforge.PDFCreator.UI.ViewModels.ActionViewModels.Translations;
using pdfforge.PDFCreator.UI.ViewModels.Helper;

namespace pdfforge.PDFCreator.UI.ViewModels.ActionViewModels
{
    public class CoverActionViewModel : ActionViewModel
    {
        private readonly IOpenFileInteractionHelper _openFileInteractionHelper;

        public CoverActionViewModel(CoverSettingsTranslation translation, IOpenFileInteractionHelper openFileInteractionHelper)
        {
            _openFileInteractionHelper = openFileInteractionHelper;
            Translation = translation;

            SelectCoverCommand = new DelegateCommand(SelectCoverExecute);

            DisplayName = Translation.DisplayName;
            Description = Translation.Description;
        }

        public CoverSettingsTranslation Translation { get; }

        public DelegateCommand SelectCoverCommand { get; set; }

        public override bool IsEnabled
        {
            get { return CurrentProfile != null && CurrentProfile.CoverPage.Enabled; }
            set
            {
                CurrentProfile.CoverPage.Enabled = value;
                RaisePropertyChanged(nameof(IsEnabled));
            }
        }

        private void SelectCoverExecute(object obj)
        {
            var title = Translation.SelectCoverFile;
            var filter = Translation.PDFFiles
                         + @" (*.pdf)|*.pdf|"
                         + Translation.AllFiles
                         + @" (*.*)|*.*";
            CurrentProfile.CoverPage.File = _openFileInteractionHelper.StartOpenFileInteraction(CurrentProfile.CoverPage.File, title, filter);
            RaisePropertyChanged(nameof(CurrentProfile));
        }

        public override HelpTopic GetContextBasedHelpTopic()
        {
            return HelpTopic.Cover;
        }
    }
}