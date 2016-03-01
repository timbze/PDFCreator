namespace pdfforge.PDFCreator.Shared.ViewModels
{
    public class EncryptionPasswordViewModelwithRemove : ViewModelBase
    {
        public DelegateCommand OkCommand { get; protected set; }
        public DelegateCommand RemoveCommand { get; protected set; }
        public DelegateCommand SkipCommand { get; protected set; }

        public EncryptionPasswordResponse Response;
        private string _ownerPassword;
        private string _userPassword;

        public bool AskOwnerPassword { get; set; }
        public bool AskUserPassword { get; set; }

        public string OwnerPassword
        {
            get { return _ownerPassword; }
            set { _ownerPassword = value; OkCommand.RaiseCanExecuteChanged(); }
        }

        public string UserPassword
        {
            get { return _userPassword; }
            set { _userPassword = value; OkCommand.RaiseCanExecuteChanged(); }
        }

        public EncryptionPasswordViewModelwithRemove()
            : this(true)
        {

        }

        public EncryptionPasswordViewModelwithRemove(bool askUserPassword)
        {
            Response = EncryptionPasswordResponse.Cancel;
            
            AskOwnerPassword = true;
            AskUserPassword = askUserPassword;
            
            OkCommand = new DelegateCommand(ExecuteOk, CanExecuteOk);
            RemoveCommand = new DelegateCommand(ExecuteRemove);
            SkipCommand = new DelegateCommand(ExecuteSkip);
        }

        protected void ExecuteOk(object obj)
        {
            Response = EncryptionPasswordResponse.OK;
            RaiseCloseView(true);
        }

        protected virtual bool CanExecuteOk(object obj)
        {
            return true;
        }

        private void ExecuteSkip(object obj)
        {
            OwnerPassword = "";
            UserPassword = "";
            Response = EncryptionPasswordResponse.Skip;
            RaiseCloseView(true);
        }

        private void ExecuteRemove(object obj)
        {
            OwnerPassword = "";
            UserPassword = "";
            Response = EncryptionPasswordResponse.Remove;
            RaiseCloseView(true);
        }
    }

    public class EncryptionPasswordViewModelwithSkip : EncryptionPasswordViewModelwithRemove
    {
        public EncryptionPasswordViewModelwithSkip(bool askOwnerPassword, bool askUserPassword)
        {
            AskOwnerPassword = askOwnerPassword;
            AskUserPassword = askUserPassword;
        }

        protected override bool CanExecuteOk(object obj)
        {
            if (AskOwnerPassword)
                if (string.IsNullOrEmpty(OwnerPassword))
                    return false;
            
            if (AskUserPassword)
                if (string.IsNullOrEmpty(UserPassword))
                    return false;

            return true;
        }
    }

    public enum EncryptionPasswordResponse
    {
        OK,
        Remove,
        Skip,
        Cancel
    }
}
