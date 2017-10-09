using NUnit.Framework;
using PDFCreator.TestUtilities;
using pdfforge.PDFCreator.Utilities;
using System;
using System.IO;

namespace PDFCreator.Utilities.IntegrationTest
{
    [TestFixture]
    internal class HashUtilTest
    {
        [SetUp]
        public void SetUp()
        {
            _hashUtil = new HashUtil();
        }

        [TearDown]
        public void CleanUp()
        {
            TempFileHelper.CleanUp();
        }

        private IHashUtil _hashUtil;

        [Test]
        public void CalculateMd5_WithEmptyFile_CalculatesCorrectHash()
        {
            var tmpFile = TempFileHelper.CreateTempFile("MD5_Test", "file.txt");
            File.WriteAllText(tmpFile, "");

            Assert.AreEqual("d41d8cd98f00b204e9800998ecf8427e", _hashUtil.CalculateFileMd5(tmpFile));
        }

        [Test]
        public void CalculateMd5_WithNullFile_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _hashUtil.CalculateFileMd5(null));
        }

        [Test]
        public void CalculateMd5_WithTestString_CalculatesCorrectHash()
        {
            var tmpFile = TempFileHelper.CreateTempFile("MD5_Test", "file.txt");
            File.WriteAllText(tmpFile, "test");

            Assert.AreEqual("098f6bcd4621d373cade4e832627b4f6", _hashUtil.CalculateFileMd5(tmpFile));
        }

        [Test]
        public void Sha1Hash_WithEmptyString_ReturnsCorrectHash()
        {
            var hash = _hashUtil.GetSha1Hash("");

            Assert.AreEqual("da39a3ee5e6b4b0d3255bfef95601890afd80709", hash);
        }

        [Test]
        public void Sha1Hash_WithLongString_ReturnsCorrectHash()
        {
            var hash = _hashUtil.GetSha1Hash("The quick brown fox jumps over the lazy dog");

            Assert.AreEqual("2fd4e1c67a2d28fced849ee1bb76e7391b93eb12", hash);
        }

        [Test]
        public void Sha1Hash_WithTestString_ReturnsCorrectHash()
        {
            var hash = _hashUtil.GetSha1Hash("test");

            Assert.AreEqual("a94a8fe5ccb19ba61c4c0873d391e987982fbbd3", hash);
        }

        [Test]
        public void VerifyMd5_WithTestStringAndCorrectMd5_ReturnsTrue()
        {
            var tmpFile = TempFileHelper.CreateTempFile("MD5_Test", "file.txt");
            File.WriteAllText(tmpFile, "test");

            Assert.IsTrue(_hashUtil.VerifyFileMd5(tmpFile, "098f6bcd4621d373cade4e832627b4f6"));
        }

        [Test]
        public void VerifyMd5_WithTestStringAndIncorrectMd5_ReturnsFalse()
        {
            var tmpFile = TempFileHelper.CreateTempFile("MD5_Test", "file.txt");
            File.WriteAllText(tmpFile, "test");

            Assert.IsFalse(_hashUtil.VerifyFileMd5(tmpFile, "d41d8cd98f00b204e9800998ecf8427e"));
        }
    }
}
