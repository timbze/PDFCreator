using System.Collections.Generic;
using NUnit.Framework;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels;
using pdfforge.PDFCreator.UnitTest.UnitTestHelper;

namespace pdfforge.PDFCreator.UnitTest.UI.ViewModels.DialogViewModels
{
    [TestFixture]
    public class EditEmailTextViewModelTest
    {
        private EditEmailTextViewModel BuildEditEmailTextViewModel()
        {
            return new EditEmailTextViewModel(new TranslationProxy());
        }

        [Test]
        public void AddSignature_IsInitializedFromInteraction()
        {
            var viewModel = BuildEditEmailTextViewModel();
            var interactionHelper = new InteractionHelper<EditEmailTextInteraction>(viewModel, new EditEmailTextInteraction("", "", true));

            Assert.IsTrue(viewModel.AddSignature);
        }

        [Test]
        public void AddSignature_WhenSet_RaisesPropertyChanged()
        {
            var changedProperties = new List<string>();
            var viewModel = BuildEditEmailTextViewModel();
            var interactionHelper = new InteractionHelper<EditEmailTextInteraction>(viewModel, new EditEmailTextInteraction("", "", false));

            viewModel.PropertyChanged += (sender, args) => changedProperties.Add(args.PropertyName);

            viewModel.AddSignature = true;

            CollectionAssert.AreEquivalent(changedProperties, new[] {nameof(viewModel.AddSignature), nameof(viewModel.Footer)});
        }

        [Test]
        public void AddSignature_WhenSet_UpdatesInteraction()
        {
            var viewModel = BuildEditEmailTextViewModel();
            var interactionHelper = new InteractionHelper<EditEmailTextInteraction>(viewModel, new EditEmailTextInteraction("", "", false));

            viewModel.AddSignature = true;

            Assert.IsTrue(viewModel.Interaction.AddSignature);
        }

        [Test]
        public void BodyViewModel_WhenReadingText_ReturnsBody()
        {
            var viewModel = BuildEditEmailTextViewModel();
            var interactionHelper = new InteractionHelper<EditEmailTextInteraction>(viewModel, new EditEmailTextInteraction("", "", true));

            viewModel.Interaction.Content = "Test";

            Assert.AreEqual("Test", viewModel.BodyTextViewModel.Text);
        }

        [Test]
        public void BodyViewModel_WhenSettingText_UpdatesBody()
        {
            var viewModel = BuildEditEmailTextViewModel();
            var interactionHelper = new InteractionHelper<EditEmailTextInteraction>(viewModel, new EditEmailTextInteraction("", "", true));

            viewModel.BodyTextViewModel.Text = "Test";

            Assert.AreEqual("Test", viewModel.Interaction.Content);
        }

        [Test]
        public void Footer_WithSignatureDisabled_ReturnsEmptyString()
        {
            var viewModel = BuildEditEmailTextViewModel();
            var interactionHelper = new InteractionHelper<EditEmailTextInteraction>(viewModel, new EditEmailTextInteraction("", "", false));

            Assert.AreEqual("", viewModel.Footer);
        }

        [Test]
        public void Footer_WithSignatureEnabled_ReturnsFooterString()
        {
            var viewModel = BuildEditEmailTextViewModel();
            var interactionHelper = new InteractionHelper<EditEmailTextInteraction>(viewModel, new EditEmailTextInteraction("", "", true));

            Assert.IsTrue(!string.IsNullOrWhiteSpace(viewModel.Footer));
        }

        [Test]
        public void InitializingViewModel_CallsRaisePropertyChanged()
        {
            var changedProperties = new List<string>();
            var viewModel = BuildEditEmailTextViewModel();
            viewModel.PropertyChanged += (sender, args) => changedProperties.Add(args.PropertyName);
            var interaction = new EditEmailTextInteraction("", "", false);
            var interactionHelper = new InteractionHelper<EditEmailTextInteraction>(viewModel, interaction);

            CollectionAssert.Contains(changedProperties, nameof(viewModel.AddSignature));
            CollectionAssert.Contains(changedProperties, nameof(viewModel.Footer));
        }

        [Test]
        public void InitializingViewModel_InitializesBindingProperties()
        {
            var viewModel = BuildEditEmailTextViewModel();

            Assert.NotNull(viewModel.Translator);
            CollectionAssert.IsNotEmpty(viewModel.SubjectTextViewModel.TokenList);
            CollectionAssert.IsNotEmpty(viewModel.BodyTextViewModel.TokenList);
        }

        [Test]
        public void SetProperties_AfterCallingOk_InteractionIsSuccessful()
        {
            var viewModel = BuildEditEmailTextViewModel();
            var interactionHelper = new InteractionHelper<EditEmailTextInteraction>(viewModel, new EditEmailTextInteraction("", "", true));

            viewModel.OkCommand.Execute(null);

            Assert.IsTrue(interactionHelper.InteractionIsFinished);
            Assert.IsTrue(viewModel.Interaction.Success);
        }

        [Test]
        public void SubjectViewModel_WhenReadingText_ReturnsSubject()
        {
            var viewModel = BuildEditEmailTextViewModel();
            var interactionHelper = new InteractionHelper<EditEmailTextInteraction>(viewModel, new EditEmailTextInteraction("", "", true));

            viewModel.Interaction.Subject = "Test";

            Assert.AreEqual("Test", viewModel.SubjectTextViewModel.Text);
        }

        [Test]
        public void SubjectViewModel_WhenSettingText_UpdatesSubject()
        {
            var viewModel = BuildEditEmailTextViewModel();
            var interactionHelper = new InteractionHelper<EditEmailTextInteraction>(viewModel, new EditEmailTextInteraction("", "", true));

            viewModel.SubjectTextViewModel.Text = "Test";

            Assert.AreEqual("Test", viewModel.Interaction.Subject);
        }
    }
}