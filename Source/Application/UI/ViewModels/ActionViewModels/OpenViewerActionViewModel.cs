using pdfforge.Obsidian;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.ViewModels.ActionViewModels.Translations;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Process;

namespace pdfforge.PDFCreator.UI.ViewModels.ActionViewModels
{
    public class OpenViewerActionViewModel : ActionViewModel
    {
        private readonly IInteractionInvoker _interactionInvoker;
        private readonly IPdfArchitectCheck _pdfArchitectCheck;
        private readonly IProcessStarter _processStarter;
        private readonly ISettingsProvider _settingsProvider;

        public OpenViewerActionViewModel(OpenViewerSettingsAndActionTranslation translation, IInteractionInvoker interactionInvoker, IPdfArchitectCheck pdfArchitectCheck,
            ISettingsProvider settingsProvider, IProcessStarter processStarter)
        {
            Translation = translation;
            _interactionInvoker = interactionInvoker;
            _pdfArchitectCheck = pdfArchitectCheck;
            _settingsProvider = settingsProvider;
            _processStarter = processStarter;

            DisplayName = Translation.DisplayName;
            Description = Translation.Description;
        }

        public OpenViewerSettingsAndActionTranslation Translation { get; }

        public override bool IsEnabled
        {
            get { return CurrentProfile != null && CurrentProfile.OpenViewer; }
            set
            {
                CurrentProfile.OpenViewer = value;
                RaisePropertyChanged(nameof(IsEnabled));
            }
        }

        public DelegateCommand OpenWithArchictectCommand => new DelegateCommand(OpenWithArchictectExecute);
        public DelegateCommand GetArchictectCommand => new DelegateCommand(GetArchictectExecute);

        public bool ShowArchitect => !_settingsProvider.GpoSettings.HidePdfArchitectInfo;

        private void OpenWithArchictectExecute(object obj)
        {
            if (!CurrentProfile.OpenWithPdfArchitect)
                return;

            if (_pdfArchitectCheck.IsInstalled())
                return;

            const string caption = "PDF Architect";
            var message = Translation.ArchitectNotInstalled;

            var interaction = new MessageInteraction(message, caption, MessageOptions.YesNo, MessageIcon.PDFForge);
            _interactionInvoker.Invoke(interaction);

            if (interaction.Response == MessageResponse.Yes)
            {
                _processStarter.Start(Urls.ArchitectDownloadUrl);
            }

            CurrentProfile.OpenWithPdfArchitect = false;
            RaisePropertyChanged(nameof(CurrentProfile));
        }

        private void GetArchictectExecute(object obj)
        {
            _processStarter.Start(Urls.ArchitectWebsiteUrl);
        }

        public override HelpTopic GetContextBasedHelpTopic()
        {
            return HelpTopic.OpenViewer;
        }
    }
}