using System.Linq;
using System.Windows.Data;
using NUnit.Framework;
using pdfforge.DataStorage;
using pdfforge.DynamicTranslator;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ApplicationSettings;

namespace pdfforge.PDFCreator.UnitTest.UI.ViewModels.UserControlViewModels.ApplicationSettings
{
    [TestFixture]
    internal class TitleTabViewModelTest
    {
        private readonly ITranslator _translator = new BasicTranslator("none", Data.CreateDataStorage());

        [Test]
        public void TitleReplacementAdd_WithEmptyList_CanExecute()
        {
            var settings = new Conversion.Settings.ApplicationSettings();
            settings.TitleReplacement = new TitleReplacement[] {};

            var titleTabViewModel = new TitleTabViewModel(_translator);
            titleTabViewModel.ApplyTitleReplacements(settings.TitleReplacement);

            Assert.IsTrue(titleTabViewModel.TitleAddCommand.CanExecute(null));
        }

        [Test]
        public void TitleReplacementAdd_WithEmptyList_InsertsOneElement()
        {
            var settings = new Conversion.Settings.ApplicationSettings();
            settings.TitleReplacement = new TitleReplacement[] {}.ToList();

            var titleTabViewModel = new TitleTabViewModel(_translator);
            titleTabViewModel.ApplyTitleReplacements(settings.TitleReplacement);

            titleTabViewModel.TitleAddCommand.Execute(null);

            Assert.AreEqual(1, settings.TitleReplacement.Count);
        }

        [Test]
        public void TitleReplacementAdd_WithNonEmptyList_NewElementIsCurrent()
        {
            var settings = new Conversion.Settings.ApplicationSettings();
            settings.TitleReplacement = new[] {new TitleReplacement(ReplacementType.Replace, "a", "b"), new TitleReplacement(ReplacementType.Replace, "c", "d")}.ToList();

            var titleTabViewModel = new TitleTabViewModel(_translator);
            titleTabViewModel.ApplyTitleReplacements(settings.TitleReplacement);
            var cv = CollectionViewSource.GetDefaultView(titleTabViewModel.TitleReplacements);

            titleTabViewModel.TitleAddCommand.Execute(null);

            Assert.AreEqual(2, cv.CurrentPosition);
        }

        [Test]
        public void TitleReplacementDelete_AfterExecute_ElementIsRemoved()
        {
            var settings = new Conversion.Settings.ApplicationSettings();
            settings.TitleReplacement = new[] {new TitleReplacement(ReplacementType.Replace, "a", "b"), new TitleReplacement(ReplacementType.Replace, "c", "d")}.ToList();

            var titleTabViewModel = new TitleTabViewModel(_translator);
            titleTabViewModel.ApplyTitleReplacements(settings.TitleReplacement);

            var deletedElement = settings.TitleReplacement[0];

            titleTabViewModel.TitleDeleteCommand.Execute(null);

            Assert.IsFalse(titleTabViewModel.TitleReplacements.Contains(deletedElement));
        }

        [Test]
        public void TitleReplacementDelete_AfterExecute_OtherElementStillThere()
        {
            var settings = new Conversion.Settings.ApplicationSettings();
            settings.TitleReplacement = new[] {new TitleReplacement(ReplacementType.Replace, "a", "b"), new TitleReplacement(ReplacementType.Replace, "c", "d")}.ToList();

            var titleTabViewModel = new TitleTabViewModel(_translator);
            titleTabViewModel.ApplyTitleReplacements(settings.TitleReplacement);

            var otherElement = settings.TitleReplacement[1];

            titleTabViewModel.TitleDeleteCommand.Execute(null);

            Assert.IsTrue(titleTabViewModel.TitleReplacements.Contains(otherElement));
        }

        [Test]
        public void TitleReplacementDelete_CurrentItemChanged_RaisesCommandCanExecuteChanged()
        {
            var wasRaised = false;
            var settings = new Conversion.Settings.ApplicationSettings();
            settings.TitleReplacement = new[] {new TitleReplacement(ReplacementType.Replace, "a", "b"), new TitleReplacement(ReplacementType.Replace, "c", "d")}.ToList();

            var titleTabViewModel = new TitleTabViewModel(_translator);
            titleTabViewModel.ApplyTitleReplacements(settings.TitleReplacement);

            titleTabViewModel.TitleDeleteCommand.CanExecuteChanged += (sender, args) => wasRaised = true;

            var cv = CollectionViewSource.GetDefaultView(titleTabViewModel.TitleReplacements);
            cv.MoveCurrentToPrevious();

            Assert.IsTrue(wasRaised);
        }

        [Test]
        public void TitleReplacementDelete_WithEmptyList_CannotExecute()
        {
            var settings = new Conversion.Settings.ApplicationSettings();
            settings.TitleReplacement = new TitleReplacement[] {};

            var titleTabViewModel = new TitleTabViewModel(_translator);
            titleTabViewModel.ApplyTitleReplacements(settings.TitleReplacement);

            Assert.IsFalse(titleTabViewModel.TitleDeleteCommand.CanExecute(null));
        }

        [Test]
        public void TitleReplacementDelete_WithNonEmptyList_CanExecute()
        {
            var settings = new Conversion.Settings.ApplicationSettings();
            settings.TitleReplacement = new[] {new TitleReplacement(ReplacementType.Replace, "a", "b"), new TitleReplacement(ReplacementType.Replace, "c", "d")}.ToList();

            var titleTabViewModel = new TitleTabViewModel(_translator);
            titleTabViewModel.ApplyTitleReplacements(settings.TitleReplacement);

            Assert.IsTrue(titleTabViewModel.TitleDeleteCommand.CanExecute(null));
        }

        [Test]
        public void TitleReplacementDelete_WithNullList_CannotExecute()
        {
            var titleTabViewModel = new TitleTabViewModel(_translator);

            Assert.IsFalse(titleTabViewModel.TitleDeleteCommand.CanExecute(null));
        }
    }
}