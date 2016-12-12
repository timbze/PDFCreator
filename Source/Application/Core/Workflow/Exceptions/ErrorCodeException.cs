using System;

namespace pdfforge.PDFCreator.Core.Workflow.Exceptions
{
    public class ErrorCodeException : Exception
    {
        public ErrorCodeException(int errorCode)
        {
            ErrorCode = errorCode;
        }

        public int ErrorCode { get; private set; }
    }
}