using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace pdfforge.PDFCreator.Utilities
{
    public enum ValidationResult
    {
        Valid,
        InvalidCharacters,
        WrongFormat
    }

    public class LicenseKeySyntaxChecker
    {
        public string NormalizeLicenseKey(string key)
        {
            if (string.IsNullOrEmpty(key))
                return "";

            var normalizedKey = key.Replace("-", "").ToUpper().Trim();
            return string.Join("-", Split(normalizedKey, 5));
        }

        private IEnumerable<string> Split(string str, int chunkSize)
        {
            var chunks = (int) Math.Ceiling(str.Length/(double) chunkSize);

            return Enumerable.Range(0, chunks)
                .Select(i => GetSafeSubstring(str, i*chunkSize, chunkSize));
        }

        private string GetSafeSubstring(string str, int position, int length)
        {
            if (position + length > str.Length)
                length = str.Length - position;

            return str.Substring(position, length);
        }

        public ValidationResult ValidateLicenseKey(string s)
        {
            var normalizedKey = NormalizeLicenseKey(s);

            var r = new Regex("^[A-Z0-9]*$");
            if (!r.IsMatch(normalizedKey.Replace("-", "")))
                return ValidationResult.InvalidCharacters;

            if (normalizedKey.Length != 35)
                return ValidationResult.WrongFormat;

            return ValidationResult.Valid;
        }
    }
}