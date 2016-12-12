using pdfforge.DynamicTranslator;
using pdfforge.PDFCreator.Conversion.Ghostscript;
using pdfforge.PDFCreator.Core.StartupInterface;

namespace pdfforge.PDFCreator.Core.Startup.StartConditions
{
    public class GhostscriptCondition : IStartupCondition
    {
        private readonly IGhostscriptDiscovery _ghostscriptDiscovery;
        private readonly ITranslator _translator;

        public GhostscriptCondition(IGhostscriptDiscovery ghostscriptDiscovery, ITranslator translator)
        {
            _ghostscriptDiscovery = ghostscriptDiscovery;
            _translator = translator;
        }

        public StartupConditionResult Check()
        {
            if (!GhoscriptIsInstalled())
                return StartupConditionResult.BuildErrorWithMessage((int)ExitCode.GhostScriptNotFound, _translator.GetTranslation("Startup", "NoSupportedGSFound"));

            return StartupConditionResult.BuildSuccess();
        }

        private bool GhoscriptIsInstalled()
        {
            var gsVersion = _ghostscriptDiscovery.GetGhostscriptInstance();

            return gsVersion != null;
        }
    }
}