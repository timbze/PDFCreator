using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.UI.Presentation.Workflow
{
    public interface IWorkflowStep
    {
        string NavigationUri { get; }

        bool IsStepRequired(Job job);

        Task ExecuteStep(Job job, IWorkflowViewModel workflowViewModel);
    }
}
