using NUnit.Framework;
using pdfforge.PDFCreator.UI.COM;

namespace pdfforge.PDFCreator.IntegrationTest.UI.COM
{
    [TestFixture]
    internal class ComDependencyTest
    {
        [Test]
        public void ComDependencies_CanBeCreatedWithoutException()
        {
            var builder = new ComDependencyBuilder();
            Assert.DoesNotThrow(() =>
            {
                var dependencies = builder.ComDependencies;
            });
        }
    }
}
