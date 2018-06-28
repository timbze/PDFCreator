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
        private ConversionProfile _profile;
        private IFile _file;
        private IPath _path;
        private IProcessStarter _processStarter;
        private ScriptAction _scriptAction;
        private readonly Accounts _unusedAccounts = null;

        [SetUp]
        public void Setup()
        {
            _profile = new ConversionProfile();

            _file = Substitute.For<IFile>();
            _file.Exists(Arg.Any<string>()).Returns(true);
            _processStarter = Substitute.For<IProcessStarter>();
            _path = Substitute.For<IPath>();
            _scriptAction = new ScriptAction(_path, _processStarter, _file);
        }

        [Test]
        public void Check_ValidScriptSettings()
        {
            _profile.Scripting.Enabled = true;
            _profile.Scripting.ScriptFile = "ScriptfileDummy.exe";

            var result = _scriptAction.Check(_profile, _unusedAccounts);

            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Check_ScriptActionDisabled_ResultIsTrue()
        {
            _profile.Scripting.Enabled = false;
            _profile.Scripting.ScriptFile = string.Empty;

            var result = _scriptAction.Check(_profile, _unusedAccounts);

            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Check_NoScriptFile()
        {
            _profile.Scripting.Enabled = true;
            _profile.Scripting.ScriptFile = "";

            var result = _scriptAction.Check(_profile, _unusedAccounts);

            Assert.Contains(ErrorCode.Script_NoScriptFileSpecified, result, "did not detect missing script file.");
            result.Remove(ErrorCode.Script_NoScriptFileSpecified);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Check_ScripFileContainsToken_ResultIsTrue()
        {
            _profile.Scripting.Enabled = true;
            _profile.Scripting.ScriptFile = "Folder\\<Token>\\Script.exe";

            var result = _scriptAction.Check(_profile, _unusedAccounts);

            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Check_InvalidFileName()
        {
            _profile.Scripting.Enabled = true;
            _profile.Scripting.ScriptFile = "Illegal Char < ";

            var result = _scriptAction.Check(_profile, _unusedAccounts);

            Assert.Contains(ErrorCode.Script_IllegalCharacters, result, "did not detect illegel char in script file.");
            result.Remove(ErrorCode.Script_IllegalCharacters);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Check_NotExistingSkriptFileInNetworkPath_ResultIsTrue()
        {
            _profile.Scripting.Enabled = true;
            _profile.Scripting.ScriptFile = "\\\\NetworkPath\\Doesnotexist.exe";
            _file.Exists(_profile.Scripting.ScriptFile).Returns(false);

            var result = _scriptAction.Check(_profile, _unusedAccounts);

            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Check_NotExistingScriptFile()
        {
            _profile.Scripting.Enabled = true;
            _profile.Scripting.ScriptFile = "Doesnotexist.exe";
            _file.Exists(_profile.Scripting.ScriptFile).Returns(false);

            var result = _scriptAction.Check(_profile, _unusedAccounts);

            Assert.Contains(ErrorCode.Script_FileDoesNotExist, result, "did not detect, that the script file does not exist.");
            result.Remove(ErrorCode.Script_FileDoesNotExist);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }
    }
}
