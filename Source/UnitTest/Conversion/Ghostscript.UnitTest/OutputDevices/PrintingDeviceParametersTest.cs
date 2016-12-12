using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SystemInterface.IO;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Ghostscript;
using pdfforge.PDFCreator.Conversion.Ghostscript.OutputDevices;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Utilities;
using Rhino.Mocks;

namespace pdfforge.PDFCreator.UnitTest.Conversion.Ghostscript.OutputDevices
{
    [TestFixture]
    internal class PrintingDeviceParametersTest
    {
        [SetUp]
        public void SetUp()
        {
            _printerStub = MockRepository.GenerateStub<PrinterWrapper>(); // TODO Use NSubstitute and Interface

            var jobStub = ParametersTestHelper.GenerateJobStub(OutputFormat.Txt);
            jobStub.OutputFiles = new List<string>();
            jobStub.OutputFiles.Add(TestFileDummie);

            var fileStub = MockRepository.GenerateStub<IFile>();

            var osHelperStub = MockRepository.GenerateStub<IOsHelper>();
            osHelperStub.Stub(x => x.WindowsFontsFolder).Return(ParametersTestHelper.WindowsFontsFolderDummie);

            var commandLineUtilStub = MockRepository.GenerateStub<ICommandLineUtil>();

            _printingDevice = new PrintingDevice(jobStub, _printerStub, fileStub, osHelperStub, commandLineUtilStub);
            _printingDevice.Job.OutputFiles.Add(TestFileDummie);

            _ghostscriptVersion = ParametersTestHelper.GhostscriptVersionDummie;
        }

        private OutputDevice _printingDevice;
        private Collection<string> _parameterStrings;
        private GhostscriptVersion _ghostscriptVersion;
        private PrinterWrapper _printerStub;

        private const string PrinterName = "PrinterTestName";
        private const string TestFileDummie = "TestFileDummie";

        public string GetMarkString(Collection<string> parameters)
        {
            var markString = parameters.First(x => x.StartsWith("mark "));
            Assert.IsNotNull(markString, "Missing mark parameter string.");
            return markString;
        }

        [Test]
        public void CheckDeviceIndependentDefaultParameters()
        {
            _parameterStrings = new Collection<string>(_printingDevice.GetGhostScriptParameters(_ghostscriptVersion));
            ParametersTestHelper.CheckDefaultParameters(_parameterStrings);
        }

        [Test]
        public void CheckDeviceSpecificDefaultParameters()
        {
            _parameterStrings = new Collection<string>(_printingDevice.GetGhostScriptParameters(_ghostscriptVersion));

            Assert.Contains("-c", _parameterStrings, "Missing device default parameter.");
            var markString = GetMarkString(_parameterStrings);

            var cIndex = _parameterStrings.IndexOf("-c");
            var markIndex = _parameterStrings.IndexOf(markString);
            Assert.Less(cIndex, markIndex, "-c must be set in front of the mark parameters.");

            Assert.IsTrue(markString.Contains("mark /NoCancel true /BitsPerPixel 24 "), "Missing mark parameter: \"mark /NoCancel true /BitsPerPixel 24 \"");

            const string markParameter = "/UserSettings 1 dict dup /DocumentName (" + TestFileDummie + ") put (mswinpr2) finddevice putdeviceprops setdevice";
            Assert.IsTrue(markString.Contains(markParameter), "Missing mark parameter: " + markParameter);
        }

        [Test]
        public void PrintingDevice_ParametersTest_DefaultPrinter_IsNotValid()
        {
            _printingDevice.Job.Profile.Printing.Enabled = true;
            _printingDevice.Job.Profile.Printing.SelectPrinter = SelectPrinter.DefaultPrinter;
            _printingDevice.Job.Profile.Printing.PrinterName = "Some different PrinterName";

            _printerStub.Stub(x => x.IsValid).Return(false);
            _printerStub.PrinterName = PrinterName;

            var exception = Assert.Throws<Exception>(
                () => { _parameterStrings = new Collection<string>(_printingDevice.GetGhostScriptParameters(_ghostscriptVersion)); });
            Assert.AreEqual("100", exception.Message, "Wrong errorcode in exception.");
        }

