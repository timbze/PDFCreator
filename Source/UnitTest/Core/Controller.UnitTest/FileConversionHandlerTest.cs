using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.DirectConversion;
using pdfforge.PDFCreator.Core.Printing.Printing;
using System.Collections.Generic;
using System.Linq;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.UnitTest.Core.Controller
{
    [TestFixture]
    internal class FileConversionHandlerTest
    {
        [SetUp]
        public void Setup()
        {
            _directConversionHelper = Substitute.For<IDirectConversionHelper>();
            _printFileHelper = Substitute.For<IPrintFileHelper>();
            _fileWrap = Substitute.For<IFile>();

            _fileConversionHandler = new FileConversionHandler(_directConversionHelper, _printFileHelper, _fileWrap);

            _droppedFiles = new List<string> { "file1", "file2", "file3" };
        }

        private IFileConversionHandler _fileConversionHandler;
        private IDirectConversionHelper _directConversionHelper;
        private IPrintFileHelper _printFileHelper;
        private IFile _fileWrap;
        private List<string> _droppedFiles;

        [Test]
        public void HandleDroppedFiles_EmptyList_DoNothing()
        {
            var droppedFiles = new List<string>();
            _fileConversionHandler.HandleFileList(droppedFiles);
            Assert.AreEqual(0, _directConversionHelper.ReceivedCalls().Count());
            Assert.AreEqual(0, _printFileHelper.ReceivedCalls().Count());
            Assert.AreEqual(0, _fileWrap.ReceivedCalls().Count());
        }

        [Test]
        public void HandleDroppedFiles_ListWithExistingAndNonExistingPrintableFiles_ExistingFilesGetPrinted()
        {
            _fileWrap.Exists(_droppedFiles[0]).Returns(true);
            _fileWrap.Exists(_droppedFiles[1]).Returns(false);
            _fileWrap.Exists(_droppedFiles[2]).Returns(true);

            _directConversionHelper.CanConvertDirectly("").ReturnsForAnyArgs(false);
            _printFileHelper.AddFiles(null).ReturnsForAnyArgs(true);
            _fileConversionHandler.HandleFileList(_droppedFiles);
            Assert.AreEqual(2, _directConversionHelper.ReceivedCalls().Count());

            _printFileHelper.Received(1).AddFiles(Arg.Is<List<string>>(x => (x.Count == 2) && x[0].Equals(_droppedFiles[0]) && x[1].Equals(_droppedFiles[2])));
            _printFileHelper.Received(1).PrintAll();
        }

        [Test]
        public void HandleDroppedFiles_ListWithExistingAndNotExitstingDirectConversionFiles_ExistingFilesGetConverteDirectly()
        {
            _fileWrap.Exists(_droppedFiles[0]).Returns(true);
            _fileWrap.Exists(_droppedFiles[1]).Returns(false);
            _fileWrap.Exists(_droppedFiles[2]).Returns(true);
            _directConversionHelper.CanConvertDirectly("").ReturnsForAnyArgs(true);

            _fileConversionHandler.HandleFileList(_droppedFiles);

            Assert.AreEqual(0, _printFileHelper.ReceivedCalls().Count());
            _directConversionHelper.Received(1).CanConvertDirectly(_droppedFiles[0]);
            _directConversionHelper.Received(1).CanConvertDirectly(_droppedFiles[2]);
            _directConversionHelper.Received(1).ConvertDirectly(_droppedFiles[0]);
            _directConversionHelper.Received(1).ConvertDirectly(_droppedFiles[2]);
        }

        [Test]
        public void HandleDroppedFiles_ListWithExistingDirectConversionFiles_GetConverteDirectly()
        {
            _fileWrap.Exists("").ReturnsForAnyArgs(true);
            _directConversionHelper.CanConvertDirectly("").ReturnsForAnyArgs(true);

            _fileConversionHandler.HandleFileList(_droppedFiles);

            Assert.AreEqual(0, _printFileHelper.ReceivedCalls().Count());
            _directConversionHelper.Received(1).CanConvertDirectly(_droppedFiles[0]);
            _directConversionHelper.Received(1).CanConvertDirectly(_droppedFiles[1]);
            _directConversionHelper.Received(1).CanConvertDirectly(_droppedFiles[2]);
            _directConversionHelper.Received(1).ConvertDirectly(_droppedFiles[0]);
            _directConversionHelper.Received(1).ConvertDirectly(_droppedFiles[1]);
            _directConversionHelper.Received(1).ConvertDirectly(_droppedFiles[2]);
        }

        [Test]
        public void HandleDroppedFiles_ListWithExistingNonPrintableFiles_PrintFileHelperAddFilesAndPrintAllGetCalled()
        {
            _fileWrap.Exists("").ReturnsForAnyArgs(true);
            _directConversionHelper.CanConvertDirectly("").ReturnsForAnyArgs(false);
            _printFileHelper.AddFiles(null).ReturnsForAnyArgs(true);
            _fileConversionHandler.HandleFileList(_droppedFiles);
            Assert.AreEqual(3, _directConversionHelper.ReceivedCalls().Count());

            _printFileHelper.Received(1).AddFiles(Arg.Is<List<string>>(x => x.SequenceEqual(_droppedFiles)));
            _printFileHelper.Received(1).PrintAll();
        }

        [Test]
        public void HandleDroppedFiles_ListWithExistingPrintableFiles_PrintFileHelperAddFilesReturnsFalse_PrintAllDoeNotGetCalled()
        {
            _fileWrap.Exists("").ReturnsForAnyArgs(true);
            _directConversionHelper.CanConvertDirectly("").ReturnsForAnyArgs(false);
            _printFileHelper.AddFiles(null).ReturnsForAnyArgs(false);
            _fileConversionHandler.HandleFileList(_droppedFiles);
            Assert.AreEqual(3, _directConversionHelper.ReceivedCalls().Count());

            _printFileHelper.Received(1).AddFiles(Arg.Is<List<string>>(x => x.SequenceEqual(_droppedFiles)));
            _printFileHelper.DidNotReceive().PrintAll();
        }

        [Test]
        public void HandleDroppedFiles_ListWithNonExistingFiles_DoNothing()
        {
            _fileWrap.Exists("").ReturnsForAnyArgs(false);
            _fileConversionHandler.HandleFileList(_droppedFiles);
            Assert.AreEqual(0, _directConversionHelper.ReceivedCalls().Count());
            Assert.AreEqual(0, _printFileHelper.ReceivedCalls().Count());
        }
    }
}
