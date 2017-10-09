using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob.UpdateHint;

namespace pdfforge.PDFCreator.UI.Presentation.Workflow.Steps
{
    public class UpdateHintStep : WorkflowStepBase
    {
        private readonly IUpdateAssistant _updateAssistant;
        public override string NavigationUri => nameof(UpdateHintView);

        public UpdateHintStep(IUpdateAssistant updateAssistant)
        {
            _updateAssistant = updateAssistant;
        }

        public override bool IsStepRequired(Job job)
        {
            return _updateAssistant.ShowUpdate;
        }
    }
}
