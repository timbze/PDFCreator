using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace pdfforge.PDFCreator.Core.Services.Macros
{
    public class MacroCommand : IMacroCommand
    {
        protected readonly List<ICommand> CommandsList = new List<ICommand>();
        private int _runIndex = 0;
        private object _parameter;
        private BooleanMacroResult _result;
        private TaskCompletionSource<IMacroResult> _resultTask;

        public event EventHandler MacroIsDone;

        protected ICommandLocator Locator { get; set; }

        public MacroCommand(ICommandLocator locator)
        {
            Locator = locator;
        }

        public IMacroCommand AddCommand(ICommand command)
        {
            if (command == null)
                return this;

            command.CanExecuteChanged += (sender, args) => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            CommandsList.Add(command);
            return this;
        }

        public ICommand GetCommand(int index)
        {
            if (index > CommandsList.Count)
                return null;
            return CommandsList[index];
        }

        public IMacroCommand AddCommand<T>() where T : class, ICommand
        {
            var command = Locator?.GetCommand<T>();
            AddCommand(command);
            return this;
        }

        public void ExecuteWithAsyncResult(object parameter, TaskCompletionSource<IMacroResult> resultTask)
        {
            _resultTask = resultTask;
            _result = new BooleanMacroResult(false);
            Execute(parameter);
        }

        public BooleanMacroResult ExecuteWithResult(object parameter)
        {
            _result = new BooleanMacroResult(false);
            Execute(parameter);
            return _result;
        }

        private void Next(object sender, MacroCommandIsDoneEventArgs e)
        {
            var previousCommand = sender as IWaitableCommand;

            if (previousCommand != null)
                previousCommand.IsDone -= Next;

            if (e.ResponseStatus == ResponseStatus.Cancel || e.ResponseStatus == ResponseStatus.Error)
            {
                InvokeAllDone(false);
                return;
            }

            _runIndex++;

            if (e.ResponseStatus == ResponseStatus.Skip)
            {
                Next(sender, new MacroCommandIsDoneEventArgs(ResponseStatus.Success));
            }

            if (CommandsList.Count > _runIndex)
            {
                var command = CommandsList.ElementAt(_runIndex);
                var macroCommand = command as IWaitableCommand;
                if (macroCommand != null)
                {
                    macroCommand.IsDone += Next;
                    macroCommand.Execute(_parameter);
                }
                else
                {
                    command.Execute(_parameter);
                    Next(command, new MacroCommandIsDoneEventArgs(ResponseStatus.Success));
                }
            }
            else
            {
                InvokeAllDone(true);
            }
        }

        private void InvokeAllDone(bool success)
        {
            _result?.SetResult(success);
            MacroIsDone?.Invoke(this, new MacroAreDoneEventArgs(_result));
            _resultTask?.SetResult(_result);
        }

        public bool CanExecute(object parameter)
        {
            foreach (var command in CommandsList)
            {
                if (!command.CanExecute(parameter))
                    return false;
            }
            return true;
        }

        public void Execute(object parameter)
        {
            _parameter = parameter;
            _runIndex = -1;

            Next(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Success));
        }

        // has to be implemented because of ICommand
#pragma warning disable 67

        public event EventHandler CanExecuteChanged;

#pragma warning restore 67
    }

    public enum ResponseStatus
    {
        Success,
        Cancel,
        Skip,
        Error
    }
}
