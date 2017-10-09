using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Utilities.Process;
using System;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.UnitTest.Conversion.Jobs.Actions
{
    [TestFixture]
    internal class ScriptActionTest
    {
        [SetUp]
        public void Setup()
        {
            _profile = new ConversionProfile();

            _file = Substitute.For<IFile>();
            _processStarter = Substitute.For<IProcessStarter>();
            _path = Substitute.For<IPath>();

            _scriptAction = new ScriptAction(_path, _processStarter, _file);
        }

        private ConversionProfile _profile;
        private IFile _file;
        private IPath _path;
        private IProcessStarter _processStarter;
        private ScriptAction _scriptAction;

        [Test]
        public void ScriptSettings_NoScriptFile()
        {
            _profile.Scripting.Enabled = true;
            _profile.Scripting.ScriptFile = "";

            var result = _scriptAction.Check(_profile, new Accounts());
            Assert.Contains(ErrorCode.Script_NoScriptFileSpecified, result, "did not detect missing script file.");
            result.Remove(ErrorCode.Script_NoScriptFileSpecified);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void ScriptSettings_NotExistingScriptFile()
        {
            _profile.Scripting.Enabled = true;
            _profile.Scripting.ScriptFile = "Doesnotexist.exe";
            var result = _scriptAction.Check(_profile, new Accounts());
            Assert.Contains(ErrorCode.Script_FileDoesNotExist, result, "did not detect, that the script file does not exist.");
            result.Remove(ErrorCode.Script_FileDoesNotExist);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void ScriptSettings_NotExistingSkriptFileInNetworkPath()
        {
            _profile.Scripting.Enabled = true;
            _profile.Scripting.ScriptFile = @"\\notexistingnetworkpath_3920ß392932013912\does_not_exist_3912839021830.exe";
            var result = _scriptAction.Check(_profile, new Accounts());
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void ScriptSettings_valid()
        {
            _profile.Scripting.Enabled = true;
            const string testFile = "ScriptfileDummy.exe";
            _file.Exists(testFile).Returns(true);
            _profile.Scripting.ScriptFile = testFile;
            var result = _scriptAction.Check(_profile, new Accounts());
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }
    }
}
