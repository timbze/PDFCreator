using pdfforge.Obsidian;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyTab.Cover
{
    public class CoverUserControlViewModel : ProfileUserControlViewModel<CoverSettingsTranslation>
    {
        private readonly IOpenFileInteractionHelper _openFileInteractionHelper;

        public CoverUserControlViewModel(IOpenFileInteractionHelper openFileInteractionHelper, ITranslationUpdater translationUpdater, ISelectedProfileProvider selectedProfile) : base(translationUpdater, selectedProfile)
        {
            _openFileInteractionHelper = openFileInteractionHelper;

            SelectCoverCommand = new DelegateCommand(SelectCoverExecute);
        }

        public DelegateCommand SelectCoverCommand { get; set; }

        public bool IsEnabled
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

            var interactionResult = _openFileInteractionHelper.StartOpenFileInteraction(CurrentProfile.CoverPage.File, title, filter);
            interactionResult.MatchSome(s =>
            {
                CurrentProfile.CoverPage.File = s;
                RaisePropertyChanged(nameof(CurrentProfile));
            });
        }
    }

    public class DesignTimeCoverUserControlViewModel : CoverUserControlViewModel
    {
        public DesignTimeCoverUserControlViewModel() : base(null, new DesignTimeTranslationUpdater(), new DesignTimeCurrentSettingsProvider())
        {
        }
    }
}
