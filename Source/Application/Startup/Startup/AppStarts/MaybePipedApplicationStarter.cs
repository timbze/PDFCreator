using System;
using NLog;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Core.Communication;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.StartupInterface;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.UI.ViewModels.Assistants.Update;
using pdfforge.PDFCreator.Utilities.Threading;

namespace pdfforge.PDFCreator.Core.Startup.AppStarts
{
    public interface IMaybePipedApplicationStarter
    {
        ICheckAllStartupConditions StartupConditions { get; }
        bool SendMessageOrStartApplication(Func<string> composePipeMessage, Func<bool> startApplication, bool startManagePrintJobs);
    }

    public class MaybePipedApplicationStarter : IMaybePipedApplicationStarter
    {
        private readonly IPdfCreatorCleanUp _cleanUp;
        private readonly IJobInfoQueue _jobInfoQueue;
        private readonly IJobInfoQueueManager _jobInfoQueueManager;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IPipeServerManager _pipeServerManager;
        private readonly ISettingsManager _settingsManager;
        private readonly ISpooledJobFinder _spooledJobFinder;
        private readonly IThreadManager _threadManager;
        private readonly IUpdateAssistant _updateAssistant;

        public MaybePipedApplicationStarter(ISettingsManager settingsManager, IUpdateAssistant updateAssistant,
            ICheckAllStartupConditions startupConditions, IThreadManager threadManager,
            IPipeServerManager pipeServerManager, IJobInfoQueueManager jobInfoQueueManager, IJobInfoQueue jobInfoQueue,
            IStaticPropertiesHack staticPropertiesHack, IPdfCreatorCleanUp cleanUp, ISpooledJobFinder spooledJobFinder)
        {
            StartupConditions = startupConditions;
            _jobInfoQueue = jobInfoQueue;
            _cleanUp = cleanUp;
            _spooledJobFinder = spooledJobFinder;
            _settingsManager = settingsManager;
            _updateAssistant = updateAssistant;
            _threadManager = threadManager;
            _pipeServerManager = pipeServerManager;
            _jobInfoQueueManager = jobInfoQueueManager;

            staticPropertiesHack.SetStaticProperties();
        }

        public int Retries { get; set; } = 5;

        public ICheckAllStartupConditions StartupConditions { get; }

        public bool SendMessageOrStartApplication(Func<string> composePipeMessage, Func<bool> startApplication, bool startManagePrintJobs)
        {
            _settingsManager.LoadAllSettings();

            var pipeMessage = composePipeMessage();

            var retry = 0;
            var success = false;

            // Make n Attempts: Look if a pipe server exists, if so, send a message. If that fails, retry (and maybe do the job yourself)
            while (!success && (retry++ < Retries))
            {
                _logger.Debug("Starting attempt {0}: ", retry);
                if (_pipeServerManager.IsServerRunning())
                {
                    success = TrySendPipeMessage(pipeMessage);
                    _logger.Debug("TrySendPipeMessage: " + success);
                }
                else
                {
                    success = TryStartApplication(startApplication, startManagePrintJobs);
                    _logger.Debug("TryStartApplication: " + success);
                }
            }

            if (success)
            {
                _logger.Debug("Starting update check thread");
                StartUpdateCheck();

                _logger.Debug("Starting Cleanup thread");
                StartCleanUpThread();
            }

            Shutdown();
            return success;
        }

        private void Shutdown()
        {
            _logger.Debug("Waiting for all threads to finish");
            _threadManager.WaitForThreads();

            _pipeServerManager.PrepareShutdown();

            _threadManager.Shutdown();

            _settingsManager.SaveCurrentSettings();

            _pipeServerManager.Shutdown();
        }

        private bool TrySendPipeMessage(string message)
        {
            _logger.Debug("Found another running instance of PDFCreator, so we send our data there");

            if (_pipeServerManager.TrySendPipeMessage(message))
            {
                _logger.Debug("Pipe message successfully sent");
                return true;
            }

            _logger.Warn("There was an error while communicating through the pipe");
            return false;
        }

        private bool TryStartApplication(Func<bool> startApplication, bool startManagePrintJobs)
        {
            var success = false;
            try
            {
                _logger.Debug("Starting pipe server");

                success = _pipeServerManager.StartServer();

                if (!success)
                    return false;

                _logger.Debug("Reloading settings");
                // Settings may have changed as this may have not been the only instance till now
                _settingsManager.LoadPdfCreatorSettings();

                if (startManagePrintJobs)
                    _jobInfoQueueManager.ManagePrintJobs();

                _logger.Debug("Finding spooled jobs");

                var spooledJobs = _spooledJobFinder.GetJobs();
                _jobInfoQueue.Add(spooledJobs);

                success = startApplication();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "There was an error while starting the application");
                success = false;
                throw;
            }
            finally
            {
                if (!success)
                    _pipeServerManager.Shutdown();
            }
            return success;
        }

        private void StartCleanUpThread()
        {
            _threadManager.StartSynchronizedThread(CleanUp, "CleanUpThread");
        }

        private void CleanUp()
        {
            _cleanUp.CleanSpoolFolder();
            _cleanUp.CleanTempFolder();
        }

        private void StartUpdateCheck()
        {
            _updateAssistant.UpdateProcedure(true, true);
        }
    }
}
