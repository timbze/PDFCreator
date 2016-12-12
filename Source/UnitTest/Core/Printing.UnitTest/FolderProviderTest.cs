using System.IO;
using SystemInterface.IO;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;
using pdfforge.PDFCreator.Core.Printing;
using pdfforge.PDFCreator.Core.Printing.Port;

namespace pdfforge.PDFCreator.UnitTest.Core.Printing
{
    [TestFixture]
    public class FolderProviderTest
    {
        [SetUp]
        public void Setup()
        {
            _printerPortReader = Substitute.For<IPrinterPortReader>();
            _path = Substitute.For<IPath>();
            _path.Combine(Arg.Any<string>(), Arg.Any<string>()).Returns(info => Path.Combine(info.ArgAt<string>(0), info.ArgAt<string>(1)));
            _path.GetTempPath().Returns(TempPath);
        }

        private IPrinterPortReader _printerPortReader;
        private IPath _path;
        private const string TempPath = "MyTempPath";

        [Test]
        public void AfterConstruction_SpoolFolderIsSetCorrectly()
        {
            var port = new PrinterPort();
            port.TempFolderName = "MyTempFolderName";
            _printerPortReader.ReadPrinterPort("pdfcmon").Returns(port);
            var folderProvider = new FolderProvider(_printerPortReader, _path);

            var expectedTempFolder = Path.Combine(TempPath, "MyTempFolderName");
            expectedTempFolder = Path.Combine(expectedTempFolder, "Spool");

            Assert.AreEqual(expectedTempFolder, folderProvider.SpoolFolder);
        }

        [Test]
        public void AfterContruction_TempFolderIsSetCorrectly()
        {
            var port = new PrinterPort();
            port.TempFolderName = "MyTempFolderName";
            _printerPortReader.ReadPrinterPort("pdfcmon").Returns(port);
            var folderProvider = new FolderProvider(_printerPortReader, _path);

            var expectedTempFolder = Path.Combine(TempPath, "MyTempFolderName");
            expectedTempFolder = Path.Combine(expectedTempFolder, "Temp");

            Assert.AreEqual(expectedTempFolder, folderProvider.TempFolder);
        }

        [Test]
        public void IfPrinterPortCantBeRead_UsesPDFCreatorAsTempFolderName()
        {
            _printerPortReader.ReadPrinterPort("pdfcmon").ReturnsNull();
            var folderProvider = new FolderProvider(_printerPortReader, _path);

            var expectedTempFolder = Path.Combine(TempPath, "PDFCreator");
            expectedTempFolder = Path.Combine(expectedTempFolder, "Temp");

            Assert.AreEqual(expectedTempFolder, folderProvider.TempFolder);
        }
    }
}