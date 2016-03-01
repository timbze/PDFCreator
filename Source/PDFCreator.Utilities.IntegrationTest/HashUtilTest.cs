using NUnit.Framework;
using pdfforge.PDFCreator.Utilities;

namespace PDFCreator.Utilities.IntegrationTest
{
    [TestFixture]
    class HashUtilTest
    {
        [Test]
        public void Sha1Hash_WithEmptyString_ReturnsCorrectHash()
        {
            var hash = HashUtil.GetSha1Hash("");

            Assert.AreEqual("da39a3ee5e6b4b0d3255bfef95601890afd80709", hash);
        }

        [Test]
        public void Sha1Hash_WithTestString_ReturnsCorrectHash()
        {
            var hash = HashUtil.GetSha1Hash("test");

            Assert.AreEqual("a94a8fe5ccb19ba61c4c0873d391e987982fbbd3", hash);
        }

        [Test]
        public void Sha1Hash_WithLongString_ReturnsCorrectHash()
        {
            var hash = HashUtil.GetSha1Hash("The quick brown fox jumps over the lazy dog");

            Assert.AreEqual("2fd4e1c67a2d28fced849ee1bb76e7391b93eb12", hash);
        }
    }
}
