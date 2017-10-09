using pdfforge.Obsidian;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Process;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Architect
{
    public class ArchitectViewModel : TranslatableViewModelBase<ArchitectViewTranslation>
    {
        private readonly IPdfArchitectCheck _pdfArchitectCheck;
        private readonly IProcessStarter _processStarter;

        public ArchitectViewModel(IPdfArchitectCheck pdfArchitectCheck, IProcessStarter processStarter, ITranslationUpdater translationUpdater) : base(translationUpdater)
        {
            _pdfArchitectCheck = pdfArchitectCheck;
            _processStarter = processStarter;
        }

        public ICommand LaunchPdfArchitectCommand => new DelegateCommand(o =>
        {
            var applicationPath = _pdfArchitectCheck.GetInstallationPath();

            if (!string.IsNullOrWhiteSpace(applicationPath))
                _processStarter.Start(applicationPath);
        });

        public ICommand LaunchWebsiteCommand => new DelegateCommand(o => _processStarter.Start(Urls.ArchitectWebsiteUrl));

        public ICommand DownloadPdfArchitectCommand => new DelegateCommand(o => _processStarter.Start(Urls.ArchitectDownloadUrl));

        public bool IsPdfArchitectInstalled => _pdfArchitectCheck.IsInstalled();
    }
}
