using pdfforge.PDFCreator.Utilities.Process;
using System.Collections.Generic;
using System.Diagnostics;

namespace pdfforge.PDFCreator.IntegrationTest.Core.Printing.PrintFile
{
    internal class MockProcessWrapperFactory : ProcessWrapperFactory
    {
        public MockProcessWrapperFactory(bool exitImmediately)
        {
            CreatedMocks = new List<ProcessWrapperMock>();
            ExitImmediately = exitImmediately;
        }

        private bool ExitImmediately { get; }
        public ProcessWrapperMock LastMock { get; private set; }

        public IList<ProcessWrapperMock> CreatedMocks { get; }

        public override ProcessWrapper BuildProcessWrapper(ProcessStartInfo startInfo)
        {
            LastMock = new ProcessWrapperMock(startInfo);
            LastMock.ExitImmediately = ExitImmediately;

            CreatedMocks.Add(LastMock);

            return LastMock;
        }
    }
}
