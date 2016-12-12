using System;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Core.Workflow.Queries;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.ViewModels.Helper;

namespace pdfforge.PDFCreator.UI.ViewModels.WorkflowQuery
{
    public class JobFinishedHandler : IJobFinishedHandler
    {
        private readonly IInteractionInvoker _interactionInvoker;
        private readonly IPlusHintHelper _plusHintHelper;

        public JobFinishedHandler(IInteractionInvoker interactionInvoker, IPlusHintHelper plusHintHelper)
        {
            _interactionInvoker = interactionInvoker;
            _plusHintHelper = plusHintHelper;
        }

        public void OnJobFinished(object sender, EventArgs eventArgs)
        {
            if (_plusHintHelper.QueryDisplayHint())
            {
                _interactionInvoker.Invoke(new PlusHintInteraction(_plusHintHelper.CurrentJobCounter));
            }
        }
    }
}