        [Test]
        public void PrintingDevice_ParametersTest_DefaultPrinter_IsValid()
        {
            _printingDevice.Job.Profile.Printing.Enabled = true;
            _printingDevice.Job.Profile.Printing.SelectPrinter = SelectPrinter.DefaultPrinter;
            _printingDevice.Job.Profile.Printing.PrinterName = "Some different PrinterName";

            _printerStub.Stub(x => x.IsValid).Return(true);
            _printerStub.PrinterName = PrinterName;

            _parameterStrings = new Collection<string>(_printingDevice.GetGhostScriptParameters(_ghostscriptVersion));

            var markString = GetMarkString(_parameterStrings);
            Assert.IsTrue(markString.Contains("/OutputFile (\\\\\\\\spool\\\\" + PrinterName + ")"));
        }

        [Test]
        public void PrintingDevice_ParametersTest_DuplexDisabled_PrinterCanDuplex()
        {
            _printingDevice.Job.Profile.Printing.Enabled = true;
            _printingDevice.Job.Profile.Printing.SelectPrinter = SelectPrinter.DefaultPrinter;
            _printingDevice.Job.Profile.Printing.Duplex = DuplexPrint.Disable;

            _printerStub.Stub(x => x.IsValid).Return(true);
            _printerStub.PrinterName = PrinterName;
            _printerStub.Stub(x => x.CanDuplex).Return(true);

            _parameterStrings = new Collection<string>(_printingDevice.GetGhostScriptParameters(_ghostscriptVersion));

            Assert.IsNull(_parameterStrings.FirstOrDefault(x => x.StartsWith("<< /Duplex")), "Falsely set duplex parameter.");
        }

        [Test]
        public void PrintingDevice_ParametersTest_DuplexLongEdge_PrinterCanDuplex()
        {
            _printingDevice.Job.Profile.Printing.Enabled = true;
            _printingDevice.Job.Profile.Printing.SelectPrinter = SelectPrinter.DefaultPrinter;
            _printingDevice.Job.Profile.Printing.Duplex = DuplexPrint.LongEdge;

            _printerStub.Stub(x => x.IsValid).Return(true);
            _printerStub.PrinterName = PrinterName;
            _printerStub.Stub(x => x.CanDuplex).Return(true);

            _parameterStrings = new Collection<string>(_printingDevice.GetGhostScriptParameters(_ghostscriptVersion));

            Assert.Contains("<< /Duplex true /Tumble false >> setpagedevice ", _parameterStrings, "Missing parameter.");
        }

        [Test]
        public void PrintingDevice_ParametersTest_DuplexLongEdge_PrinterCanDuplexIsFalse()
        {
            _printingDevice.Job.Profile.Printing.Enabled = true;
            _printingDevice.Job.Profile.Printing.SelectPrinter = SelectPrinter.DefaultPrinter;
            _printingDevice.Job.Profile.Printing.Duplex = DuplexPrint.LongEdge;

            _printerStub.Stub(x => x.IsValid).Return(true);
            _printerStub.PrinterName = PrinterName;
            _printerStub.Stub(x => x.CanDuplex).Return(false);

            _parameterStrings = new Collection<string>(_printingDevice.GetGhostScriptParameters(_ghostscriptVersion));

            Assert.IsNull(_parameterStrings.FirstOrDefault(x => x.StartsWith("<< /Duplex")), "Falsely set duplex parameter.");
        }

