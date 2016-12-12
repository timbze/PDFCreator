using System.Collections.Generic;
using pdfforge.PDFCreator.Utilities.Tokens;

namespace pdfforge.PDFCreator.Conversion.Actions
{
    public interface IScriptActionHelper
    {
        string ComposeScriptPath(string path, TokenReplacer tokenReplacer);
        string ComposeScriptParameters(string parameterString, IList<string> outputFiles, TokenReplacer tokenReplacer);
    }
}