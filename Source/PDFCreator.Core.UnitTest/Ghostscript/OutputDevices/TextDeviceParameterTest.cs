using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using NUnit.Framework;
using pdfforge.PDFCreator.Core.Ghostscript;
using pdfforge.PDFCreator.Core.Ghostscript.OutputDevices;
using pdfforge.PDFCreator.Core.Settings.Enums;

namespace PDFCreator.Core.UnitTest.Ghostscript.OutputDevices
{
    [TestFixture]
    class TextDeviceParameterTest
    {
        private OutputDevice _txtDevice;
        private Collection<string> _parameterStrings;
        private GhostscriptVersion _ghostscriptVersion;
        
        [SetUp]
        public void SetUp()
        {
            _txtDevice = ParametersTestHelper.GenerateDevice(OutputFormat.Txt);
            _ghostscriptVersion = ParametersTestHelper.GhostscriptVersionDummie;
        }

        [Test]
        public void CheckDeviceIndependentDefaultParameters()
        {
            _parameterStrings = new Collection<string>(_txtDevice.GetGhostScriptParameters(_ghostscriptVersion));
            ParametersTestHelper.CheckDefaultParameters(_parameterStrings);
        }

        /*
        //Test for Ps2Ascii solution
        [Test]
        public void CheckDeviceSpecificParameters()
        {
            _parameterStrings = new Collection<string>(_txtDevice.GetGhostScriptParameters(_ghostscriptVersion));
            Assert.Contains("-dNODISPLAY", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dDELAYBIND", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dWRITESYSTEMDICT", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dSIMPLE", _parameterStrings, "Missing parameter.");
            Assert.Contains("-q", _parameterStrings, "Missing parameter.");
            Assert.Contains("ps2ascii.ps", _parameterStrings, "Missing parameter.");

            var outputFileParameter = _parameterStrings.First(x => x.StartsWith("-sstdout="));
            Assert.IsNotNull(outputFileParameter, "Missing -sstdout parameter.");
            Assert.IsTrue(outputFileParameter.EndsWith(".txt", true, null), "Outputfile does not end with .txt");
        }
        */

        [Test]
        public void CheckDeviceSpecificParameters()
        {
            _parameterStrings = new Collection<string>(_txtDevice.GetGhostScriptParameters(_ghostscriptVersion));
            Assert.Contains("-sDEVICE=txtwrite", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dTextFormat=2", _parameterStrings, "Missing parameter.");

            var outputFileParameter = _parameterStrings.First(x => x.StartsWith("-sOutputFile="));
            Assert.IsNotNull(outputFileParameter, "Missing -sOutputFile parameter.");
            outputFileParameter = outputFileParameter.Replace("-sOutputFile=", "");
            var requiredOutput = Path.Combine(_txtDevice.Job.JobTempOutputFolder, _txtDevice.Job.JobTempFileName) + ".txt";
            Assert.AreEqual(requiredOutput, outputFileParameter, "Failure in output");
        }
    }
}