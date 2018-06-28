using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Utilities.Process;
using System;

namespace pdfforge.PDFCreator.UI.Presentation.Commands
{
    public class UrlOpenCommand : IInitializedCommand<string>
    {
        private readonly IProcessStarter _processStarter;
        protected string Url { get; set; }

        public UrlOpenCommand(IProcessStarter processStarter)
        {
            _processStarter = processStarter;
        }

        public virtual bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var url = parameter as string ?? Url;

            try
            {
                _processStarter.Start(url);
            }
            catch
            {
                // ignored
            }
        }

        public void Init(string parameter)
        {
            Url = parameter;
        }

#pragma warning disable CS0067

        public event EventHandler CanExecuteChanged;

#pragma warning restore CS0067
    }
}
