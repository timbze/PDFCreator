using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.UI.Presentation.Workflow;
using System;
using System.Windows.Controls;

namespace Presentation.UnitTest.Workflow.Steps
{
    [TestFixture]
    public class WorkflowStepTest
    {
        private IWorkflowViewModel _workflowViewModel;
        private Action<Job> _handleJobAction = job => { };

        [SetUp]
        public void Setup()
        {
            _workflowViewModel = Substitute.For<IWorkflowViewModel>();
            _workflowViewModel
                .When(x => x.ExecuteWorkflowStep(Arg.Any<Job>()))
                .Do(x =>
                {
                    _handleJobAction(x.Arg<Job>());
                    _workflowViewModel.StepFinished += Raise.EventWith(new object(), new EventArgs());
                });
        }

        [Test]
        public void Create_WithoutPredicate_StepIsRequired()
        {
            var step = WorkflowStep.Create<UserControl>();

            Assert.IsTrue(step.IsStepRequired(null));
        }

        [Test]
        public void Create_NavigationUri_IsTypeName()
        {
            var step = WorkflowStep.Create<UserControl>();

            Assert.AreEqual(nameof(UserControl), step.NavigationUri);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Create_WithPredicate_EvaluatesPredicate(bool isRequired)
        {
            var step = WorkflowStep.Create<UserControl>(job => isRequired);

            Assert.AreEqual(isRequired, step.IsStepRequired(null));
        }

        [Test]
        public void ExecuteStep_ExecutesViewModelStep()
        {
            var stepWasExecuted = false;
            _handleJobAction = job => stepWasExecuted = true;
            var step = WorkflowStep.Create<UserControl>();

            step.ExecuteStep(null, _workflowViewModel);

            Assert.IsTrue(stepWasExecuted);
        }
    }
}
