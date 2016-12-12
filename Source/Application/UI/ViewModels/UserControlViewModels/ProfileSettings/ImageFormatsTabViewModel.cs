using System.Collections.Generic;
using System.Windows;
using pdfforge.DynamicTranslator;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.UI.ViewModels.Helper;

namespace pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ProfileSettings
{
    public class ImageFormatsTabViewModel : CurrentProfileViewModel
    {
        public ImageFormatsTabViewModel(ITranslator translator)
        {
            Translator = translator;
            LostFocusCommand = new DelegateCommand<RoutedEventArgs>(OnLostFocus);
        }

        public DelegateCommand<RoutedEventArgs> LostFocusCommand { get; set; }

        public ITranslator Translator { get; }

        public IEnumerable<EnumValue<JpegColor>> JpegColorValues => Translator.GetEnumTranslation<JpegColor>();

        public IEnumerable<EnumValue<PngColor>> PngColorValues => Translator.GetEnumTranslation<PngColor>();

        public IEnumerable<EnumValue<TiffColor>> TiffColorValues => Translator.GetEnumTranslation<TiffColor>();

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