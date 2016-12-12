using System.Windows.Input;
using pdfforge.DynamicTranslator;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.Utilities.UserGuide;

namespace pdfforge.PDFCreator.UI.ViewModels.ActionViewModels
{
    public class UserTokenActionViewModel : ActionViewModel
    {
        public UserTokenActionViewModel(ITranslator translator, IUserGuideLauncher userGuideLauncher)
        {
            Translator = translator;
            DisplayName = Translator.GetTranslation("UserTokenActionViewModel", "DisplayName");
            Description = Translator.GetTranslation("UserTokenActionViewModel", "Description");
            OpenUserGuideCommand = new DelegateCommand(o => userGuideLauncher.ShowHelpTopic(HelpTopic.UserTokens));
        }

        public ITranslator Translator { get; }

        public ICommand OpenUserGuideCommand { get; private set; }

        public override HelpTopic GetContextBasedHelpTopic()
        {
            return HelpTopic.UserTokens;
        }

        public override bool IsEnabled
        {
            get { return CurrentProfile != null && CurrentProfile.UserTokens.Enabled; }
            set
            {
                CurrentProfile.UserTokens.Enabled = value;
                RaisePropertyChanged(nameof(IsEnabled));
            }
        }
    }
}
