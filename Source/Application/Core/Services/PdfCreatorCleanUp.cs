using System;
using System.Linq;
using NLog;
using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.Utilities.IO;

namespace pdfforge.PDFCreator.Core.Services
{
    public interface IPdfCreatorCleanUp
    {
        void CleanTempFolder();
        void CleanSpoolFolder();
    }

    public class PdfCreatorCleanUp : IPdfCreatorCleanUp
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ISpoolerProvider _spoolFolderProvider;
        private readonly ITempFolderProvider _tempFolderProvider;
        private readonly TimeSpan _timeSpan;

        public PdfCreatorCleanUp(ITempFolderProvider tempFolderProvider, ISpoolerProvider spoolFolderProvider)
        {
            _tempFolderProvider = tempFolderProvider;
            _spoolFolderProvider = spoolFolderProvider;
            _timeSpan = TimeSpan.FromDays(1);
        }

        public void CleanTempFolder()
        {
            CleanFolder(_tempFolderProvider.TempFolder);
        }

        public void CleanSpoolFolder()
        {
            CleanFolder(_spoolFolderProvider.SpoolFolder);
        }

        private void CleanFolder(string folder)
        {
            var folderCleaner = new FolderCleaner(folder);
            folderCleaner.Clean(_timeSpan);

            if (folderCleaner.CleanupExceptions.Any())
            {
                var exception = folderCleaner.CleanupExceptions.First();
                _logger.Debug($"Exception while cleaning up {exception.Key}: {exception.Value}");
            }
        }
    }
}