using System.Runtime.InteropServices;
using pdfforge.PDFCreator.COM;
using NUnit.Framework;
using pdfforge.PDFCreator;
using PDFCreator.TestUtilities;

namespace PDFCreator.IntegrationTest.COM
{
    [TestFixture]
    class PdfCreatorTest
    {
        private PdfCreatorObj _pdfCreatorObj;

        [SetUp]
        public void SetUp()
        {
            _pdfCreatorObj = new PdfCreatorObj();
        }

        [TearDown]
        public void TearDown()
        {
            
        }

        #region AddFileToQueue_Tests

        [Test]
        public void WithNullStringAsArgument_ThrowComException()
        {
            var ex = Assert.Throws<COMException>(() => _pdfCreatorObj.AddFileToQueue(null));
            StringAssert.Contains("The specified path must not be empty or uninitiliazed.", ex.Message);
        }

        [Test]
        public void WithEmptyStringAsArgument_ThrowComException()
        {
            var ex = Assert.Throws<COMException>(() => _pdfCreatorObj.AddFileToQueue(string.Empty));
            StringAssert.Contains("The specified path must not be empty or uninitiliazed.", ex.Message);
        }

        [Test]
        public void WithNonExistingFile_ThrowComException()
        {
            var ex = Assert.Throws<COMException>(() => _pdfCreatorObj.AddFileToQueue("asdasdasdasd"));
            StringAssert.Contains("File with such a path doesn't exist. Please check if the specified path is correct.", ex.Message);
        }

        [Test]
        public void WithPdfFileAsArgument_AddItToQueue()
        {
            var testHelper = new TestHelper("PDFTest");
            var path = testHelper.GenerateTestFile(TestFile.PDFCreatorTestpagePDF);    //The pdf file content is irrelevant for this test.
            var queueInstance = JobInfoQueue.Instance;
            var jobNumber = queueInstance.Count;

            _pdfCreatorObj.AddFileToQueue(path);

            Assert.AreEqual(++jobNumber, queueInstance.Count);
            queueInstance.Remove(queueInstance.JobInfos[--jobNumber], true);
            testHelper.CleanUp();
        }

        [Test]
        public void WithPsFileAsArgument_AddItToQueue()
        {
            var testHelper = new TestHelper("PsTest");
            var path = testHelper.GenerateTestFile(TestFile.PDFCreatorTestpagePs);
            var queueInstance = JobInfoQueue.Instance;
            var jobNumber = queueInstance.Count;

            _pdfCreatorObj.AddFileToQueue(path);

            Assert.AreEqual(++jobNumber, queueInstance.Count);
            queueInstance.Remove(queueInstance.JobInfos[--jobNumber], true);
            testHelper.CleanUp();
        }

        [Test]
        public void WithInvalidFileExtensionAsArgument_ThrowComException()
        {
            var testHelper = new TestHelper("CmdFileTest");                                         //Important here: Neither a .ps file nor a .pdf file
            var path = testHelper.GenerateTestFile(TestFile.ScriptCopyFilesToDirectoryCMD);
            var ex = Assert.Throws<COMException>(() => _pdfCreatorObj.AddFileToQueue(path));
            
            testHelper.CleanUp();
            StringAssert.Contains("Only .ps and .pdf files can be directly added to the queue.", ex.Message);
        }

        [Test]
        public void WithOtherInvalidFileExtensionAsArgument_ThrowComException()
        {
            var testHelper = new TestHelper("CertFileTest");
            var filePath = testHelper.GenerateTestFile(TestFile.CertificationFileP12);
            var ex = Assert.Throws<COMException>(() => _pdfCreatorObj.AddFileToQueue(filePath));

            testHelper.CleanUp();
            StringAssert.Contains("Only .ps and .pdf files can be directly added to the queue.", ex.Message);
        }

        #endregion
    }
}
