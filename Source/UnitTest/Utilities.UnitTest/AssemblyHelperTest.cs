using NUnit.Framework;
using System.IO;

namespace pdfforge.PDFCreator.Utilities.UnitTest
{
    [TestFixture]
    public class AssemblyHelperTest
    {
        [Test]
        public void GetCurrentAssemblyDirectory_ReturnsDirectoryOfTestAssembly()
        {
            var expectedPath = Path.GetDirectoryName(GetType().Assembly.CodeBase.Substring(8));
            var assemblyHelper = new AssemblyHelper(GetType().Assembly);

            var assemblyDirectory = assemblyHelper.GetAssemblyDirectory();

            Assert.AreEqual(expectedPath, assemblyDirectory);
        }
    }
}
