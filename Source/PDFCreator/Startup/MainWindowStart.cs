using NLog;
using pdfforge.PDFCreator.Threading;

namespace pdfforge.PDFCreator.Startup
{
    internal class MainWindowStart : MaybePipedStart
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        
        internal override string ComposePipeMessage()
        {
            return "ShowMain|";
        }

        internal override bool StartApplication()
        {
            _logger.Debug("Starting main form");
            ThreadManager.Instance.StartMainWindowThread();

            return true;
        }
    }
}
