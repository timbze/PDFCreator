using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Core.Workflow.Queries;

namespace pdfforge.PDFCreator.UI.COM
{
    public class ComTargetFileNameComposer : ITargetFileNameComposer
    {
        private readonly string _targetFilename;

        public ComTargetFileNameComposer(string targetFilename)
        {
            _targetFilename = targetFilename;
        }

        public void ComposeTargetFileName(Job job)
        {
            job.OutputFilenameTemplate = _targetFilename;
        }
    }
}
