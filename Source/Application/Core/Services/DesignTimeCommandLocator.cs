using pdfforge.PDFCreator.Core.Services.Macros;
using System.Windows.Input;

namespace pdfforge.PDFCreator.Core.Services
{
    public class DesignTimeCommandLocator : ICommandLocator
    {
        public IMacroCommand GetMacroCommand()
        {
            return new MacroCommand(this);
        }

        public ICommand GetInitializedCommand<TCommand, TParameter>(TParameter parameter) where TCommand : class, IInitializedCommand<TParameter>
        {
            return null;
        }

        public ICommand GetCommand<T>() where T : class, ICommand
        {
            return null;
        }
    }
}
