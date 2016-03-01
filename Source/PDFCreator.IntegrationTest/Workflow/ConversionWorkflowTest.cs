using iTextSharp.text.pdf;
using NUnit.Framework;
using pdfforge.PDFCreator.Core.Settings;
using pdfforge.PDFCreator.Core.Settings.Enums;
using pdfforge.PDFCreator.Workflow;
using PDFCreator.TestUtilities;

namespace PDFCreator.IntegrationTest.Workflow
{
    [TestFixture]
    [Category("LongRunning")]
    public class ConversionWorkflowTest
    {
        private TestHelper _th;
        private ConversionWorkflow _workflow;
        private PdfCreatorSettings _settings;

        private ConversionProfile _interactiveProfile;
        private PrinterMapping _interactiveProfileMapping;
        private const string InteractivePrinterName = "InteractivePrinterName";
        private const string InteractiveProfileName = "InteractiveProfileName";
        const string InteractiveProfileGuid = "InteractiveProfileGuid";

        private ConversionProfile _autosaveProfile;
        private PrinterMapping _autosaveProfileMapping;
        private const string AutosavePrinterName = "AutosavePrinterName";
        private const string AutosaveProfileName = "AutosaveProfileName";
        const string AutosaveProfileGuid = "AutosaveProfileGuid";

        [SetUp]
        public void SetUp()
        {
            _interactiveProfile = new ConversionProfile();
            _interactiveProfile.Name = InteractivePrinterName;
            _interactiveProfile.Guid = InteractiveProfileGuid;
            _interactiveProfile.AutoSave.Enabled = false;
            _interactiveProfileMapping = new PrinterMapping(InteractivePrinterName, InteractiveProfileGuid);

            _autosaveProfile = new ConversionProfile();
            _autosaveProfile.Name = AutosaveProfileName;
            _autosaveProfile.Guid = AutosaveProfileGuid;
            _autosaveProfile.AutoSave.Enabled = true;
            _autosaveProfileMapping = new PrinterMapping(AutosavePrinterName, AutosaveProfileGuid);

            _th = new TestHelper("ConversionWorklowTest");
            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.Pdf);
            _settings = new PdfCreatorSettings(null);
        }

        [TearDown]
        public void CleanUp()
        {
            _th.CleanUp();
        }

        [Test]
        public void CreateConversionWorkflowTest_SingleProfileIsAutosave_ReturnAutosaveWorkflow()
        {
            _settings.ConversionProfiles.Add(_autosaveProfile);
            _workflow = WorkflowFactory.CreateWorkflow(_th.JobInfo, _settings);
            Assert.IsInstanceOf<AutoSaveWorkflow>(_workflow, "Wrong type of workflow.");
        }

        [Test]
        public void CreateConversionWorkflowTest_SingleProfileInteractive_ReturnInteractiveWorkflow()
        {
            _settings.ConversionProfiles.Add(_interactiveProfile);
            _workflow = WorkflowFactory.CreateWorkflow(_th.JobInfo, _settings);
            Assert.IsInstanceOf<InteractiveWorkflow>(_workflow, "Wrong type of workflow.");
        }

        [Test]
        public void CreateConversionWorkflowTest_NoMapping_TwoProfiles_OneIsDefault_NoneIsLastUsedProfile_ReturnWorkflowWithDefaultProfile()
        {
            _settings.ApplicationSettings.LastUsedProfileGuid = "NoneOfTheProfileGuids";

            _settings.ConversionProfiles.Add(_autosaveProfile); //For this test the default profile must not be the first in list!
            _interactiveProfile.Guid = "DefaultGuid";
            _settings.ConversionProfiles.Add(_interactiveProfile);
            _workflow = WorkflowFactory.CreateWorkflow(_th.JobInfo, _settings);
            Assert.AreEqual(_interactiveProfile, _workflow.Job.Profile, "Wrong Profile in Job of workflow.");
        }

        [Test]
        public void CreateConversionWorkflowTest_NoMapping_TwoProfiles_NoneIsDefault_NoneIsLastUsedProfile_ReturnWorkflowWithFirstProfile()
        {
            _settings.ApplicationSettings.LastUsedProfileGuid = "NoneOfTheProfileGuids";

            _settings.ConversionProfiles.Add(_autosaveProfile);
            _settings.ConversionProfiles.Add(_interactiveProfile);
            _workflow = WorkflowFactory.CreateWorkflow(_th.JobInfo, _settings);
            Assert.AreEqual(_autosaveProfile, _workflow.Job.Profile, "Wrong Profile in Job of workflow.");
        }

        [Test]
        public void CreateConversionWorkflowTest_PrinterMappingToProfile_ReturnWorkflowWithMappedProfile()
        {
            _settings.ApplicationSettings.PrinterMappings.Add(_autosaveProfileMapping);
            _settings.ApplicationSettings.PrinterMappings.Add(_interactiveProfileMapping);
            _settings.ConversionProfiles.Add(_autosaveProfile);
            _settings.ConversionProfiles.Add(_interactiveProfile);

            _th.JobInfo.SourceFiles[0].PrinterName = _autosaveProfileMapping.PrinterName;
            _workflow = WorkflowFactory.CreateWorkflow(_th.JobInfo, _settings);
            Assert.AreEqual(_autosaveProfile, _workflow.Job.Profile, "Wrong Profile in Job of workflow.");
        }

