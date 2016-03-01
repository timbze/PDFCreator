using System;

namespace pdfforge.PDFCreator.Core.Ghostscript.OutputDevices
{
    public class DeviceException : Exception
    {
        public int ErrorCode { get; private set; }

        public DeviceException(string message, int errorCode)
            : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}
