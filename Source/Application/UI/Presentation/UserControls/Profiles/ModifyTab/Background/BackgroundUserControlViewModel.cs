using pdfforge.Obsidian;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyTab.Background
{
    public class BackgroundUserControlViewModel : ProfileUserControlViewModel<BackgroundSettingsAndActionTranslation>
    {
        private readonly IOpenFileInteractionHelper _openFileInteractionHelper;

        public BackgroundUserControlViewModel(IOpenFileInteractionHelper openFileInteractionHelper, ITranslationUpdater translationUpdater, ISelectedProfileProvider selectedProfile) : base(translationUpdater, selectedProfile)
        {
            _openFileInteractionHelper = openFileInteractionHelper;
            SelectBackgroundCommand = new DelegateCommand(SelectBackgroundExecute);
        }

        public DelegateCommand SelectBackgroundCommand { get; private set; }

        public bool IsEnabled
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

            var interactionResult = _openFileInteractionHelper.StartOpenFileInteraction(CurrentProfile.BackgroundPage.File, titel, filter);
            interactionResult.MatchSome(s =>
            {
                CurrentProfile.BackgroundPage.File = s;
                RaisePropertyChanged(nameof(CurrentProfile));
            });
        }
    }

    public class DesignTimeBackgroundUserControlViewModel : BackgroundUserControlViewModel
    {
        public DesignTimeBackgroundUserControlViewModel() : base(null, new DesignTimeTranslationUpdater(), new DesignTimeCurrentSettingsProvider())
        {
        }
    }
}
