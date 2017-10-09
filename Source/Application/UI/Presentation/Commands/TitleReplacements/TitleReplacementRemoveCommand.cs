using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using System;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.Commands.TitleReplacements
{
    public class TitleReplacementRemoveCommand : ICommand
    {
        private readonly ICurrentSettingsProvider _settingsProvider;

        public TitleReplacementRemoveCommand(ICurrentSettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider;
        }

        public bool CanExecute(object parameter)
        {
            return parameter is TitleReplacement;
        }

        public void Execute(object parameter)
        {
            _settingsProvider.Settings.ApplicationSettings.TitleReplacement.Remove(parameter as TitleReplacement);
        }

#pragma warning disable CS0067

        public event EventHandler CanExecuteChanged;

#pragma warning restore CS0067
    }
}
