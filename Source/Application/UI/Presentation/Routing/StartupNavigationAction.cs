using pdfforge.PDFCreator.Core.Controller.Routing;

namespace pdfforge.PDFCreator.UI.Presentation.Routing
{
    public class StartupNavigationAction : StartupAction
    {
        public readonly string Region;
        public readonly string Target;

        public StartupNavigationAction(string region, string target)
        {
            Region = region;
            Target = target;
        }
    }
}
