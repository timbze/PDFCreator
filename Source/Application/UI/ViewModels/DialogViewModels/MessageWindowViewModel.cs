using System.Media;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels.Translations;
using pdfforge.PDFCreator.UI.ViewModels.Helper;

namespace pdfforge.PDFCreator.UI.ViewModels.DialogViewModels
{
    public class MessageWindowViewModel : InteractionAwareViewModelBase<MessageInteraction>
    {
        private readonly ISoundPlayer _soundPlayer;
        private readonly MessageWindowTranslation _translation;

        public MessageWindowViewModel(MessageWindowTranslation translation, ISoundPlayer soundPlayer)
        {
            _translation = translation;
            _soundPlayer = soundPlayer;
            ButtonRightCommand = new DelegateCommand(ExecuteButtonRight, CanExecuteButtonRight);
            ButtonLeftCommand = new DelegateCommand(ExecuteButtonLeft);
        }

        public DelegateCommand ButtonLeftCommand { get; private set; }
        public DelegateCommand ButtonRightCommand { get; }

        public MessageIcon Icon { get; set; }
        public int IconSize { get; set; } = 32;

        public string LeftButtonContent { get; set; }
        public string MiddleButtonContent { get; set; }
        public string RightButtonContent { get; set; }

        private void ExecuteButtonRight(object obj)
        {
            switch (Interaction.Buttons)
            {
                case MessageOptions.OKCancel:
                case MessageOptions.RetryCancel:
                case MessageOptions.MoreInfoCancel:
                    Interaction.Response = MessageResponse.Cancel;
                    FinishInteraction();
                    break;
                case MessageOptions.YesNo:
                    Interaction.Response = MessageResponse.No;
                    FinishInteraction();
                    break;
            }
        }

        private void ExecuteButtonLeft(object obj)
        {
            switch (Interaction.Buttons)
            {
                case MessageOptions.OK:
                    Interaction.Response = MessageResponse.OK;
                    break;
                case MessageOptions.MoreInfoCancel:
                    Interaction.Response = MessageResponse.MoreInfo;
                    break;
                case MessageOptions.OKCancel:
                    Interaction.Response = MessageResponse.OK;
                    break;
                case MessageOptions.RetryCancel:
                    Interaction.Response = MessageResponse.Retry;
                    break;
                case MessageOptions.YesNo:
                    Interaction.Response = MessageResponse.Yes;
                    break;
            }
            FinishInteraction();
        }

        private bool CanExecuteButtonRight(object obj)
        {
            return Interaction?.Buttons != MessageOptions.OK;
        }

        protected override void HandleInteractionObjectChanged()
        {
            Interaction.Buttons = Interaction.Buttons;
            SetButtonContent(Interaction.Buttons);

            ButtonRightCommand.RaiseCanExecuteChanged();

            SetIcon(Interaction.Icon);
        }

        private void SetIcon(MessageIcon icon)
        {
            Icon = icon;
            switch (icon)
            {
                case MessageIcon.Error:
                    _soundPlayer.Play(SystemSounds.Hand);
                    break;
                case MessageIcon.Exclamation:
                    _soundPlayer.Play(SystemSounds.Exclamation);
                    break;
                case MessageIcon.Info:
                    _soundPlayer.Play(SystemSounds.Asterisk);
                    break;
                case MessageIcon.Question:
                    _soundPlayer.Play(SystemSounds.Question);
                    break;
                case MessageIcon.Warning:
                    _soundPlayer.Play(SystemSounds.Exclamation);
                    break;
                case MessageIcon.PDFCreator:
                    IconSize = 45;
                    break;
                case MessageIcon.PDFForge:
                    IconSize = 45;
                    break;
            }

            RaisePropertyChanged(nameof(Icon));
            RaisePropertyChanged(nameof(IconSize));
        }

        private void SetButtonContent(MessageOptions option)
        {
            switch (option)
            {
                case MessageOptions.MoreInfoCancel:
                    LeftButtonContent = _translation.MoreInfo;
                    RightButtonContent = _translation.Cancel;
                    break;
                case MessageOptions.OK:
                    LeftButtonContent = _translation.Ok;
                    break;
                case MessageOptions.OKCancel:
                    LeftButtonContent = _translation.Ok;
                    RightButtonContent = _translation.Cancel;
                    break;
                case MessageOptions.RetryCancel:
                    LeftButtonContent = _translation.Retry;
                    RightButtonContent = _translation.Cancel;
                    break;
                case MessageOptions.YesNo:
                    LeftButtonContent = _translation.Yes;
                    RightButtonContent = _translation.No;
                    break;
            }

            RaisePropertyChanged(nameof(RightButtonContent));
            RaisePropertyChanged(nameof(MiddleButtonContent));
            RaisePropertyChanged(nameof(LeftButtonContent));
        }
    }
}