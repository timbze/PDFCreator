using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.Core.Workflow.Exceptions;
using pdfforge.PDFCreator.Core.Workflow.Queries;

namespace pdfforge.PDFCreator.UnitTest.Core.Workflow
{
    [TestFixture]
    public class ConversionWorkflowTest
    {
        [SetUp]
        public void SetUp()
        {
            
            _jobInfo = new JobInfo();
            _jobInfo.Metadata = new Metadata();
            _job = new Job(_jobInfo, _profile, new JobTranslations(), new Accounts());
            _profileChecker = Substitute.For<IProfileChecker>();
            _profileChecker.ProfileCheck(Arg.Any<ConversionProfile>(), Arg.Any<Accounts>()).Returns(_validActionResult);

            _query = Substitute.For<ITargetFileNameComposer>();
            _jobRunner = Substitute.For<IJobRunner>();
            _jobDataUpdater = Substitute.For<IJobDataUpdater>();
            _errorNotifier = Substitute.For<IErrorNotifier>();
            _workflow = new ConversionWorkflow(_profileChecker, _query, _jobRunner,_jobDataUpdater, _errorNotifier);
        }

        private Job _job;
        private JobInfo _jobInfo;
        private readonly ConversionProfile _profile = new ConversionProfile();
        private IProfileChecker _profileChecker;
        private readonly ActionResult _validActionResult = new ActionResult();
        
        private ConversionWorkflow _workflow;
        private ITargetFileNameComposer _query;
        private IJobRunner _jobRunner;
        private IJobDataUpdater _jobDataUpdater;
        private IErrorNotifier _errorNotifier;

        private void SetUpConditionsForCompleteWorkflow()
        {
            _profileChecker.ProfileCheck(Arg.Any<ConversionProfile>(), Arg.Any<Accounts>()).Returns(_validActionResult);
        }

        [Test]
        public void DoWorkflow_OnFailedJob_CallsErrorNotifier()
        {
            SetUpConditionsForCompleteWorkflow();
            _jobRunner.When(x => x.RunJob(_job)).Do(x => { throw new ProcessingException("", ErrorCode.Conversion_UnknownError);});

            _workflow.RunWorkflow(_job);

            _errorNotifier.Received().Notify(Arg.Any<ActionResult>());
        }

        [Test]
        public void DoWorkflow_OnSuccessfulJob_DoesNotCallErrorNotifier()
        {
            SetUpConditionsForCompleteWorkflow();
            _jobRunner.When(x => x.RunJob(_job)).Do(x => { _job.Completed = true; });

            _workflow.RunWorkflow(_job);

            Assert.IsTrue(_job.Completed);
            _errorNotifier.DidNotReceive().Notify(Arg.Any<ActionResult>());
        }

        [Test]
        public void RunWorkFlow_AbortWorkflowExceptionGetsCatched_WorkflowStepIsAbortedByUser()
        {
            SetUpConditionsForCompleteWorkflow();
            _query.When(x => x.ComposeTargetFileName(Arg.Any<Job>())).Do(x => { throw new AbortWorkflowException("message"); });

            var workflowResult = _workflow.RunWorkflow(_job);
            Assert.AreEqual(WorkflowResult.AbortedByUser, workflowResult);
        }

        [Test]
        public void RunWorkFlow_CheckOrderOfCallsAndActionAssignmentsForCompleteProcess()
        {
            SetUpConditionsForCompleteWorkflow();
            _workflow.RunWorkflow(_job);

            Received.InOrder(() =>
            {
                _jobDataUpdater.UpdateTokensAndMetadata(_job);
                _query.ComposeTargetFileName(_job);
                _profileChecker.ProfileCheck(_job.Profile, Arg.Any<Accounts>());
                _jobRunner.RunJob(_job);
            });
        }

        [Test]
        public void
            RunWorkFlow_QueryTargetFileThrowsManagePrintJobsException_ThrowsManagePrintJobsExceptionAndMetadataGetsReverted
            ()
        {
            _jobDataUpdater.When(x => x.UpdateTokensAndMetadata(Arg.Any<Job>())).Do(x => _workflow.Job.JobInfo.Metadata = null);
            
            _query.When(x => x.ComposeTargetFileName(Arg.Any<Job>())).Do(x => { throw new ManagePrintJobsException(); });
            Assert.Throws<ManagePrintJobsException>(() => _workflow.RunWorkflow(_job), "Did not throw exception");
            Assert.NotNull(_workflow.Job.JobInfo.Metadata, "Metadata not reverted"); //ToDo: Compare with _metadata
        }

        [Test]
        public void RunWorkFlow_WorkflowExceptionGetsCatched_WorkflowStepIsError()
        {
            SetUpConditionsForCompleteWorkflow();
            _query.When(x => x.ComposeTargetFileName(Arg.Any<Job>())).Do(x => { throw new WorkflowException("message"); });

            var workflowResult = _workflow.RunWorkflow(_job);
            Assert.AreEqual(WorkflowResult.Error, workflowResult);
        }

        [Test]
        public void RunWorkFlow_WithErrorsInProfileCheck_ThrowsProcessingException()
        {
            var errorResult = new ActionResult(ErrorCode.Conversion_UnknownError);
            SetUpConditionsForCompleteWorkflow();
            _profileChecker.ProfileCheck(_profile, Arg.Any<Accounts>()).Returns(errorResult);

            var workflowResult = _workflow.RunWorkflow(_job);
            Assert.AreEqual(WorkflowResult.Error, workflowResult);
        }
    }
}