        [Test]
        public void PrintingDevice_ParametersTest_DuplexShortEdge_PrinterCanDuplex()
        {
            _printingDevice.Job.Profile.Printing.Enabled = true;
            _printingDevice.Job.Profile.Printing.SelectPrinter = SelectPrinter.DefaultPrinter;
            _printingDevice.Job.Profile.Printing.Duplex = DuplexPrint.ShortEdge;

            _printerStub.Stub(x => x.IsValid).Return(true);
            _printerStub.PrinterName = PrinterName;
            _printerStub.Stub(x => x.CanDuplex).Return(true);

            _parameterStrings = new Collection<string>(_printingDevice.GetGhostScriptParameters(_ghostscriptVersion));

            Assert.Contains("<< /Duplex true /Tumble true >> setpagedevice ", _parameterStrings, "Missing parameter.");
        }

        [Test]
        public void PrintingDevice_ParametersTest_DuplexShortEdge_PrinterCanDuplexIsFalse()
        {
            _printingDevice.Job.Profile.Printing.Enabled = true;
            _printingDevice.Job.Profile.Printing.SelectPrinter = SelectPrinter.DefaultPrinter;
            _printingDevice.Job.Profile.Printing.Duplex = DuplexPrint.ShortEdge;

            _printerStub.Stub(x => x.IsValid).Return(true);
            _printerStub.PrinterName = PrinterName;
            _printerStub.Stub(x => x.CanDuplex).Return(false);

            _parameterStrings = new Collection<string>(_printingDevice.GetGhostScriptParameters(_ghostscriptVersion));

            Assert.IsNull(_parameterStrings.FirstOrDefault(x => x.StartsWith("<< /Duplex")), "Falsely set duplex parameter.");
        }

        [Test]
        public void PrintingDevice_ParametersTest_PrinterDialog()
        {
            _printingDevice.Job.Profile.Printing.Enabled = true;
            _printingDevice.Job.Profile.Printing.SelectPrinter = SelectPrinter.ShowDialog;
            _printingDevice.Job.Profile.Printing.PrinterName = PrinterName;

            _printerStub.Stub(x => x.IsValid).Return(true);
            _printerStub.PrinterName = "Some different PrinterName";

            _parameterStrings = new Collection<string>(_printingDevice.GetGhostScriptParameters(_ghostscriptVersion));

            var markString = GetMarkString(_parameterStrings);
            Assert.IsFalse(markString.Contains("/OutputFile (\\\\\\\\spool\\\\"));
        }

        [Test]
        public void PrintingDevice_ParametersTest_SelectedPrinter_IsNotValid()
        {
            _printingDevice.Job.Profile.Printing.Enabled = true;
            _printingDevice.Job.Profile.Printing.SelectPrinter = SelectPrinter.SelectedPrinter;
            _printingDevice.Job.Profile.Printing.PrinterName = PrinterName;

            _printerStub.Stub(x => x.IsValid).Return(false);
            _printerStub.PrinterName = "Some different PrinterName";

            var exception = Assert.Throws<Exception>(
                () => { _parameterStrings = new Collection<string>(_printingDevice.GetGhostScriptParameters(_ghostscriptVersion)); });
            Assert.AreEqual("101", exception.Message, "Wrong errorcode in exception.");
        }

        [Test]
        public void PrintingDevice_ParametersTest_SelectedPrinter_IsValid()
        {
            _printingDevice.Job.Profile.Printing.Enabled = true;
            _printingDevice.Job.Profile.Printing.SelectPrinter = SelectPrinter.SelectedPrinter;
            _printingDevice.Job.Profile.Printing.PrinterName = PrinterName;

            _printerStub.Stub(x => x.IsValid).Return(true);
            _printerStub.PrinterName = "Some different PrinterName";

            _parameterStrings = new Collection<string>(_printingDevice.GetGhostScriptParameters(_ghostscriptVersion));

            var markString = GetMarkString(_parameterStrings);
            Assert.IsTrue(markString.Contains("/OutputFile (\\\\\\\\spool\\\\" + PrinterName + ")"));
        }
    }
}