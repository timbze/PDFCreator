using System;

namespace pdfforge.PDFCreator.Conversion.Jobs.Jobs
{
    public class JobLoginFailedEventArgs : EventArgs
    {
        public Action<string> ContinueAction { get; }
        public Action AbortAction { get; }
        public string ActionDisplayName { get; }

        public JobLoginFailedEventArgs(Action<string> continueAction, Action abortAction, string actionDisplayName)
        {
            ContinueAction = continueAction;
            AbortAction = abortAction;
            ActionDisplayName = actionDisplayName;
        }
    }
}
