using System.Windows;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.Translations;

namespace pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ProfileSettings
{
    public class ImageFormatsTabViewModel : CurrentProfileViewModel
    {
        public ImageFormatsTabViewModel(ImageFormatsTabTranslation translation)
        {
            Translation = translation;
            LostFocusCommand = new DelegateCommand<RoutedEventArgs>(OnLostFocus);
        }

        public DelegateCommand<RoutedEventArgs> LostFocusCommand { get; set; }

        public ImageFormatsTabTranslation Translation { get; }

        private void OnLostFocus(RoutedEventArgs obj)
        {
            var quality = CurrentProfile.JpegSettings.Quality;
            if (quality < 1)
                CurrentProfile.JpegSettings.Quality = 1;
            if (quality > 100)
                CurrentProfile.JpegSettings.Quality = 100;

            RaisePropertyChanged(nameof(CurrentProfile));
        }

        public override HelpTopic GetContextBasedHelpTopic()
        {
            return HelpTopic.ProfileImageFormats;
        }
    }
}