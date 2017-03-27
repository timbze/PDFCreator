using System;
using System.Collections.Generic;
using System.Linq;
using pdfforge.PDFCreator.Conversion.Jobs;
using Translatable;

namespace pdfforge.PDFCreator.Core.Services.Translation
{
    public class ErrorCodeInterpreter
    {
        private readonly IList<EnumTranslation<ErrorCode>> _errorCodeTranslations;

        public ErrorCodeInterpreter(ITranslationFactory translationFactory)
        {
            _errorCodeTranslations = translationFactory.CreateEnumTranslation<ErrorCode>();
        }

        public string GetFirstErrorText(ActionResult actionResult, bool withNumber)
        {
            if (actionResult.Count > 0)
                return GetErrorText(actionResult[0], withNumber);
            return "";
        }

        public string GetErrorText(ErrorCode errorCode, bool withNumber)
        {
            var errorTranslation = _errorCodeTranslations.FirstOrDefault(val => val.Value == errorCode);

            if (errorTranslation == null)
                throw new ArgumentException("The error code is not part of the ErrorCode enum!", nameof(errorTranslation));

            var errorNumber = (int) errorCode;

            var errorMessage = errorTranslation.Translation;

            if (withNumber)
                return errorNumber + " - " + errorMessage;

            return errorMessage;
        }
    }
}