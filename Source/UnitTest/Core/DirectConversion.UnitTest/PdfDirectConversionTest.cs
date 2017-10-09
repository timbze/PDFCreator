using NSubstitute;
using NUnit.Framework;
using Ploeh.AutoFixture;
using System.IO;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Core.DirectConversion.UnitTest
{
    [TestFixture]
    internal class PdfDirectConversionTest : DirectConversionTestBase
    {
        protected override DirectConversionBase BuildDirectConversion()
        {
            return Fixture.Freeze<PdfDirectConversion>();
        }

        protected override void ConfigureValidFileOpenRead(IFile file, string filename)
        {
            ConfigureFileOpenRead(file, filename, "%PDF");
        }

        protected virtual void ConfigureInvalidFileOpenRead(IFile file, string filename)
        {
            ConfigureFileOpenRead(file, filename, "");
        }

        private void ConfigureFileOpenRead(IFile file, string filename, string content)
        {
            var fs = Fixture.Create<IFileStream>();
            fs.StreamInstance.Returns(GenerateStreamFromString(content));
            fs.Length.Returns(x => fs.StreamInstance.Length);
            file.OpenRead(filename).Returns(fs);
        }

        private Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        [Test]
        public void TransformToInfFile_WithInvalidFile_ReturnsEmptyString()
        {
            var filename = @"X:\My Test\file.pdf";
            FileMock.Exists(filename).Returns(true);
            ConfigureInvalidFileOpenRead(FileMock, filename);
            var directConversion = Fixture.Create<DirectConversionBase>();

            var infFile = directConversion.TransformToInfFile(filename);

            Assert.AreEqual("", infFile);
        }
    }
}
