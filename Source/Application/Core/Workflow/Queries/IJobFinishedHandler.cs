using System;

namespace pdfforge.PDFCreator.Core.Workflow.Queries
{
    public interface IJobFinishedHandler
    {
        void OnJobFinished(object sender, EventArgs eventArgs);
    }
}