using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.Commands.QuickAction
{
    public class JobQuickActionOpenExplorerLocationCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object jobObj)
        {
            var job = (jobObj as Job);
            if (job != null)
            {
                string args = $"/e, /select, \"{job.OutputFiles.First()}\"";
                Process.Start("explorer", args);
            }
        }

#pragma warning disable CS0067

        public event EventHandler CanExecuteChanged;

#pragma warning restore CS0067
    }
}
