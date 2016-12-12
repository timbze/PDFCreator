using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels;
using pdfforge.PDFCreator.UnitTest.UnitTestHelper;
using pdfforge.PDFCreator.Utilities.Process;

namespace pdfforge.PDFCreator.UnitTest.UI.ViewModels.DialogViewModels
{
    [TestFixture]
    public class UpdateAvailableWindowViewModelTest
    {
        private IProcessStarter _processStarter;

        [SetUp]
        public void Setup()
        {
            _processStarter = Substitute.For<IProcessStarter>();
        }

        private UpdateAvailableViewModel BuildViewModel(UpdateAvailableInteraction interaction)
        {
            var viewModel = new UpdateAvailableViewModel(new SectionNameTranslator(), _processStarter, new ApplicationNameProvider("PDFCreator"));
            new InteractionHelper<UpdateAvailableInteraction>(viewModel, interaction);
            return viewModel;
        }

        [Test]
        public void AskLaterCommand_SetsResponseToLater()
        {
            var interaction = new UpdateAvailableInteraction("", "");
            var viewModel = BuildViewModel(interaction);
            
            viewModel.AskLaterCommand.Execute(null);

            Assert.AreEqual(UpdateAvailableResponse.Later, interaction.Response);
        }

        [Test]
        public void NothingIsExecuted_SetsResponseToLater()
        {
            var interaction = new UpdateAvailableInteraction("", "");
            var viewModel = BuildViewModel(interaction);

            Assert.AreEqual(UpdateAvailableResponse.Later, interaction.Response);
            Assert.AreSame(interaction, viewModel.Interaction);
        }

        [Test]
        public void SkipVersionCommand_SetsResponseToSkipVersion()
        {
            var interaction = new UpdateAvailableInteraction("", "");
            var viewModel = BuildViewModel(interaction);

            viewModel.SkipVersionCommand.Execute(null);

            Assert.AreEqual(UpdateAvailableResponse.Skip, interaction.Response);
        }

        [Test]
        public void InstallUpdateCommand_SetsResponseToSkipVersion()
        {
            var interaction = new UpdateAvailableInteraction("", "");
            var viewModel = BuildViewModel(interaction);

            viewModel.InstallUpdateCommand.Execute(null);

            Assert.AreEqual(UpdateAvailableResponse.Install, interaction.Response);
        }

        [Test]
        public void WhatsNewCommand_ShowsWhatsNew()
        {
            var updateUrl = "http://update.local";
            var interaction = new UpdateAvailableInteraction(updateUrl, "");
            var viewModel = BuildViewModel(interaction);

            viewModel.WhatsNewCommand.Execute(null);

            _processStarter.Received(1).Start(updateUrl);
        }

        [Test]
        public void TestTranslations()
        {
            var updateUrl = "http://update.local";
            var interaction = new UpdateAvailableInteraction(updateUrl, "1.0");
            var viewModel = BuildViewModel(interaction);

            Assert.AreEqual("UpdateManager\\NewUpdateMessage", viewModel.Text);
            Assert.AreEqual("UpdateManager\\ApplicationUpdate", viewModel.Title);
        }

        [Test]
        public void SetInteraction_RaisesPropertyChanged()
        {
            var updateUrl = "http://update.local";
            var interaction = new UpdateAvailableInteraction(updateUrl, "1.0");
            var viewModel = BuildViewModel(interaction);
            var changedProperties = new List<string>();
            viewModel.PropertyChanged += (sender, args) => changedProperties.Add(args.PropertyName);

            viewModel.SetInteraction(interaction);

            var expectedProperties = new[] {nameof(viewModel.Interaction), nameof(viewModel.Text), nameof(viewModel.Title) };

            CollectionAssert.AreEquivalent(expectedProperties, changedProperties);
        }
    }
}
