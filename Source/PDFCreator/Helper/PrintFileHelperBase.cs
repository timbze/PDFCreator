using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;
using pdfforge.PDFCreator.PrintFile;
using pdfforge.PDFCreator.Shared.Helper;

namespace pdfforge.PDFCreator.Helper
{
    /// <summary>
    /// The PrintFileHelperBase class provides reusable functionality for printing files
    /// </summary>
    internal abstract class PrintFileHelperBase
    {
        private readonly PrintCommandGroup _printCommands = new PrintCommandGroup();

        private readonly string _pdfCreatorPrinter;
        private readonly PrinterHelper _printerHelper = new PrinterHelper();

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        protected PrintFileHelperBase()
        {
            _pdfCreatorPrinter = _printerHelper.GetApplicablePDFCreatorPrinter(SettingsHelper.Settings.ApplicationSettings.PrimaryPrinter);
        }

        protected PrintFileHelperBase(string printerName)
        {
            _pdfCreatorPrinter = printerName;
        }

        /// <summary>
        /// Add a single file. The file is checked and dialogs are presented to the user, if there are problems.
        /// </summary>
        /// <param name="file">A single file. If this is the path of a directory or an unprintable file, an error message will be shown.</param>
        /// <returns>true, if all files are printable</returns>
        public bool AddFile(string file)
        {
            return AddFiles(new[] { file });
        }

        /// <summary>
        /// Add multiple files. The files are checked - if there are problems, abstract functions (that might present dialogs to the user) get called.
        /// </summary>
        /// <param name="files">A list of files. If this contains a directory or files are not printable, an error message will be shown.</param>
        /// <returns>true, if all files are printable</returns>
        public bool AddFiles(IEnumerable<string> files)
        {
            var printerName = _printerHelper.GetApplicablePDFCreatorPrinter(_pdfCreatorPrinter);
            
            foreach (var f in files)
            {
                _printCommands.Add(new PrintCommand(f, printerName));
            }

            var directories = _printCommands.FindAll(p => Directory.Exists(p.Filename));

            if (directories.Count > 0)
            {
                DirectoriesNotSupportedHint();
                return false;
            }

            var unprintable = _printCommands.FindAll(p => !p.IsPrintable);

            if (unprintable.Any())
            {
                UnprintableFilesHint(unprintable);

                return false;
            }

            return true;
        }

        /// <summary>
        /// Prints all files in the list.
        /// </summary>
        /// <returns>true, if all files could be printed</returns>
        public bool PrintAll()
        {
            if (String.IsNullOrEmpty(_pdfCreatorPrinter))
            {
                _logger.Error("No PDFCreator is installed.");
                return false;
            }

            var printerHelper = new PrinterHelper();

            bool requiresDefaultPrinter = _printCommands.RequiresDefaultPrinter;
            string defaultPrinter = printerHelper.GetDefaultPrinter();

            try
            {
                if (requiresDefaultPrinter)
                {
                    if (SettingsHelper.Settings.ApplicationSettings.AskSwitchDefaultPrinter)
                    {
                        if (!QuerySwitchDefaultPrinter())
                            return false;
                    }

                    PrinterHelper.SetDefaultPrinter(_pdfCreatorPrinter);
                }

                return _printCommands.PrintAll();
            }
            finally
            {
                if (requiresDefaultPrinter)
                    PrinterHelper.SetDefaultPrinter(defaultPrinter);
            }
        }

        protected abstract void DirectoriesNotSupportedHint();
        protected abstract void UnprintableFilesHint(IList<PrintCommand> unprintable);
        protected abstract bool QuerySwitchDefaultPrinter();
    }
}
