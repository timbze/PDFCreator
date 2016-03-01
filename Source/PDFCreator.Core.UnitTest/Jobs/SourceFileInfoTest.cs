using NUnit.Framework;
using pdfforge.DataStorage;
using pdfforge.PDFCreator.Core.Jobs;

namespace PDFCreator.Core.UnitTest.Jobs
{
    [TestFixture]
    public class SourceFileInfoTest
    {
        [Test]
        public void WriteSourceFileInfo_WritesCorrectInfoToData()
        {
            var sfi = new SourceFileInfo();
            sfi.DocumentTitle = "title";
            sfi.WinStation = "winstation";
            sfi.Author = "author";
            var data = Data.CreateDataStorage();

            sfi.WriteSourceFileInfo(data, "0\\");
            
            Assert.AreEqual(sfi.DocumentTitle, data.GetValue("0\\DocumentTitle"));
            Assert.AreEqual(sfi.WinStation, data.GetValue("0\\WinStation"));
            Assert.AreEqual(sfi.Author, data.GetValue("0\\UserName"));

            // TODO extend this
        }

        [Test]
        public void WriteSourceFileInfo_WritesCorrectSourceFileType()
        {
            var data = Data.CreateDataStorage();

            var sfi = new SourceFileInfo();
            sfi.Type = JobType.XpsJob;
            sfi.WriteSourceFileInfo(data, "0\\");

            Assert.AreEqual("xps", data.GetValue("0\\SourceFileType"));
        }
    }
}
