using NUnit.Framework;
using pdfforge.PDFCreator.Core.Jobs;

namespace PDFCreator.Core.UnitTest
{
    [TestFixture]
    public class GhostscriptJobTest
    {
        [Test]
        public void ExtractGhostscriptErrors_WithJustHeader_ReturnEmptyString()
        {
            string output = "GPL Ghostscript 9.10 (2013-08-30)";
            output += "\nCopyright (C) 2013 Artifex Software, Inc.  All rights reserved.";
            output += "\nThis software comes with NO WARRANTY: see the file PUBLIC for details.";

            var result = GhostscriptJob.ExtractGhostscriptErrors(output);

            Assert.AreEqual("", result);
        }

        [Test]
        public void ExtractGhostscriptErrors_WithHeaderAndLoadingFonts_ReturnEmptyString()
        {
            string output = "GPL Ghostscript 9.10 (2013-08-30)";
            output += "\nCopyright (C) 2013 Artifex Software, Inc.  All rights reserved.";
            output += "\nThis software comes with NO WARRANTY: see the file PUBLIC for details.";
            output += "\nLoading NimbusMonL-Regu font from %rom%Resource/Font/NimbusMonL-Regu... 3678736 2125723 1801368 456346 1 done.";

            var result = GhostscriptJob.ExtractGhostscriptErrors(output);

            Assert.AreEqual("", result);
        }

        [Test]
        public void ExtractGhostscriptErrors_WithHeaderAndLoadingFontsAndPageOutput_ReturnEmptyString()
        {
            string output = "GPL Ghostscript 9.10 (2013-08-30)";
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

            var result = GhostscriptJob.ExtractGhostscriptErrors(output);

            Assert.AreEqual("", result);
        }

        [Test]
        public void ExtractGhostscriptErrors_WithHeaderAndError_ReturnsError()
        {
            string output = "GPL Ghostscript 9.10 (2013-08-30)";
            output += "\nCopyright (C) 2013 Artifex Software, Inc.  All rights reserved.";
            output += "\nThis software comes with NO WARRANTY: see the file PUBLIC for details.";
            output += "\nError: /undefinedfilename in XYZ";

            var result = GhostscriptJob.ExtractGhostscriptErrors(output);

            Assert.AreEqual("Error: /undefinedfilename in XYZ\r\n", result);
        }
    }
}
