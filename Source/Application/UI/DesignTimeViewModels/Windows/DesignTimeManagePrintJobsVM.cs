using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.UI.DesignTimeViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels.Translations;
using pdfforge.PDFCreator.UI.ViewModels.Wrapper;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.Windows
{
    public class DesignTimeManagePrintJobsVm : ManagePrintJobsViewModel
    {
        public DesignTimeManagePrintJobsVm() : base(new DesignTimeJobInfoQueue(), new DesignTimeDragAndDropHandler(), new JobInfoManager(null), new DispatcherWrapper(), new ManagePrintJobsWindowTranslation())
        {
        }
    }
}