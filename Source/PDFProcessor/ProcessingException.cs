using System;

namespace pdfforge.PDFProcessing
{
    public class ProcessingException : Exception
    {
        public int ErrorCode { get; private set; }

        public ProcessingException(string message, int errorCode)
            : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}
