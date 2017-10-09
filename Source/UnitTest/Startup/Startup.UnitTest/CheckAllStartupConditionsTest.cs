using NSubstitute;
using NUnit.Framework;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Core.Startup.StartConditions;
using pdfforge.PDFCreator.Core.StartupInterface;
using pdfforge.PDFCreator.UI.Interactions;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.UnitTest.Startup
{
    [TestFixture]
    public class CheckAllStartupConditionsTest
    {
        [SetUp]
        public void Setup()
        {
            _interactionInvoker = Substitute.For<IInteractionInvoker>();
        }

        private IInteractionInvoker _interactionInvoker;

        private CheckAllStartupConditions BuildCheckAllStartupConditions(IList<IStartupCondition> startupConditions)
        {
            return new CheckAllStartupConditions(startupConditions, _interactionInvoker);
        }

        private CheckAllStartupConditions BuildCheckAllStartupConditions(IStartupCondition startupCondition)
        {
            return BuildCheckAllStartupConditions(new[] { startupCondition });
        }

        [Test]
        public void Failure_WithShowMessageFalse_ShowsMessageInteraction()
        {
            var expectedResult = (int)ExitCode.GhostScriptNotFound;
            var message = "some message";

            var failingCondition = Substitute.For<IStartupCondition>();
            failingCondition.Check().Returns(StartupConditionResult.BuildErrorWithMessage(expectedResult, message, false));

            var checker = BuildCheckAllStartupConditions(failingCondition);

            var ex = Assert.Throws<StartupConditionFailedException>(() => checker.CheckAll());
            Assert.AreEqual(expectedResult, ex.ExitCode);

            _interactionInvoker.DidNotReceive().Invoke(Arg.Any<MessageInteraction>());
        }

        [Test]
        public void FailureWithMessage_ShowsMessageInteraction()
        {
            var expectedResult = (int)ExitCode.GhostScriptNotFound;
            var message = "some message";

            var failingCondition = Substitute.For<IStartupCondition>();
            failingCondition.Check().Returns(StartupConditionResult.BuildErrorWithMessage(expectedResult, message));

            var checker = BuildCheckAllStartupConditions(failingCondition);

            var ex = Assert.Throws<StartupConditionFailedException>(() => checker.CheckAll());
            Assert.AreEqual(expectedResult, ex.ExitCode);

            _interactionInvoker.Received().Invoke(Arg.Is<MessageInteraction>(i => i.Text == message));
        }

        [Test]
        public void WithFailingAfterSuccessfulCheck_ReturnsFailure()
        {
            var expectedResult = (int)ExitCode.GhostScriptNotFound;

            var failingCondition = Substitute.For<IStartupCondition>();
            failingCondition.Check().Returns(StartupConditionResult.BuildErrorWithMessage(expectedResult, ""));
            var successfulCondition = Substitute.For<IStartupCondition>();
            successfulCondition.Check().Returns(StartupConditionResult.BuildSuccess());

            var checker = BuildCheckAllStartupConditions(new[] { successfulCondition, failingCondition });

            var ex = Assert.Throws<StartupConditionFailedException>(() => checker.CheckAll());

            Assert.AreEqual(expectedResult, ex.ExitCode);
        }

        [Test]
        public void WithFailingBeforeSuccessfulCheck_ReturnsFailure()
        {
            var expectedResult = (int)ExitCode.GhostScriptNotFound;

            var failingCondition = Substitute.For<IStartupCondition>();
            failingCondition.Check().Returns(StartupConditionResult.BuildErrorWithMessage(expectedResult, ""));
            var successfulCondition = Substitute.For<IStartupCondition>();
            successfulCondition.Check().Returns(StartupConditionResult.BuildSuccess());

            var checker = BuildCheckAllStartupConditions(new[] { failingCondition, successfulCondition });

            var ex = Assert.Throws<StartupConditionFailedException>(() => checker.CheckAll());

            Assert.AreEqual(expectedResult, ex.ExitCode);
        }

        [Test]
        public void WithFailingCheck_ReturnsThatResult()
        {
            var expectedResult = (int)ExitCode.GhostScriptNotFound;
            var condition = Substitute.For<IStartupCondition>();
            condition.Check().Returns(StartupConditionResult.BuildErrorWithMessage(expectedResult, ""));

            var checker = BuildCheckAllStartupConditions(condition);

            var ex = Assert.Throws<StartupConditionFailedException>(() => checker.CheckAll());

            Assert.AreEqual(expectedResult, ex.ExitCode);
        }

        [Test]
        public void WithMultipleSuccessfulChecks_CallsAllChecks()
        {
            var conditions = new[]
            {
                Substitute.For<IStartupCondition>(),
                Substitute.For<IStartupCondition>(),
                Substitute.For<IStartupCondition>()
            };

            foreach (var startupCondition in conditions)
                startupCondition.Check().Returns(x => StartupConditionResult.BuildSuccess());

            var checker = BuildCheckAllStartupConditions(conditions);

            Assert.DoesNotThrow(() => checker.CheckAll());

            foreach (var startupCondition in conditions)
                startupCondition.Received().Check();
        }
    }
}
