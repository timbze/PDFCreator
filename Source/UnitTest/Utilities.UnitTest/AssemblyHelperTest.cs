using System;
using System.IO;
using NUnit.Framework;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.Utilities.UnitTest
{
    [TestFixture]
    public class AssemblyHelperTest
    {
        [Test]
        public void GetCurrentAssemblyDirectory_ReturnsDirectoryOfTestAssembly()
        {
            var expectedPath = Path.GetDirectoryName(GetType().Assembly.CodeBase.Substring(8));
            var assemblyHelper = new AssemblyHelper();

            var assemblyDirectory = assemblyHelper.GetPdfforgeAssemblyDirectory();

            Assert.AreEqual(expectedPath, assemblyDirectory);
        }

        [Test]
        public void GetCurrentAssemblyVersion_ReturnsVersionOfTestAssembly()
        {
            var assemblyHelper = new AssemblyHelper();

            var assemblyVersion = assemblyHelper.GetPdfforgeAssemblyVersion();

            Assert.AreEqual(GetType().Assembly.GetName().Version, assemblyVersion);
        }
    }
}

// ReSharper disable once CheckNamespace
namespace SomeNamespaceThatDoesntStartWithPdfforge
{
    [TestFixture]
    public class AssemblyHelperTestWithOtherNamespace
    {
        [Test]
        public void GetAssemblyVersion_OutsideOfPdfforgeNamespace_ThrowsInvalidOperationException()
        {
            var assemblyHelper = new AssemblyHelper();
            Assert.Throws<InvalidOperationException>(() => assemblyHelper.GetPdfforgeAssemblyVersion());
        }
    }
}
