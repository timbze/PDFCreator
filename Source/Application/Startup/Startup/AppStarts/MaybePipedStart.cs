using pdfforge.PDFCreator.Core.StartupInterface;

namespace pdfforge.PDFCreator.Core.Startup.AppStarts
{
    public abstract class MaybePipedStart : AppStartBase
    {
        private readonly IMaybePipedApplicationStarter _maybePipedApplicationStarter;

        protected MaybePipedStart(IMaybePipedApplicationStarter maybePipedApplicationStarter)
            : base(maybePipedApplicationStarter.StartupConditions)
        {
            _maybePipedApplicationStarter = maybePipedApplicationStarter;
        }

        public bool StartManagePrintJobs { get; protected internal set; }

        public override ExitCode Run()
        {
            var success = _maybePipedApplicationStarter.SendMessageOrStartApplication(ComposePipeMessage, StartApplication, StartManagePrintJobs);
            if (!success)
                return ExitCode.ErrorWhileManagingPrintJobs;

            return ExitCode.Ok;
        }

        protected abstract string ComposePipeMessage();

        protected abstract bool StartApplication();
    }
}
