using SystemInterface.IO;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Query;
using pdfforge.PDFCreator.Core.Workflow.Queries;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.Core.Workflow.Output
{
    public class InteractiveOutputFileMover : OutputFileMoverBase
    {
        public InteractiveOutputFileMover(IDirectory directory, IFile file, IPathUtil pathUtil, IRetypeFileNameQuery retypeFileNameQuery)
        {
            Directory = directory;
            File = file;
            PathUtil = pathUtil;
            RetypeFileNameQuery = retypeFileNameQuery;
        }

        protected override IDirectory Directory { get; }
        protected override IFile File { get; }
        protected override IPathUtil PathUtil { get; }
        private IRetypeFileNameQuery RetypeFileNameQuery { get; }

        protected override QueryResult<string> HandleFirstFileFailed(Job job)
        {
            return RetypeFileNameQuery.RetypeFileName(job);
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