using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using Prism.Regions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.UI.Presentation.Workflow
{
    public class InteractiveWorkflowManager
    {
        private readonly IWorkflowNavigationHelper _workflowNavigationHelper;
        private readonly IRegionManager _regionManager;
        public Job Job { get; set; }
        public bool Cancel { private get; set; }

        private readonly List<IWorkflowStep> _steps = new List<IWorkflowStep>();

        public InteractiveWorkflowManager(IWorkflowNavigationHelper workflowNavigationHelper, IRegionManager regionManager, IEnumerable<IWorkflowStep> workflowSteps)
        {
            _workflowNavigationHelper = workflowNavigationHelper;
            _regionManager = regionManager;

            _steps.AddRange(workflowSteps);
        }

        public async Task Run()
        {
            var region = _regionManager.Regions[PrintJobRegionNames.PrintJobMainRegion];

            foreach (var step in _steps)
            {
                if (Cancel)
                    return;

                if (!step.IsStepRequired(Job))
                    continue;

                region.RequestNavigate(step.NavigationUri);

                var viewModel = _workflowNavigationHelper.GetValidatedViewModel(region, step.NavigationUri);

                await step.ExecuteStep(Job, viewModel);
            }
        }
    }
}
