using System;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels.Translations;

namespace pdfforge.PDFCreator.UI.ViewModels.DialogViewModels
{
    public class EncryptionPasswordViewModel : InteractionAwareViewModelBase<EncryptionPasswordInteraction>
    {
        private EncryptionPasswordsWindowTranslation _translation;
        public Action<string, string> SetPasswordInUi;

        public EncryptionPasswordViewModel(EncryptionPasswordsWindowTranslation translation)
        {
            _translation = translation;
            OkCommand = new DelegateCommand(ExecuteOk, CanExecuteOk);
            RemoveCommand = new DelegateCommand(ExecuteRemove);
            SkipCommand = new DelegateCommand(ExecuteSkip);
        }

        public DelegateCommand OkCommand { get; protected set; }
        public DelegateCommand RemoveCommand { get; protected set; }
        public DelegateCommand SkipCommand { get; protected set; }

        public string OwnerPassword
        {
            set
            {
                Interaction.OwnerPassword = value;
                OkCommand.RaiseCanExecuteChanged();
            }
        }

        public string UserPassword
        {
            set
            {
                Interaction.UserPassword = value;
                OkCommand.RaiseCanExecuteChanged();
            }
        }

        public EncryptionPasswordsWindowTranslation Translation
        {
            get { return _translation; }
            set { _translation = value; RaisePropertyChanged(nameof(Translation)); }
        }

        protected override void HandleInteractionObjectChanged()
        {
            SetPasswordInUi?.Invoke(Interaction.OwnerPassword, Interaction.UserPassword);
            OkCommand.RaiseCanExecuteChanged();
        }

        private void ExecuteOk(object obj)
        {
            Interaction.Response = PasswordResult.StorePassword;
            FinishInteraction();
        }

        private bool CanExecuteOk(object obj)
        {
            if (Interaction == null)
                return false;

            if (!Interaction.Skip)
                return true;

            if (Interaction.AskOwnerPassword)
                if (string.IsNullOrWhiteSpace(Interaction.OwnerPassword))
                    return false;

            if (Interaction.AskUserPassword)
                if (string.IsNullOrWhiteSpace(Interaction.UserPassword))
                    return false;

            return true;
        }

        private void ExecuteSkip(object obj)
        {
            Interaction.OwnerPassword = "";
            Interaction.UserPassword = "";
            Interaction.Response = PasswordResult.Skip;
            FinishInteraction();
        }

        private void ExecuteRemove(object obj)
        {
            Interaction.OwnerPassword = "";
            Interaction.UserPassword = "";
            Interaction.Response = PasswordResult.RemovePassword;
            FinishInteraction();
        }
    }
}