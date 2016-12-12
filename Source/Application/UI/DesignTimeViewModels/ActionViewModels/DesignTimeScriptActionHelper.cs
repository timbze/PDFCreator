using System.Collections.Generic;
using pdfforge.PDFCreator.Conversion.Actions;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Utilities.Tokens;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.ActionViewModels
{
    public class DesignTimeScriptActionHelper : IScriptActionHelper
    {
        public ActionResult Check(ConversionProfile profile)
        {
            return new ActionResult();
        }

        public string ComposeScriptPath(string path, TokenReplacer tokenReplacer)
        {
            return "My Script path";
        }

        public string ComposeScriptParameters(string parameterString, IList<string> outputFiles, TokenReplacer tokenReplacer)
        {
            return "My parameter";
        }
    }
}