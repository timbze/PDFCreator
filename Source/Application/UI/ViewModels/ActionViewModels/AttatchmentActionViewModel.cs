using pdfforge.DynamicTranslator;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.UI.ViewModels.Helper;

namespace pdfforge.PDFCreator.UI.ViewModels.ActionViewModels
{
    public class AttatchmentActionViewModel : ActionViewModel
    {
        private readonly IOpenFileInteractionHelper _openFileInteractionHelper;

        public AttatchmentActionViewModel(ITranslator translator, IOpenFileInteractionHelper openFileInteractionHelper)
        {
            _openFileInteractionHelper = openFileInteractionHelper;
            Translator = translator;

            DisplayName = Translator.GetTranslation("AttachmentSettings", "DisplayName");
            Description = Translator.GetTranslation("AttachmentSettings", "Description");
            SelectAttatchmenCommand = new DelegateCommand(SelectAttatchmentExecute);
        }

        public ITranslator Translator { get; }

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
            var title = Translator.GetTranslation("AttachmentSettings", "SelectAttachmentFile");
            var filter = Translator.GetTranslation("AttachmentSettings", "PDFFiles")
                         + @" (*.pdf)|*.pdf|"
                         + Translator.GetTranslation("AttachmentSettings", "AllFiles")
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