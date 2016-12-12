using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Ghostscript;
using pdfforge.PDFCreator.Conversion.Ghostscript.OutputDevices;
using pdfforge.PDFCreator.Conversion.Settings.Enums;

namespace pdfforge.PDFCreator.UnitTest.Conversion.Ghostscript.OutputDevices
{
    internal class PngDeviceParametersTest
    {
        private GhostscriptVersion _ghostscriptVersion;
        private Collection<string> _parameterStrings;
        private OutputDevice _pngDevice;

        [SetUp]
        public void SetUp()
        {
            _pngDevice = ParametersTestHelper.GenerateDevice(OutputFormat.Png);
            _ghostscriptVersion = ParametersTestHelper.GhostscriptVersionDummie;
        }

        [Test]
        public void CheckDeviceIndependentDefaultParameters()
        {
            _parameterStrings = new Collection<string>(_pngDevice.GetGhostScriptParameters(_ghostscriptVersion));
            ParametersTestHelper.CheckDefaultParameters(_parameterStrings);
        }

        [Test]
        public void CheckDeviceSpecificDefaultParameters()
        {
            _parameterStrings = new Collection<string>(_pngDevice.GetGhostScriptParameters(_ghostscriptVersion));

            var dpiParameter = "-r" + _pngDevice.Job.Profile.PngSettings.Dpi;
            Assert.Contains(dpiParameter, _parameterStrings, "Missing default device parameter.");

            Assert.Contains("-dTextAlphaBits=4", _parameterStrings, "Missing default device parameter.");
            Assert.Contains("-dGraphicsAlphaBits=4", _parameterStrings, "Missing default device parameter.");

            var outputFileParameter = _parameterStrings.First(x => x.StartsWith("-sOutputFile="));
            Assert.IsNotNull(outputFileParameter, "Missing -sOutputFile parameter.");
            outputFileParameter = outputFileParameter.Replace("-sOutputFile=", "");
            var requiredOutput = Path.Combine(_pngDevice.Job.JobTempOutputFolder, _pngDevice.Job.JobTempFileName) + "%d.png";
            Assert.AreEqual(requiredOutput, outputFileParameter, "Failure in output");
        }

        [Test]
        public void ParametersTest_ColorBlackWhite_4Dpi()
        {
            _pngDevice.Job.Profile.PngSettings.Color = PngColor.BlackWhite;
            _pngDevice.Job.Profile.PngSettings.Dpi = 4;

            _parameterStrings = new Collection<string>(_pngDevice.GetGhostScriptParameters(_ghostscriptVersion));

            Assert.Contains("-sDEVICE=pngmonod", _parameterStrings, "Missing parameter.");
        }

        [Test]
        public void ParametersTest_Color24Bit_5Dpi()
        {
            _pngDevice.Job.Profile.PngSettings.Color = PngColor.Color24Bit;
            _pngDevice.Job.Profile.PngSettings.Dpi = 5;

            _parameterStrings = new Collection<string>(_pngDevice.GetGhostScriptParameters(_ghostscriptVersion));

            Assert.Contains("-sDEVICE=png16m", _parameterStrings, "Missing parameter.");
        }

        [Test]
        public void ParametersTest_Color32BitTransp_800Dpi()
        {
            _pngDevice.Job.Profile.PngSettings.Color = PngColor.Color32BitTransp;
            _pngDevice.Job.Profile.PngSettings.Dpi = 800;

            _parameterStrings = new Collection<string>(_pngDevice.GetGhostScriptParameters(_ghostscriptVersion));

            Assert.Contains("-sDEVICE=pngalpha", _parameterStrings, "Missing parameter.");
        }

        [Test]
        public void ParametersTest_Color4Bit_1600Dpi()
        {
            _pngDevice.Job.Profile.PngSettings.Color = PngColor.Color4Bit;
            _pngDevice.Job.Profile.PngSettings.Dpi = 1600;

            _parameterStrings = new Collection<string>(_pngDevice.GetGhostScriptParameters(_ghostscriptVersion));

            Assert.Contains("-sDEVICE=png16", _parameterStrings, "Missing parameter.");
        }

        [Test]
        public void ParametersTest_Color8Bit_2400Dpi()
        {
            _pngDevice.Job.Profile.PngSettings.Color = PngColor.Color8Bit;
            _pngDevice.Job.Profile.PngSettings.Dpi = 2400;

            _parameterStrings = new Collection<string>(_pngDevice.GetGhostScriptParameters(_ghostscriptVersion));

            Assert.Contains("-sDEVICE=png256", _parameterStrings, "Missing parameter.");
        }

        [Test]
        public void ParametersTest_ColorGray8Bit_2401Dpi()
        {
            _pngDevice.Job.Profile.PngSettings.Color = PngColor.Gray8Bit;
            _pngDevice.Job.Profile.PngSettings.Dpi = 2401;

            _parameterStrings = new Collection<string>(_pngDevice.GetGhostScriptParameters(_ghostscriptVersion));

            Assert.Contains("-sDEVICE=pnggray", _parameterStrings, "Missing parameter.");
        }
    }
}