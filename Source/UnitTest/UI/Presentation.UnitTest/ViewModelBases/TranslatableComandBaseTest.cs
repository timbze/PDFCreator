using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.Utilities.Threading;
using System;
using Translatable;

namespace Presentation.UnitTest.ViewModelBases
{
    public class TranslatableCommandBaseTestTranslation : ITranslatable
    {
    }

    internal class TranslatableCommandBaseTestClass : TranslatableCommandBase<TranslatableCommandBaseTestTranslation>
    {
        public TranslatableCommandBaseTestClass(ITranslationUpdater translationUpdater) : base(translationUpdater)
        {
        }

        public override bool CanExecute(object parameter)
        {
            return false;
        }

        public override void Execute(object parameter)
        {
        }

        public void InvokeRaiseCanExecuteChangedForTesting()
        {
            RaiseCanExecuteChanged();
        }

        public TranslatableCommandBaseTestTranslation GetTranslation()
        {
            return Translation;
        }
    }

    [TestFixture]
    public class TranslatableComandBaseTest
    {
        private TranslatableCommandBaseTestClass _translatableCommandBase;
        private ITranslationFactory _translationFactory;

        [SetUp]
        public void SetUp()
        {
            var translationUpdater = new TranslationUpdater(new TranslationFactory(), new ThreadManager());
            _translatableCommandBase = new TranslatableCommandBaseTestClass(translationUpdater);
        }

        [Test]
        public void Initialize_UpdateOrCreateTranslationViaTranslationFactoryOfTranslationUpdater()
        {
            var translation = new TranslatableCommandBaseTestTranslation();
            _translationFactory = Substitute.For<ITranslationFactory>();
            _translationFactory.UpdateOrCreateTranslation(Arg.Any<TranslatableCommandBaseTestTranslation>()).Returns(translation);
            var translationUpdater = new TranslationUpdater(_translationFactory, new ThreadManager());

            _translatableCommandBase = new TranslatableCommandBaseTestClass(translationUpdater);

            Assert.AreSame(translation, _translatableCommandBase.GetTranslation());
        }

        [Test]
        public void RaiseCanExecuteChanged_InvokesCanExecuteChangedHandler()
        {
            Object sender = null;
            EventArgs args = null;
            _translatableCommandBase.CanExecuteChanged += (s, a) =>
            {
                sender = s;
                args = a;
            };

            _translatableCommandBase.InvokeRaiseCanExecuteChangedForTesting();

            Assert.AreEqual(EventArgs.Empty, args, "EventArgs not empty");
            Assert.AreSame(_translatableCommandBase, sender, "Sender not this");
        }
    }
}
