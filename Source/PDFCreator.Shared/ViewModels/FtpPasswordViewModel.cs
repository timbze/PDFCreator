namespace pdfforge.PDFCreator.Shared.ViewModels
{
    public class FtpPasswordViewModel : ViewModelBase
    {
        public DelegateCommand OkCommand { get; protected set; }
        public DelegateCommand RemoveCommand { get; protected set; }
        public DelegateCommand SkipCommand { get; protected set; }

        public FtpPasswordResponse Response;
        private string _ftpPassword;

        public string FtpPassword
        {
            get { return _ftpPassword; }
            set { _ftpPassword = value; OkCommand.RaiseCanExecuteChanged(); }
        }

        public FtpPasswordViewModel()
        {
            Response = FtpPasswordResponse.Cancel;
            
            OkCommand = new DelegateCommand(ExecuteOk, CanExecuteOk);
            RemoveCommand = new DelegateCommand(ExecuteRemove);
            SkipCommand = new DelegateCommand(ExecuteSkip);
        }

        private void ExecuteSkip(object obj)
        {
            _ftpPassword = "";
            Response = FtpPasswordResponse.Skip;
            RaiseCloseView(true);
        }

        private void ExecuteRemove(object obj)
        {
            _ftpPassword = "";
            Response = FtpPasswordResponse.Remove;
            RaiseCloseView(true);
        }

        private bool CanExecuteOk(object obj)
        {
            return !string.IsNullOrEmpty(_ftpPassword); 
        }

        private void ExecuteOk(object obj)
        {
            Response = FtpPasswordResponse.OK;
            RaiseCloseView(true);
        }
    }

    public enum FtpPasswordResponse
    {
        OK,
        Remove,
        Skip,
        Cancel
    }
}
