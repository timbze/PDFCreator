using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Ghostscript;
using pdfforge.PDFCreator.Conversion.Ghostscript.Conversion;

namespace pdfforge.PDFCreator.UnitTest.Conversion.Jobs
{
    [TestFixture]
    public class GhostscriptConverterTest
    {
        private GhostscriptConverter _ghostscriptConverter;

        [SetUp]
        public void Setup()
        {
            _ghostscriptConverter = new GhostscriptConverter(new GhostscriptVersion("internal", "G:\\gs.exe", "G:\\lib"));
        }

        [Test]
        public void ExtractGhostscriptErrors_WithHeaderAndError_ReturnsError()
        {
            var output = "GPL Ghostscript 9.10 (2013-08-30)";
            output += "\nCopyright (C) 2013 Artifex Software, Inc.  All rights reserved.";
            output += "\nThis software comes with NO WARRANTY: see the file PUBLIC for details.";
            output += "\nError: /undefinedfilename in XYZ";

            var result = _ghostscriptConverter.ExtractGhostscriptErrors(output);

            Assert.AreEqual("Error: /undefinedfilename in XYZ\r\n", result);
        }

        [Test]
        public void ExtractGhostscriptErrors_WithHeaderAndLoadingFonts_ReturnEmptyString()
        {
            var output = "GPL Ghostscript 9.10 (2013-08-30)";
            output += "\nCopyright (C) 2013 Artifex Software, Inc.  All rights reserved.";
            output += "\nThis software comes with NO WARRANTY: see the file PUBLIC for details.";
            output += "\nLoading NimbusMonL-Regu font from %rom%Resource/Font/NimbusMonL-Regu... 3678736 2125723 1801368 456346 1 done.";

            var result = _ghostscriptConverter.ExtractGhostscriptErrors(output);

            Assert.AreEqual("", result);
        }

        [Test]
        public void ExtractGhostscriptErrors_WithHeaderAndLoadingFontsAndPageOutput_ReturnEmptyString()
        {
            var output = "GPL Ghostscript 9.10 (2013-08-30)";
            output += "\nCopyright (C) 2013 Artifex Software, Inc.  All rights reserved.";
            output += "\nThis software comes with NO WARRANTY: see the file PUBLIC for details.";
            output += "\nLoading NimbusMonL-Regu font from %rom%Resource/Font/NimbusMonL-Regu... 3678736 2125723 1801368 456346 1 done.";
            output += "\n%%[ ProductName: GPL Ghostscript ]";
            output += "\n%%";
            output += "\n%%[Page: 1]";
            output += "\n%%";
            output += "\n%%[Page: 2]";
            output += "\n%%";
            output += "\n%%[LastPage]";
            output += "\n%%";

            var result = _ghostscriptConverter.ExtractGhostscriptErrors(output);

            Assert.AreEqual("", result);
        }

        [Test]
        public void ExtractGhostscriptErrors_WithJustHeader_ReturnEmptyString()
        {
            var output = "GPL Ghostscript 9.10 (2013-08-30)";
            output += "\nCopyright (C) 2013 Artifex Software, Inc.  All rights reserved.";
            output += "\nThis software comes with NO WARRANTY: see the file PUBLIC for details.";

            var result = _ghostscriptConverter.ExtractGhostscriptErrors(output);

            Assert.AreEqual("", result);
        }
    }
}