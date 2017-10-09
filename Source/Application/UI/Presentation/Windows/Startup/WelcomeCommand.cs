using System;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.Windows.Startup
{
    public abstract class WelcomeCommand : ICommand
    {
        public void Execute(object parameter)
        {
            ExecuteWelcomeAction();
        }

        protected abstract void ExecuteWelcomeAction();

        public bool CanExecute(object parameter)
        {
            return true;
        }

#pragma warning disable CS0067

        public event EventHandler CanExecuteChanged;

#pragma warning restore CS0067
    }
}
