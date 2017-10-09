using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob;

namespace pdfforge.PDFCreator.UI.Presentation.Workflow.Steps
{
    public class DropboxSharedLinkStep : WorkflowStepBase
    {
        public override string NavigationUri => nameof(DropboxShareLinkStepView);

        public override bool IsStepRequired(Job job)
        {
            if (!job.Profile.DropboxSettings.Enabled)
                return false;

            if (!job.Profile.DropboxSettings.CreateShareLink)
                return false;

            return !string.IsNullOrEmpty(job.ShareLinks.DropboxShareUrl);
        }
    }
}
