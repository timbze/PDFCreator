using System;
using NUnit.Framework;
using pdfforge.PDFCreator.Utilities;

namespace PDFCreator.Utilities.UnitTest
{
    [TestFixture]
    public class DownloadSpeedTest
    {
        [Test]
        public void SpeedItem_WithValues_PropertiesAreFilledCorrectly()
        {
            var itm = new SpeedItem(5000, new TimeSpan(0, 0, 1));

            Assert.AreEqual(5000, itm.Bytes);
            Assert.AreEqual(new TimeSpan(0, 0, 1), itm.Duration);
        }

        [Test]
        public void DownloadSpeed_SettingsCapacity_SetsNewCapacity()
        {
            var startDate = DateTime.Now;
            var downloadSpeed = new DownloadSpeed(startDate);

            downloadSpeed.Capacity = 10;

            Assert.AreEqual(10, downloadSpeed.Capacity);
        }

        [Test]
        public void DownloadSpeed()
        {
            const int totalBytes = 10000;
            var startDate = DateTime.Now;
            var downloadSpeed = new DownloadSpeed(startDate);

            downloadSpeed.ReportProgress(startDate, totalBytes, 0);

            Assert.AreEqual(0, downloadSpeed.BytesPerSecond);
        }

        [Test]
        public void DownloadSpeed_WithSingleItem_CalculatesCorrectSpeed()
        {
            const int totalBytes = 10000;
            var startDate = DateTime.Now;
            var downloadSpeed = new DownloadSpeed(startDate);

            downloadSpeed.ReportProgress(startDate.AddMilliseconds(1000), totalBytes, 500);

            Assert.AreEqual(500, downloadSpeed.BytesPerSecond);
        }

        [Test]
        public void DownloadSpeed_WithTwoItems_CalculatesCorrectSpeed()
        {
            const int totalBytes = 10000;
            var startDate = DateTime.Now;
            var downloadSpeed = new DownloadSpeed(startDate);

            downloadSpeed.ReportProgress(startDate.AddMilliseconds(500), totalBytes, 500);
            downloadSpeed.ReportProgress(startDate.AddMilliseconds(1000), totalBytes, 1000);

            Assert.AreEqual(1000, downloadSpeed.BytesPerSecond);
        }

        [Test]
        public void DownloadSpeed_WithTwoItemsAndDifferentSpeeds_CalculatesCorrectSpeed()
        {
            const int totalBytes = 10000;
            var startDate = DateTime.Now;
            var downloadSpeed = new DownloadSpeed(startDate);

            downloadSpeed.ReportProgress(startDate.AddMilliseconds(400), totalBytes, 500);
            downloadSpeed.ReportProgress(startDate.AddMilliseconds(1000), totalBytes, 1000);

            Assert.AreEqual(1000, downloadSpeed.BytesPerSecond);
        }

        [Test]
        public void DownloadSpeed_WithCapacity_WithDoubleCapacityItems_CalculatesCorrectSpeedForSecondHalf()
        {
            const int totalBytes = 10000;
            var startDate = DateTime.Now;
            var downloadSpeed = new DownloadSpeed(startDate);
            downloadSpeed.Capacity = 2;

            downloadSpeed.ReportProgress(startDate.AddMilliseconds(500), totalBytes, 500);
            downloadSpeed.ReportProgress(startDate.AddMilliseconds(1000), totalBytes, 1000);

            downloadSpeed.ReportProgress(startDate.AddMilliseconds(1500), totalBytes, 2000);
            downloadSpeed.ReportProgress(startDate.AddMilliseconds(2000), totalBytes, 3000);

            Assert.AreEqual(2000, downloadSpeed.BytesPerSecond);
        }

        [Test]
        public void DownloadSpeedWithoutData_EstimatedDuration_ReturnsInfinity()
        {
            var startDate = DateTime.Now;
            var downloadSpeed = new DownloadSpeed(startDate);

            Assert.AreEqual(TimeSpan.FromHours(1), downloadSpeed.EstimatedDuration);
        }

        [Test]
        public void DownloadSpeedWithTwoItems_EstimatedDuration_CalculatesCorrectDuration()
        {
            const int totalBytes = 10000;
            var startDate = DateTime.Now;
            var downloadSpeed = new DownloadSpeed(startDate);

            downloadSpeed.ReportProgress(startDate.AddMilliseconds(500), totalBytes, 500);
            downloadSpeed.ReportProgress(startDate.AddMilliseconds(1000), totalBytes, 1000);

            Assert.AreEqual(TimeSpan.FromSeconds(10), downloadSpeed.EstimatedDuration);
        }

        [Test]
        public void DownloadSpeedWithTwoItems_AfterReset_ValuesAreResetted()
        {
            const int totalBytes = 10000;
            var startDate = DateTime.Now;
            var downloadSpeed = new DownloadSpeed(startDate);
            downloadSpeed.ReportProgress(startDate.AddMilliseconds(500), totalBytes, 500);
            downloadSpeed.ReportProgress(startDate.AddMilliseconds(1000), totalBytes, 1000);

            downloadSpeed.Reset();

            Assert.AreEqual(0, downloadSpeed.BytesPerSecond);
        }

        [Test]
        public void DownloadSpeedWithoutData_EstimatedRemainingDuration_ReturnsInfinity()
        {
            var startDate = DateTime.Now;
            var downloadSpeed = new DownloadSpeed(startDate);

            Assert.AreEqual(TimeSpan.FromHours(1), downloadSpeed.EstimatedRemainingDuration);
        }

        [Test]
        public void DownloadSpeedWithTwoItems_EstimatedRemainingDuration_CalculatesCorrectDuration()
        {
            const int totalBytes = 10000;
            var startDate = DateTime.Now;
            var downloadSpeed = new DownloadSpeed(startDate);

            downloadSpeed.ReportProgress(startDate.AddMilliseconds(500), totalBytes, 500);
            downloadSpeed.ReportProgress(startDate.AddMilliseconds(1000), totalBytes, 1000);

            Assert.AreEqual(TimeSpan.FromSeconds(9), downloadSpeed.EstimatedRemainingDuration);
        }
    }
}
