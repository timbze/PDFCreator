using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    public class PdfJobStepPasswordViewModel : JobStepPasswordViewModelBase<PdfPasswordTranslation>
    {
        private string _ownerPassword;
        private string _userPassword;
        private bool _askOwnerPassword = true;
        private bool _askUserPassword = true;

        public bool AskOwnerPassword
        {
            get { return _askOwnerPassword; }
            set { SetProperty(ref _askOwnerPassword, value); }
        }

        public bool AskUserPassword
        {
            get { return _askUserPassword; }
            set { SetProperty(ref _askUserPassword, value); }
        }

        public string OwnerPassword
        {
            get { return _ownerPassword; }
            set
            {
                SetProperty(ref _ownerPassword, value);
                PasswordButtonController.PrintJobPasswordButtonViewModel.OkCommand.RaiseCanExecuteChanged();
            }
        }

        public string UserPassword
        {
            get { return _userPassword; }
            set
            {
                SetProperty(ref _userPassword, value);
                PasswordButtonController.PrintJobPasswordButtonViewModel.OkCommand.RaiseCanExecuteChanged();
            }
        }

        public Action<string, string> SetPasswordsInUi { get; set; }

        public PdfJobStepPasswordViewModel(ITranslationUpdater translationUpdater) : base(translationUpdater, string.Empty)
        {
        }

        protected override void ExecuteWorkflow()
        {
            var securitySettings = Job.Profile.PdfSettings.Security;
            AskOwnerPassword = securitySettings.OwnerPassword == "";
            AskUserPassword = securitySettings.RequireUserPassword && securitySettings.UserPassword == "";
            SetPasswordsInUi?.Invoke(OwnerPassword, UserPassword);
        }

        protected override void ReadPassword()
        {
            var securitySettings = Job.Profile.PdfSettings.Security;
            OwnerPassword = securitySettings.OwnerPassword;
            UserPassword = securitySettings.UserPassword;
        }

        public override bool CanExecuteHook()
        {
            if (AskOwnerPassword)
                if (string.IsNullOrWhiteSpace(OwnerPassword))
                    return false;

            if (AskUserPassword)
                if (string.IsNullOrWhiteSpace(UserPassword))
                    return false;

            return true;
        }

        public override void SkipHook()
        {
            Job.Profile.PdfSettings.Security.Enabled = false;
        }

        public override void FinishedHook()
        {
            Job.Passwords.PdfUserPassword = UserPassword;
            Job.Passwords.PdfOwnerPassword = OwnerPassword;

            base.FinishedHook();
        }

        public override void ClearPasswordFields()
        {
            UserPassword = string.Empty;
            OwnerPassword = string.Empty;
        }

        protected override string GetCancelErrorMessage()
        {
            return "User cancelled in the PdfPasswordView";
        }
    }
}
