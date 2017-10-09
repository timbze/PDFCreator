using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs;

namespace pdfforge.PDFCreator.UnitTest.Conversion.Jobs.Actions
{
    [TestFixture]
    internal class ActionResultTest
    {
        [Test]
        public void NewActionResult_NoParameters_ClaimsSuccess()
        {
            var result = new ActionResult();

            Assert.IsTrue(result.IsSuccess);
        }

        [Test]
        public void NewActionResult_NoParameters_HasEmptyErrorList()
        {
            var result = new ActionResult();

            Assert.IsTrue(result.Count == 0);
        }

        [Test]
        public void NewActionResult_WithAddErrorCodeCall_HasThisErrorCode()
        {
            var result = new ActionResult();
            result.Add(ErrorCode.Attachment_NoPdf);

            Assert.Contains(ErrorCode.Attachment_NoPdf, result);
        }

        [Test]
        public void NewActionResult_WithOneFailAndOneSuccess_IsUnsuccessful()
        {
            var result = new ActionResult();
            result.Add(ErrorCode.Attachment_NoPdf);
            result.Add(ErrorCode.Script_GenericError);

            Assert.IsFalse(result.IsSuccess);
        }

        [Test]
        public void NewActionResult_WithSingleActionIdAndError_HasThisErrorCode()
        {
            var result = new ActionResult(ErrorCode.Encryption_Error);

            Assert.Contains(ErrorCode.Encryption_Error, result);
        }

        [Test]
        public void NewActionResult_WithSingleErrorCode_HasThisErrorCode()
        {
            var result = new ActionResult(ErrorCode.Encryption_Error);

            Assert.Contains(ErrorCode.Encryption_Error, result);
        }

        [Test]
        public void NewActionResult_WithTwoAddErrorCodeCall_HasBothErrorCodes()
        {
            var result = new ActionResult();
            result.Add(ErrorCode.Encryption_Error);
            result.Add(ErrorCode.Conversion_UnknownError);

            Assert.Contains(ErrorCode.Encryption_Error, result);
            Assert.Contains(ErrorCode.Conversion_UnknownError, result);
        }
    }
}
