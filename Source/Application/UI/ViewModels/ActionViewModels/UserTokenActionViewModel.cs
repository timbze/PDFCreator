using System.Windows.Input;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.UI.ViewModels.ActionViewModels.Translations;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.Utilities.UserGuide;

namespace pdfforge.PDFCreator.UI.ViewModels.ActionViewModels
{
    public class UserTokenActionViewModel : ActionViewModel
    {
        public UserTokenActionViewModel(UserTokenActionViewModelTranslation translation, IUserGuideLauncher userGuideLauncher)
        {
            Translation = translation;
            DisplayName = Translation.DisplayName;
            Description = Translation.Description;
            OpenUserGuideCommand = new DelegateCommand(o => userGuideLauncher.ShowHelpTopic(HelpTopic.UserTokens));
        }

        public UserTokenActionViewModelTranslation Translation { get; }

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
