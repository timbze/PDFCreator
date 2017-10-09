using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Controller.Routing;
using pdfforge.PDFCreator.UI.Presentation.Routing;
using pdfforge.PDFCreator.UI.Presentation.UserControls;
using System;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.Commands.ProfileCommands
{
    public class OpenProfileCommand : ICommand
    {
        private readonly IMainWindowThreadLauncher _mainWindowThreadLauncher;

        public OpenProfileCommand(IMainWindowThreadLauncher mainWindowThreadLauncher)
        {
            _mainWindowThreadLauncher = mainWindowThreadLauncher;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var startup = new StartupRoutine();
            startup.AddAction(new StartupNavigationAction(RegionNames.MainRegion, MainRegionViewNames.ProfilesView));
            _mainWindowThreadLauncher.LaunchMainWindow(startup);
        }

#pragma warning disable 67

        public event EventHandler CanExecuteChanged;

#pragma warning restore 67
    }
}
