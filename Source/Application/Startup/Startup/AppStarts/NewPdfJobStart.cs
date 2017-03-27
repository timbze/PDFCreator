using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Core.DirectConversion;
using pdfforge.PDFCreator.Core.Workflow;

namespace pdfforge.PDFCreator.Core.Startup.AppStarts
{
    public class NewPdfJobStart : NewDirectConversionJobStart
    {
        public NewPdfJobStart(IJobInfoQueue jobInfoQueue, ISpoolerProvider spoolerProvider,
            IMaybePipedApplicationStarter maybePipedApplicationStarter, IJobInfoManager jobInfoManager, PdfDirectConversion directConversion)
            : base(jobInfoQueue, maybePipedApplicationStarter, jobInfoManager)
        {
            DirectConversionBase = directConversion;
        }

        protected override DirectConversionBase DirectConversionBase { get; }
    }
}