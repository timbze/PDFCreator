using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.UI.ViewModels;
using System;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.Commands
{
    public class SaveSettingsCommand : ICommand
    {
        private readonly ICurrentSettingsProvider _currentSettingsProvider;
        private readonly ISettingsProvider _settingsProvider;
        private readonly ISettingsLoader _settingsLoader;

        public SaveSettingsCommand(ICurrentSettingsProvider currentSettingsProvider,
            ISettingsProvider settingsProvider, ISettingsLoader settingsLoader)
        {
            _currentSettingsProvider = currentSettingsProvider;
            _settingsProvider = settingsProvider;
            _settingsLoader = settingsLoader;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _settingsProvider.UpdateSettings(_currentSettingsProvider.Settings);
            _settingsLoader.SaveSettingsInRegistry(_currentSettingsProvider.Settings);
        }

#pragma warning disable CS0067

        public event EventHandler CanExecuteChanged;

#pragma warning restore CS0067
    }
}
