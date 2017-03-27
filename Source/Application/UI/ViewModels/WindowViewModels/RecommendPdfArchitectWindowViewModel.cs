using System.Media;
using System.Windows.Input;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels.Translations;
using pdfforge.PDFCreator.Utilities.Process;

namespace pdfforge.PDFCreator.UI.ViewModels.WindowViewModels
{
    public class RecommendPdfArchitectWindowViewModel : InteractionAwareViewModelBase<RecommendPdfArchitectInteraction>
    {
        private readonly IProcessStarter _processStarter;
        private readonly ISoundPlayer _soundPlayer;
        private RecommendPdfArchitectWindowTranslation _translation;

        // ReSharper disable once MemberCanBeProtected.Global
        public RecommendPdfArchitectWindowViewModel(ISoundPlayer soundPlayer, IProcessStarter processStarter, RecommendPdfArchitectWindowTranslation translation)
        {
            _soundPlayer = soundPlayer;
            _processStarter = processStarter;
            _translation = translation;
            InfoCommand = new DelegateCommand(ExecuteInfo);
            DownloadCommand = new DelegateCommand(ExecuteDownload);
        }

        public RecommendPdfArchitectWindowTranslation Translation
        {
            get { return _translation; }
            set { _translation = value; RaisePropertyChanged(nameof(Translation)); }
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