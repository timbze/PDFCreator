using pdfforge.Obsidian;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyTab.Attachment
{
    public class AttachmentUserControlViewModel : ProfileUserControlViewModel<AttachmentSettingsAndActionTranslation>
    {
        private readonly IOpenFileInteractionHelper _openFileInteractionHelper;

        public AttachmentUserControlViewModel(IOpenFileInteractionHelper openFileInteractionHelper, ITranslationUpdater transupdater, ISelectedProfileProvider selectedProfile) : base(transupdater, selectedProfile)
        {
            _openFileInteractionHelper = openFileInteractionHelper;

            SelectAttatchmenCommand = new DelegateCommand(SelectAttatchmentExecute);
        }

        public DelegateCommand SelectAttatchmenCommand { get; set; }

        public bool IsEnabled
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

            var interactionResult = _openFileInteractionHelper.StartOpenFileInteraction(CurrentProfile.AttachmentPage.File, title, filter);

            interactionResult.MatchSome(s =>
            {
                CurrentProfile.AttachmentPage.File = s;
                RaisePropertyChanged(nameof(CurrentProfile));
            });
        }
    }

    public class DesignTimeAttachmentUserControlViewModel : AttachmentUserControlViewModel
    {
        public DesignTimeAttachmentUserControlViewModel() : base(null, new DesignTimeTranslationUpdater(), new DesignTimeCurrentSettingsProvider())
        {
        }
    }
}
