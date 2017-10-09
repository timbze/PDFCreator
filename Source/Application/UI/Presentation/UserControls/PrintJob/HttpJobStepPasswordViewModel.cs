using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    public class HttpJobStepPasswordViewModel : JobStepPasswordViewModelBase<HttpPasswordTranslation>
    {
        public string HttpAccountInfo { get; set; }

        public HttpJobStepPasswordViewModel(ITranslationUpdater translationUpdater) : base(translationUpdater, nameof(JobPasswords.HttpPassword))
        {
        }

        protected override void ExecuteWorkflow()
        {
            HttpAccountInfo = Job.Accounts.GetHttpAccount(Job.Profile).AccountInfo;
            RaisePropertyChanged(nameof(HttpAccountInfo));
        }

        public override void SkipHook()
        {
            Job.Profile.HttpSettings.Enabled = false;
        }
    }
}
