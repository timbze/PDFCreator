using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.ServiceLocator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.Macros
{
    public class MacroControllerCommand : IMacroControllerCommand, IWhitelisted
    {
        protected List<ICommand> CommandsList;

        private int _runIndex = 0;
        private object _parameter;

        public event EventHandler MacrosAreDone;

        protected ICommandLocator Locator { get; set; }

        public MacroControllerCommand(ICommandLocator locator)
        {
            Locator = locator;
            CommandsList = new List<ICommand>();
        }

        public ICommand GetCommand(int index)
        {
            if (index >= CommandsList.Count)
                return null;

            return CommandsList[index];
        }

        public MacroControllerCommand AddCommand(ICommand command)
        {
            if (command == null)
                return this;

            CommandsList.Add(command);
            return this;
        }

        public MacroControllerCommand AddCommand<T>() where T : class, ICommand
        {
            var command = Locator?.GetCommand<T>();

            if (command == null)
                return this;

            CommandsList.Add(command);

            return this;
        }

        private void Next(object sender, MacroCommandIsDoneEventArgs e)
        {
            var previousCommand = sender as IMacroCommand;
            if (previousCommand != null)
                previousCommand.IsDone -= Next;

            if (e.ResponseStatus == ResponseStatus.Cancel || e.ResponseStatus == ResponseStatus.Error)
            {
                MacrosAreDone?.Invoke(this, EventArgs.Empty);
                return;
            }

            _runIndex++;

            if (CommandsList.Count > _runIndex)
            {
                var command = CommandsList.ElementAt(_runIndex);
                if (command is IMacroCommand)
                {
                    (command as IMacroCommand).IsDone += Next;
                    command.Execute(_parameter);
                }
                else
                {
                    command.Execute(_parameter);
                    Next(command, new MacroCommandIsDoneEventArgs(ResponseStatus.Success));
                }
            }
            else
            {
                MacrosAreDone?.Invoke(this, EventArgs.Empty);
            }
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
        public event EventHandler CanExecuteChanged;
    }

    public interface IMacroCommand : ICommand
    {
        //TODO convert to event
        EventHandler<MacroCommandIsDoneEventArgs> IsDone { get; set; }
    }

    public class MacroCommandIsDoneEventArgs : EventArgs
    {
        public ResponseStatus ResponseStatus { get; }

        public MacroCommandIsDoneEventArgs(ResponseStatus responseStatus)
        {
            ResponseStatus = responseStatus;
        }
    }

    public enum ResponseStatus
    {
        Success,
        Cancel,
        Error
    }
}
