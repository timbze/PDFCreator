using System;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels.Translations;

namespace pdfforge.PDFCreator.UI.ViewModels.DialogViewModels
{
    public class PasswordViewModel : InteractionAwareViewModelBase<PasswordInteraction>
    {
        private  PasswordWindowTranslation _translation;

        public PasswordViewModel(PasswordWindowTranslation translation)
        {
            _translation = translation;
            OkCommand = new DelegateCommand(ExecuteOk, CanExecuteOk);
            RemoveCommand = new DelegateCommand(ExecuteRemove);
            SkipCommand = new DelegateCommand(ExecuteSkip);
        }

        public DelegateCommand OkCommand { get; protected set; }
        public DelegateCommand RemoveCommand { get; protected set; }
        public DelegateCommand SkipCommand { get; protected set; }

        public string Password
        {
            set
            {
                Interaction.Password = value;
                OkCommand.RaiseCanExecuteChanged();
            }
        }

        public PasswordWindowTranslation Translation
        {
            get { return _translation; }
            set { _translation = value; RaisePropertyChanged(nameof(Translation)); }
        }

        public bool ShowIntroText => !string.IsNullOrWhiteSpace(Interaction?.IntroText);

        public bool CanSkip => Interaction?.MiddleButtonAction == PasswordMiddleButton.Skip;
        public bool CanRemovePassword => Interaction?.MiddleButtonAction == PasswordMiddleButton.Remove;

        public Action<string> SetPasswordAction { get; set; }

        protected override void HandleInteractionObjectChanged()
        {
            SetPasswordAction?.Invoke(Interaction.Password);
            OkCommand.RaiseCanExecuteChanged();

            RaisePropertyChanged(nameof(CanSkip));
            RaisePropertyChanged(nameof(CanRemovePassword));
            RaisePropertyChanged(nameof(ShowIntroText));
        }

        private void ExecuteSkip(object obj)
        {
            Interaction.Password = "";
            Interaction.Result = PasswordResult.Skip;
            FinishInteraction();
        }

        private void ExecuteRemove(object obj)
        {
            Interaction.Password = "";
            Interaction.Result = PasswordResult.RemovePassword;
            FinishInteraction();
        }

        private bool CanExecuteOk(object obj)
        {
            return !string.IsNullOrEmpty(Interaction?.Password);
        }

        private void ExecuteOk(object obj)
        {
            Interaction.Result = PasswordResult.StorePassword;
            FinishInteraction();
        }
    }
}