using System;
using System.Collections.Generic;
using System.IO;
using SystemInterface.IO;
using SystemWrapper.IO;
using NLog;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.Core.Jobs
{
    public class JobCleanUp
    {
        private IDirectory _directory = new DirectoryWrap();
        private IFile _file = new FileWrap();
        private readonly string _infFile;
        private readonly string _jobTempFolder;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IList<SourceFileInfo> _sourceFileInfos;

        public JobCleanUp(string jobTempFolder, IList<SourceFileInfo> sourceFileInfos, string infFile)
        {
            _jobTempFolder = jobTempFolder;
            _sourceFileInfos = sourceFileInfos;
            _infFile = infFile;
        }

        public IDirectory Directory
        {
            get { return _directory; }
            set { _directory = value; }
        }

        public IFile File
        {
            get { return _file; }
            set { _file = value; }
        }

        public void DoCleanUp()
        {
            DeleteTemporaryOutput();
            DeleteSourceFiles();
            DeleteInfFile();
        }

        private void DeleteTemporaryOutput()
        {
            if (!String.IsNullOrEmpty(_jobTempFolder) && Path.IsPathRooted(_jobTempFolder) &&
                Directory.Exists(_jobTempFolder))
            {
                try
                {
                    Directory.Delete(_jobTempFolder, true);
                }
                catch (IOException ex)
                {
                    _logger.Warn("Error while deleting temporary folder: " + ex.Message);
                }
            }
        }

        private void DeleteSourceFiles()
        {
            foreach (var file in _sourceFileInfos)
            {
                try
                {
                    File.Delete(file.Filename);

                    var folder = Path.GetDirectoryName(file.Filename);
                    DeleteFolderIfEmptyAndNotSpool(folder);
                }
                catch (IOException ex)
                {
                    _logger.Warn("Error while deleting source file: " + ex.Message);
                }
            }
        }

        private void DeleteFolderIfEmptyAndNotSpool(string directoryPath)
        {
            var directoryInfo = new DirectoryInfo(directoryPath);
            var name = directoryInfo.Name;

            // ensure the spool folder is never deleted, because doing so can lead to problems
            if (string.Equals("Spool", name, StringComparison.OrdinalIgnoreCase))
                return;

            DeleteFolderIfEmpty(directoryPath);
        }

        private void DeleteFolderIfEmpty(string folder)
        {
            if (FileUtil.Instance.DirectoryIsEmpty(folder))
            {
                Directory.Delete(folder);
            }
        }

        private void DeleteInfFile()
        {
            try
            {
                File.Delete(_infFile);

                var folder = Path.GetDirectoryName(_infFile);
                DeleteFolderIfEmptyAndNotSpool(folder);
            }
            catch (IOException ex)
            {
                _logger.Warn("Error while deleting job file: " + ex.Message);
            }
        }
    }
}