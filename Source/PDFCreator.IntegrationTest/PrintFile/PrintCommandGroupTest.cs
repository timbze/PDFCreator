using System;
using System.Linq;
using NUnit.Framework;
using pdfforge.PDFCreator.PrintFile;
using PDFCreator.TestUtilities;

namespace PDFCreator.IntegrationTest.PrintFile
{
    [TestFixture]
    class PrintCommandGroupTest
    {
        [TearDown]
        public void CleanUp()
        {
            TempFileHelper.CleanUp();
        }

        [Test]
        public void Group_WithEmptyList_PrintReturnsTrue()
        {
            var printCommandGroup = new PrintCommandGroup();

            Assert.IsTrue(printCommandGroup.PrintAll());
        }

        [Test]
        public void Group_WithUnprintableFile_PrintThrowsException()
        {
            var printCommandGroup = new PrintCommandGroup();
            printCommandGroup.ProcessWrapperFactory = new MockProcessWrapperFactory(true);

            const string printer = "SomePrinter";

            printCommandGroup.Add(new PrintCommand(TempFileHelper.CreateTempFile("PrintCommandGroup", "test.invalid"), printer));

            Assert.Throws<InvalidOperationException>(() => printCommandGroup.PrintAll());
        }

        [Test]
        public void Group_WithManyFilesAndOneUnprintableFile_PrintThrowsExceptionWithoutPrintingOneFile()
        {
            var printCommandGroup = new PrintCommandGroup();
            var factory = new MockProcessWrapperFactory(true);
            printCommandGroup.ProcessWrapperFactory = factory;

            const string printer = "SomePrinter";

            printCommandGroup.Add(new PrintCommand(TempFileHelper.CreateTempFile("PrintCommandGroup", "test.txt"), printer));
            printCommandGroup.Add(new PrintCommand(TempFileHelper.CreateTempFile("PrintCommandGroup", "test.invalid"), printer));

            try
            {
                printCommandGroup.PrintAll();
            }
            catch (InvalidOperationException)
            {
            }

            Assert.IsEmpty(factory.CreatedMocks);
        }

        [Test]
        public void Group_WithManyFiles_PrintsEveryFile()
        {
            var printCommandGroup = new PrintCommandGroup();
            var factory = new MockProcessWrapperFactory(true);
            printCommandGroup.ProcessWrapperFactory = factory;

            const string printer = "SomePrinter";

            printCommandGroup.Add(new PrintCommand(TempFileHelper.CreateTempFile("PrintCommandGroup", "test1.txt"), printer));
            printCommandGroup.Add(new PrintCommand(TempFileHelper.CreateTempFile("PrintCommandGroup", "test2.txt"), printer));
            printCommandGroup.Add(new PrintCommand(TempFileHelper.CreateTempFile("PrintCommandGroup", "test3.txt"), printer));

            printCommandGroup.PrintAll();

            foreach (var mock in factory.CreatedMocks)
            {
                Assert.IsTrue(mock.WasStarted, "Print Process was not started for " + mock.StartInfo.FileName);
            }

            Assert.AreEqual(printCommandGroup.Count(), factory.CreatedMocks.Count);
        }

        [Test]
        public void Group_WithPprintableFile_PrintReturnsTrue()
        {
            var printCommandGroup = new PrintCommandGroup();
            printCommandGroup.ProcessWrapperFactory = new MockProcessWrapperFactory(true);

            const string printer = "SomePrinter";

            printCommandGroup.Add(new PrintCommand(TempFileHelper.CreateTempFile("PrintCommandGroup", "test.txt"), printer));

            Assert.IsTrue(printCommandGroup.PrintAll());
        }
    }
}
