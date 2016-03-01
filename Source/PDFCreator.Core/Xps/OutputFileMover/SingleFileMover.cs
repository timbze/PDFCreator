using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SystemInterface.IO;
using SystemWrapper.IO;
using NLog;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.IO;

namespace pdfforge.PDFCreator.Core.Xps.OutputFileMover
{
    public class SingleFileMover 
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private static Object _lockObject = new Object();

        protected IFile FileWrap;
        protected IDirectory DirectoryWrap;
        protected IPathSafe PathSafe = new PathWrapSafe();

        public SingleFileMover() : this(new FileWrap(), new DirectoryWrap()) 
        { }

        public SingleFileMover(IFile file, IDirectory directory)
        {
            FileWrap = file;
            DirectoryWrap = directory;
        }

        public bool UniqueFileNameEnabled { get; set; }

        public string MoveSingleOutputFile(string tempFile, string outputFilenameTemplate)
        {
            CreateOutputDirectory(outputFilenameTemplate);

            lock (_lockObject)
            {
                var uniqueFilename = new UniqueFilename(outputFilenameTemplate, DirectoryWrap, FileWrap);

                if (UniqueFileNameEnabled)
                    outputFilenameTemplate = uniqueFilename.CreateUniqueFileName();

                var success = CopyFile(tempFile, outputFilenameTemplate);

                if (!success)
                {
                    success = CopyFile(tempFile, uniqueFilename.CreateUniqueFileName());
                    if (!success)
                        throw new IOException("copy not successful on second try");
                }
            }

            DeleteFile(tempFile);
            return outputFilenameTemplate;
        }

        private void CreateOutputDirectory(string outputFilenameTemplate)
        {
            string directory = PathSafe.GetDirectoryName(outputFilenameTemplate);
            if (directory != null && !DirectoryWrap.Exists(directory))
            {
                DirectoryWrap.CreateDirectory(directory);
            }
        }

        private bool CopyFile(string tempFile, string outputFile)
        {
            try
            {
                FileWrap.Copy(tempFile, outputFile);
                _logger.Debug("Copied output file \"{0}\" \r\nto \"{1}\"", tempFile, outputFile);
                return true;
            }
            catch (IOException ioException)
            {
                _logger.Warn("Error while copying to target file.\r\nfrom\"{0}\" \r\nto \"{1}\"\r\n{2}", tempFile, outputFile, ioException.Message);
            }
            return false;
        }

        private void DeleteFile(string file)
        {
            try
            {
                FileWrap.Delete(file);
            }
            catch (IOException)
            {
                _logger.Warn("Could not delete temporary file \"" + file + "\"");
            }
        }

        private IList<string> CollectTemopraryOutputFiles(string tempOutputFolder)
        {
            var files = DirectoryWrap.GetFiles(tempOutputFolder);

            return files.ToList();
        } 
    }
}
