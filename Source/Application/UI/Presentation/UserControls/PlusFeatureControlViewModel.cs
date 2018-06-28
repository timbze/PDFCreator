using pdfforge.Obsidian;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ServiceLocator;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.Utilities.Process;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls
{
    public enum RequiredEdition
    {
        AllLicensed,
        BusinessOrTerminalServer
    }

    public class PlusFeatureControlViewModel : TranslatableViewModelBase<PlusFeatureControlTranslation>, IWhitelisted
    {
        private readonly IProcessStarter _processStarter;
        private RequiredEdition _edition;

        public PlusFeatureControlViewModel(EditionHintOptionProvider editionHintOptionProvider, IProcessStarter processStarter, ITranslationUpdater translationUpdater)
            : base(translationUpdater)
        {
            _processStarter = processStarter;
            ShowPlusHint = editionHintOptionProvider.ShowOnlyForPlusAndBusinessHint;
            RaisePropertyChanged(nameof(ShowPlusHint));
        }

        public RequiredEdition Edition
        {
            get { return _edition; }
            set
            {
                SetProperty(ref _edition, value);
                RaisePropertyChanged(nameof(FeatureText));
                RaisePropertyChanged(nameof(ToolTip));
            }
        }

        public string FeatureText
        {
            get
            {
                return Edition == RequiredEdition.AllLicensed
                    ? Translation.PlusFeature.ToUpper()
                    : Translation.BusinessFeature.ToUpper();
            }
        }

        public string ToolTip
        {
            get
            {
                return Edition == RequiredEdition.AllLicensed
                    ? Translation.PlusRequiredHint
                    : Translation.BusinessRequiredHint;
            }
        }

        public bool ShowPlusHint { get; }

        public ICommand ShowEditionWebsiteCommand => new DelegateCommand(ShowEditionWebsiteExecute);

        private void ShowEditionWebsiteExecute(object o)
        {
            var url = Edition == RequiredEdition.AllLicensed
                ? Urls.PlusHintLink
                : Urls.BusinessHintLink;

            _processStarter.Start(url);
        }
    }
}
