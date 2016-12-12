using System.Globalization;
using pdfforge.DynamicTranslator;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.Core.Services.Translation
{
    public class ErrorCodeInterpreter
    {
        private readonly ITranslator _translator;

        public ErrorCodeInterpreter(ITranslator translator)
        {
            _translator = translator;
        }

        public string GetFirstErrorText(ActionResult actionResult, bool withNumber)
        {
            if (actionResult.Count > 0)
                return GetErrorText(actionResult[0], withNumber);
            return "";
        }

        public string GetErrorText(ErrorCode errorCode, bool withNumber)
        {
            var errorNumber = (int) errorCode;

            var errorCodeSection = errorNumber.ToString();
            var errorMessage = _translator.GetTranslation("ErrorCodes", errorCodeSection);

            if (string.IsNullOrWhiteSpace(errorMessage))
                errorMessage = StringValueAttribute.GetValue(errorCode);

            if (string.IsNullOrWhiteSpace(errorMessage))
                errorMessage = _translator.GetTranslation("ErrorCodes", "Default");

            if (withNumber)
                return errorNumber + " - " + errorMessage;

            return errorMessage;
        }
    }
}