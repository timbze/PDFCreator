using System.Windows.Input;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ApplicationSettings.Translations;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Process;

namespace pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ApplicationSettings
{
    public class PdfArchitectTabViewModel : ObservableObject
    {
        private readonly IPdfArchitectCheck _pdfArchitectCheck;
        private readonly IProcessStarter _processStarter;
        private PdfArchitectTabTranslation _translation;

        public PdfArchitectTabViewModel(IPdfArchitectCheck pdfArchitectCheck, IProcessStarter processStarter, PdfArchitectTabTranslation translation)
        {
            _processStarter = processStarter;
            Translation = translation;
            _pdfArchitectCheck = pdfArchitectCheck;
        }

        public PdfArchitectTabTranslation Translation
        {
            get { return _translation; }
            set { _translation = value; RaisePropertyChanged(nameof(Translation)); }
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