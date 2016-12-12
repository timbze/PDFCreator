using SystemInterface;
using NUnit.Framework;
using pdfforge.PDFCreator.Utilities.Tokens;
using Rhino.Mocks;

namespace pdfforge.PDFCreator.Utilities.UnitTest.Tokens
{
    [TestFixture]
    public class SingleEnvironmentTokenTest
    {
        [SetUp]
        public void SetUp()
        {
            _environmentMock = MockRepository.GenerateStub<IEnvironment>();
        }

        private IEnvironment _environmentMock;

        private SingleEnvironmentToken CreateSingleEnvironmentToken(EnvironmentVariable variableName)
        {
            return new SingleEnvironmentToken(variableName, _environmentMock);
        }

        [Test]
        public void GetValue_ForMachineName_CallsEnvironmentWrapper()
        {
            const EnvironmentVariable variable = EnvironmentVariable.ComputerName;
            var environmentToken = CreateSingleEnvironmentToken(variable);

            environmentToken.GetValue();

            _environmentMock.AssertWasCalled(x => x.MachineName);
        }

        [Test]
        public void GetValue_ForUserName_CallsEnvironmentWrapper()
        {
            const EnvironmentVariable variable = EnvironmentVariable.Username;
            var environmentToken = CreateSingleEnvironmentToken(variable);

            environmentToken.GetValue();

            _environmentMock.AssertWasCalled(x => x.UserName);
        }

        [Test]
        public void GetValue_ReturnsDesiredTokenValue()
        {
            const EnvironmentVariable variable = EnvironmentVariable.Username;
            const string expectedUsername = "MyUser";
            var environmentToken = CreateSingleEnvironmentToken(variable);
            _environmentMock.Stub(x => x.UserName).Return(expectedUsername);

            var value = environmentToken.GetValue();

            Assert.AreEqual(expectedUsername, value);
        }

        [Test]
        public void GetValueWithFormat_ReturnsDesiredTokenValueWithoutFormatting()
        {
            const EnvironmentVariable variable = EnvironmentVariable.Username;
            const string expectedUsername = "MyUser";
            var environmentToken = CreateSingleEnvironmentToken(variable);
            _environmentMock.Stub(x => x.UserName).Return(expectedUsername);

            var value = environmentToken.GetValueWithFormat("xyz");

            Assert.AreEqual(expectedUsername, value);
        }

        [Test]
        public void Initialization_SetsTokenNameProperty()
        {
            const EnvironmentVariable variable = EnvironmentVariable.Username;

            var environmentToken = CreateSingleEnvironmentToken(variable);

            Assert.AreEqual(variable.ToString(), environmentToken.GetName());
        }

        [Test]
        public void Initialization_SetsVariableNameProperty()
        {
            const EnvironmentVariable variable = EnvironmentVariable.ComputerName;

            var environmentToken = CreateSingleEnvironmentToken(variable);

            Assert.AreEqual(variable, environmentToken.EnvironmentVariable);
        }
    }
}