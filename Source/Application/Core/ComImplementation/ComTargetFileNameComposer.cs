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

        public string ComposeTargetFileName(Job job)
        {
            return _targetFilename;
        }
    }
}