        [Test]
        public void CreateConversionWorkflowTest_PrinterMappingToInvalidProfile_ReturnWorkflowWithDefaultProfile()
        {
            _settings.ApplicationSettings.PrinterMappings.Add(_autosaveProfileMapping);
            _settings.ApplicationSettings.PrinterMappings.Add(_interactiveProfileMapping);
            const string somePrinterName = "somePrinterName";
            const string someProfileGuid = "someProfileNotInProfilesListGuid";
            var somePrinterMapping = new PrinterMapping(somePrinterName, someProfileGuid);
            _settings.ApplicationSettings.PrinterMappings.Add(somePrinterMapping);
            _settings.ConversionProfiles.Add(_autosaveProfile); //For this test the default profile must not be the first in list!
            _interactiveProfile.Guid = "DefaultGuid";
            _settings.ConversionProfiles.Add(_interactiveProfile);

            _th.JobInfo.SourceFiles[0].PrinterName = somePrinterName;

            _workflow = WorkflowFactory.CreateWorkflow(_th.JobInfo, _settings);
            Assert.AreEqual(_interactiveProfile, _workflow.Job.Profile, "Wrong Profile in Job of workflow.");
        }

        [Test]
        public void CreateConversionWorkflowTest_PrinterIsNotListedInMapping_ReturnWorkflowWithDefaultProfile()
        {
            _settings.ApplicationSettings.PrinterMappings.Add(_autosaveProfileMapping);
            _settings.ApplicationSettings.PrinterMappings.Add(_interactiveProfileMapping);
            _settings.ConversionProfiles.Add(_autosaveProfile); //For this test the default profile must not be the first in list!
            _interactiveProfile.Guid = "DefaultGuid";
            _settings.ConversionProfiles.Add(_interactiveProfile);

            _th.JobInfo.SourceFiles[0].PrinterName = "PrinterNameNotInMapping";

            _workflow = WorkflowFactory.CreateWorkflow(_th.JobInfo, _settings);
            Assert.AreEqual(_interactiveProfile, _workflow.Job.Profile, "Wrong Profile in Job of workflow.");
        }

        [Test]
        public void TitleTemplateTest()
        {
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Pdf);
            _th.JobInfo.Metadata.PrintJobName = "Title from Job";
            _th.JobInfo.Metadata.PrintJobAuthor = "Author from Job";

            _th.Job.Profile.AutoSave.Enabled = true;

            _settings = new PdfCreatorSettings(null);
            _settings.ConversionProfiles.Add(_th.Job.Profile);
            _settings.ConversionProfiles[0].AutoSave.EnsureUniqueFilenames = false;
            _settings.ConversionProfiles[0].AutoSave.TargetDirectory = _th.TmpTestFolder;
            _settings.ConversionProfiles[0].FileNameTemplate = "AutoSaveTestOutput";
            _settings.ConversionProfiles[0].OutputFormat = OutputFormat.Pdf;
            _settings.ConversionProfiles[0].TitleTemplate = "<PrintJobName> by <PrintJobAuthor>";

            var autoSaveWorkflow = new AutoSaveWorkflow(_th.Job, _settings);

            autoSaveWorkflow.RunWorkflow();

            var pdf = new PdfReader(_th.Job.OutputFiles[0]);
            Assert.AreEqual("Title from Job by Author from Job", pdf.Info["Title"], "Wrong title in PDF Metadata");
        }

        [Test]
        public void AuthorTemplateTest()
        {
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Pdf);
            _th.JobInfo.Metadata.PrintJobAuthor = "Author from Job";

            _th.Job.Profile.AutoSave.Enabled = true;

            _settings = new PdfCreatorSettings(null);
            _settings.ConversionProfiles.Add(_th.Job.Profile);
            _settings.ConversionProfiles[0].AutoSave.EnsureUniqueFilenames = false;
            _settings.ConversionProfiles[0].AutoSave.TargetDirectory = _th.TmpTestFolder;
            _settings.ConversionProfiles[0].FileNameTemplate = "AutoSaveTestOutput";
            _settings.ConversionProfiles[0].OutputFormat = OutputFormat.Pdf;
            _settings.ConversionProfiles[0].AuthorTemplate = "<PrintJobAuthor> + some text";

            var autoSaveWorkflow = new AutoSaveWorkflow(_th.Job, _settings);

            autoSaveWorkflow.RunWorkflow();

            var pdf = new PdfReader(_th.Job.OutputFiles[0]);
            Assert.AreEqual("Author from Job + some text", pdf.Info["Author"], "Wrong author in PDF Metadata");
        }
    }
}
