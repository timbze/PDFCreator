using System.Windows.Input;
using pdfforge.DynamicTranslator;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Process;

namespace pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ApplicationSettings
{
    public class PdfArchitectTabViewModel : ObservableObject
    {
        private readonly IPdfArchitectCheck _pdfArchitectCheck;
        private readonly IProcessStarter _processStarter;

        public PdfArchitectTabViewModel(ITranslator translator, IPdfArchitectCheck pdfArchitectCheck, IProcessStarter processStarter)
        {
            _processStarter = processStarter;
            Translator = translator;
            _pdfArchitectCheck = pdfArchitectCheck;
        }

        public ITranslator Translator { get; private set; }

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