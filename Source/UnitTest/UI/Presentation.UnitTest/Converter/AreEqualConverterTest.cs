using System.Globalization;
using NUnit.Framework;
using pdfforge.PDFCreator.UI.Presentation.Converter;

namespace Presentation.UnitTest.Converter
{
    [TestFixture]
    internal class AreEqualConverterTest
    {
        [Test]
        public void AreEqualConverter_WithEqualValue_ReturnsEqualsValue()
        {
            var converter = new AreEqualConverter();
            converter.Reference = "ref";
            converter.EqualValue = "equal";
            converter.NotEqualValue = "notequal";

            Assert.AreSame(converter.EqualValue,
                converter.Convert("ref", typeof(object), null, CultureInfo.InvariantCulture));
        }

        [Test]
        public void ConverterWithReference_WithNonEqualObject_ReturnsNotEqualValue()
        {
            var converter = new AreEqualConverter();
            converter.Reference = "ref";
            converter.EqualValue = "equal";
            converter.NotEqualValue = "notequal";

            Assert.AreSame(converter.NotEqualValue,
                converter.Convert("x", typeof(object), null, CultureInfo.InvariantCulture));
        }

        [Test]
        public void ConverterWithReference_WithoutValues_ReturnsNull()
        {
            var converter = new AreEqualConverter();
            converter.Reference = "ref";
            Assert.IsNull(converter.Convert("x", typeof(object), null, CultureInfo.InvariantCulture));
        }

        [Test]
        public void EmtpyConverter_ReturnsNull()
        {
            var converter = new AreEqualConverter();
            Assert.IsNull(converter.Convert("x", typeof(object), null, CultureInfo.InvariantCulture));
        }
    }
}
