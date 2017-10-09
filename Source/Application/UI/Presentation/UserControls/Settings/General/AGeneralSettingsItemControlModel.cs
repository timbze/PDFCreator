using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using Prism.Mvvm;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.General
{
    public abstract class AGeneralSettingsItemControlModel : TranslatableViewModelBase<GeneralSettingsTranslation>
    {
        public AGeneralSettingsItemControlModel(ITranslationUpdater translationUpdater, ICurrentSettingsProvider settingsProvider, IGpoSettings gpoSettings):base(translationUpdater)
        {
            ApplicationSettings = settingsProvider.Settings?.ApplicationSettings;
            GpoSettings = gpoSettings;
        }

        public ApplicationSettings ApplicationSettings { get; private set; }

        protected IGpoSettings GpoSettings { get; }

    }
}
