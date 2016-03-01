using pdfforge.PDFCreator.Core.Jobs;

namespace pdfforge.PDFCreator.Core.Actions
{
    /// <summary>
    ///     The interface Action defines actions that can process a set of files (i.e. encrypt, send as mail)
    ///     and return a set of files after processing them
    /// </summary>
    public interface IAction
    {
        /// <summary>
        /// Process all output files
        /// </summary>
        /// <param name="job">The job to process</param>
        /// <returns>An ActionResult to determine the success and a list of errors</returns>
        ActionResult ProcessJob(IJob job);
    }
}