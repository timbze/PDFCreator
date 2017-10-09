using NUnit.Framework;
using PDFCreator.TestUtilities;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using System.IO;
using System.Text;
using System.Xml;

namespace pdfforge.PDFCreator.IntegrationTest.Conversion.Jobs.ConversionTests
{
    [TestFixture]
    internal class TextTest
    {
        private TestHelper _th;

        [SetUp]
        public void SetUp()
        {
            var bootstrapper = new IntegrationTestBootstrapper();
            var container = bootstrapper.ConfigureContainer();
            _th = container.GetInstance<TestHelper>();
            _th.InitTempFolder("TextTest");
        }

        [TearDown]
        public void CleanUp()
        {
            _th.CleanUp();
        }

        [Test]
        public void TestSinglePageFileToText_TextFormat2()
        {
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Txt);
            _th.Job.Profile.TextSettings.Format = 2;
            _th.RunGsJob();

            Assert.AreEqual(1, _th.Job.OutputFiles.Count, "Job created more or less than one pdf text.");
            Assert.IsTrue(File.Exists(_th.Job.OutputFiles[0]), "Outputfile does not exist");
            string content = File.ReadAllText(_th.Job.OutputFiles[0], Encoding.Unicode);

            StringAssert.Contains("[INFOTITLE]", content);
            StringAssert.Contains("The quick brown fox jumps over the lazy dog. 0123456789", content);
        }

        [Test]
        public void TestSinglePageFileToText_TextFormat0()
        {
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Txt);
            _th.Job.Profile.TextSettings.Format = 0;
            _th.RunGsJob();

            Assert.AreEqual(1, _th.Job.OutputFiles.Count, "Job created more or less than one pdf text.");
            Assert.IsTrue(File.Exists(_th.Job.OutputFiles[0]), "Outputfile does not exist");

            var doc = new XmlDocument();
            doc.Load(_th.Job.OutputFiles[0]);

            Assert.IsNotNull(doc.SelectSingleNode("/page/span[@bbox='68 174 112 174']"));
            Assert.AreEqual("O", doc.SelectSingleNode("/page/span/char[@bbox='150 167 179 167']/@c").InnerText);
        }
    }
}
