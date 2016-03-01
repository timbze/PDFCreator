using System;
using SystemInterface;
using SystemInterface.Reflection;
using NUnit.Framework;
using pdfforge.PDFCreator.Core.Jobs;
using pdfforge.PDFCreator.Utilities;
using Rhino.Mocks;

namespace PDFCreator.Core.UnitTest.Jobs
{
    [TestFixture]
    public class MetadataTest
    {
        [Test]
        public void MetadataWithTitle_OnCopy_ContainsSameTitle()
        {
            var metadata = new Metadata {Title = "MyTitle"};

            var clone = metadata.Copy();

            Assert.AreEqual(metadata.Title, clone.Title);
        }

        [Test]
        public void MetadataWithTitle_OnCopyAndEdit_OriginalNotModified()
        {
            var metadata = new Metadata {Title = "MyTitle"};

            var clone = metadata.Copy();
            clone.Title = "Title2";

            Assert.AreNotEqual(metadata.Title, clone.Title);
        }

        [Test]
        public void MetadataWithAuthor_OnCopy_ContainsSameAuthor()
        {
            var metadata = new Metadata {Author = "MyAuthor"};

            var clone = metadata.Copy();

            Assert.AreEqual(metadata.Author, clone.Author);
        }

        [Test]
        public void MetadataWithAuthor_OnCopyAndEdit_OriginalNotModified()
        {
            var metadata = new Metadata {Author = "MyAuthor"};

            var clone = metadata.Copy();
            clone.Author = "Author2";

            Assert.AreNotEqual(metadata.Author, clone.Author);
        }

        [Test]
        public void MetadataWithSubject_OnCopy_ContainsSameSubject()
        {
            var metadata = new Metadata {Subject = "MySubject"};

            var clone = metadata.Copy();

            Assert.AreEqual(metadata.Subject, clone.Subject);
        }

        [Test]
        public void MetadataWithSubject_OnCopyAndEdit_OriginalNotModified()
        {
            var metadata = new Metadata {Subject = "MySubject"};

            var clone = metadata.Copy();
            clone.Subject = "Subject2";

            Assert.AreNotEqual(metadata.Subject, clone.Subject);
        }

        [Test]
        public void MetadataWithKeywords_OnCopy_ContainsSameKeywords()
        {
            var metadata = new Metadata {Keywords = "MyKeywords"};

            var clone = metadata.Copy();

            Assert.AreEqual(metadata.Keywords, clone.Keywords);
        }

        [Test]
        public void MetadataWithKeywords_OnCopyAndEdit_OriginalNotModified()
        {
            var metadata = new Metadata {Keywords = "MyKeywords"};

            var clone = metadata.Copy();
            clone.Keywords = "Keywords2";

            Assert.AreNotEqual(metadata.Keywords, clone.Keywords);
        }

        [Test]
        public void MetadataWithPrintJobName_OnCopy_ContainsSamePrintJobName()
        {
            var metadata = new Metadata { PrintJobName = "NameOfThePrintJob" };

            var clone = metadata.Copy();

            Assert.AreEqual(metadata.PrintJobName, clone.PrintJobName);
        }

        [Test]
        public void MetadataWithPrintJobName_OnCopyAndEdit_OriginalNotModified()
        {
            var metadata = new Metadata { PrintJobName = "NameOfThePrintJob" };

            var clone = metadata.Copy();
            clone.PrintJobName = "NewNameOfThePrintJob";

            Assert.AreNotEqual(metadata.PrintJobName, clone.PrintJobName);
        }

        [Test]
        public void MetadataWithPrintJobAuthor_OnCopy_ContainsSamePrintJobAuthor()
        {
            var metadata = new Metadata { PrintJobAuthor = "SomeAuthor" };

            var clone = metadata.Copy();

            Assert.AreEqual(metadata.PrintJobAuthor, clone.PrintJobAuthor);
        }

        [Test]
        public void MetadataWithPrintJobAuthor_OnCopyAndEdit_OriginalNotModified()
        {
            var metadata = new Metadata { PrintJobAuthor = "SomeAuthor" };

            var clone = metadata.Copy();
            clone.PrintJobAuthor = "SomeOtherAuthor";

            Assert.AreNotEqual(metadata.PrintJobAuthor, clone.PrintJobAuthor);
        }

        [Test]
        public void ProducerProperty_ReturnsCorrectProducerString()
        {
            
            var assemblyHelper = MockRepository.GenerateStub<IAssemblyHelper>();
            assemblyHelper.Stub(v => v.GetCurrentAssemblyVersion()).Return(new Version("1.1.0"));

            var metadata = new Metadata(assemblyHelper);

            Assert.AreEqual("PDFCreator 1.1.0", metadata.Producer);
        }
    }
}
