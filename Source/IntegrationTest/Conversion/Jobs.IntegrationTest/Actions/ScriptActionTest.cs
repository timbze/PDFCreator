using NUnit.Framework;
using PDFCreator.TestUtilities;
using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Utilities.Process;
using pdfforge.PDFCreator.Utilities.Tokens;
using System.IO;
using SystemWrapper.IO;

namespace pdfforge.PDFCreator.IntegrationTest.Conversion.Jobs.Actions
{
    [TestFixture]
    internal class ScriptActionTest
    {
        private TestHelper _th;

        private string _scriptFile;

        [SetUp]
        public void SetUp()
        {
            var bootstrapper = new IntegrationTestBootstrapper();
            var container = bootstrapper.ConfigureContainer();
            _th = container.GetInstance<TestHelper>();
            _th.InitTempFolder("ScriptActionTest");

            _scriptFile = _th.GenerateTestFile(TestFile.ScriptCopyFilesToDirectoryCMD);

            /*
            Script:
            rem Script copies multiple files into the directory given as first parameter
            if %1 == """" goto end
            if %2 == """" goto end
            set targetDir=%1
            md %targetDir%
            :copy
            if %2 == """" goto end
            copy %2 %targetDir%
            shift
            goto copy
            :end
            */
        }

        [TearDown]
        public void CleanUp()
        {
            try
            {
                _th.CleanUp();
            }
            catch
            {
                // ignore errors while deleting temp files
            }
        }

        private ScriptAction BuildScriptAction()
        {
            return new ScriptAction(new PathWrap(), new ProcessStarter(), new FileWrap());
        }

        [Test]
        public void Check_WithDisabledScriptingAndIllegalPath_ReturnsTrue()
        {
            var scriptAction = BuildScriptAction();
            var profile = new ConversionProfile();
            profile.Scripting.Enabled = false;
            profile.Scripting.ScriptFile = ">test.exe";

            var result = scriptAction.Check(profile, new Accounts());

            Assert.IsTrue(result);
        }

        [Test]
        public void Check_WithTokenInPath_AllowsPath()
        {
            var scriptAction = BuildScriptAction();
            var profile = new ConversionProfile();
            profile.Scripting.Enabled = true;
            profile.Scripting.ScriptFile = "<Environment:TMP>\\unique_file_name-3453454353453453462.exe";

            var result = scriptAction.Check(profile, new Accounts());

            // ignore File not found error code
            result.RemoveAll(x => x == ErrorCode.Script_FileDoesNotExist);

            Assert.IsTrue(result);
        }

        [Test]
        public void ComposeScriptPath_WithoutTokens_ConcatenatesParamsAndFiles()
        {
            var tokenReplacer = new TokenReplacer();
            tokenReplacer.AddStringToken("foo", "bar");

            var scriptAction = BuildScriptAction();
            var result = scriptAction.ComposeScriptParameters("--myparam", new[] { @"C:\file1.pdf" }, tokenReplacer);

            Assert.AreEqual("--myparam \"C:\\file1.pdf\"", result);
        }

        [Test]
        public void ComposeScriptPath_WithTokenReplacer_ReplacesTokens()
        {
            var tokenReplacer = new TokenReplacer();
            tokenReplacer.AddStringToken("foo", "bar");

            var scriptAction = BuildScriptAction();
            var result = scriptAction.ComposeScriptPath(@"C:\Test\<foo>", tokenReplacer);

            Assert.AreEqual(@"C:\Test\bar", result);
        }

        [Test]
        public void ComposeScriptPath_WithTokensInParams_ReplacesTokens()
        {
            var tokenReplacer = new TokenReplacer();
            tokenReplacer.AddStringToken("foo", "bar");

            var scriptAction = BuildScriptAction();
            var result = scriptAction.ComposeScriptParameters("--myparam <foo>", new[] { @"C:\file1.pdf" }, tokenReplacer);

            Assert.AreEqual("--myparam bar \"C:\\file1.pdf\"", result);
        }

        [Test]
        public void ComposeScriptPath_WithUnkownToken_ReturnsValidFilename()
        {
            var tokenReplacer = new TokenReplacer();

            var scriptAction = BuildScriptAction();
            var result = scriptAction.ComposeScriptPath(@"C:\Test\<unknown>", tokenReplacer);

            Assert.AreEqual(@"C:\Test\_unknown_", result);
        }

        [Test]
        public void TestScripting()
        {
            _th.Profile.Scripting.Enabled = true;
            var scriptDir = Path.Combine(_th.TmpTestFolder, "TestScriptDirectory");
            _th.Profile.Scripting.ParameterString = "\"" + scriptDir + "\"";
            _th.Profile.Scripting.ScriptFile = _scriptFile;

            _th.GenerateGsJob(PSfiles.EmptyPage, OutputFormat.Png);
            _th.RunGsJob();

            Assert.IsTrue(Directory.Exists(scriptDir), "Script did not create the TargetDirectory \"" + scriptDir + "\"");

            foreach (var file in _th.Job.OutputFiles)
                Assert.IsTrue(File.Exists(Path.Combine(scriptDir, file)), "Script did not copy the file \"" + file + "\"\nto: " + scriptDir);
        }
    }
}
