using System;
using System.Diagnostics;

namespace pdfforge.PDFCreator.Utilities.Process
{
    public class ProcessWrapper
    {
        public ProcessStartInfo StartInfo { get; private set; }

        public virtual bool HasExited
        {
            get
            {
                if (_process == null)
                    return false;

                return _process.HasExited;
            }
        }

        private System.Diagnostics.Process _process;

        public ProcessWrapper(ProcessStartInfo startInfo)
        {
            StartInfo = startInfo;
        }

        public virtual void Start()
        {
            _process = System.Diagnostics.Process.Start(StartInfo);
        }

        public virtual void WaitForExit(TimeSpan timeSpan)
        {
            _process.WaitForExit((int) timeSpan.TotalMilliseconds);
        }

        public virtual void Kill()
        {
            _process.Kill();
        }
    }
}
