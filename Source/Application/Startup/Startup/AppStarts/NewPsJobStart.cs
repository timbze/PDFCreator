using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Core.DirectConversion;
using pdfforge.PDFCreator.Core.Workflow;

namespace pdfforge.PDFCreator.Core.Startup.AppStarts
{
    public class NewPsJobStart : NewDirectConversionJobStart
    {
        public NewPsJobStart(IJobInfoQueue jobInfoQueue, ISpoolerProvider spoolerProvider,
            IMaybePipedApplicationStarter maybePipedApplicationStarter, IJobInfoManager jobInfoManager, PsDirectConversion directConversion)
            : base(jobInfoQueue, maybePipedApplicationStarter, jobInfoManager)
        {
            DirectConversionBase = directConversion;
        }

        protected override DirectConversionBase DirectConversionBase { get; }
    }
}
