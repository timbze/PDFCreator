using System;
using System.IO;
using SystemInterface.IO;
using NLog;
using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;

namespace pdfforge.PDFCreator.Core.Startup.StartConditions
{
    public interface ISpoolFolderAccess
    {
        bool CanAccess();
    }

    public class SpoolFolderAccess : ISpoolFolderAccess
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ISpoolerProvider _spoolerProvider;
        private readonly IDirectory _directory;

        public SpoolFolderAccess(ISpoolerProvider spoolerProvider, IDirectory directory)
        {
            _spoolerProvider = spoolerProvider;
            _directory = directory;
        }

        public bool CanAccess()
        {
            var spoolFolder = _spoolerProvider.SpoolFolder;
            try
            {
                if (!_directory.Exists(spoolFolder))
                {
                    _directory.CreateDirectory(spoolFolder);
                }

                _directory.GetAccessControl(spoolFolder);

                foreach (var directory in _directory.EnumerateDirectories(spoolFolder, "*", SearchOption.AllDirectories))
                {
                    try
                    {
                        _logger.Debug("Checking directory " + directory);
                        _directory.GetAccessControl(directory);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        _logger.Info("Exception while checking spool folder: " + ex);
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.Debug("The spool folder seems to be broken: " + ex);
                return false;
            }

            return true;
        }
    }
}