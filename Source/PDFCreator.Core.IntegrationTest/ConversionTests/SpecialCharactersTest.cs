using System.IO;
using NUnit.Framework;
using pdfforge.PDFCreator.Core.Settings.Enums;
using PDFCreator.TestUtilities;

namespace PDFCreator.Core.IntegrationTest.ConversionTests
{
    [TestFixture]
    class SpecialCharactersTest
    {
        private TestHelper _th;
        
        [SetUp]
        public void SetUp()
        {
            _th = new TestHelper("SpecialCharactersTest");
        }

        [TearDown]
        public void CleanUp()
        {
            _th.CleanUp();
        }

        private void CheckOutputFiles()
        {
            Assert.True(_th.Job.OutputFiles.Count > 0, "Did not create output files.");

            foreach (string file in _th.Job.OutputFiles)
            {
                Assert.IsTrue(File.Exists(file), "Output file '" + file + "' does not exist!");
                FileInfo fi = new FileInfo(file);
                Assert.IsTrue(fi.Length > 0, "Output file '" + file + "' is empty!");
            }
        }

        [Test]
        public void SpecialCharacters_in_OutputFilename()
        {
            _th.GenerateGsJob(PSfiles.ElevenTextPages, OutputFormat.Pdf);

            _th.SetFilenameTemplate(TestHelper.SpecialCharactersString + ".pdf");

            _th.RunGsJob();

            CheckOutputFiles();
        }

        [Test]
        public void SpecialCharacters_in_TempFolder_Pdf()
        {
            var subTempFolderWithSpecialCharacters = Path.Combine(_th.TmpTestFolder, TestHelper.SpecialCharactersString);
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Pdf, subTempFolderWithSpecialCharacters);

            _th.RunGsJob();

            CheckOutputFiles();
        }

        [Test]
        public void SpecialCharacters_in_TempFolder_PdfA1B()
        {
            var subTempFolderWithSpecialCharacters = Path.Combine(_th.TmpTestFolder, TestHelper.SpecialCharactersString);
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.PdfA1B, subTempFolderWithSpecialCharacters);

            _th.RunGsJob();

            CheckOutputFiles();
        }

        [Test]
        public void SpecialCharacters_in_TempFolder_PdfA2B()
        {
            var subTempFolderWithSpecialCharacters = Path.Combine(_th.TmpTestFolder, TestHelper.SpecialCharactersString);
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.PdfA2B, subTempFolderWithSpecialCharacters);

            _th.RunGsJob();

            CheckOutputFiles();
        }

        [Test]
        public void SpecialCharacters_in_TempFolder_PdfX()
        {
            var subTempFolderWithSpecialCharacters = Path.Combine(_th.TmpTestFolder, TestHelper.SpecialCharactersString);
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.PdfX, subTempFolderWithSpecialCharacters);

            _th.RunGsJob();

            CheckOutputFiles();
        }

        [Test]
        public void SpecialCharacters_in_SourceFile()
        {
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Pdf);
            var sourceFileWithSpecialCharacters =
                Path.Combine(Path.GetDirectoryName(_th.Job.JobInfo.SourceFiles[0].Filename), 
                        Path.GetFileNameWithoutExtension(_th.Job.JobInfo.SourceFiles[0].Filename)
                      + TestHelper.SpecialCharactersString
                      + Path.GetExtension(_th.Job.JobInfo.SourceFiles[0].Filename));

            File.Move(_th.Job.JobInfo.SourceFiles[0].Filename, sourceFileWithSpecialCharacters);
            _th.Job.JobInfo.SourceFiles[0].Filename = sourceFileWithSpecialCharacters;
            _th.RunGsJob();

            CheckOutputFiles();
        }

        [Test]
        public void SpecialCharacters_Stamping()
        {
            var subTempFolderWithSpecialCharacters = Path.Combine(_th.TmpTestFolder, TestHelper.SpecialCharactersString);
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.PdfX, subTempFolderWithSpecialCharacters);

            _th.Job.Profile.Stamping.Enabled = true;
            _th.Job.Profile.Stamping.StampText = TestHelper.SpecialCharactersString;

            _th.RunGsJob();

            CheckOutputFiles();
        }

        [Test]
        public void SpecialCharacters_in_CoverFile()
        {
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Pdf);

            var coverFile = _th.GenerateTestFile(TestFile.Cover2PagesPDF);
            var coverFileWithSpecialCharacters =
                      Path.Combine(Path.GetDirectoryName(coverFile),
                      Path.GetFileNameWithoutExtension(coverFile)
                      + TestHelper.SpecialCharactersString
                      + Path.GetExtension(coverFile));
            File.Move(coverFile, coverFileWithSpecialCharacters);
            
            _th.Job.Profile.CoverPage.Enabled = true;
            _th.Job.Profile.CoverPage.File = coverFileWithSpecialCharacters;

            _th.RunGsJob();

            CheckOutputFiles();
        }

        [Test]
        public void SpecialCharacters_in_AttachmentFile()
        {
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Pdf);

            var attachmentFile = _th.GenerateTestFile(TestFile.Attachment3PagesPDF);
            var attachmentFileWithSpecialCharacters =
                      Path.Combine(Path.GetDirectoryName(attachmentFile),
                      Path.GetFileNameWithoutExtension(attachmentFile)
                      + TestHelper.SpecialCharactersString
                      + Path.GetExtension(attachmentFile));
            File.Move(attachmentFile, attachmentFileWithSpecialCharacters);

            _th.Job.Profile.AttachmentPage.Enabled = true;
            _th.Job.Profile.AttachmentPage.File = attachmentFileWithSpecialCharacters;

            _th.RunGsJob();

            CheckOutputFiles();
        }
    }
}
