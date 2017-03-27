using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Processing.PdfToolsProcessing;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;

namespace PDFProcessing.PdfTools.UnitTest
{
    [TestFixture]
    public class BackgroundPageMapperTest
    {
        private BackgroundPage _backgroundSettings;

        [SetUp]
        public void Setup()
        {
            _backgroundSettings = new BackgroundPage
            {
                Repetition = BackgroundRepetition.NoRepetition,
                OnCover = false,
                OnAttachment = false
            };
        }

        [TestCase(-1)]
        [TestCase(5)]
        public void GetBackgroundPage_ForInvalidPage_ThrowsArgumentException(int invalidPageNumber)
        {
            int numPages = 1;
            var pageMapper = new BackgroundPageMapper(1, new BackgroundPage(), 0, 0);

            int bgPage;
            Assert.Throws<ArgumentException>(() => pageMapper.GetBackgroundPageNumber(invalidPageNumber, numPages, out bgPage));
        }

        [TestCase(3, new[] { 1, 2, 3})]
        [TestCase(4, new[] { 1, 2, 3, 4, -1 })]
        public void BackgroundPageMapper_WithoutRepetition_DoesNotAddBackgroundAfterFirstIteration(int numBackgroundPages, int[] expectedPages)
        {
            _backgroundSettings.Repetition = BackgroundRepetition.NoRepetition;

            var pageMapper = new BackgroundPageMapper(numBackgroundPages, _backgroundSettings, 0, 0);

            CollectionAssert.AreEqual(expectedPages, GetPageSequence(expectedPages.Length, pageMapper));
        }

        [TestCase(3, new[] { 1, 2, 3, 3, 3, 3, 3 })]
        public void BackgroundPageMapper_WithRepeatLast_AddsLastPageAfterFirstIteration(int numBackgroundPages, int[] expectedPages)
        {
            _backgroundSettings.Repetition = BackgroundRepetition.RepeatLastPage;

            var pageMapper = new BackgroundPageMapper(numBackgroundPages, _backgroundSettings, 0, 0);

            CollectionAssert.AreEqual(expectedPages, GetPageSequence(expectedPages.Length, pageMapper));
        }

        [TestCase(3, new[] { 1, 2, 3, 1, 2, 3, 1 })]
        public void BackgroundPageMapper_WithRepeatAll_RepeatsAllPages(int numBackgroundPages, int[] expectedPages)
        {
            _backgroundSettings.Repetition = BackgroundRepetition.RepeatAllPages;

            var pageMapper = new BackgroundPageMapper(numBackgroundPages, _backgroundSettings, 0, 0);

            CollectionAssert.AreEqual(expectedPages, GetPageSequence(expectedPages.Length, pageMapper));
        }

        [TestCase(3, new[] { 1, 2, 3, 1, 2, 3, 1 })]
        public void BackgroundPageMapper_WithRepeatAllAndCover_RepeatsAllPages(int numBackgroundPages, int[] expectedPages)
        {
            _backgroundSettings.Repetition = BackgroundRepetition.RepeatAllPages;
            _backgroundSettings.OnCover = true;

            var pageMapper = new BackgroundPageMapper(numBackgroundPages, _backgroundSettings, 3, 0);

            CollectionAssert.AreEqual(expectedPages, GetPageSequence(expectedPages.Length, pageMapper));
        }

        [TestCase(3, new[] { -1, -1, -1, 1, 2, 3, 1 })]
        public void BackgroundPageMapper_WithRepeatAllAndSkipCover_RepeatsAllPages(int numBackgroundPages, int[] expectedPages)
        {
            _backgroundSettings.Repetition = BackgroundRepetition.RepeatAllPages;
            _backgroundSettings.OnCover = false;

            var pageMapper = new BackgroundPageMapper(numBackgroundPages, _backgroundSettings, 3, 0);

            CollectionAssert.AreEqual(expectedPages, GetPageSequence(expectedPages.Length, pageMapper));
        }

        [TestCase(3, new[] { 1, 2, 3, 1, 2, 3, 1 })]
        public void BackgroundPageMapper_WithRepeatAllAndAttachment_RepeatsAllPages(int numBackgroundPages, int[] expectedPages)
        {
            _backgroundSettings.Repetition = BackgroundRepetition.RepeatAllPages;
            _backgroundSettings.OnAttachment = true;

            var pageMapper = new BackgroundPageMapper(numBackgroundPages, _backgroundSettings, 0, 3);

            CollectionAssert.AreEqual(expectedPages, GetPageSequence(expectedPages.Length, pageMapper));
        }

        [TestCase(3, new[] { 1, 2, 3, 1, -1, -1, -1 })]
        public void BackgroundPageMapper_WithRepeatAllAndSkipAttachment_RepeatsAllPagesWithoutAttachment(int numBackgroundPages, int[] expectedPages)
        {
            _backgroundSettings.Repetition = BackgroundRepetition.RepeatAllPages;
            _backgroundSettings.OnAttachment = false;

            var pageMapper = new BackgroundPageMapper(numBackgroundPages, _backgroundSettings, 0, 3);

            CollectionAssert.AreEqual(expectedPages, GetPageSequence(expectedPages.Length, pageMapper));
        }

        [TestCase(3, new[] { -1, -1, 1, 2, 3, -1, -1, -1 })]
        public void BackgroundPageMapper_WithRepeatAllAndSkipCoverAndAttachment_RepeatsAllPagesWithoutCoverAndAttachment(int numBackgroundPages, int[] expectedPages)
        {
            _backgroundSettings.Repetition = BackgroundRepetition.RepeatAllPages;
            _backgroundSettings.OnAttachment = false;
            _backgroundSettings.OnCover = false;

            var pageMapper = new BackgroundPageMapper(numBackgroundPages, _backgroundSettings, 2, 3);

            CollectionAssert.AreEqual(expectedPages, GetPageSequence(expectedPages.Length, pageMapper));
        }

        private IEnumerable<int> GetPageSequence(int numPages, BackgroundPageMapper backgroundPageMapper)
        {
            return Enumerable
                .Range(1, numPages)
                .Select(p => MapPage(p, numPages, backgroundPageMapper));
        }

        private int MapPage(int page, int numPages, BackgroundPageMapper mapper)
        {
            int bgPage;

            if (!mapper.GetBackgroundPageNumber(page, numPages, out bgPage))
                return -1;

            return bgPage;
        }
    }
}
