using System.Collections.ObjectModel;
using System.Linq;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Ghostscript;
using pdfforge.PDFCreator.Conversion.Ghostscript.OutputDevices;
using pdfforge.PDFCreator.Conversion.Settings.Enums;

namespace pdfforge.PDFCreator.UnitTest.Conversion.Ghostscript.OutputDevices
{
    [TestFixture]
    internal class GeneralTests
    {
        [SetUp]
        public void SetUp()
        {
            _outputDevice = ParametersTestHelper.GenerateDevice(OutputFormat.Pdf);
            _ghostscriptVersion = ParametersTestHelper.GhostscriptVersionDummie;
        }

        private OutputDevice _outputDevice;
        private Collection<string> _parameterStrings;
        private GhostscriptVersion _ghostscriptVersion;

        [Test]
        public void ParametersTest_AttachmentPage()
        {
            _outputDevice.Job.Profile.AttachmentPage.Enabled = true;
            const string attachmentFile = "AttachmentFile.pdf";
            _outputDevice.Job.Profile.AttachmentPage.File = attachmentFile;

            _parameterStrings = new Collection<string>(_outputDevice.GetGhostScriptParameters(_ghostscriptVersion));

            Assert.Contains(attachmentFile, _parameterStrings, "Missing parameter.");
            var fIndex = _parameterStrings.IndexOf("-f");
            Assert.Less(fIndex, _parameterStrings.IndexOf(attachmentFile), "AttachmentFile not behind -f parameter.");

            _outputDevice.Job.Profile.AttachmentPage.Enabled = false;
            _parameterStrings = new Collection<string>(_outputDevice.GetGhostScriptParameters(_ghostscriptVersion));
            Assert.AreEqual(-1, _parameterStrings.IndexOf(attachmentFile), "Falsely set AttachmentFile.");
        }

        [Test]
        public void ParametersTest_CoverAndAttachmentPage()
        {
            _outputDevice.Job.Profile.CoverPage.Enabled = true;
            const string coverFile = "CoverFile.pdf";
            _outputDevice.Job.Profile.CoverPage.File = coverFile;
            _outputDevice.Job.Profile.AttachmentPage.Enabled = true;
            const string attachmentFile = "AttachmentFile.pdf";
            _outputDevice.Job.Profile.AttachmentPage.File = attachmentFile;

            _parameterStrings = new Collection<string>(_outputDevice.GetGhostScriptParameters(_ghostscriptVersion));

            var coverIndex = _parameterStrings.IndexOf(coverFile);
            var attachmentIndex = _parameterStrings.IndexOf(attachmentFile);

            Assert.LessOrEqual(coverIndex - attachmentIndex, -2,
                "No further (file)parameter between cover and attachment file.");
        }

        [Test]
        public void ParametersTest_CoverPage()
        {
            _outputDevice.Job.Profile.CoverPage.Enabled = true;
            const string coverFile = "CoverFile.pdf";
            _outputDevice.Job.Profile.CoverPage.File = coverFile;

            _parameterStrings = new Collection<string>(_outputDevice.GetGhostScriptParameters(_ghostscriptVersion));

            Assert.Contains(coverFile, _parameterStrings, "Missing parameter.");
            var fIndex = _parameterStrings.IndexOf("-f");
            Assert.Less(fIndex, _parameterStrings.IndexOf(coverFile), "CoverFile not behind -f parameter.");

            _outputDevice.Job.Profile.CoverPage.Enabled = false;
            _parameterStrings = new Collection<string>(_outputDevice.GetGhostScriptParameters(_ghostscriptVersion));
            Assert.AreEqual(-1, _parameterStrings.IndexOf(coverFile), "Falsely set CoverFile.");
        }

        [Test]
        public void ParametersTest_Stamping()
        {
            _outputDevice.Job.Profile.Stamping.Enabled = true;
            _parameterStrings = new Collection<string>(_outputDevice.GetGhostScriptParameters(_ghostscriptVersion));

            Assert.IsNotNull(_parameterStrings.FirstOrDefault(x => x.EndsWith(".stm")), "Missing stamp file.");
        }
    }
}