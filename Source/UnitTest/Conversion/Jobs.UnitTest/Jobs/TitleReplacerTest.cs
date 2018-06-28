using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using System;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.UnitTest.Conversion.Jobs.Jobs
{
    [TestFixture]
    internal class TitleReplacerTest
    {
        private void AddSearchAndReplace(TitleReplacer titleReplacer, string searchString, string replaceWith)
        {
            titleReplacer.AddReplacement(new TitleReplacement(ReplacementType.Replace, searchString, replaceWith));
        }

        private void AddRegex(TitleReplacer titleReplacer, string searchString, string replaceWith)
        {
            titleReplacer.AddReplacement(new TitleReplacement(ReplacementType.RegEx, searchString, replaceWith));
        }

        [Test]
        public void AfterAddingReplacementCollection_WhenReplacing_ReplacesTitleParts()
        {
            var replacements = new List<TitleReplacement>();
            replacements.Add(new TitleReplacement(ReplacementType.Replace, "One", "Two"));
            replacements.Add(new TitleReplacement(ReplacementType.Replace, "Alpha", "Beta"));
            var titleReplacer = new TitleReplacer();
            titleReplacer.AddReplacements(replacements);
            const string originalTitle = "Alpha - One";

            var title = titleReplacer.Replace(originalTitle);

            Assert.AreEqual(" - ", title);
        }

        [Test]
        public void AfterAddingReplacementCollection_WhenRemovingAtEnd_IgnoreCasing()
        {
            var replacements = new List<TitleReplacement>();
            replacements.Add(new TitleReplacement(ReplacementType.End, ".doc", ""));
            var titleReplacer = new TitleReplacer();
            titleReplacer.AddReplacements(replacements);

            const string originalTitle = "Filename.DOC";

            var title = titleReplacer.Replace(originalTitle);

            Assert.AreEqual("Filename", title);
        }

        [Test]
        public void AfterAddingReplacementCollection_WhenRemovingAtBeginning_IgnoreCasing()
        {
            var replacements = new List<TitleReplacement>();
            replacements.Add(new TitleReplacement(ReplacementType.Start, "Word - ", ""));
            var titleReplacer = new TitleReplacer();
            titleReplacer.AddReplacements(replacements);

            const string originalTitle = "WoRd - Filename.doc";

            var title = titleReplacer.Replace(originalTitle);

            Assert.AreEqual("Filename.doc", title);
        }

        [Test]
        public void Replace_WithNullTitle_ThrowsArgumentException()
        {
            var titleReplacer = new TitleReplacer();

            Assert.Throws<ArgumentException>(() => titleReplacer.Replace(null));
        }

        [Test]
        public void WithAddedRegexReplacement_WhenReplacing_ReplacesTitleParts()
        {
            var titleReplacer = new TitleReplacer();
            AddRegex(titleReplacer, "(.*) - .*(Microsoft).*", "$1 $2");
            const string originalTitle = "My Sample Title - Microsoft Word";

            var title = titleReplacer.Replace(originalTitle);

            Assert.AreEqual("My Sample Title Microsoft", title);
        }

        [Test]
        public void WithAddedRegexReplacement_WithEmptyReplaceString_DoesNotReplaceAnything()
        {
            var titleReplacer = new TitleReplacer();
            AddRegex(titleReplacer, ".*", "");
            const string originalTitle = "My Sample Title - Microsoft Word";

            var title = titleReplacer.Replace(originalTitle);

            Assert.AreEqual("", title);
        }

        [Test]
        public void WithAddedReplaceEndReplacement_WhenReplacing_ReplacesTitleParts()
        {
            var titleReplacer = new TitleReplacer();
            titleReplacer.AddReplacement(new TitleReplacement(ReplacementType.End, "Word", "xx"));
            const string originalTitle = "My Sample Word - Microsoft Word";

            var title = titleReplacer.Replace(originalTitle);

            Assert.AreEqual("My Sample Word - Microsoft ", title);
        }

        [Test]
        public void WithAddedReplacements_WhenReplacing_ReplacesTitleParts()
        {
            var titleReplacer = new TitleReplacer();
            AddSearchAndReplace(titleReplacer, " - Microsoft Word", "");
            const string originalTitle = "My Sample Title - Microsoft Word";

            var title = titleReplacer.Replace(originalTitle);

            Assert.AreEqual("My Sample Title", title);
        }

        [Test]
        public void WithAddedReplaceStartReplacement_WhenReplacing_ReplacesTitleParts()
        {
            var titleReplacer = new TitleReplacer();
            titleReplacer.AddReplacement(new TitleReplacement(ReplacementType.Start, "My", "xx"));
            const string originalTitle = "My Sample My Title - Microsoft Word";

            var title = titleReplacer.Replace(originalTitle);

            Assert.AreEqual(" Sample My Title - Microsoft Word", title);
        }

        [Test]
        public void WithAddedSearchAndReplaceReplacement_WhenReplacing_ReplacesTitleParts()
        {
            var titleReplacer = new TitleReplacer();
            titleReplacer.AddReplacement(new TitleReplacement(ReplacementType.Replace, "- Microsoft Word", "abc"));
            const string originalTitle = "My Sample Title - Microsoft Word";

            var title = titleReplacer.Replace(originalTitle);

            Assert.AreEqual("My Sample Title ", title);
        }

        [Test]
        public void WithEmptyConstructor_DoesNotReplaceAnything()
        {
            var titleReplacer = new TitleReplacer();
            const string originalTitle = "My Sample Title - Microsoft Word";

            var title = titleReplacer.Replace(originalTitle);

            Assert.AreEqual(originalTitle, title);
        }

        [Test]
        public void WithEmptySearchString_DoesNotReplaceAnything()
        {
            var titleReplacer = new TitleReplacer();
            const string originalTitle = "My Sample Title - Microsoft Word";

            AddSearchAndReplace(titleReplacer, "", "This string must be ignored");

            var title = titleReplacer.Replace(originalTitle);

            Assert.AreEqual(originalTitle, title);
        }

        [Test]
        public void WithMultipleReplacement_ReplacedInCorrectOrder()
        {
            var titleReplacer = new TitleReplacer();

            AddSearchAndReplace(titleReplacer, ".doc", "Must not be visible, because .docx should be already removed");
            AddSearchAndReplace(titleReplacer, ".docx", "Empty");

            const string originalTitle = "My Sample Title.docx";

            var title = titleReplacer.Replace(originalTitle);

            Assert.AreEqual("My Sample Title", title);
        }

        [TestCase(ReplacementType.Start)]
        [TestCase(ReplacementType.End)]
        [TestCase(ReplacementType.Replace)]
        [TestCase(ReplacementType.RegEx)]
        public void TitleReplacement_WithEmptySearch_IsNotValid(ReplacementType type)
        {
            var replacement = new TitleReplacement(type, "", "");
            Assert.IsFalse(replacement.IsValid());
        }

        [TestCase("", "")]
        [TestCase("(.*", "abc")]
        public void TitleReplacement_(string search, string replace)
        {
            var replacement = new TitleReplacement(ReplacementType.RegEx, search, replace);
            Assert.IsFalse(replacement.IsValid());
        }

        [TestCase("(.*)", "$1")]
        [TestCase("(.*)=(.*)", "$1 = $2")]
        public void TitleReplacement_WithValidRegEx_IsValid(string search, string replace)
        {
            var replacement = new TitleReplacement(ReplacementType.RegEx, search, replace);
            Assert.IsTrue(replacement.IsValid());
        }
    }
}
