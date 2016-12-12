using SystemInterface.IO;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.Workflow;

namespace pdfforge.PDFCreator.Core.ComImplementation
{
    public interface IPrintJobAdapterFactory
    {
        PrintJobAdapter BuildPrintJobAdapter(Job job);
    }

    public class PrintJobAdapterFactory : IPrintJobAdapterFactory
    {
        private readonly IDirectory _directory;
        private readonly ErrorCodeInterpreter _errorCodeInterpreter;
        private readonly IJobInfoQueue _jobInfoQueue;
        private readonly IPathSafe _pathSafe;
        private readonly ISettingsProvider _settingsProvider;
        private readonly ThreadPool _threadPool;
        private readonly IComWorkflowFactory _workflowFactory;

        public PrintJobAdapterFactory(ISettingsProvider settingsProvider, IComWorkflowFactory workflowFactory, ThreadPool threadPool, IJobInfoQueue jobInfoQueue, ErrorCodeInterpreter errorCodeInterpreter, IPathSafe pathSafe, IDirectory directory)
        {
            _settingsProvider = settingsProvider;
            _workflowFactory = workflowFactory;
            _threadPool = threadPool;
            _jobInfoQueue = jobInfoQueue;
            _errorCodeInterpreter = errorCodeInterpreter;
            _pathSafe = pathSafe;
            _directory = directory;
        }

        public PrintJobAdapter BuildPrintJobAdapter(Job job)
        {
            var printJobAdapter = new PrintJobAdapter(_settingsProvider, _workflowFactory, _threadPool, _jobInfoQueue, _errorCodeInterpreter, _pathSafe, _directory);

            printJobAdapter.Job = job;
            printJobAdapter.SetProfileByGuid(ProfileGuids.DEFAULT_PROFILE_GUID);

            return printJobAdapter;
        }
    }
}
