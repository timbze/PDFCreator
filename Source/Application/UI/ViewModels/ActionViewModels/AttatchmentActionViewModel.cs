using pdfforge.Obsidian;
using pdfforge.PDFCreator.UI.ViewModels.ActionViewModels.Translations;
using pdfforge.PDFCreator.UI.ViewModels.Helper;

namespace pdfforge.PDFCreator.UI.ViewModels.ActionViewModels
{
    public class AttatchmentActionViewModel : ActionViewModel
    {
        private readonly IOpenFileInteractionHelper _openFileInteractionHelper;

        public AttatchmentActionViewModel(IOpenFileInteractionHelper openFileInteractionHelper, AttachmentSettingsAndActionTranslation translations)
        {
            _openFileInteractionHelper = openFileInteractionHelper;
            Translation = translations;

            DisplayName = Translation.DisplayName;
            Description = Translation.Description;
            SelectAttatchmenCommand = new DelegateCommand(SelectAttatchmentExecute);
        }

        public AttachmentSettingsAndActionTranslation Translation { get; }

        public DelegateCommand SelectAttatchmenCommand { get; set; }

        public override bool IsEnabled
        {
            get { return CurrentProfile != null && CurrentProfile.AttachmentPage.Enabled; }
            set
            {
                CurrentProfile.AttachmentPage.Enabled = value;
                RaisePropertyChanged(nameof(IsEnabled));
            }
        }

        private void SelectAttatchmentExecute(object obj)
        {
            var title = Translation.SelectAttachmentFile;
            var filter = Translation.PDFFiles
                         + @" (*.pdf)|*.pdf|"
                         + Translation.AllFiles
                         + @" (*.*)|*.*";

            CurrentProfile.AttachmentPage.File = _openFileInteractionHelper.StartOpenFileInteraction(CurrentProfile.AttachmentPage.File, title, filter);
            RaisePropertyChanged(nameof(CurrentProfile));
        }

        public override HelpTopic GetContextBasedHelpTopic()
        {
            return HelpTopic.Attachment;
        }
    }
}