using pdfforge.Obsidian;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System.Media;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Misc
{
    public class MessageUserControlViewModel : OverlayViewModelBase<MessageInteraction, MessageWindowTranslation>
    {
        private readonly ISoundPlayer _soundPlayer;

        public MessageUserControlViewModel(ITranslationUpdater translationUpdater, ISoundPlayer soundPlayer)
            : base(translationUpdater)
        {
            _soundPlayer = soundPlayer;
            ButtonRightCommand = new DelegateCommand(ExecuteButtonRight, CanExecuteButtonRight);
            ButtonLeftCommand = new DelegateCommand(ExecuteButtonLeft);
            ButtonMiddleCommand = new DelegateCommand(ExecuteButtonMiddle);
        }

        public DelegateCommand ButtonLeftCommand { get; private set; }
        public DelegateCommand ButtonRightCommand { get; }
        public DelegateCommand ButtonMiddleCommand { get; }

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
                    break;

                case MessageOptions.YesNo:
                    Interaction.Response = MessageResponse.No;
                    break;

                case MessageOptions.YesNoCancel:
                    Interaction.Response = MessageResponse.Cancel;
                    break;
            }
            FinishInteraction();
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

                case MessageOptions.YesNoCancel:
                    Interaction.Response = MessageResponse.Yes;
                    break;
            }
            FinishInteraction();
        }

        private void ExecuteButtonMiddle(object obj)
        {
            switch (Interaction.Buttons)
            {
                case MessageOptions.YesNoCancel:
                    Interaction.Response = MessageResponse.No;
                    break;
            }
            FinishInteraction();
        }

        public bool ShowMiddleButton => Interaction?.Buttons == MessageOptions.YesNoCancel;

        private bool CanExecuteButtonRight(object obj)
        {
            return Interaction?.Buttons != MessageOptions.OK;
        }

        protected override void HandleInteractionObjectChanged()
        {
            SetButtonContent(Interaction.Buttons);

            ButtonRightCommand.RaiseCanExecuteChanged();

            SetIcon(Interaction.Icon);
            RaisePropertyChanged(nameof(ShowMiddleButton));
        }

        public override string Title => Interaction.Title;

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
                    LeftButtonContent = Translation.MoreInfo;
                    RightButtonContent = Translation.Cancel;
                    break;

                case MessageOptions.OK:
                    LeftButtonContent = Translation.Ok;
                    break;

                case MessageOptions.OKCancel:
                    LeftButtonContent = Translation.Ok;
                    RightButtonContent = Translation.Cancel;
                    break;

                case MessageOptions.RetryCancel:
                    LeftButtonContent = Translation.Retry;
                    RightButtonContent = Translation.Cancel;
                    break;

                case MessageOptions.YesNo:
                    LeftButtonContent = Translation.Yes;
                    RightButtonContent = Translation.No;
                    break;

                case MessageOptions.YesNoCancel:
                    LeftButtonContent = Translation.Yes;
                    RightButtonContent = Translation.Cancel;
                    MiddleButtonContent = Translation.No;
                    break;
            }

            RaisePropertyChanged(nameof(RightButtonContent));
            RaisePropertyChanged(nameof(MiddleButtonContent));
            RaisePropertyChanged(nameof(LeftButtonContent));
        }
    }
}
