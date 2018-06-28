using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Core.Startup.StartConditions;
using pdfforge.PDFCreator.Core.Startup.Translations;
using pdfforge.PDFCreator.Core.StartupInterface;
using pdfforge.PDFCreator.UI.Presentation.Customization;
using pdfforge.PDFCreator.Utilities;
using System;
using Translatable;

namespace pdfforge.PDFCreator.UnitTest.Startup
{
    [TestFixture]
    public class TrialConditionTest
    {
        private TrialStartupCondition _trialStartupCondition;
        private readonly ProgramTranslation _translation = new ProgramTranslation();
        private IDateTimeProvider _dateTimeProvider;
        private ViewCustomization _viewCustomization;

        [SetUp]
        public void SetUp()
        {
            var translator = new TranslationFactory();
            _dateTimeProvider = Substitute.For<IDateTimeProvider>();
            _viewCustomization = ViewCustomization.DefaultCustomization;

            _trialStartupCondition = new TrialStartupCondition(translator, _dateTimeProvider, _viewCustomization);
        }

        [Test]
        public void CanRequestUserInteraction_IsFalse()
        {
            Assert.IsFalse(_trialStartupCondition.CanRequestUserInteraction);
        }

        [Test]
        public void DefaultViewCustomization_CheckIsSuccessful()
        {
            //Trial must be disbaled for ViewCustomization.DefaultCustomization
            _viewCustomization = ViewCustomization.DefaultCustomization;

            var result = _trialStartupCondition.Check();

            Assert.IsTrue(result.IsSuccessful);
        }

        [Test]
        public void ViewCustomizationWithEmptyExpirationDate_CheckIsSuccessful()
        {
            _viewCustomization.ApplyTrial("");

            var result = _trialStartupCondition.Check();

            Assert.IsTrue(result.IsSuccessful);
        }

        [Test]
        public void ViewCustomizationWithNotExpiredExpirationDate_CheckIsSuccessful()
        {
            _viewCustomization.ApplyTrial("IBj+Z+Lo24NKq82TWC8KlA==");
            _dateTimeProvider.Now().Returns(new DateTime(2052, 2, 23, 23, 59, 59, 999));

            var result = _trialStartupCondition.Check();

            Assert.IsTrue(result.IsSuccessful);
        }

        [Test]
        public void ViewCustomizationWithExpiredExpirationDate_CheckFails()
        {
            _viewCustomization.ApplyTrial("IBj+Z+Lo24NKq82TWC8KlA==");
            _dateTimeProvider.Now().Returns(new DateTime(2052, 2, 24, 0, 0, 0, 0));

            var result = _trialStartupCondition.Check();

            Assert.IsFalse(result.IsSuccessful, "IsSuccessful");
            Assert.AreEqual((int)ExitCode.TrialExpired, result.ExitCode, "ExitCode");
            Assert.IsTrue(result.ShowMessage, "ShowMessage");
            Assert.AreEqual(_translation.GetFormattedTrialExpiredTranslation("2052-02-23"), result.Message, "Message");
        }
    }
}
