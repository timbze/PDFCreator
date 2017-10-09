using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using System.ComponentModel;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Tabs.ConvertTabs
{
    public class ConvertPdfViewModel : ProfileUserControlViewModel<ConvertPdfTranslation>
    {
        private readonly OutputFormatHelper _outputFormatHelper = new OutputFormatHelper();

        public ConvertPdfViewModel(ITranslationUpdater translationUpdater, ISelectedProfileProvider selectedProfile) : base(translationUpdater, selectedProfile)
        {
            CurrentProfileChanged += (sender, args) =>
            {
                RaisePropertyChanged(nameof(IsPdfOutput));
                CurrentProfile.PropertyChanged += RaiseIsPdfOutputChanged;
            };

            if (CurrentProfile != null)
                CurrentProfile.PropertyChanged += RaiseIsPdfOutputChanged;
        }

        private void RaiseIsPdfOutputChanged(object sender = null, PropertyChangedEventArgs args = null)
        {
            if (args == null || args.PropertyName == nameof(CurrentProfile.OutputFormat))
                RaisePropertyChanged(nameof(IsPdfOutput));
        }

        public bool IsPdfOutput => _outputFormatHelper.IsPdfFormat(CurrentProfile.OutputFormat);
    }
}
