using System;
using NLog;
using pdfforge.PDFCreator.Assistants;
using pdfforge.PDFCreator.Helper;
using pdfforge.PDFCreator.Threading;
using pdfforge.PDFCreator.Utilities.Communication;

namespace pdfforge.PDFCreator.Startup
{
    internal abstract class MaybePipedStart : IAppStart
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public bool StartManagePrintJobs { get; protected internal set; }

        internal abstract string ComposePipeMessage();
        internal abstract bool StartApplication();

        public bool Run()
        {
            int retry = 0;
            bool success = false;

            // Make n Attempts: Look if a pipe server exists, if so, send a message. If that fails, retry (and maybe do the job yourself)
            while (!success && retry++ <= 5)
            {
                _logger.Debug("Starting attempt {0}: ", retry);
                if (PipeServer.SessionServerInstanceRunning(ThreadManager.PipeName))
                {
                    success = TrySendPipeMessage();
                    _logger.Debug("TrySendPipeMessage: " + success);
                }
                else
                {
                    success = TryStartApplication();
                    _logger.Debug("TryStartApplication: " + success);
                }
            }

            return success;
        }

        private bool TrySendPipeMessage()
        {
            _logger.Debug("Found another running instance of PDFCreator, so we send our data there");

            string message = ComposePipeMessage();
            
            PipeClient pipeClient = PipeClient.CreateSessionPipeClient(ThreadManager.PipeName);

            if (pipeClient.SendMessage(message))
            {
                _logger.Debug("Pipe message successfully sent");
                return true;
            }

            _logger.Warn("There was an error while communicating through the pipe");
            return false;
        }

        private bool TryStartApplication()
        {
            try
            {
                _logger.Debug("Starting pipe server");
                var success = ThreadManager.Instance.StartPipeServerThread();
                
                if (!success)
                    return false;

                _logger.Debug("Reloading settings");
                // Settings may have changed as this may have not been the only instance till now
                SettingsHelper.ReloadSettings();

                ThreadManager.Instance.PipeServer.OnNewMessage += PipeServer_OnNewMessage;

                JobRunner.Instance.RegisterJobInfoQueueHandler();

                if (StartManagePrintJobs)
                {
                    JobRunner.Instance.ManagePrintJobs();
                }

                _logger.Debug("Finding spooled jobs");
                JobInfoQueue.Instance.FindSpooledJobs();

                if (StartApplication())
                {
                    _logger.Debug("Starting update check thread");
                    StartUpdateCheck();

                    _logger.Debug("Starting Cleanup thread");
                    ThreadManager.Instance.StartCleanUpThread();
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "There was an error while starting the application");
                return false;
            }
        }

        void PipeServer_OnNewMessage(object sender, MessageEventArgs e)
        {
            if (e.Message.StartsWith("ReloadSettings|", StringComparison.OrdinalIgnoreCase))
            {
                _logger.Info("Pipe Command: Reloading settings");
                SettingsHelper.ReloadSettings();
            }
        }

        private static void StartUpdateCheck()
        {
            UpdateAssistant.Instance.Initialize(SettingsHelper.Settings.ApplicationSettings, SettingsHelper.Settings.ApplicationProperties, SettingsHelper.GpoSettings);
            UpdateAssistant.Instance.NewVersionEvent += UpdateAssistant.LaunchNewUpdateForm;
            UpdateAssistant.Instance.FinishedEvent += StoreNextUpdate;
            UpdateAssistant.Instance.UpdateProcedure(true);
        }

        private static void StoreNextUpdate(object sender, UpdateEventArgs eventArgs)
        {
            SettingsHelper.Settings.ApplicationProperties.NextUpdate = UpdateAssistant.NextUpdate;
        }
    }
}
