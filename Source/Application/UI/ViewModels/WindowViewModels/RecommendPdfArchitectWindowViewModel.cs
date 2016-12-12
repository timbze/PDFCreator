using System.Media;
using System.Windows.Input;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.Utilities.Process;

namespace pdfforge.PDFCreator.UI.ViewModels.WindowViewModels
{
    public class RecommendPdfArchitectWindowViewModel : InteractionAwareViewModelBase<RecommendPdfArchitectInteraction>
    {
        private readonly IProcessStarter _processStarter;
        private readonly ISoundPlayer _soundPlayer;

        // ReSharper disable once MemberCanBeProtected.Global
        public RecommendPdfArchitectWindowViewModel(ISoundPlayer soundPlayer, IProcessStarter processStarter)
        {
            _soundPlayer = soundPlayer;
            _processStarter = processStarter;

            InfoCommand = new DelegateCommand(ExecuteInfo);
            DownloadCommand = new DelegateCommand(ExecuteDownload);
        }

        public ICommand InfoCommand { get; }

        public ICommand DownloadCommand { get; }

        private void ExecuteInfo(object o)
        {
            _processStarter.Start(Urls.ArchitectWebsiteUrl);
            FinishInteraction();
        }

        private void ExecuteDownload(object o)
        {
            _processStarter.Start(Urls.ArchitectDownloadUrl);
            FinishInteraction();
        }

        protected override void HandleInteractionObjectChanged()
        {
            if (Interaction.ShowViewerWarning)
            {
                _soundPlayer.Play(SystemSounds.Hand);
            }
        }
    }
}