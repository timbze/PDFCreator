using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using pdfforge.PDFCreator.UI.COM;

namespace pdfforge.PDFCreator.IntegrationTest.UI.COM
{
    [TestFixture]
    class ComDependencyTest
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
