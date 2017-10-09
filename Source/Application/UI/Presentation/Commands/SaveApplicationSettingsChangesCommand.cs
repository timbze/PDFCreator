using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.UI.ViewModels;
using System;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.Commands
{
    public class SaveApplicationSettingsChangesCommand : ICommand
    {
        private readonly ICurrentSettingsProvider _currentSettingsProvider;
        private readonly ISettingsProvider _settingsProvider;
        private readonly ISettingsLoader _settingsLoader;
        private readonly ILanguageProvider _languageProvider;

        public SaveApplicationSettingsChangesCommand(ICurrentSettingsProvider currentSettingsProvider, ISettingsProvider settingsProvider, ISettingsLoader settingsLoader, ILanguageProvider languageProvider)
        {
            _currentSettingsProvider = currentSettingsProvider;
            _settingsProvider = settingsProvider;
            _settingsLoader = settingsLoader;
            _languageProvider = languageProvider;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (SettingsChanged())
            {
                _settingsProvider.UpdateSettings(_currentSettingsProvider.Settings);
                _settingsLoader.SaveSettingsInRegistry(_currentSettingsProvider.Settings);
            }
        }

        private bool SettingsChanged()
        {
            var currentSettings = _currentSettingsProvider.Settings;
            var storedSettings = _settingsProvider.Settings;

            if (currentSettings.ApplicationSettings.Language != _languageProvider.CurrentLanguage.Iso2)
            {
                return true;
            }

            return !currentSettings.Equals(storedSettings);
        }

#pragma warning disable CS0067

        public event EventHandler CanExecuteChanged;

#pragma warning restore CS0067
    }
}
