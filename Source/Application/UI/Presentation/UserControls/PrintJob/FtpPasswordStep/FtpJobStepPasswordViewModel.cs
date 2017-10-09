using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    public class FtpJobStepPasswordViewModel : JobStepPasswordViewModelBase<FtpActionTranslation>
    {
        public string FtpAccountInfo { get; set; }

        public FtpJobStepPasswordViewModel(ITranslationUpdater translationUpdater) : base(translationUpdater, nameof(JobPasswords.FtpPassword))
        {
        }

        protected override void ExecuteWorkflow()
        {
            FtpAccountInfo = Job.Accounts.GetFtpAccount(Job.Profile).AccountInfo;
            RaisePropertyChanged(nameof(FtpAccountInfo));
        }

        public override void SkipHook()
        {
            Job.Profile.Ftp.Enabled = false;
        }

        protected override string GetCancelErrorMessage()
        {
            return "User cancelled in the FtpPasswordView";
        }
    }
}
