using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using NUnit.Framework;
using pdfforge.PDFCreator.Core.Ghostscript;
using pdfforge.PDFCreator.Core.Ghostscript.OutputDevices;
using pdfforge.PDFCreator.Core.Settings.Enums;

namespace PDFCreator.Core.UnitTest.Ghostscript.OutputDevices
{
    class TiffDeviceParametersTest
    {   
        private OutputDevice _tiffDevice;
        private Collection<string> _parameterStrings;
        private GhostscriptVersion _ghostscriptVersion;

        [SetUp]
        public void SetUp()
        {
            _tiffDevice = ParametersTestHelper.GenerateDevice(OutputFormat.Tif);
            _ghostscriptVersion = ParametersTestHelper.GhostscriptVersionDummie;
        }

        [Test]
        public void CheckDeviceIndependentDefaultParameters()
        {
            _parameterStrings = new Collection<string>(_tiffDevice.GetGhostScriptParameters(_ghostscriptVersion));
            ParametersTestHelper.CheckDefaultParameters(_parameterStrings);
        }

        [Test]
        public void CheckDeviceSpecificDefaultParameters()
        {
            _parameterStrings = new Collection<string>(_tiffDevice.GetGhostScriptParameters(_ghostscriptVersion));

            Assert.Contains("-dTextAlphaBits=4", _parameterStrings, "Missing default device parameter.");
            Assert.Contains("-dGraphicsAlphaBits=4", _parameterStrings, "Missing default device parameter.");
            string dpiParameter = "-r" + _tiffDevice.Job.Profile.TiffSettings.Dpi;
            Assert.Contains(dpiParameter, _parameterStrings, "Missing default  device parameter.");

            var outputFileParameter = _parameterStrings.First(x => x.StartsWith("-sOutputFile="));
            Assert.IsNotNull(outputFileParameter, "Missing -sOutputFile parameter.");
            outputFileParameter = outputFileParameter.Replace("-sOutputFile=", "");
            var requiredOutput = Path.Combine(_tiffDevice.Job.JobTempOutputFolder, _tiffDevice.Job.JobTempFileName) + ".tif";
            Assert.AreEqual(requiredOutput, outputFileParameter, "Failure in output");
        }

        [Test]
        public void ParametersTest_BlackWhite_G4Fax()
        {
            _tiffDevice.Job.Profile.TiffSettings.Color = TiffColor.BlackWhiteG4Fax;

            _parameterStrings = new Collection<string>(_tiffDevice.GetGhostScriptParameters(_ghostscriptVersion));

            Assert.Contains("-sDEVICE=tiffg4", _parameterStrings, "Missing parameter.");
        }

        [Test]
        public void ParametersTest_BlackWhite_G3Fax()
        {
            _tiffDevice.Job.Profile.TiffSettings.Color = TiffColor.BlackWhiteG3Fax;

            _parameterStrings = new Collection<string>(_tiffDevice.GetGhostScriptParameters(_ghostscriptVersion));

            Assert.Contains("-sDEVICE=tiffg3", _parameterStrings, "Missing parameter.");
        }

        [Test]
        public void ParametersTest_BlackWhite_Lzw()
        {
            _tiffDevice.Job.Profile.TiffSettings.Color = TiffColor.BlackWhiteLzw;

            _parameterStrings = new Collection<string>(_tiffDevice.GetGhostScriptParameters(_ghostscriptVersion));

            Assert.Contains("-sDEVICE=tifflzw", _parameterStrings, "Missing parameter.");
        }

        [Test]
        public void ParametersTest_Color12Bit()
        {
            _tiffDevice.Job.Profile.TiffSettings.Color = TiffColor.Color12Bit;

            _parameterStrings = new Collection<string>(_tiffDevice.GetGhostScriptParameters(_ghostscriptVersion));

            Assert.Contains("-sDEVICE=tiff12nc", _parameterStrings, "Missing parameter.");
        }

        [Test]
        public void ParametersTest_Color24Bit()
        {
            _tiffDevice.Job.Profile.TiffSettings.Color = TiffColor.Color24Bit;

            _parameterStrings = new Collection<string>(_tiffDevice.GetGhostScriptParameters(_ghostscriptVersion));

            Assert.Contains("-sDEVICE=tiff24nc", _parameterStrings, "Missing parameter.");
        }

        [Test]
        public void ParametersTest_Grey8Bit()
        {
            _tiffDevice.Job.Profile.TiffSettings.Color = TiffColor.Gray8Bit;

            _parameterStrings = new Collection<string>(_tiffDevice.GetGhostScriptParameters(_ghostscriptVersion));

            Assert.Contains("-sDEVICE=tiffgray", _parameterStrings, "Missing parameter.");
        }

        [Test]
        public void ParametersTest_100_Dpi()
        {
            _tiffDevice.Job.Profile.TiffSettings.Dpi = 100;

            _parameterStrings = new Collection<string>(_tiffDevice.GetGhostScriptParameters(_ghostscriptVersion));

            string dpiParameter = "-r" + _tiffDevice.Job.Profile.TiffSettings.Dpi;
            Assert.Contains(dpiParameter, _parameterStrings, "Missing default  device parameter.");
        }
    }
}
