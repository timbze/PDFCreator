using System.Collections.Generic;
using NUnit.Framework;
using pdfforge.PDFCreator.Core.Settings;
using pdfforge.PDFCreator.ViewModels.Wrapper;

namespace PDFCreator.UnitTest.ViewModels.Wrapper
{
    [TestFixture]
    public class PrinterMappingWrapperTest
    {
        //private PdfCreatorSettings _settings;
        private IList<ConversionProfile> _profiles;
        private ConversionProfile _profile1;
        private ConversionProfile _profile2;

        [SetUp]
        public void SetUp()
        {
            //_settings = new PdfCreatorSettings(new IniStorage());
            _profiles = new List<ConversionProfile>();

            _profile1 = new ConversionProfile();
            _profile1.Name = "Profile1";
            _profile1.Guid = "Profile1Guid";
            
            _profile2 = new ConversionProfile();
            _profile2.Name = "Profile2";
            _profile2.Guid = "Profile2Guid";

            _profiles.Add(_profile1);
            _profiles.Add(_profile2);
        }

        [Test]
        public void PrinterMappingWrapper_WithMapping_ExposesSameObject()
        {
            var mapping = new PrinterMapping();

            var wrapper = new PrinterMappingWrapper(mapping, _profiles);

            Assert.AreSame(mapping, wrapper.PrinterMapping);
        }

        [Test]
        public void PrinterMappingWrapper_WithMapping_ExposesSamePrinterName()
        {
            var mapping = new PrinterMapping("SomePrinter", "SomeGuid");
            var wrapper = new PrinterMappingWrapper(mapping, _profiles);

            Assert.AreEqual(mapping.PrinterName, wrapper.PrinterName);
        }

        [Test]
        public void PrinterMappingWrapper_WithMapping_ExposesProfileWithSameGuid()
        {
            var mapping = new PrinterMapping("SomePrinter", "Profile1Guid");
            var wrapper = new PrinterMappingWrapper(mapping, _profiles);

            Assert.AreEqual(mapping.ProfileGuid, wrapper.Profile.Guid);
        }

        [Test]
        public void PrinterMappingWrapper_WithUnknownProfile_ProfileIsNull()
        {
            var mapping = new PrinterMapping("SomePrinter", "UnknwonProfile");
            var wrapper = new PrinterMappingWrapper(mapping, _profiles);

            Assert.IsNull(wrapper.Profile);
        }

        [Test]
        public void PrinterMappingWrapper_ChangingPrinterName_ChangesPrinterName()
        {
            var mapping = new PrinterMapping("SomePrinter", "SomeGuid");
            var wrapper = new PrinterMappingWrapper(mapping, _profiles);

            wrapper.PrinterName = "OtherPrinter";

            Assert.AreEqual("OtherPrinter", wrapper.PrinterName);
        }

        [Test]
        public void PrinterMappingWrapper_ChangingProfile_ChangesProfile()
        {
            var mapping = new PrinterMapping("SomePrinter", _profile1.Guid);
            var wrapper = new PrinterMappingWrapper(mapping, _profiles);

            wrapper.Profile = _profile2;

            Assert.AreEqual(_profile2, wrapper.Profile);
        }

        [Test]
        public void PrinterMappingWrapper_ChangingProfile_ChangesWrappedGuid()
        {
            var mapping = new PrinterMapping("SomePrinter", _profile1.Guid);
            var wrapper = new PrinterMappingWrapper(mapping, _profiles);

            wrapper.Profile = _profile2;

            Assert.AreEqual(_profile2.Guid, mapping.ProfileGuid);
        }

        [Test]
        public void PrinterMappingWrapper_ChangingPrinterName_ReplicatesToWrappedObject()
        {
            var mapping = new PrinterMapping("SomePrinter", "SomeGuid");
            var wrapper = new PrinterMappingWrapper(mapping, _profiles);

            wrapper.PrinterName = "OtherPrinter";

            Assert.AreEqual(mapping.PrinterName, wrapper.PrinterName);
        }

        [Test]
        public void PrinterMappingWrapper_ChangingPrinterName_RaisesPropertyChanged()
        {
            var mapping = new PrinterMapping("SomePrinter", "SomeGuid");
            var wrapper = new PrinterMappingWrapper(mapping, _profiles);

            bool wasRaised = false;

            wrapper.PropertyChanged += (sender, args) => { if (args.PropertyName == "PrinterName") wasRaised = true; };

            wrapper.PrinterName = "OtherPrinter";

            Assert.IsTrue(wasRaised);
        }

        [Test]
        public void PrinterMappingWrapper_ChangingPrinterName_RaisesPropertyChangedForIsPrimaryPrinter()
        {
            var mapping = new PrinterMapping("SomePrinter", "SomeGuid");
            var wrapper = new PrinterMappingWrapper(mapping, _profiles);

            bool wasRaised = false;

            wrapper.PropertyChanged += (sender, args) => { if (args.PropertyName == "IsPrimaryPrinter") wasRaised = true; };

            wrapper.PrinterName = "OtherPrinter";

            Assert.IsTrue(wasRaised);
        }

        [Test]
        public void PrinterMappingWrapper_ChangingPrimaryPrinter_RaisesPropertyChanged()
        {
            var mapping = new PrinterMapping("SomePrinter", "SomeGuid");
            var wrapper = new PrinterMappingWrapper(mapping, _profiles);

            bool wasRaised = false;

            wrapper.PropertyChanged += (sender, args) => { if (args.PropertyName == "PrimaryPrinter") wasRaised = true; };

            wrapper.PrimaryPrinter = "OtherPrinter";

            Assert.IsTrue(wasRaised);
        }

        [Test]
        public void PrinterMappingWrapper_ChangingPrimaryPrinter_RaisesPropertyChangedForIsPrimaryPrinter()
        {
            var mapping = new PrinterMapping("SomePrinter", "SomeGuid");
            var wrapper = new PrinterMappingWrapper(mapping, _profiles);

            bool wasRaised = false;

            wrapper.PropertyChanged += (sender, args) => { if (args.PropertyName == "IsPrimaryPrinter") wasRaised = true; };

            wrapper.PrimaryPrinter = "OtherPrinter";

            Assert.IsTrue(wasRaised);
        }

        [Test]
        public void PrinterMappingWrapper_ChangingProfile_RaisesPropertyChanged()
        {
            var mapping = new PrinterMapping("SomePrinter", "SomeGuid");
            var wrapper = new PrinterMappingWrapper(mapping, _profiles);

            bool wasRaised = false;

            wrapper.PropertyChanged += (sender, args) => { if (args.PropertyName == "Profile") wasRaised = true; };

            wrapper.Profile = _profile2;

            Assert.IsTrue(wasRaised);
        }

        [Test]
        public void IsPrimaryPrinter_WithUnknownPrinter_IsFalse()
        {
            var mapping = new PrinterMapping("SomePrinter", "SomeGuid");
            var wrapper = new PrinterMappingWrapper(mapping, _profiles);

            wrapper.PrimaryPrinter = "UnknownPrinter";

            Assert.IsFalse(wrapper.IsPrimaryPrinter);
        }

        [Test]
        public void IsPrimaryPrinter_WithMappedPrinter_IsTrue()
        {
            var mapping = new PrinterMapping("SomePrinter", "SomeGuid");
            var wrapper = new PrinterMappingWrapper(mapping, _profiles);

            wrapper.PrimaryPrinter = mapping.PrinterName;

            Assert.IsTrue(wrapper.IsPrimaryPrinter);
        }
    }
}
