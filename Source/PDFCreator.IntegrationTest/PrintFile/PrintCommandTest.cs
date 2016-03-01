using System;
using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;
using pdfforge.PDFCreator.PrintFile;
using pdfforge.PDFCreator.Shared.Helper;
using pdfforge.PDFCreator.Utilities.Process;
using PDFCreator.TestUtilities;
using PDFCreator.UnitTest.Mocks;

namespace PDFCreator.IntegrationTest.PrintFile
{
    [TestFixture]
    class PrintCommandTest
    {
        [TearDown]
        public void CleanUp()
        {
            TempFileHelper.CleanUp();
        }

        [Test]
        public void PrintCommand_WithNonExistingFile_IsNotPrintable()
        {
            var printCommand = new PrintCommand("NotExistingFile", "SomePrinter");

            Assert.IsFalse(printCommand.IsPrintable);
        }

        [Test]
        public void PrintCommand_OnPrintingNonExistingFile_ThrowsException()
        {
            var printCommand = new PrintCommand("NotExistingFile", "SomePrinter");

            Assert.Throws<InvalidOperationException>(() => printCommand.Print());
        }

        [Test]
        public void PrintCommand_GivenFileWithUnknownExtension_IsUnprintable()
        {
            string tempFile = TempFileHelper.CreateTempFile("PrintCommand", "test.unknownExtension");

            var printCommand = new PrintCommand(tempFile, "SomePrinter");

            Assert.IsFalse(printCommand.IsPrintable);
            Assert.AreEqual(PrintType.Unprintable, printCommand.CommandType);
        }

        [Test]
        public void PrintCommand_GivenValidTextFile_IsPrintable()
        {
            string tempFile = TempFileHelper.CreateTempFile("PrintCommand", "test.txt");

            var printCommand = new PrintCommand(tempFile, "SomePrinter");

            Assert.IsTrue(printCommand.IsPrintable);
        }

        [Test]
        public void PrintCommand_GivenValidTextFile_AllowsPrintTo()
        {
            string tempFile = TempFileHelper.CreateTempFile("PrintCommand", "test.txt");

            var printCommand = new PrintCommand(tempFile, "SomePrinter");

            Assert.IsTrue(printCommand.CommandType == PrintType.PrintTo);
        }

        [Test]
        public void PrintCommand_GivenValidPrintOnlyFile_AllowsPrint()
        {
            string tempFile = TempFileHelper.CreateTempFile("PrintCommand", "test.ini");

            var printCommand = new PrintCommand(tempFile, "SomePrinter");

            Assert.IsTrue(printCommand.CommandType == PrintType.Print);
        }

        [Test]
        public void PrintCommand_GivenValidTextFile_HasCorrectStartInfo()
        {
            string tempFile = TempFileHelper.CreateTempFile("PrintCommand", "test.txt");
            const string printer = "SomePrinter";

            var printCommand = new PrintCommand(tempFile, printer);

            var factory = new MockProcessWrapperFactory(true);
            printCommand.ProcessWrapperFactory = factory;

            printCommand.Print();

            Assert.IsTrue(printCommand.Successful);
            Assert.AreEqual(tempFile, factory.LastMock.StartInfo.FileName);
            Assert.AreEqual("\"" + printer + "\"", factory.LastMock.StartInfo.Arguments);
        }

        [Test]
        public void PrintCommand_GivenValidTextFile_PrintsSuccessfully()
        {
            string tempFile = TempFileHelper.CreateTempFile("PrintCommand", "test.txt");

            var printCommand = new PrintCommand(tempFile, "SomePrinter");

            var factory = new MockProcessWrapperFactory(true);
            printCommand.ProcessWrapperFactory = factory;

            printCommand.Print();

            Assert.IsTrue(printCommand.Successful);
            Assert.IsFalse(factory.LastMock.WasKilled);
        }

        [Test]
        public void PrintCommand_GivenValidPrintOnlyFile_HasCorrectStartInfo()
        {
            string tempFile = TempFileHelper.CreateTempFile("PrintCommand", "test.ini");
            var printerHelper = new PrinterHelper();
            var printCommand = new PrintCommand(tempFile, printerHelper.GetDefaultPrinter());

            var factory = new MockProcessWrapperFactory(true);
            printCommand.ProcessWrapperFactory = factory;

            printCommand.Print();

            Assert.IsTrue(printCommand.Successful);
            Assert.AreEqual(tempFile, factory.LastMock.StartInfo.FileName);
            Assert.IsNullOrEmpty(factory.LastMock.StartInfo.Arguments);
        }

        [Test]
        public void PrintCommand_GivenValidPrintOnlyFile_PrintsSuccessfully()
        {
            string tempFile = TempFileHelper.CreateTempFile("PrintCommand", "test.ini");
            var printerHelper = new PrinterHelper();
            var printCommand = new PrintCommand(tempFile, printerHelper.GetDefaultPrinter());

            var factory = new MockProcessWrapperFactory(true);
            printCommand.ProcessWrapperFactory = factory;

            printCommand.Print();

            Assert.IsTrue(printCommand.Successful);
            Assert.IsFalse(factory.LastMock.WasKilled);
        }

        [Test]
        public void PrintCommand_WithProcessNotFinishing_GetsKilled()
        {
            string tempFile = TempFileHelper.CreateTempFile("PrintCommand", "test.txt");

            var printCommand = new PrintCommand(tempFile, "SomePrinter");

            var factory = new MockProcessWrapperFactory(false);
            printCommand.ProcessWrapperFactory = factory;

            printCommand.Print();

            Assert.IsFalse(printCommand.Successful);
            Assert.IsTrue(factory.LastMock.WasKilled);
        }

        [Test]
        public void PrintOnlyFile_GivenWithoutSettingsDefaultPrinter_ThrowsException()
        {
            string tempFile = TempFileHelper.CreateTempFile("PrintCommand", "test.ini");

            var printCommand = new PrintCommand(tempFile, "SomePrinter");

            printCommand.ProcessWrapperFactory = new MockProcessWrapperFactory(true);

            Assert.Throws<InvalidOperationException>(() => printCommand.Print());
            
        }
    }

    internal class MockProcessWrapperFactory : ProcessWrapperFactory
    {
        private bool ExitImmediately { get; set; }
        public ProcessWrapperMock LastMock { get; private set; }

        public IList<ProcessWrapperMock> CreatedMocks { get; private set; }

        public MockProcessWrapperFactory(bool exitImmediately)
        {
            CreatedMocks = new List<ProcessWrapperMock>();
            ExitImmediately = exitImmediately;
        }

        public override ProcessWrapper BuildProcessWrapper(ProcessStartInfo startInfo)
        {
            LastMock = new ProcessWrapperMock(startInfo);
            LastMock.ExitImmediately = ExitImmediately;

            CreatedMocks.Add(LastMock);

            return LastMock;
        }
    }
}
