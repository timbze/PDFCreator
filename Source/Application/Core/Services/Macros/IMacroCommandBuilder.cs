using System.Windows.Input;

namespace pdfforge.PDFCreator.Core.Services.Macros
{
    public interface IMacroCommandBuilder
    {
        IMacroCommand Build();

        IMacroCommandBuilder AddCommand(ICommand command);

        IMacroCommandBuilder AddCommand<T>() where T : class, ICommand;
    }
}
