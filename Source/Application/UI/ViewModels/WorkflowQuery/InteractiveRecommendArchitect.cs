using NLog;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Actions.Queries;
using pdfforge.PDFCreator.UI.Interactions;

namespace pdfforge.PDFCreator.UI.ViewModels.WorkflowQuery
{
    public class InteractiveRecommendArchitect : IRecommendArchitect
    {
        private readonly IInteractionInvoker _interactionInvoker;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public InteractiveRecommendArchitect(IInteractionInvoker interactionInvoker)
        {
            _interactionInvoker = interactionInvoker;
        }

        public bool Show()
        {
            _logger.Info("Recommend PDF Architect");
            _interactionInvoker.Invoke(new RecommendPdfArchitectInteraction(true));
            return true;
        }
    }
}