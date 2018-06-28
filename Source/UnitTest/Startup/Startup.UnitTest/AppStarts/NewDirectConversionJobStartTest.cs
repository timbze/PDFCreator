using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Internal;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Core.DirectConversion;
using pdfforge.PDFCreator.Core.Startup.AppStarts;
using pdfforge.PDFCreator.Core.Workflow;

namespace pdfforge.PDFCreator.UnitTest.Startup.AppStarts
{
    internal class NewDirectConversionJobStartTestClass : NewDirectConversionJobStart
    {
        public NewDirectConversionJobStartTestClass(IDirectConversion directConversion)
            : this(Substitute.For<IJobInfoQueue>(), Substitute.For<IMaybePipedApplicationStarter>(), Substitute.For<IJobInfoManager>(), directConversion)
        { }

        public NewDirectConversionJobStartTestClass(IJobInfoQueue jobInfoQueue, IMaybePipedApplicationStarter maybePipedApplicationStarter, IJobInfoManager jobInfoManager, IDirectConversion directConversion)
            : base(jobInfoQueue, maybePipedApplicationStarter, jobInfoManager, directConversion)
        { }

        public string PublicComposePipeMessage()
        {
            return ComposePipeMessage();
        }

        public bool PublicStartApplication()
        {
            return StartApplication();
        }
    }

    [TestFixture]
    public class NewDirectConversionJobStartTest
    {
        private NewDirectConversionJobStartTestClass _newDirectConversionJobStart;
        private IDirectConversion _directConversion;

        [SetUp]
        public void SetUp()
        {
            _directConversion = Substitute.For<IDirectConversion>();
            _newDirectConversionJobStart = new NewDirectConversionJobStartTestClass(_directConversion);
        }

        [Test]
        public void NewDirectConversionJobStart_CallTransformToInfFile_ReutrnsSameNewInfFile()
        {
            _directConversion.TransformToInfFile(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns("someNewInfFile");

            var newInfFile = _newDirectConversionJobStart.PublicComposePipeMessage();

            Assert.AreEqual("NewJob|someNewInfFile", newInfFile);
        }

        [Test]
        public void NewDirecConversionJobStart_TransformToInfFile_StartApplicationReturnsTrue()
        {
            _directConversion.TransformToInfFile(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns("someNewInfFile");

            _newDirectConversionJobStart.PublicComposePipeMessage();
            var resultOfStartApplication = _newDirectConversionJobStart.PublicStartApplication();

            Assert.IsTrue(resultOfStartApplication);
        }

        [Test]
        public void NewDirecConversionJobStart_TransformToInfFileReturnsEmptyString_StartApplicationReturnsFalse()
        {
            _directConversion.TransformToInfFile(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns("");

            _newDirectConversionJobStart.PublicComposePipeMessage();
            var resultOfStartApplication = _newDirectConversionJobStart.PublicStartApplication();

            Assert.IsFalse(resultOfStartApplication);
        }
    }
}
