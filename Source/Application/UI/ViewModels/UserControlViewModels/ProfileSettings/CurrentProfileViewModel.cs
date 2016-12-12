using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.ViewModels.Helper;

namespace pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ProfileSettings
{
    public abstract class CurrentProfileViewModel : ObservableObject
    {
        private ConversionProfile _currentProfile;

        public ConversionProfile CurrentProfile
        {
            get { return _currentProfile; }
            set
            {
                _currentProfile = value;
                RaisePropertyChanged(nameof(CurrentProfile));

                if (CurrentProfile == null)
                    return;

                HandleCurrentProfileChanged();
            }
        }

        public abstract HelpTopic GetContextBasedHelpTopic();

        protected virtual void HandleCurrentProfileChanged()
        {
            // this method can be overridden to react when the current profile changes
        }
    }
}