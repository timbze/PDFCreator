using NLog;
using pdfforge.PDFCreator.Conversion.Actions.Queries;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Utilities;
using System;
using System.Diagnostics;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions
{
    /// <summary>
    ///     DefaultViewerAction opens the output files in the default viewer
    /// </summary>
    public class DefaultViewerAction : IAction
    {
        protected static Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IPdfArchitectCheck _architectCheck;
        private readonly IFileAssoc _fileAssoc;
        private readonly IRecommendArchitect _recommendArchitect;

        /// <summary>
        ///     Creates a new default viewer action.
        /// </summary>
        public DefaultViewerAction(IFileAssoc fileAssoc, IRecommendArchitect recommendArchitect, IPdfArchitectCheck architectCheck)
        {
            _fileAssoc = fileAssoc;
            _recommendArchitect = recommendArchitect;
            _architectCheck = architectCheck;
        }

        /// <summary>
        ///     Open all files in the default viewer
        /// </summary>
        /// <param name="job">Job information</param>
        /// <returns>An ActionResult to determine the success and a list of errors</returns>
        public ActionResult ProcessJob(Job job)
        {
            Logger.Debug("Launched Viewer-Action");

            var isPdfFile = job.Profile.OutputFormat == OutputFormat.Pdf ||
                            job.Profile.OutputFormat == OutputFormat.PdfA1B ||
                            job.Profile.OutputFormat == OutputFormat.PdfA2B ||
                            job.Profile.OutputFormat == OutputFormat.PdfX;

            if (!isPdfFile)
                return OpenJobOutput(job);

            if (job.Profile.OpenWithPdfArchitect && IsArchitectInstalled())
            {
                return OpenWithArchitect(job);
            }

            if (!_fileAssoc.HasOpen(".pdf"))
            {
                Logger.Error("No program associated with pdf.");

                _recommendArchitect.Show();
                return new ActionResult(); //return true, to avoid another message window.
            }

            return OpenJobOutput(job);
        }

        private string GetArchitectPath()
        {
            try
            {
                return _architectCheck.GetInstallationPath();
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "There was an exception while checking the PDF Architect path");
                return null;
            }
        }

        private bool IsArchitectInstalled()
        {
            return _architectCheck.IsInstalled();
        }

        private ActionResult OpenWithArchitect(Job job)
        {
            string architectPath = GetArchitectPath();

            Logger.Debug("Open with PDF Architect");
            foreach (var file in job.OutputFiles)
            {
                try
                {
                    var p = new Process();
                    p.StartInfo.FileName = architectPath;
                    p.StartInfo.Arguments = "\"" + file + "\"";
                    p.Start();
                    Logger.Trace("Openend: " + file);
                }
                catch
                {
                    Logger.Error("PDF Architect could not open file: " + file);
                    return new ActionResult(ErrorCode.Viewer_ArchitectCouldNotOpenOutput);
                }
            }
            return new ActionResult();
        }

        public bool IsEnabled(ConversionProfile profile)
        {
            return profile.OpenViewer;
        }

        private ActionResult OpenJobOutput(Job job)
        {
            Logger.Trace("Open file(s) with default programm");
            try
            {
                Process.Start(job.OutputFiles[0]);
                Logger.Trace("Openend (only first) file: " + job.OutputFiles[0]);
            }
            catch
            {
                Logger.Error("File could not be opened.");
            }
            return new ActionResult();
        }
    }
}
