using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs;
using System;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.UnitTest.Conversion.Jobs.Jobs
{
    [TestFixture]
    public class ErrorCodeTest
    {
        [Test]
        public void EveryErrorCodeHasUniqueIntValue()
        {
            var values = Enum.GetValues(typeof(ErrorCode));
            var existingValues = new List<int>();

            foreach (var value in values)
            {
                var intValue = (int)value;
                Assert.IsFalse(existingValues.Contains(intValue), $"Error Number {intValue} used more than once");
                existingValues.Add((int)value);
            }
        }
    }
}
