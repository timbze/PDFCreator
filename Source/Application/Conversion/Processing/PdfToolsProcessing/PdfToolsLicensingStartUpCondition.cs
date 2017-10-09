using pdfforge.PDFCreator.Core.Startup.Translations;
using pdfforge.PDFCreator.Core.StartupInterface;
using pdfforge.PDFCreator.Utilities;
using Translatable;

namespace pdfforge.PDFCreator.Conversion.Processing.PdfToolsProcessing
{
    public class PdfToolsLicensingStartUpCondition : IStartupCondition
    {
        private readonly IPdfToolsLicensing _pdfToolsLicensing;
        private readonly IOsHelper _osHelper;
        private readonly IAssemblyHelper _assemblyHelper;
        private readonly ProgramTranslation _translation;

        public bool CanRequestUserInteraction => false;

        public PdfToolsLicensingStartUpCondition(IPdfToolsLicensing pdfToolsLicensing, ITranslationFactory translationFactory, IOsHelper osHelper, IAssemblyHelper assemblyHelper)
        {
            _pdfToolsLicensing = pdfToolsLicensing;
            _osHelper = osHelper;
            _assemblyHelper = assemblyHelper;
            _translation = translationFactory.CreateTranslation<ProgramTranslation>();
        }

        public StartupConditionResult Check()
        {
            var libPath = _assemblyHelper.GetAssemblyDirectory() + "\\lib\\";
            libPath += _osHelper.Is64BitProcess ? "x64" : "x86";
            _osHelper.AddDllDirectorySearchPath(libPath);

            if (_pdfToolsLicensing.Apply())
                return StartupConditionResult.BuildSuccess();

            var exitCode = (int)_pdfToolsLicensing.ExitCode;

            return StartupConditionResult.BuildErrorWithMessage(exitCode, _translation.GetFormattedErrorWithLicensedComponentTranslation(exitCode), showMessage: true);
        }
    }
}
