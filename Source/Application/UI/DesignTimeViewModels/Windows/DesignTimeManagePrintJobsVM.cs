using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.UI.DesignTimeViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.Windows
{
    public class DesignTimeManagePrintJobsVm : ManagePrintJobsViewModel
    {
        public DesignTimeManagePrintJobsVm() : base(new DesignTimeJobInfoQueue(), new DesignTimeDragAndDropHandler(), new JobInfoManager(null))
        {
        }
    }
}