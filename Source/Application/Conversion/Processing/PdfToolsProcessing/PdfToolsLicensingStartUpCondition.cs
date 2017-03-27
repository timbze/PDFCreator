using pdfforge.PDFCreator.Core.Startup.Translations;
using pdfforge.PDFCreator.Core.StartupInterface;

namespace pdfforge.PDFCreator.Conversion.Processing.PdfToolsProcessing
{ 
    public class PdfToolsLicensingStartUpCondition : IStartupCondition
    {
        private readonly IPdfToolsLicensing _pdfToolsLicensing;
        private readonly ProgramTranslation _translation;

        public PdfToolsLicensingStartUpCondition(IPdfToolsLicensing pdfToolsLicensing, ProgramTranslation translation)
        {
            _pdfToolsLicensing = pdfToolsLicensing;
            _translation = translation;
        }

        public StartupConditionResult Check()
        {
            if(_pdfToolsLicensing.Apply())
                return StartupConditionResult.BuildSuccess();

            var exitCode = (int) _pdfToolsLicensing.ExitCode;

            return StartupConditionResult.BuildErrorWithMessage(exitCode, _translation.GetFormattedErrorWithLicensedComponentTranslation(exitCode), showMessage: true);
        }
    }
}
