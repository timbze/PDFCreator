using System;

namespace pdfforge.PDFCreator.Conversion.Jobs
{
    public class ProcessingException : Exception
    {
        public ProcessingException(string message, ErrorCode errorCode)
            : base(message)
        {
            ErrorCode = errorCode;
        }

        public ErrorCode ErrorCode { get; private set; }
    }
}