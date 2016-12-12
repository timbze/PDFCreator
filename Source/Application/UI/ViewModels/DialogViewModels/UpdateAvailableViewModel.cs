using System.Windows.Input;
using pdfforge.DynamicTranslator;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.Utilities.Process;

namespace pdfforge.PDFCreator.UI.ViewModels.DialogViewModels
{
    public class UpdateAvailableViewModel : InteractionAwareViewModelBase<UpdateAvailableInteraction>
    {
        public ITranslator Translator { get; }
        private readonly IProcessStarter _processStarter;
        private readonly ApplicationNameProvider _applicationNameProvider;

        public UpdateAvailableViewModel(ITranslator translator, IProcessStarter processStarter, ApplicationNameProvider applicationNameProvider)
        {
            Translator = translator;
            _processStarter = processStarter;
            _applicationNameProvider = applicationNameProvider;
        }

        public string Title => Translator?.GetFormattedTranslation("UpdateManager", "ApplicationUpdate", _applicationNameProvider.ApplicationName) ?? "PDFCreator Update";

        public string Text => Translator?.GetFormattedTranslation("UpdateManager", "NewUpdateMessage",
            Interaction.AvailableVersion) ?? "new version available";

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
