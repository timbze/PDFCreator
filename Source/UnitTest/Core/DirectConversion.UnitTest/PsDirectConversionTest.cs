using System.IO;
using SystemInterface.IO;
using NSubstitute;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace pdfforge.PDFCreator.Core.DirectConversion.UnitTest
{
    [TestFixture]
    class PsDirectConversionTest : DirectConversionTestBase
    {
        protected override DirectConversionBase BuildDirectConversion()
        {
            return Fixture.Freeze<PsDirectConversion>();
        }

        protected override void ConfigureValidFileOpenRead(IFile file, string filename)
        {
            ConfigureFileOpenRead(file, filename, "something...\r\n%%Page:\r\n");
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
    }
}
