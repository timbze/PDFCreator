using System.Windows.Input;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels.Translations;
using pdfforge.PDFCreator.Utilities.Process;

namespace pdfforge.PDFCreator.UI.ViewModels.DialogViewModels
{
    public class UpdateAvailableViewModel : InteractionAwareViewModelBase<UpdateAvailableInteraction>
    {
        public UpdateManagerTranslation Translation { get; }
        private readonly IProcessStarter _processStarter;
        private readonly ApplicationNameProvider _applicationNameProvider;

        public UpdateAvailableViewModel(UpdateManagerTranslation translation, IProcessStarter processStarter, ApplicationNameProvider applicationNameProvider)
        {
            Translation = translation;
            _processStarter = processStarter;
            _applicationNameProvider = applicationNameProvider;
        }

        public string Title => Translation.GetFormattedTitle(_applicationNameProvider.ApplicationName);

        public string Text => Interaction != null ?  Translation.GetNewUpdateMessage(Interaction.AvailableVersion) : "";


        public ICommand InstallUpdateCommand => new DelegateCommand(o => Close(UpdateAvailableResponse.Install));
        public ICommand SkipVersionCommand => new DelegateCommand(o => Close(UpdateAvailableResponse.Skip));
        public ICommand AskLaterCommand => new DelegateCommand(o => Close(UpdateAvailableResponse.Later));
        public ICommand WhatsNewCommand => new DelegateCommand(o => _processStarter.Start(Interaction.WhatsNewUrl));

        protected override void HandleInteractionObjectChanged()
        {
            RaisePropertyChanged(nameof(Text));
            RaisePropertyChanged(nameof(Title));
        }

        private void Close(UpdateAvailableResponse response)
        {
            Interaction.Response = response;
            FinishInteraction();
        }
    }
}
