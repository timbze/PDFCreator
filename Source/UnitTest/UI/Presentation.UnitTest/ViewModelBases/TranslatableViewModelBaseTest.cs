using NSubstitute;
using NUnit.Framework;
using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System.Collections.Generic;
using Translatable;

namespace Presentation.UnitTest.ViewModelBases
{
    #region Test implemantations of abstract viewmodel bases

    internal class TestClassForTranslatableViewModelBase : TranslatableViewModelBase<TestTranslation>
    {
        public TestClassForTranslatableViewModelBase(ITranslationUpdater translationUpdater) : base(translationUpdater)
        {
        }
    }

    internal class TestClassForOverlayViewModelBase : OverlayViewModelBase<TestInteraction, TestTranslation>
    {
        public TestClassForOverlayViewModelBase(ITranslationUpdater translationUpdater) : base(translationUpdater)
        {
        }

        public override string Title { get; }
    }

    public class TestTranslation : ITranslatable
    { }

    internal class TestInteraction : IInteraction
    { }

    #endregion Test implemantations of abstract viewmodel bases

    [TestFixture]
    internal class TranslatableViewModelBaseTest : TranslatableViewModelBaseTestBase
    {
        protected override ITranslatableViewModel<TestTranslation> BuildViewModel(ITranslationUpdater translationUpdater)
        {
            return new TestClassForOverlayViewModelBase(translationUpdater);
        }
    }

    [TestFixture]
    internal class OverlayViewModelBaseTest : TranslatableViewModelBaseTestBase
    {
        protected override ITranslatableViewModel<TestTranslation> BuildViewModel(ITranslationUpdater translationUpdater)
        {
            return new TestClassForTranslatableViewModelBase(translationUpdater);
        }
    }

    public abstract class TranslatableViewModelBaseTestBase
    {
        private ITranslatableViewModel<TestTranslation> _translatableViewModel;
        private ITranslationUpdater _translationUpdater;

        protected abstract ITranslatableViewModel<TestTranslation> BuildViewModel(ITranslationUpdater translationUpdater);

        [SetUp]
        public void Setup()
        {
            _translationUpdater = Substitute.For<ITranslationUpdater>();
            _translatableViewModel = BuildViewModel(_translationUpdater);
        }

        [Test]
        public void Initialize_TranslationUpdaterCallsRegisterAndSetTranslation()
        {
            _translationUpdater.Received().RegisterAndSetTranslation(_translatableViewModel);
        }

        [Test]
        public void SetTranslation_RaisePropertyChangedWasCalled()
        {
            var changedProperties = new List<string>();
            _translatableViewModel.PropertyChanged += (sender, args) => changedProperties.Add(args.PropertyName);

            _translatableViewModel.Translation = new TestTranslation();

            Assert.Contains(nameof(_translatableViewModel.Translation), changedProperties);
        }
    }
}
