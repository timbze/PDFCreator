using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob.PlusHint;
using pdfforge.PDFCreator.UI.Presentation.Workflow.Steps;

namespace Presentation.UnitTest.Workflow.Steps
{
    [TestFixture]
    public class PlusHintStepTest
    {
        private IPlusHintHelper _plusHintHelper;

        [SetUp]
        public void Setup()
        {
            _plusHintHelper = Substitute.For<IPlusHintHelper>();
        }

        [Test]
        public void PlusHintStep_NavigatinUriIsSetProperly()
        {
            var plustHintStep = new PlusHintStep(_plusHintHelper);
            Assert.AreEqual(nameof(PlusHintView), plustHintStep.NavigationUri);
        }

        [Test]
        public void PlusHintStep_IsStepRequired_ReturnsSameValueAsPlusHintHelper()
        {
            _plusHintHelper.QueryDisplayHint().Returns(true);
            var plusHintStep = new PlusHintStep(_plusHintHelper);
            Assert.IsTrue(plusHintStep.IsStepRequired(null));

            _plusHintHelper.QueryDisplayHint().Returns(false);
            plusHintStep = new PlusHintStep(_plusHintHelper);
            Assert.IsFalse(plusHintStep.IsStepRequired(null));
        }
    }
}
