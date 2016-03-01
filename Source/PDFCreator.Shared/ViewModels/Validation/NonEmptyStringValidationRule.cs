using System;
using System.Globalization;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.Shared.ViewModels.Validation
{
    /// <summary>
    /// NonEmptyStringValidationRule ensures that a string is neither null nor empty
    /// </summary>
    public class NonEmptyStringValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var str = value as string;
            if (String.IsNullOrEmpty(str))
                return new ValidationResult(false, null);
            
            return new ValidationResult(true, null);
        }
    }
}
