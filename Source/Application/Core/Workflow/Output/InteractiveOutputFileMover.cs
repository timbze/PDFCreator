using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Query;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Workflow.Queries;
using pdfforge.PDFCreator.Utilities;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Core.Workflow.Output
{
    public class InteractiveOutputFileMover : OutputFileMoverBase
    {
        private readonly IDispatcher _dispatcher;

        public InteractiveOutputFileMover(IDirectory directory, IFile file, IPathUtil pathUtil, IRetypeFileNameQuery retypeFileNameQuery, IDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
            Directory = directory;
            File = file;
            PathUtil = pathUtil;
            RetypeFileNameQuery = retypeFileNameQuery;
        }

        protected override IDirectory Directory { get; }
        protected override IFile File { get; }
        protected override IPathUtil PathUtil { get; }
        private IRetypeFileNameQuery RetypeFileNameQuery { get; }

        protected override QueryResult<string> HandleFirstFileFailed(string filename, OutputFormat outputFormat)
        {
            var result = _dispatcher.InvokeAsync(() => RetypeFileNameQuery.RetypeFileName(filename, outputFormat));
            return result.Result;
        }

        protected override HandleCopyErrorResult QueryHandleCopyError(int fileNumber)
        {
            if (fileNumber == 1)
                return HandleCopyErrorResult.Requery;

            return HandleCopyErrorResult.EnsureUniqueFilename;
        }

        protected override bool ApplyUniqueFilename(Job job)
        {
            return false;
        }
    }
}
