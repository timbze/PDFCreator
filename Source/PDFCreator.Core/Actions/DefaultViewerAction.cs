using System;
using System.Diagnostics;
using NLog;
using pdfforge.PDFCreator.Core.Jobs;
using pdfforge.PDFCreator.Core.Settings;
using pdfforge.PDFCreator.Core.Settings.Enums;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.Core.Actions
{
    /// <summary>
    ///     DefaultViewerAction opens the output files in the default viewer
    /// </summary>
    public class DefaultViewerAction : IAction
    {
        private const int ActionId = 10;
        protected static Logger Logger = LogManager.GetCurrentClassLogger();

        public event EventHandler RecommendPdfArchitect;
        /// <summary>
        ///     Creates a new default viewer action.
        /// </summary>
        /// <param name="firstFileOnly">
        ///     If true, only the first output file will be opened. If false, all files will be openend
        ///     indidually
        /// </param>
        public DefaultViewerAction(bool firstFileOnly)
        {
            FirstFileOnly = firstFileOnly;
        }

        /// <summary>
        ///     Gets whether only the first output file is opened. If false, all files will be opened
        /// </summary>
        public bool FirstFileOnly { get; private set; }

        /// <summary>
        ///     Open all files in the default viewer
        /// </summary>
        /// <param name="job">Job information</param>
        /// <returns>An ActionResult to determine the success and a list of errors</returns>
        public ActionResult ProcessJob(IJob job)
        {
            Logger.Debug("Launched Viewer-Action");
            
            bool isPdfFile = job.Profile.OutputFormat == OutputFormat.Pdf ||
                             job.Profile.OutputFormat == OutputFormat.PdfA1B ||
                             job.Profile.OutputFormat == OutputFormat.PdfA2B ||
                             job.Profile.OutputFormat == OutputFormat.PdfX;

            if (!isPdfFile)
                return OpenJobOutput(job);

            if (job.Profile.OpenWithPdfArchitect)
            {
                string architectPath = null;
                
                try
                {
                    architectPath = PdfArchitectCheck.InstallationPath();
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "There was an exception while checking the PDF Architect path");
                }

                if (architectPath != null)
                {
                    Logger.Debug("Open with PDF Architect");
                    foreach (string file in job.OutputFiles)
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
                            return new ActionResult(ActionId, 100);
                        }
                    }
                    return new ActionResult();
                }
                Logger.Warn("Open with PDF Architect selected, but not installed");
            }

            if (!FileUtil.Instance.FileAssocHasOpen(".pdf"))
            {
                Logger.Error("No program associated with pdf.");

                if (RecommendPdfArchitect != null)
                {
                    Logger.Info("Recommend PDF Architect.");
                    RecommendPdfArchitect(this, null);
                    return new ActionResult(); //return true, to avoid another message window.
                }
            }

            return OpenJobOutput(job);
        }

        private ActionResult OpenJobOutput(IJob job)
        {
            Logger.Trace("Open file(s) with default programm");
            try
            {
                if (FirstFileOnly)
                {
                    Process.Start(job.OutputFiles[0]);
                    Logger.Trace("Openend (only first) file: " + job.OutputFiles[0]);
                }
                else
                {
                    foreach (string file in job.OutputFiles)
                    {
                        Process.Start(file);
                        Logger.Trace("Openend file: " + file);
                    }
                }
            }
            catch
            {
                Logger.Error("File could not be opened.");
                return new ActionResult(ActionId, 101);
            }
            return new ActionResult();
        }
    }
}