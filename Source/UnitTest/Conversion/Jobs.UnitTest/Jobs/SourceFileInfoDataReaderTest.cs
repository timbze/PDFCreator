using NUnit.Framework;
using pdfforge.DataStorage;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Utilities.Tokens;

namespace pdfforge.PDFCreator.UnitTest.Conversion.Jobs.Jobs
{
    [TestFixture]
    public class SourceFileInfoDataReaderTest
    {
        [TestCase(JobType.PsJob)]
        [TestCase(JobType.XpsJob)]
        public void ValidSourceFileInfo_CanBeWrittenAndReadAgain(JobType type)
        {
            var sfi = BuildSourceFileInfo(type);

            var infFileName = "somefile.inf";

            var reader = new SourceFileInfoDataReader();
            var data = Data.CreateDataStorage();

            reader.WriteSourceFileInfoToData(data, "theSection", sfi);

            var result = reader.ReadSourceFileInfoFromData(infFileName, data, "theSection");

            AssertSfiAreEqual(result, sfi);
        }

        [Test]
        public void IfDataIsEmpty_SetsDefaults()
        {
            var reader = new SourceFileInfoDataReader();
            var data = Data.CreateDataStorage();

            var result = reader.ReadSourceFileInfoFromData(@"some\inf\file", data, "theSection");
            Assert.IsNotNull(result);
        }

        private SourceFileInfo BuildSourceFileInfo(JobType type)
        {
            var sfi = new SourceFileInfo();
            sfi.Filename = "MyFileName";
            sfi.SessionId = 321;
            sfi.WinStation = "SomeStation";
            sfi.Author = "Me";
            sfi.ClientComputer = "MyComputer";
            sfi.PrinterName = "SomePrinterName";
            sfi.JobCounter = 4243;
            sfi.JobId = 24;
            sfi.DocumentTitle = "My awesome Document";
            sfi.Type = type;
            sfi.TotalPages = 341;
            sfi.Copies = 3;
            sfi.UserTokenEvaluated = true;

            var userToken = new UserToken();
            userToken.AddKeyValuePair("testkey", "test value");
            sfi.UserToken = userToken;

            return sfi;
        }

        private static void AssertSfiAreEqual(SourceFileInfo result, SourceFileInfo sfi)
        {
            Assert.AreEqual(result.Filename, sfi.Filename);
            Assert.AreEqual(result.SessionId, sfi.SessionId);
            Assert.AreEqual(result.WinStation, sfi.WinStation);
            Assert.AreEqual(result.Author, sfi.Author);
            Assert.AreEqual(result.ClientComputer, sfi.ClientComputer);
            Assert.AreEqual(result.PrinterName, sfi.PrinterName);
            Assert.AreEqual(result.JobCounter, sfi.JobCounter);
            Assert.AreEqual(result.JobId, sfi.JobId);
            Assert.AreEqual(result.DocumentTitle, sfi.DocumentTitle);
            Assert.AreEqual(result.Type, sfi.Type);
            Assert.AreEqual(result.Copies, sfi.Copies);
            Assert.AreEqual(result.UserTokenEvaluated, sfi.UserTokenEvaluated);
            Assert.AreEqual(result.UserToken.KeyValueDict, sfi.UserToken.KeyValueDict);
        }
    }
}
