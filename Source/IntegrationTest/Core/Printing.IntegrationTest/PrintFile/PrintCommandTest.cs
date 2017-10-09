using NUnit.Framework;
using PDFCreator.TestUtilities;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.Core.Printing.Printing;
using pdfforge.PDFCreator.Utilities;
using System;

namespace pdfforge.PDFCreator.IntegrationTest.Core.Printing.PrintFile
{
    [TestFixture]
    internal class PrintCommandTest
    {
        [TearDown]
        public void CleanUp()
        {
            TempFileHelper.CleanUp();
        }

        [Test]
        public void PrintCommand_GivenFileWithUnknownExtension_IsUnprintable()
        {
            var tempFile = TempFileHelper.CreateTempFile("PrintCommand", "test.unknownExtension");

            var printCommand = new PrintCommand(tempFile, "SomePrinter", new FileAssoc());

            Assert.IsFalse(printCommand.IsPrintable);
            Assert.AreEqual(PrintType.Unprintable, printCommand.CommandType);
        }

        [Test]
        public void PrintCommand_GivenValidPrintOnlyFile_AllowsPrint()
        {
            var tempFile = TempFileHelper.CreateTempFile("PrintCommand", "test.ini");

            var printCommand = new PrintCommand(tempFile, "SomePrinter", new FileAssoc());

            Assert.IsTrue(printCommand.CommandType == PrintType.Print);
        }

        [Test]
        public void PrintCommand_GivenValidPrintOnlyFile_HasCorrectStartInfo()
        {
            var tempFile = TempFileHelper.CreateTempFile("PrintCommand", "test.ini");
            var printerHelper = new PrinterHelper();
            var printCommand = new PrintCommand(tempFile, printerHelper.GetDefaultPrinter(), new FileAssoc());

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
            var tempFile = TempFileHelper.CreateTempFile("PrintCommand", "test.ini");
            var printerHelper = new PrinterHelper();
            var printCommand = new PrintCommand(tempFile, printerHelper.GetDefaultPrinter(), new FileAssoc());

            var factory = new MockProcessWrapperFactory(true);
            printCommand.ProcessWrapperFactory = factory;

            printCommand.Print();

            Assert.IsTrue(printCommand.Successful);
            Assert.IsFalse(factory.LastMock.WasKilled);
        }

        [Test]
        public void PrintCommand_GivenValidTextFile_AllowsPrintTo()
        {
            var tempFile = TempFileHelper.CreateTempFile("PrintCommand", "test.txt");

            var printCommand = new PrintCommand(tempFile, "SomePrinter", new FileAssoc());

            Assert.IsTrue(printCommand.CommandType == PrintType.PrintTo);
        }

        [Test]
        public void PrintCommand_GivenValidTextFile_HasCorrectStartInfo()
        {
            var tempFile = TempFileHelper.CreateTempFile("PrintCommand", "test.txt");
            const string printer = "SomePrinter";

            var printCommand = new PrintCommand(tempFile, printer, new FileAssoc());

            var factory = new MockProcessWrapperFactory(true);
            printCommand.ProcessWrapperFactory = factory;

            printCommand.Print();

            Assert.IsTrue(printCommand.Successful);
            Assert.AreEqual(tempFile, factory.LastMock.StartInfo.FileName);
            Assert.AreEqual("\"" + printer + "\"", factory.LastMock.StartInfo.Arguments);
        }

        [Test]
        public void PrintCommand_GivenValidTextFile_IsPrintable()
        {
            var tempFile = TempFileHelper.CreateTempFile("PrintCommand", "test.txt");

            var printCommand = new PrintCommand(tempFile, "SomePrinter", new FileAssoc());

            Assert.IsTrue(printCommand.IsPrintable);
        }

        [Test]
        public void PrintCommand_GivenValidTextFile_PrintsSuccessfully()
        {
            var tempFile = TempFileHelper.CreateTempFile("PrintCommand", "test.txt");

            var printCommand = new PrintCommand(tempFile, "SomePrinter", new FileAssoc());

            var factory = new MockProcessWrapperFactory(true);
            printCommand.ProcessWrapperFactory = factory;

            printCommand.Print();

            Assert.IsTrue(printCommand.Successful);
            Assert.IsFalse(factory.LastMock.WasKilled);
        }

        [Test]
        public void PrintCommand_OnPrintingNonExistingFile_ThrowsException()
        {
            var printCommand = new PrintCommand("NotExistingFile", "SomePrinter", new FileAssoc());

            Assert.Throws<InvalidOperationException>(() => printCommand.Print());
        }

        [Test]
        public void PrintCommand_WithNonExistingFile_IsNotPrintable()
        {
            var printCommand = new PrintCommand("NotExistingFile", "SomePrinter", new FileAssoc());

            Assert.IsFalse(printCommand.IsPrintable);
        }

        [Test]
        public void PrintCommand_WithProcessNotFinishing_GetsKilled()
        {
            var tempFile = TempFileHelper.CreateTempFile("PrintCommand", "test.txt");

            var printCommand = new PrintCommand(tempFile, "SomePrinter", new FileAssoc());

            var factory = new MockProcessWrapperFactory(false);
            printCommand.ProcessWrapperFactory = factory;

            printCommand.Print();

            Assert.IsFalse(printCommand.Successful);
            Assert.IsTrue(factory.LastMock.WasKilled);
        }

        [Test]
        public void PrintOnlyFile_GivenWithoutSettingsDefaultPrinter_ThrowsException()
        {
            var tempFile = TempFileHelper.CreateTempFile("PrintCommand", "test.ini");

            var printCommand = new PrintCommand(tempFile, "SomePrinter", new FileAssoc());

            printCommand.ProcessWrapperFactory = new MockProcessWrapperFactory(true);

            Assert.Throws<InvalidOperationException>(() => printCommand.Print());
        }
    }
}
