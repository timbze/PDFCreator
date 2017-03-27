using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels.Translations;

namespace pdfforge.PDFCreator.UI.ViewModels.DialogViewModels
{
    public class SignaturePasswordViewModel : InteractionAwareViewModelBase<SignaturePasswordInteraction>
    {
        private readonly ISignaturePasswordCheck _signaturePasswordCheck;

        public SignaturePasswordViewModel(ISignaturePasswordCheck passwordCheck, SignaturePasswordWindowTranslation translation)
        {
            Translation = translation;
            _signaturePasswordCheck = passwordCheck;

            StorePasswordCommand = new DelegateCommand(ExecuteStorePasswordCommand, CanExecuteStorePasswordCommand);
            SkipCommand = new DelegateCommand(ExecuteSkipCommand);
            RemovePasswordCommand = new DelegateCommand(ExecuteRemovePasswordCommand);
        }

        public SignaturePasswordWindowTranslation Translation { get; }
        public DelegateCommand StorePasswordCommand { get; }
        public DelegateCommand SkipCommand { get; private set; }
        public DelegateCommand RemovePasswordCommand { get; private set; }

        public string Password
        {
            get { return Interaction.Password; }
            set
            {
                Interaction.Password = value;
                StorePasswordCommand.RaiseCanExecuteChanged();
            }
        }

        public bool CanSkip => Interaction?.MiddleButtonAction == PasswordMiddleButton.Skip;
        public bool CanRemovePassword => Interaction?.MiddleButtonAction == PasswordMiddleButton.Remove;

        private bool CanExecuteStorePasswordCommand(object o)
        {
            if (Interaction == null)
                return false;

            return _signaturePasswordCheck.IsValidPassword(Interaction.CertificateFile, Password);
        }

        private void ExecuteStorePasswordCommand(object o)
        {
            if (!CanExecuteStorePasswordCommand(o))
                throw new InvalidOperationException();

            Interaction.Result = PasswordResult.StorePassword;

            FinishInteraction();
        }

        private void ExecuteSkipCommand(object obj)
        {
            Interaction.Result = PasswordResult.Skip;
            Password = "";

            FinishInteraction();
        }

        private void ExecuteRemovePasswordCommand(object obj)
        {
            Interaction.Result = PasswordResult.RemovePassword;
            Password = "";

            FinishInteraction();
        }

        protected override void HandleInteractionObjectChanged()
        {
            RaisePropertyChanged(nameof(Password));
            RaisePropertyChanged(nameof(CanSkip));
            RaisePropertyChanged(nameof(CanRemovePassword));
        }
    }

    public interface ISignaturePasswordCheck
    {
        bool IsValidPassword(string certificateFile, string password);
    }

    public class SignaturePasswordCheck : ISignaturePasswordCheck
    {
        public bool IsValidPassword(string certificateFile, string password)
        {
            if ((password == null) || (certificateFile == null))
                return false;

            return IsValidCertificatePassword(certificateFile, password);
        }

        /// <summary>
        ///     This method returns true if a password for a given certificate file is valid.
        /// </summary>
        /// <param name="certficateFilename">Name of p12 or pfx certificate file.</param>
        /// <param name="certifcatePassword">A password string.</param>
        /// <returns>
        ///     True if the password is valid.
        /// </returns>
        private bool IsValidCertificatePassword(string certficateFilename, string certifcatePassword)
        {
            try
            {
                var cert = new X509Certificate2(certficateFilename, certifcatePassword);
                return true;
            }
            catch (CryptographicException)
            {
                return false;
            }
        }
    }
}