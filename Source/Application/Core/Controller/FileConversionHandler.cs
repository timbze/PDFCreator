using NLog;
using pdfforge.PDFCreator.Core.DirectConversion;
using pdfforge.PDFCreator.Core.Printing.Printing;
using System.Collections.Generic;
using System.Linq;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Core.Controller
{
    public interface IFileConversionHandler
    {
        /// <summary>
        ///     Removes invalid files and launches print jobs for the files that needs to be printed.
        ///     If successful, the direct convertable files are added to the current JobInfoQueue.
        /// </summary>
        void HandleFileList(IEnumerable<string> droppedFiles);
    }

    public class FileConversionHandler : IFileConversionHandler
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IDirectConversionHelper _directConversionHelper;
        private readonly IFile _file;
        private readonly IPrintFileHelper _printFileHelper;

        public FileConversionHandler(IDirectConversionHelper directConversionHelper, IPrintFileHelper printFileHelper, IFile file)
        {
            _directConversionHelper = directConversionHelper;
            _printFileHelper = printFileHelper;
            _file = file;
        }

        /// <summary>
        ///     Removes invalid files and launches print jobs for the files that needs to be printed.
        ///     If successful, the direct convertable files are added to the current JobInfoQueue.
        /// </summary>
        public void HandleFileList(IEnumerable<string> droppedFiles)
        {
            if (droppedFiles == null)
                return;

            Logger.Debug("Launched Drag & Drop");
            var validFiles = RemoveInvalidFiles(droppedFiles);
            HandleFiles(validFiles);
        }

        private IEnumerable<string> RemoveInvalidFiles(IEnumerable<string> droppedFiles)
        {
            var validFiles = new List<string>();
            foreach (var file in droppedFiles)
            {
                if (!_file.Exists(file))
                {
                    Logger.Warn("The file " + file + " does not exist.");
                    continue;
                }
                validFiles.Add(file);
            }

            return validFiles;
        }

        /// <summary>
        ///     Launches a print job for all dropped files that can be printed.
        ///     Return false if cancelled because of unprintable files
        /// </summary>
        private void PrintPrintableFiles(IEnumerable<string> printFiles)
        {
            if (!_printFileHelper.AddFiles(printFiles))
                return;

            _printFileHelper.PrintAll();
        }

        private void HandleFiles(IEnumerable<string> droppedFiles)
        {
            var filesToPrint = new List<string>();
            foreach (var file in droppedFiles)
            {
                if (_directConversionHelper.CanConvertDirectly(file))
                {
                    _directConversionHelper.ConvertDirectly(file);
                    continue;
                }

                filesToPrint.Add(file);
            }

            if (filesToPrint.Any())
                PrintPrintableFiles(filesToPrint);
        }
    }
}
