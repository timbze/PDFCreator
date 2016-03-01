using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;

namespace pdfforge.PDFCreator.Utilities
{
    public class DownloadSpeed
    {
        private readonly List<SpeedItem> _speedItems = new List<SpeedItem>();
        private DateTime _lastProgressEvent;
        private long _lastByteCount;
        private int _capacity = 10000;
        private long _totalBytes;
        private double _bytesPerSecond;

        private readonly object _lockObject = new object();

        public int Capacity
        {
            get { return _capacity; }
            set {
                _capacity = value;
                CleanUpSpeedItems();
            }
        }

        public double BytesPerSecond
        {
            get { return _bytesPerSecond; }
        }

        public TimeSpan EstimatedDuration
        {
            get
            {
                var seconds = _totalBytes / _bytesPerSecond;
                
                try
                {
                    return TimeSpan.FromSeconds(seconds);
                }
                catch (Exception)
                {
                    return TimeSpan.FromHours(1);
                }
            }
        }

        public TimeSpan EstimatedRemainingDuration
        {
            get
            {
                var seconds = (_totalBytes - _lastByteCount) / _bytesPerSecond;

                try
                {
                    return TimeSpan.FromSeconds(seconds);
                }
                catch (Exception)
                {
                    return TimeSpan.FromHours(1);
                }
            }
        }

        public DownloadSpeed(DateTime startTime)
        {
            _lastProgressEvent = startTime;
        }

        public DownloadSpeed(WebClient webClient)
            : this(DateTime.Now)
        {
            webClient.DownloadProgressChanged += DownloadProgressChanged;
            webClient.DownloadFileCompleted += webClient_DownloadFileCompleted;
        }

        private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            ReportProgress(DateTime.Now, e.TotalBytesToReceive, e.BytesReceived);
        }

        private void webClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            Reset();
        }

        private void CalculateSpeed()
        {
            var totalDuration = new TimeSpan(0);
            long totalBytes = 0;

            SpeedItem[] items;

            lock (_lockObject)
            {
                items = _speedItems.ToArray();
            }

            foreach (var speedItem in items)
            {
                totalDuration += speedItem.Duration;
                totalBytes += speedItem.Bytes;
            }

            _bytesPerSecond = totalBytes / totalDuration.TotalSeconds;

            if (double.IsNaN(_bytesPerSecond))
                _bytesPerSecond = 0;
        }

        /// <summary>
        /// Report a download progress
        /// </summary>
        /// <param name="time">The current time when this took place</param>
        /// <param name="totalBytes">The total number of bytes that are transferred</param>
        /// <param name="bytesReceived">The total number of bytes received so far</param>
        public void ReportProgress(DateTime time, long totalBytes, long bytesReceived)
        {
            _totalBytes = totalBytes;

            var bytes = bytesReceived - _lastByteCount;
            var duration = time - _lastProgressEvent;

            var speed = new SpeedItem(bytes, duration);
            AddItem(speed);

            _lastByteCount = bytesReceived;
            _lastProgressEvent = time;

            CalculateSpeed();
        }

        private void AddItem(SpeedItem speedItem)
        {
            lock (_lockObject)
            {
                _speedItems.Insert(0, speedItem);

                CleanUpSpeedItems();
            }
        }

        /// <summary>
        /// remove items that are over the capacity
        /// </summary>
        private void CleanUpSpeedItems()
        {
            lock (_lockObject)
            {
                if (_speedItems.Count > _capacity)
                {
                    int overCapactiy = _speedItems.Count - _capacity;
                    _speedItems.RemoveRange(_capacity, overCapactiy);
                }
            }
        }

        public void Reset()
        {
            _speedItems.Clear();
            CalculateSpeed();
        }
    }

    public class SpeedItem
    {
        public long Bytes { get; private set; }
        public TimeSpan Duration { get; private set; }

        public SpeedItem(long bytes, TimeSpan duration)
        {
            Bytes = bytes;
            Duration = duration;
        }
    }
}
