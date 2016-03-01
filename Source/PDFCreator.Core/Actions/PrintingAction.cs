using System;
using NLog;
using pdfforge.PDFCreator.Core.Ghostscript;
using pdfforge.PDFCreator.Core.Ghostscript.OutputDevices;
using pdfforge.PDFCreator.Core.Jobs;

namespace pdfforge.PDFCreator.Core.Actions
{
    /// <summary>
    /// Implements the action to print the input files
    /// </summary>
    public class PrintingAction : IAction
    {
        private readonly GhostScript _ghostscript;
        private const int ActionId = 13;
        protected static Logger Logger = LogManager.GetCurrentClassLogger();

        public PrintingAction(GhostScript ghostscript)
        {
            _ghostscript = ghostscript;
        }

        /// <summary>
        /// Prints the input files to the configured printer
        /// </summary>
        /// <param name="job">The job to process</param>
        /// <returns>An ActionResult to determine the success and a list of errors</returns>
        public ActionResult ProcessJob(IJob job)
        {
            Logger.Debug("Launched Printing-Action");

            try
            {
                OutputDevice printingDevice = new PrintingDevice(job);
                _ghostscript.Run(printingDevice, job.JobTempFolder);
                return new ActionResult();
            }
            catch (Exception ex)
            {
                try
                {
                    int errorCode = Convert.ToInt32(ex.Message);
                    return new ActionResult(ActionId, errorCode);
                }
                catch
                {
                    Logger.Error("Error while printing");
                    return new ActionResult(ActionId, 999);
                }
            }
        }
    }
}