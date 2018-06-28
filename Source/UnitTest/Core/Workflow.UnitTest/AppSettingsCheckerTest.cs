using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Workflow;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.UnitTest.Core.Workflow
{
    public class AppSettingsCheckerTest
    {
        private AppSettingsChecker _appSettingsChecker;
        private IDefaultViewerCheck _defaultViewerCheck;
        private ApplicationSettings _applicationSettings;
        private DefaultViewer _defaultViewer;
        private IFile _file;

        [SetUp]
        public void SetUp()
        {
            _defaultViewerCheck = Substitute.For<IDefaultViewerCheck>();
            _file = Substitute.For<IFile>();
            _appSettingsChecker = new AppSettingsChecker(_defaultViewerCheck);

            _applicationSettings = new ApplicationSettings();
            _defaultViewer = new DefaultViewer();
            _defaultViewer.IsActive = true;
            _defaultViewer.Path = "Some Path";
            _applicationSettings.DefaultViewers.Add(_defaultViewer);
        }

        [Test]
        public void DefaultViewerWithoutErrors_ReturnsTrue()
        {
            _defaultViewerCheck.Check(Arg.Any<DefaultViewer>()).Returns(new ActionResult());

            var result = _appSettingsChecker.CheckDefaultViewers(_applicationSettings);

            Assert.IsTrue(result);
        }

        [Test]
        public void DefaultViewerWithError_ReturnsError()
        {
            _defaultViewer.OutputFormat = OutputFormat.Pdf;
            _defaultViewer.Path = "";
            _defaultViewerCheck.Check(_defaultViewer).Returns(new ActionResult(ErrorCode.DefaultViewer_Not_Found));

            var result = _appSettingsChecker.CheckDefaultViewers(_applicationSettings);

            Assert.Contains(ErrorCode.DefaultViewer_Not_Found, result, "Did not detect error.");
        }

        [Test]
        public void DefaultViewerWithMultipleErrors_ReturnsError()
        {
            _defaultViewer.OutputFormat = OutputFormat.Pdf;
            _defaultViewer.Path = "";
            var checkResult = new ActionResult();
            checkResult.AddRange(new[] { ErrorCode.DefaultViewer_Not_Found, ErrorCode.DefaultViewer_PathIsEmpty_for_Pdf });
            _defaultViewerCheck.Check(_defaultViewer).Returns(checkResult);

            var result = _appSettingsChecker.CheckDefaultViewers(_applicationSettings);

            Assert.Contains(ErrorCode.DefaultViewer_Not_Found, result, "Did not detect error.");
            Assert.Contains(ErrorCode.DefaultViewer_PathIsEmpty_for_Pdf, result, "Did not detect error.");
        }
    }
}
