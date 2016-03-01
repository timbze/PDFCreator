using System.IO;
using NUnit.Framework;
using pdfforge.PDFCreator.Utilities;

namespace PDFCreator.Utilities.UnitTest
{
    [TestFixture]
    public class AssemblyHelperTest
    {
        
        [Test]
        public void GetCurrentAssemblyDirectory_ReturnsDirectoryOfTestAssembly()
        {
            string expectedPath = Path.GetDirectoryName(this.GetType().Assembly.CodeBase.Substring(8));
            var assemblyHelper = new AssemblyHelper();

            var assemblyDirectory = assemblyHelper.GetCurrentAssemblyDirectory();

            Assert.AreEqual(expectedPath, assemblyDirectory);
        }

        [Test]
        public void GetCurrentAssemblyVersion_ReturnsVersionOfTestAssembly()
        {
            var assemblyHelper = new AssemblyHelper();

            var assemblyVersion = assemblyHelper.GetCurrentAssemblyVersion();

            Assert.AreEqual(this.GetType().Assembly.GetName().Version, assemblyVersion);
        }
    }
}
