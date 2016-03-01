using System.IO;
using System.Text;
using NUnit.Framework;
using pdfforge.PDFCreator.Core.Settings.Enums;
using PDFCreator.TestUtilities;

namespace PDFCreator.Core.IntegrationTest.ConversionTests
{
    [TestFixture]
    [Category("LongRunning")]
    class TextTest
    {
        private TestHelper _th;

        private const string TestPageContentTextdevice =
            "                           PDF                                                         PDFCreator\r\n" +
            "                                        Now                                            Opensource\r\n" +
            "                                   [INFOTITLE]\r\n" +
            "                                   Date:                                                                 [INFODATE]\r\n" +
            "                                   Authors:                                                              [INFOAUTHORS]\r\n" +
            "                                   Homepage:                                                             [INFOHOMEPAGE]\r\n" +
            "                                   Pdfcreator.exe:                                                       [INFOPDFCREATOR]\r\n" +
            "                                   Computer:                                                             [INFOCOMPUTER]\r\n" +
            "                                   Windows:                                                              [INFOWINDOWS]\r\n" +
            "                                   64bit:                                                                [INFO64BIT]\r\n" +
            "                                                                                                                            Copyright by\r\n" +
            "                                                                                           Frank Heindörfer & Philip Chinery\r\n" +
            "                                         The quick brown fox jumps over the lazy dog. 0123456789\r\n" +
            "                                         The quick brown fox jumps over the lazy dog. 0123456789\r\n" +
            "                                         The quick brown fox jumps over the lazy dog. 0123456789\r\n" +
            "                                         The quick brown fox jumps over the lazy dog. 0123456789\r\n" +
            "                                         The quick brown fox jumps over the lazy dog. 0123456789\r\n" +
            "                                         The quick brown fox jumps over the lazy dog. 0123456789\r\n" +
            "                                         The quick brown fox jumps over the lazy dog. 0123456789\r\n" +
            "                                         The quick brown fox jumps over the lazy dog. 0123456789\r\n" +
            "                                         The quick brown fox jumps over the lazy dog. 0123456789\r\n" +

            "                                         Color image                                                                                Gray image                                                                                Mono image\r\n"
            ; 

        [SetUp]
        public void SetUp()
        {
            _th = new TestHelper("TextTest");
        }

        [TearDown]
        public void CleanUp()
        {
            _th.CleanUp();
        }

        [Test]
        public void TestSinglePageFileToText()
        {
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Txt);
            _th.RunGsJob();

            Assert.AreEqual(1, _th.Job.OutputFiles.Count, "Job created more or less than one pdf text.");
            Assert.IsTrue(File.Exists(_th.Job.OutputFiles[0]), "Outputfile does not exist");
            string content = File.ReadAllText(_th.Job.OutputFiles[0], Encoding.Unicode);

            Assert.AreEqual(TestPageContentTextdevice, content, "Faulty content in text file.");
        }

        /*
        //For ps2ascii solution 
        
        private const string TestPageTextContentPs2Ascii =
                "\r\n" +
                "\r\n" +
                "Now\r\n" +
                "PDF PDFCreatorOpensource\r\n" +
                "\r\n" +
                "[INFOTITLE]\r\n" +
                "Date: [INFODATE]Authors: [INFOAUTHORS]\r\n" +
                "Homepage: [INFOHOMEPAGE]\r\n" +
                "Pdfcreator.exe: [INFOPDFCREATOR]Computer: [INFOCOMPUTER]\r\n" +
                "Windows: [INFOWINDOWS]64bit: [INFO64BIT]\r\n" +
                "\r\n" +
                "The quick brown fox jumps over the lazy dog. 0123456789The quick brown fox jumps over the lazy dog. 0123456789\r\n" +
                "The quick brown fox jumps over the lazy dog. 0123456789The quick brown fox jumps over the lazy dog. 0123456789\r\n" +
                "\r\n" +
                "The quick brown fox jumps over the lazy dog. 0123456789The quick brown fox jumps over the lazy dog. 0123456789\r\n" +
                "\r\n" +
                "The quick brown fox jumps over the lazy dog. 0123456789The quick brown fox jumps over the lazy dog. 0123456789\r\n" +
                "\r\n" +
                "The quick brown fox jumps over the lazy dog. 0123456789\r\n" +
                "\r\n" +
                "Color image Gray image Mono image";
        
        [Test]
        public void TestSinglePageFileToText()
        {
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Txt);
            _th.RunGsJob();

            Assert.AreEqual(1, _th.Job.OutputFiles.Count, "Job created more or less than one pdf text.");
            Assert.IsTrue(File.Exists(_th.Job.OutputFiles[0]), "Outputfile does not exist");
            string content = File.ReadAllText(_th.Job.OutputFiles[0], System.Text.Encoding.Default);

            Assert.IsTrue(content.StartsWith(_testPageTextContent), "Faulty content in text file.");
        }
        */
    }
}
