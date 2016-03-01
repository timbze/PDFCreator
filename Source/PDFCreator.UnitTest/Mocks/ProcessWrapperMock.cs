using System;
using System.Diagnostics;
using pdfforge.PDFCreator.Utilities.Process;

namespace PDFCreator.UnitTest.Mocks
{
    public class ProcessWrapperMock : ProcessWrapper
    {
        /// <summary>
        /// If true, the mock will set HasExited immediately
        /// </summary>
        public bool ExitImmediately { get; set; }

        /// <summary>
        /// A flag that indicates that Kill() was called
        /// </summary>
        public bool WasKilled { get; private set; }

        /// <summary>
        /// A flag that indicates that Start() was called
        /// </summary>
        public bool WasStarted { get; private set; }

        public ProcessWrapperMock(ProcessStartInfo startInfo) : base(startInfo)
        {
        }

        private bool _hasExited;
        public override bool HasExited
        {
            get { return _hasExited; }
        }

        public override void Start()
        {
            WasStarted = true;

            if (ExitImmediately)
                _hasExited = true;
        }

        public override void WaitForExit(TimeSpan timeSpan)
        {

        }

        public override void Kill()
        {
            WasKilled = true;
        }
    }
}
