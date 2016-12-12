using pdfforge.DynamicTranslator;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.UI.ViewModels.Helper;

namespace pdfforge.PDFCreator.UI.ViewModels.ActionViewModels
{
    public class CoverActionViewModel : ActionViewModel
    {
        private readonly IOpenFileInteractionHelper _openFileInteractionHelper;

        public CoverActionViewModel(ITranslator translator, IOpenFileInteractionHelper openFileInteractionHelper)
        {
            _openFileInteractionHelper = openFileInteractionHelper;
            Translator = translator;

            SelectCoverCommand = new DelegateCommand(SelectCoverExecute);

            DisplayName = Translator.GetTranslation("CoverSettings", "DisplayName");
            Description = Translator.GetTranslation("CoverSettings", "Description");
        }

        public ITranslator Translator { get; }

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
            var title = Translator.GetTranslation("CoverSettings", "SelectCoverFile");
            var filter = Translator.GetTranslation("CoverSettings", "PDFFiles")
                         + @" (*.pdf)|*.pdf|"
                         + Translator.GetTranslation("CoverSettings", "AllFiles")
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