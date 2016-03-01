using System;
using NUnit.Framework;
using pdfforge.PDFCreator.Utilities;

namespace PDFCreator.Utilities.IntegrationTest
{
    /// <summary>
    /// IntegrationTest to test the file association verb functions
    /// </summary>
    [TestFixture]
    class FileAssocTest
    {
        [TestCase]
        public static void TestDefaultAssoc()
        {
            Assert.IsTrue(FileUtil.Instance.FileAssocHasPrintTo(".txt"), "PrintTo association for .txt files not detected!");
            Assert.IsTrue(FileUtil.Instance.FileAssocHasPrint(".txt"), "Print association for .txt files not detected!");
            Assert.IsTrue(FileUtil.Instance.FileAssocHasPrintTo("txt"), "PrintTo association for .txt files not detected!");
            Assert.IsFalse(FileUtil.Instance.FileAssocHasPrintTo(".invalidfileextension"), "PrintTo association for .invalidfileextension files detected, but should not exist!");
            Assert.IsFalse(FileUtil.Instance.FileAssocHasPrint(".invalidfileextension"), "Print association for .invalidfileextension files detected, but should not exist!");
        }

        [TestCase]
        public static void TestExceptions()
        {
            Assert.Throws<ArgumentException>(IllegalDotAssocCall);
            Assert.Throws<ArgumentException>(IllegalShortAssocCall);
            Assert.Throws<ArgumentNullException>(NullAssocCall);
            Assert.Throws<ArgumentNullException>(EmptyAssocCall);
        }

        public static void IllegalDotAssocCall()
        {
            FileUtil.Instance.FileAssocHasPrint(".illegal.fileextension");
        }

        private static void IllegalShortAssocCall()
        {
            FileUtil.Instance.FileAssocHasPrint(".");
        }

        private static void NullAssocCall()
        {
            FileUtil.Instance.FileAssocHasPrint(null);
        }

        private static void EmptyAssocCall()
        {
            FileUtil.Instance.FileAssocHasPrint("");
        }
    }
}
