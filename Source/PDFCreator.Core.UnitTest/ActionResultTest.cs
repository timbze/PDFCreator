using NUnit.Framework;
using pdfforge.PDFCreator.Core.Actions;

namespace PDFCreator.Core.UnitTest
{
    [TestFixture]
    class ActionResultTest
    {
        [Test]
        public void NewActionResult_NoParameters_ClaimsSuccess()
        {
            var result = new ActionResult();

            Assert.IsTrue(result.Success);
        }

        [Test]
        public void NewActionResult_NoParameters_HasEmptyErrorList()
        {
            var result = new ActionResult();

            Assert.IsTrue(result.Count == 0);
        }

        [Test]
        public void NewActionResult_WithSingleErrorCode_HasThisErrorCode()
        {
            var result = new ActionResult(10123);

            Assert.Contains(10123, result);
        }

        [Test]
        public void NewActionResult_WithSingleActionIdAndError_HasThisErrorCode()
        {
            var result = new ActionResult(10, 123);

            Assert.Contains(10123, result);
        }

        [Test]
        public void NewActionResult_WithAddErrorCodeCall_HasThisErrorCode()
        {
            var result = new ActionResult();
            result.Add(10, 123);

            Assert.Contains(10123, result);
        }

        [Test]
        public void NewActionResult_WithTwoAddErrorCodeCall_HasBothErrorCodes()
        {
            var result = new ActionResult();
            result.Add(10, 123);
            result.Add(11, 456);

            Assert.Contains(10123, result);
            Assert.Contains(11456, result);
        }

        [Test]
        public void NewActionResult_WithOneFailAndOneSuccess_IsUnsuccessful()
        {
            var result = new ActionResult();
            result.Add(10, 123);
            result.Add(11, 456);

            Assert.IsFalse(result.Success);
        }
    }
}
