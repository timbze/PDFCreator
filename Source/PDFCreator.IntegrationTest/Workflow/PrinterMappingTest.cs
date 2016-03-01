using NUnit.Framework;
using pdfforge.DataStorage.Storage;
using pdfforge.PDFCreator.Core.Settings;
using pdfforge.PDFCreator.Core.Settings.Enums;
using pdfforge.PDFCreator.Workflow;
using PDFCreator.TestUtilities;

namespace PDFCreator.IntegrationTest.Workflow
{
    [TestFixture]
    class PrinterMappingTest
    {
        private PdfCreatorSettings _settings;
        TestHelper _testHelper;
        
        [SetUp]
        public void SetupSettings()
        {
            var ini = new IniStorage();
            _testHelper = new TestHelper("MappingTest");

            _settings = new PdfCreatorSettings(ini);
            _settings.ConversionProfiles.Add(new ConversionProfile { Guid = "Profile0", Name = "Profile0" });
            _settings.ConversionProfiles.Add(new ConversionProfile { Guid = "Profile1", Name = "Profile1" });

            _testHelper.GenerateGsJob(PSfiles.EmptyPage, OutputFormat.Pdf);
        }

        [TearDown]
        public void Cleanup()
        {
            if (_testHelper != null)
                _testHelper.CleanUp();
        }

        [Test]
        public void PrinterMapping_EmptyMapping_CreatesInteractiveWorkflow()
        {
            _settings.ConversionProfiles[0].AutoSave.Enabled = false;
            var workflow = WorkflowFactory.CreateWorkflow(_testHelper.JobInfo, _settings);
            Assert.True(workflow is InteractiveWorkflow, "InteractiveWorkflow was expected");
        }

        [Test]
        public void PrinterMapping_EmptyMappingWithAutoSave_CreatesAutoSaveWorkflow()
        {
            _settings.ConversionProfiles[0].AutoSave.Enabled = true;
            var workflow = WorkflowFactory.CreateWorkflow(_testHelper.JobInfo, _settings);
            Assert.True(workflow is AutoSaveWorkflow, "AutoSaveWorkflow was expected");
        }

        [Test]
        public void PrinterMapping_WithProfileMapping_PreselectsProfile()
        {
            _settings.ApplicationSettings.PrinterMappings.Add(new PrinterMapping("PDFCreator", "Profile1"));

            ConversionWorkflow workflow = WorkflowFactory.CreateWorkflow(_testHelper.JobInfo, _settings);
            Assert.AreEqual(_settings.ConversionProfiles[1].Name, workflow.Job.Profile.Name, "Profile names should match");
            Assert.AreEqual(_settings.ConversionProfiles[1], workflow.Job.Profile, "Profile1 should have been mapped");
        }

        [Test]
        public void PrinterMapping_WithProfileMappingWithOtherCasing_PreselectsProfile()
        {
            _settings.ApplicationSettings.PrinterMappings.Add(new PrinterMapping("pdfcreator", "Profile1"));

            ConversionWorkflow workflow = WorkflowFactory.CreateWorkflow(_testHelper.JobInfo, _settings);
            Assert.AreEqual(_settings.ConversionProfiles[1], workflow.Job.Profile, "Profile1 should have been mapped");
        }

        [Test]
        public void PrinterMapping_WithInvalidProfile_SelectsDefaultProfile()
        {
            _settings.ApplicationSettings.PrinterMappings.Add(new PrinterMapping("PDFCreator", "NonExistentProfile"));

            ConversionWorkflow workflow = WorkflowFactory.CreateWorkflow(_testHelper.JobInfo, _settings);
            Assert.AreEqual(_settings.ConversionProfiles[0], workflow.Job.Profile, "Default profile should have been mapped");
        }

        [Test]
        public void PrinterMapping_WithInvalidPrinter_SelectsDefaultProfile()
        {
            _settings.ApplicationSettings.PrinterMappings.Add(new PrinterMapping("InvalidPrinter", "Profile1"));

            ConversionWorkflow workflow = WorkflowFactory.CreateWorkflow(_testHelper.JobInfo, _settings);
            Assert.AreEqual(_settings.ConversionProfiles[0], workflow.Job.Profile, "Default profile should have been mapped");
        }
    }
}
