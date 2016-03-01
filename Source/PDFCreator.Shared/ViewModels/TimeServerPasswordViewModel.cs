namespace pdfforge.PDFCreator.Shared.ViewModels
{
    public class TimeServerPasswordViewModel : ViewModelBase
    {
        public DelegateCommand OkCommand { get; protected set; }
        public DelegateCommand RemoveCommand { get; protected set; }

        public TimeServerPasswordResponse Response;

        private string _timeServerPassword;
        public string TimeServerPassword
        {
            get { return _timeServerPassword; }
            set { _timeServerPassword = value; OkCommand.RaiseCanExecuteChanged(); }
        }

        private string _timeServerLoginName;
        public string TimeServerLoginName
        {
            get { return _timeServerLoginName; }
            set { _timeServerLoginName = value; OkCommand.RaiseCanExecuteChanged(); }
        }

        public TimeServerPasswordViewModel()
        {
            Response = TimeServerPasswordResponse.Cancel;

            OkCommand = new DelegateCommand(ExecuteOk, CanExecuteOk);
            RemoveCommand = new DelegateCommand(ExecuteRemove);
        }

        private void ExecuteRemove(object obj)
        {
            _timeServerPassword = "";
            _timeServerLoginName = "";
            Response = TimeServerPasswordResponse.Remove;
            RaiseCloseView(true);
        }

        private bool CanExecuteOk(object obj)
        {
            return !string.IsNullOrEmpty(_timeServerPassword) 
                && !string.IsNullOrEmpty(_timeServerLoginName);
        }

        private void ExecuteOk(object obj)
        {
            Response = TimeServerPasswordResponse.OK;
            RaiseCloseView(true);
        }
    }

    public enum TimeServerPasswordResponse
    {
        OK,
        Remove,
        Cancel
    }
}

