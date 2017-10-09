using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.TabHelper;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SecureTab
{
    public class SecureMasterTabViewModel : TranslatableViewModelBase<ProfileSecureTranslation>, ITabViewModel
    {
        public SecureMasterTabViewModel(ITranslationUpdater translationUpdater) : base(translationUpdater)
        {
        }

        protected override void OnTranslationChanged()
        {
            base.OnTranslationChanged();
            RaisePropertyChanged(Title);
        }

        public string Title => Translation.Secure;

        public IconList Icon { get; set; } = IconList.SecureSettings;
        public bool HiddenByGPO => false;
        public bool BlockedByGPO => false;
    }
}
