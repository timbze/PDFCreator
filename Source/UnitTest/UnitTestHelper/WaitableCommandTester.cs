using pdfforge.PDFCreator.Core.Services.Macros;
using System;

namespace pdfforge.PDFCreator.UnitTest.UnitTestHelper
{
    public class WaitableCommandTester<T> where T : IWaitableCommand
    {
        private ResponseStatus? _lastResponseStatus;

        public WaitableCommandTester(T waitableCommand)
        {
            WaitableCommand = waitableCommand;

            waitableCommand.IsDone += (sender, args) => _lastResponseStatus = args.ResponseStatus;
        }

        public T WaitableCommand { get; }

        public bool IsDoneWasRaised => _lastResponseStatus.HasValue;

        public ResponseStatus LastResponseStatus
        {
            get
            {
                if (!_lastResponseStatus.HasValue)
                    throw new InvalidOperationException($"{nameof(LastResponseStatus)} has no value because {nameof(WaitableCommand.IsDone)} was not raised yet!");
                return _lastResponseStatus.Value;
            }
        }
    }
}
