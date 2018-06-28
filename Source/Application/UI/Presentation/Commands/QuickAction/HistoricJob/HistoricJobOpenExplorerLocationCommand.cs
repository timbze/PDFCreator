using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.Commands.QuickAction.HistoricJob
{
    public class HistoricJobOpenExplorerLocationCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object target)
        {
            var historicJob = target as Core.Services.JobHistory.HistoricJob;
            if (historicJob != null)
            {
                string args = $"/e, /select, \"{historicJob.HistoricFiles.First().Path}\"";
                Process.Start("explorer", args);
            }
        }

#pragma warning disable CS0067

        public event EventHandler CanExecuteChanged;

#pragma warning restore CS0067
    }
}
