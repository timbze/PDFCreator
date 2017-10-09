using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace pdfforge.PDFCreator.Core.Services.Macros
{
    public interface IMacroCommand : ICommand
    {
        event EventHandler MacroIsDone;

        IMacroCommand AddCommand(ICommand command);

        ICommand GetCommand(int index);

        IMacroCommand AddCommand<T>() where T : class, ICommand;

        void ExecuteWithAsyncResult(object parameter, TaskCompletionSource<IMacroResult> resultTask);

        BooleanMacroResult ExecuteWithResult(object parameter);
    }
}
