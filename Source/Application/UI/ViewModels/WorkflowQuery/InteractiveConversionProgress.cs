using System;
using System.Threading;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Core.Workflow.Queries;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.Utilities.Threading;

namespace pdfforge.PDFCreator.UI.ViewModels.WorkflowQuery
{
    public class InteractiveConversionProgress : IConversionProgress
    {
        private readonly IInteractionInvoker _interactionInvoker;
        private readonly IThreadManager _threadManager;

        public InteractiveConversionProgress(IThreadManager threadManager, IInteractionInvoker interactionInvoker)
        {
            _threadManager = threadManager;
            _interactionInvoker = interactionInvoker;
        }

        public void Show(Job job)
        {
            if (!job.Profile.ShowProgress)
                return;

            var progressWindowThread = new SynchronizedThread(() => ShowConversionProgressDialog(job));
            progressWindowThread.SetApartmentState(ApartmentState.STA);

            progressWindowThread.Name = "ProgressForm";

            _threadManager.StartSynchronizedThread(progressWindowThread);
        }

        [STAThread]
        private void ShowConversionProgressDialog(Job job)
        {
            var conversionProgressInteraction = new ConversionProgressInteraction(job);

            _interactionInvoker.Invoke(conversionProgressInteraction);
        }
    }
}