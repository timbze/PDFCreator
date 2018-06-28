using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace pdfforge.PDFCreator.Core.Services.Macros
{
    public interface IMacroCommand : ICommand
    {
        event EventHandler MacroIsDone;

        ICommand GetCommand(int index);

        void ExecuteWithAsyncResult(object parameter, TaskCompletionSource<IMacroResult> resultTask);

        BooleanMacroResult ExecuteWithResult(object parameter);
    }
}
