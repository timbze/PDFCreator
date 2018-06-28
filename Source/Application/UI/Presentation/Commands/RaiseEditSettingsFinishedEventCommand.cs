using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using pdfforge.PDFCreator.UI.Presentation.Events;
using Prism.Events;

namespace pdfforge.PDFCreator.UI.Presentation.Commands
{
    public class RaiseEditSettingsFinishedEventCommand : ICommand
    {
        private readonly IEventAggregator _eventAggregator;

        public RaiseEditSettingsFinishedEventCommand(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            _eventAggregator.GetEvent<EditSettingsFinishedEvent>().Publish();
        }

        public event EventHandler CanExecuteChanged;
    }
}