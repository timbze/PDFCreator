using System;
using System.Threading;

namespace PDFCreator.Utilities.IntegrationTest.Communication
{
    internal static class RandomHelper
    {
        private static int _seedCounter = new Random().Next();

        [ThreadStatic] private static Random _rng;

        public static Random Instance
        {
            get
            {
                if (_rng == null)
                {
                    var seed = Interlocked.Increment(ref _seedCounter);
                    _rng = new Random(seed);
                }
                return _rng;
            }
        }
    }
}