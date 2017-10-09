using NUnit.Framework;
using System;

namespace pdfforge.PDFCreator.Utilities.UnitTest
{
    [TestFixture]
    public class RetryTest
    {
        [Test]
        public void Retry_WithSuccessfulAction_DoesNotRetry()
        {
            var tries = 0;

            Retry.Do(() =>
            {
                tries++;
            },
            retryInterval: TimeSpan.FromSeconds(0),
            retryCount: 3);

            Assert.AreEqual(1, tries);
        }

        [Test]
        public void Retry_ActionFailingTwice_DoesThreeAttempts()
        {
            var tries = 0;

            Retry.Do(() =>
            {
                tries++;
                if (tries < 3)
                    throw new Exception();
            },
            retryInterval: TimeSpan.FromSeconds(0),
            retryCount: 3);

            Assert.AreEqual(3, tries);
        }

        [Test]
        public void Retry_ActionFailing_DoesThreeAttemptsAndFails()
        {
            var ex = Assert.Throws<AggregateException>(() =>
                Retry.Do(() =>
                    {
                        throw new Exception();
                    },
                    retryInterval: TimeSpan.FromSeconds(0),
                    retryCount: 3)
            );

            Assert.AreEqual(3, ex.InnerExceptions.Count, "The action did not fail three times!");
        }

        [Test]
        public void Retry_WithExceptionConditionNotMet_DoesNotRetry()
        {
            var exeption = Assert.Throws<AggregateException>(() =>
                Retry.Do(() =>
                {
                    throw new NotImplementedException();
                },
                    retryInterval: TimeSpan.FromSeconds(0),
                    retryCount: 3,
            retryCondition: ex => ex is NotImplementedException)
            );

            Assert.AreEqual(3, exeption.InnerExceptions.Count);
        }

        [Test]
        public void Retry_WithExceptionConditionMet_DoesNotRetry()
        {
            var exeption = Assert.Throws<AggregateException>(() =>
                Retry.Do(() =>
                {
                    throw new Exception();
                },
                    retryInterval: TimeSpan.FromSeconds(0),
                    retryCount: 3,
            retryCondition: ex => ex is NotImplementedException)
            );

            Assert.AreEqual(1, exeption.InnerExceptions.Count);
        }
    }
}
