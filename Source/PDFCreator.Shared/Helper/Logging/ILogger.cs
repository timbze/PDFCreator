using NLog;

namespace pdfforge.PDFCreator.Shared.Helper.Logging
{
    internal interface ILogger
    {
        void ChangeLogLevel(LogLevel logLevel);
        string GetLogPath();
    }
}