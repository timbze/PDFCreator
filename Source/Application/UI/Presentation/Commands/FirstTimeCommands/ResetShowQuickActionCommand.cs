using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using System;

namespace pdfforge.PDFCreator.UI.Presentation.Commands.FirstTimeCommands
{
    public class ResetShowQuickActionCommand : IFirstTimeCommand
    {
        private readonly ICurrentSettingsProvider _settingsProvider;
        private readonly ICommandLocator _commandLocator;

        public ResetShowQuickActionCommand(ICurrentSettingsProvider settingsProvider, ICommandLocator commandLocator)
        {
            _settingsProvider = settingsProvider;
            _commandLocator = commandLocator;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (!(parameter is Version))
                return;

            var hasChanges = false;

            var lastLoginVersion = _settingsProvider.Settings.ApplicationSettings.LastLoginVersion;
            var numberOfVersionPosition = lastLoginVersion.Split('.').Length;
            if (numberOfVersionPosition == 2)
            {
                lastLoginVersion += ".0.0";
            }
            else if (numberOfVersionPosition == 1)
            {
                lastLoginVersion = "0.0.0.0";
            }
            var lastVersion = new Version(lastLoginVersion);
            var newVersion = (Version)parameter;

            // only change Quickaction settings for version with newer version, not counting Revision or Build
            if (newVersion.Major < lastVersion.Major)
                return;

            if (lastVersion.Major == newVersion.Major && lastVersion.Minor >= newVersion.Minor)
                return;

            foreach (var profile in _settingsProvider.Settings.ConversionProfiles)
            {
                if (!profile.ShowQuickActions)
                {
                    profile.ShowQuickActions = true;
                    hasChanges = true;
                }
            }
            if (hasChanges)
            {
                _commandLocator.GetCommand<SaveChangedSettingsCommand>().Execute(null);
            }
        }

#pragma warning disable CS0067

        public event EventHandler CanExecuteChanged;

#pragma warning restore CS0067
    }
}
