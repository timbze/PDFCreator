using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.TabHelper;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyTab
{
    public class ModifyMasterTabViewModel : TranslatableViewModelBase<ProfileModifyTranslation>, ITabViewModel
    {
        private string _title;

        public ModifyMasterTabViewModel(ITranslationUpdater translationUpdater) : base(translationUpdater)
        {
        }

        protected override void OnTranslationChanged()
        {
            base.OnTranslationChanged();
            RaisePropertyChanged(Title);
        }

        public string Title
        {
            get { return Translation.Modify; }
            set { _title = value; }
        }

        public IconList Icon { get; set; } = IconList.ModifySettings;
        public bool HiddenByGPO => false;
        public bool BlockedByGPO => false;
    }
}
