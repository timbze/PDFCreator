using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation.Events;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using Prism.Events;
using System;

namespace pdfforge.PDFCreator.UI.Presentation.Commands.ProfileCommands
{
    public class WaitProfileModificationCommand : IWaitableCommand
    {
        private readonly ISettingsProvider _baseProvider;
        private readonly ICurrentSettingsProvider _currentSettingsProvider;
        private readonly IEventAggregator _eventAggregator;

        public WaitProfileModificationCommand(ISettingsProvider baseProvider, ICurrentSettingsProvider currentSettingsProvider, IEventAggregator eventAggregator)
        {
            _baseProvider = baseProvider;
            _currentSettingsProvider = currentSettingsProvider;
            _eventAggregator = eventAggregator;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _baseProvider.SettingsChanged += SettingsOnPropertyChanged;
            _currentSettingsProvider.SettingsChanged += SettingsOnPropertyChanged;
            _eventAggregator.GetEvent<MainWindowClosedEvent>().Subscribe(OnWindowClosedAction);
        }

        private void OnWindowClosedAction()
        {
            SettingsOnPropertyChanged(this, EventArgs.Empty);
        }

        private void SettingsOnPropertyChanged(object sender, EventArgs args)
        {
            _eventAggregator.GetEvent<MainWindowClosedEvent>().Unsubscribe(OnWindowClosedAction);
            _baseProvider.Settings.PropertyChanged -= SettingsOnPropertyChanged;
            _currentSettingsProvider.Settings.PropertyChanged -= SettingsOnPropertyChanged;
            IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Success));
        }

#pragma warning disable 67

        public event EventHandler CanExecuteChanged;

        public event EventHandler<MacroCommandIsDoneEventArgs> IsDone;

#pragma warning restore 67
    }
}
