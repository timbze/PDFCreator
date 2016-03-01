namespace pdfforge.PDFCreator.Shared.ViewModels
{
    public class SmtpPasswordViewModel : ViewModelBase
    {
        public DelegateCommand OkCommand { get; protected set; }
        public DelegateCommand RemoveCommand { get; protected set; }
        public DelegateCommand SkipCommand { get; protected set; }

        public SmtpPasswordResponse Response;
        private string _smtpPassword;

        public string SmtpPassword
        {
            get { return _smtpPassword; }
            set { _smtpPassword = value; OkCommand.RaiseCanExecuteChanged(); }
        }

        public SmtpPasswordViewModel()
        {
            Response = SmtpPasswordResponse.Cancel;

            OkCommand = new DelegateCommand(ExecuteOk, CanExecuteOk);
            RemoveCommand = new DelegateCommand(ExecuteRemove);
            SkipCommand = new DelegateCommand(ExecuteSkip);
        }

        private void ExecuteSkip(object obj)
        {
            _smtpPassword = "";
            Response = SmtpPasswordResponse.Skip;
            RaiseCloseView(true);
        }

        private void ExecuteRemove(object obj)
        {
            _smtpPassword = "";
            Response = SmtpPasswordResponse.Remove;
            RaiseCloseView(true);
        }

        private bool CanExecuteOk(object obj)
        {
            return !string.IsNullOrEmpty(_smtpPassword);
        }

        private void ExecuteOk(object obj)
        {
            Response = SmtpPasswordResponse.OK;
            RaiseCloseView(true);
        }
    }

    public enum SmtpPasswordResponse
    {
        OK,
        Remove,
        Skip,
        Cancel
    }
}
