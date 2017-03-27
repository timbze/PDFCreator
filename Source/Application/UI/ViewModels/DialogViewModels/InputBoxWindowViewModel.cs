using System.Windows.Controls;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels.Translations;

namespace pdfforge.PDFCreator.UI.ViewModels.DialogViewModels
{
    public class InputBoxWindowViewModel : InteractionAwareViewModelBase<InputInteraction>
    {
        public InputBoxWindowTranslation Translation { get; }
        private string _validationMessage;

        public InputBoxWindowViewModel(InputBoxWindowTranslation translation)
        {
            Translation = translation;
            TextChangedCommand = new DelegateCommand<TextChangedEventArgs>(OnInputTextChanged);
            ConfirmDialogCommand = new DelegateCommand(ConfirmTextInput, CanConfirmTextInput);
            CancelDialogCommand = new DelegateCommand(CancelInputDialog);
        }

        public DelegateCommand<TextChangedEventArgs> TextChangedCommand { get; }
        public DelegateCommand ConfirmDialogCommand { get; }
        public DelegateCommand CancelDialogCommand { get; }

        public string ValidationMessage
        {
            get { return _validationMessage; }
            set
            {
                _validationMessage = value;
                RaisePropertyChanged(nameof(ValidationMessage));
            }
        }

        private void OnInputTextChanged(TextChangedEventArgs textChangedEventArgs)
        {
            if (Interaction.IsValidInput == null)
                return;

            ConfirmDialogCommand.RaiseCanExecuteChanged();
        }

        private void CancelInputDialog(object o)
        {
            Interaction.Success = false;
            FinishInteraction();
        }

        private bool CanConfirmTextInput(object o)
        {
            if (Interaction?.IsValidInput == null)
                return true;

            var validation = Interaction.IsValidInput(Interaction.InputText);
            ValidationMessage = validation.Message;
            return validation.IsValid;
        }

        private void ConfirmTextInput(object o)
        {
            Interaction.Success = true;
            FinishInteraction();
        }
    }
}