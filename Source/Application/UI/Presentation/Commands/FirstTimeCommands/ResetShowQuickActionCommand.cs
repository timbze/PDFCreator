using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using System;

namespace pdfforge.PDFCreator.UI.Presentation.Commands.FirstTimeCommands
{
    public class ResetShowQuickActionCommand : IFirstTimeCommand
    {
        private readonly ICurrentSettingsProvider _settings;
        private readonly ICommandLocator _commandLocator;

        public ResetShowQuickActionCommand(ICurrentSettingsProvider settings, ICommandLocator commandLocator)
        {
            _settings = settings;
            _commandLocator = commandLocator;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var hasChanges = false;

            foreach (var profile in _settings.Settings.ConversionProfiles)
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
