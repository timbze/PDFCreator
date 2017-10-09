using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob.PlusHint;

namespace pdfforge.PDFCreator.UI.Presentation.Workflow.Steps
{
    public class PlusHintStep : WorkflowStepBase
    {
        public override string NavigationUri => nameof(PlusHintView);

        private readonly IPlusHintHelper _plusHintHelper;

        public PlusHintStep(IPlusHintHelper plusHintHelper)
        {
            _plusHintHelper = plusHintHelper;
        }

        public override bool IsStepRequired(Job job)
        {
            return _plusHintHelper.QueryDisplayHint();
        }
    }
}
