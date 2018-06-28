using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;

namespace pdfforge.PDFCreator.Core.Workflow
{
    public interface IAppSettingsChecker
    {
        ActionResult CheckDefaultViewers(ApplicationSettings appSettings);
    }

    public class AppSettingsChecker : IAppSettingsChecker
    {
        private readonly IDefaultViewerCheck _defaultViewerCheck;

        public AppSettingsChecker(IDefaultViewerCheck defaultViewerCheck)
        {
            _defaultViewerCheck = defaultViewerCheck;
        }

        public ActionResult CheckDefaultViewers(ApplicationSettings appSettings)
        {
            var result = new ActionResult();

            foreach (var defaultViewer in appSettings.DefaultViewers)
            {
                result.AddRange(_defaultViewerCheck.Check(defaultViewer));
            }

            return result;
        }
    }
}
