using SystemInterface.IO;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Query;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.Core.Workflow.Output
{
    public class AutosaveOutputFileMover : OutputFileMoverBase
    {
        public AutosaveOutputFileMover(IDirectory directory, IFile file, IPathUtil pathUtil)
        {
            Directory = directory;
            File = file;
            PathUtil = pathUtil;
        }

        protected override IDirectory Directory { get; }
        protected override IFile File { get; }
        protected override IPathUtil PathUtil { get; }

        protected override QueryResult<string> HandleFirstFileFailed(Job job)
        {
            return new QueryResult<string>(false, null);
        }

        protected override HandleCopyErrorResult QueryHandleCopyError(int fileNumber)
        {
            return HandleCopyErrorResult.EnsureUniqueFilename;
        }

        protected override bool ApplyUniqueFilename(Job job)
        {
            return job.Profile.AutoSave.EnsureUniqueFilenames;
        }
    }
}