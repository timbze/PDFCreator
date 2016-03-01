
using System.ComponentModel;
using NUnit.Framework;
using pdfforge.PDFCreator.Core.Settings;
using pdfforge.PDFCreator.Core.Settings.Enums;
using pdfforge.PDFCreator.Shared.ViewModels.UserControls;
using PDFCreator.UnitTest.ViewModels.Helper;
using Rhino.Mocks;

namespace PDFCreator.Shared.Test.ViewModels.UserControls
{
    [TestFixture]
    public class PdfTabViewModelTest
    {
        [Test]
        public void SetEncryptionEnabled_RaisePropertyChanged()
        {
            var pdfTabViewModel = new PdfTabViewModel();
            pdfTabViewModel.CurrentProfile = new ConversionProfile();

            var eventStub = MockRepository.GenerateStub<IEventHandler<PropertyChangedEventArgs>>();

            pdfTabViewModel.PropertyChanged += eventStub.OnEventRaised;

            var lowEncryptionEnabledPropertyListener = new PropertyChangedListenerMock(pdfTabViewModel, "LowEncryptionEnabled");
            var mediumEncryptionEnabledPropertyListener = new PropertyChangedListenerMock(pdfTabViewModel, "MediumEncryptionEnabled");
            var highEncryptionEnabledPropertyListener = new PropertyChangedListenerMock(pdfTabViewModel, "HighEncryptionEnabled");
            var extendedPermissonsEnabledPropertyListener = new PropertyChangedListenerMock(pdfTabViewModel, "ExtendedPermissonsEnabled");
            var restrictLowQualityPrintingEnabledPropertyListener = new PropertyChangedListenerMock(pdfTabViewModel, "RestrictLowQualityPrintingEnabled");
            var allowFillFormsEnabledPropertyListener = new PropertyChangedListenerMock(pdfTabViewModel, "AllowFillFormsEnabled");
            var allowScreenReadersEnabledPropertyListener = new PropertyChangedListenerMock(pdfTabViewModel, "AllowScreenReadersEnabled");
            var allowEditingAssemblyEnabledPropertyListener = new PropertyChangedListenerMock(pdfTabViewModel, "AllowEditingAssemblyEnabled");
            var pdfVersionPropertyListener = new PropertyChangedListenerMock(pdfTabViewModel, "PdfVersion");

            pdfTabViewModel.EncryptionEnabled = true;

            Assert.IsTrue(lowEncryptionEnabledPropertyListener.WasCalled, "RaisePropertyChanged for LowEncryptionEnabled was not called");
            Assert.IsTrue(mediumEncryptionEnabledPropertyListener.WasCalled, "RaisePropertyChanged for MediumEncryptionEnabled was not called");
            Assert.IsTrue(highEncryptionEnabledPropertyListener.WasCalled, "RaisePropertyChanged for HighEncryptionEnabled was not called");
            Assert.IsTrue(extendedPermissonsEnabledPropertyListener.WasCalled, "RaisePropertyChanged for Settings was not called");
            Assert.IsTrue(restrictLowQualityPrintingEnabledPropertyListener.WasCalled, "RaisePropertyChanged for Settings was not called");
            Assert.IsTrue(allowFillFormsEnabledPropertyListener.WasCalled, "RaisePropertyChanged for Settings was not called");
            Assert.IsTrue(allowScreenReadersEnabledPropertyListener.WasCalled, "RaisePropertyChanged for Settings was not called");
            Assert.IsTrue(allowEditingAssemblyEnabledPropertyListener.WasCalled, "RaisePropertyChanged for Settings was not called");
            Assert.IsTrue(pdfVersionPropertyListener.WasCalled, "RaisePropertyChanged for PdfVersion was not called");
        }

        [Test]
        public void Rc40BitEncryptionEnabled_RaisePropertyChanged()
        {
            var pdfTabViewModel = new PdfTabViewModel();
            pdfTabViewModel.CurrentProfile = new ConversionProfile();

            var eventStub = MockRepository.GenerateStub<IEventHandler<PropertyChangedEventArgs>>();

            pdfTabViewModel.PropertyChanged += eventStub.OnEventRaised;

            var lowEncryptionEnabledPropertyListener = new PropertyChangedListenerMock(pdfTabViewModel, "LowEncryptionEnabled");
            var mediumEncryptionEnabledPropertyListener = new PropertyChangedListenerMock(pdfTabViewModel, "MediumEncryptionEnabled");
            var highEncryptionEnabledPropertyListener = new PropertyChangedListenerMock(pdfTabViewModel, "HighEncryptionEnabled");
            var extendedPermissonsEnabledPropertyListener = new PropertyChangedListenerMock(pdfTabViewModel, "ExtendedPermissonsEnabled");
            var restrictLowQualityPrintingEnabledPropertyListener = new PropertyChangedListenerMock(pdfTabViewModel, "RestrictLowQualityPrintingEnabled");
            var allowFillFormsEnabledPropertyListener = new PropertyChangedListenerMock(pdfTabViewModel, "AllowFillFormsEnabled");
            var allowScreenReadersEnabledPropertyListener = new PropertyChangedListenerMock(pdfTabViewModel, "AllowScreenReadersEnabled");
            var allowEditingAssemblyEnabledPropertyListener = new PropertyChangedListenerMock(pdfTabViewModel, "AllowEditingAssemblyEnabled");
            var pdfVersionPropertyListener = new PropertyChangedListenerMock(pdfTabViewModel, "PdfVersion");

            pdfTabViewModel.LowEncryptionEnabled = true;

            Assert.IsTrue(lowEncryptionEnabledPropertyListener.WasCalled, "RaisePropertyChanged for LowEncryptionEnabled was not called");
            Assert.IsTrue(mediumEncryptionEnabledPropertyListener.WasCalled, "RaisePropertyChanged for MediumEncryptionEnabled was not called");
            Assert.IsTrue(highEncryptionEnabledPropertyListener.WasCalled, "RaisePropertyChanged for HighEncryptionEnabled was not called");
            Assert.IsTrue(extendedPermissonsEnabledPropertyListener.WasCalled, "RaisePropertyChanged for Settings was not called");
            Assert.IsTrue(restrictLowQualityPrintingEnabledPropertyListener.WasCalled, "RaisePropertyChanged for Settings was not called");
            Assert.IsTrue(allowFillFormsEnabledPropertyListener.WasCalled, "RaisePropertyChanged for Settings was not called");
            Assert.IsTrue(allowScreenReadersEnabledPropertyListener.WasCalled, "RaisePropertyChanged for Settings was not called");
            Assert.IsTrue(allowEditingAssemblyEnabledPropertyListener.WasCalled, "RaisePropertyChanged for Settings was not called");
            Assert.IsTrue(pdfVersionPropertyListener.WasCalled, "RaisePropertyChanged for PdfVersion was not called");
        }

        [Test]
        public void Rc40BitEncryption_EnumToBooleanPropertiesTest()
        {
            var pdfTabViewModel = new PdfTabViewModel();
            pdfTabViewModel.CurrentProfile = new ConversionProfile();

            pdfTabViewModel.CurrentProfile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Rc40Bit;

            Assert.IsTrue(pdfTabViewModel.LowEncryptionEnabled,
                "LowEncryptionEnabled false for low encryption");
            Assert.IsFalse(pdfTabViewModel.MediumEncryptionEnabled,
                "MediumEncryptionEnabled true for low encryption");
            Assert.IsFalse(pdfTabViewModel.HighEncryptionEnabled,
                "HighEncryptionEnabled true for low encryption");
        }

        [Test]
        public void Rc40BitEncryption_BooleanPropertiesToEnumTest()
        {
            var pdfTabViewModel = new PdfTabViewModel();
            pdfTabViewModel.CurrentProfile = new ConversionProfile();

            pdfTabViewModel.LowEncryptionEnabled = true;

            Assert.AreEqual(EncryptionLevel.Rc40Bit, pdfTabViewModel.CurrentProfile.PdfSettings.Security.EncryptionLevel, "LowEncryptionEnabled but EncryptionLevel is not Low40Bit");
            Assert.IsFalse(pdfTabViewModel.MediumEncryptionEnabled, "MediumEncryptionEnabled true for low encryption");
            Assert.IsFalse(pdfTabViewModel.HighEncryptionEnabled, "HighEncryptionEnabled true for low encryption");
        }

        [Test]
        public void Rc40BitEncryption_ExtendedPermissionsDisabled() 
        {
            var pdfTabViewModel = new PdfTabViewModel();
            pdfTabViewModel.CurrentProfile = new ConversionProfile();

            pdfTabViewModel.CurrentProfile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Rc40Bit;

            Assert.IsFalse(pdfTabViewModel.ExtendedPermissonsEnabled,
                "Extended permissions enabled for low encryption");
        }

        [Test]
        public void Rc40BitEncryption_CheckExtendedPermissionValues()
        {
            var pdfTabViewModel = new PdfTabViewModel();
            pdfTabViewModel.CurrentProfile = new ConversionProfile();

            pdfTabViewModel.CurrentProfile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Rc40Bit;

            pdfTabViewModel.CurrentProfile.PdfSettings.Security.RestrictPrintingToLowQuality = true;
            pdfTabViewModel.CurrentProfile.PdfSettings.Security.AllowToFillForms = true;
            pdfTabViewModel.CurrentProfile.PdfSettings.Security.AllowScreenReader = true;
            pdfTabViewModel.CurrentProfile.PdfSettings.Security.AllowToEditAssembly = true;
            Assert.IsFalse(pdfTabViewModel.RestrictLowQualityPrintingEnabled,
                "RestrictLowQualityPrintingEnabled is true for low encryption");
            Assert.IsTrue(pdfTabViewModel.AllowFillFormsEnabled,
                "AllowFillFormsEnabled is false for low encryption");
            Assert.IsTrue(pdfTabViewModel.AllowScreenReadersEnabled,
                "AllowScreenReadersEnabled is false for low encryption");
            Assert.IsTrue(pdfTabViewModel.AllowEditingAssemblyEnabled,
                "AllowEditingAssemblyEnabled is false for low encryption");

            pdfTabViewModel.CurrentProfile.PdfSettings.Security.RestrictPrintingToLowQuality = false;
            pdfTabViewModel.CurrentProfile.PdfSettings.Security.AllowToFillForms = false;
            pdfTabViewModel.CurrentProfile.PdfSettings.Security.AllowScreenReader = false;
            pdfTabViewModel.CurrentProfile.PdfSettings.Security.AllowToEditAssembly = false;
            Assert.IsFalse(pdfTabViewModel.RestrictLowQualityPrintingEnabled,
                "RestrictLowQualityPrintingEnabled is true for low encryption");
            Assert.IsTrue(pdfTabViewModel.AllowFillFormsEnabled,
                "AllowFillFormsEnabled is false for low encryption");
            Assert.IsTrue(pdfTabViewModel.AllowScreenReadersEnabled,
                "AllowScreenReadersEnabled is false for low encryption");
            Assert.IsTrue(pdfTabViewModel.AllowEditingAssemblyEnabled,
                "AllowEditingAssemblyEnabled is false for low encryption");
        }

        [Test]
        public void Rc128BitEncryptionEnabled_RaisePropertyChanged()
        {
            var pdfTabViewModel = new PdfTabViewModel();
            pdfTabViewModel.CurrentProfile = new ConversionProfile();

            var eventStub = MockRepository.GenerateStub<IEventHandler<PropertyChangedEventArgs>>();

            pdfTabViewModel.PropertyChanged += eventStub.OnEventRaised;

            var lowEncryptionEnabledPropertyListener = new PropertyChangedListenerMock(pdfTabViewModel, "LowEncryptionEnabled");
            var mediumEncryptionEnabledPropertyListener = new PropertyChangedListenerMock(pdfTabViewModel, "MediumEncryptionEnabled");
            var highEncryptionEnabledPropertyListener = new PropertyChangedListenerMock(pdfTabViewModel, "HighEncryptionEnabled");
            var extendedPermissonsEnabledPropertyListener = new PropertyChangedListenerMock(pdfTabViewModel, "ExtendedPermissonsEnabled");
            var restrictLowQualityPrintingEnabledPropertyListener = new PropertyChangedListenerMock(pdfTabViewModel, "RestrictLowQualityPrintingEnabled");
            var allowFillFormsEnabledPropertyListener = new PropertyChangedListenerMock(pdfTabViewModel, "AllowFillFormsEnabled");
            var allowScreenReadersEnabledPropertyListener = new PropertyChangedListenerMock(pdfTabViewModel, "AllowScreenReadersEnabled");
            var allowEditingAssemblyEnabledPropertyListener = new PropertyChangedListenerMock(pdfTabViewModel, "AllowEditingAssemblyEnabled");
            var pdfVersionPropertyListener = new PropertyChangedListenerMock(pdfTabViewModel, "PdfVersion");

            pdfTabViewModel.MediumEncryptionEnabled = true;

            Assert.IsTrue(lowEncryptionEnabledPropertyListener.WasCalled, "RaisePropertyChanged for LowEncryptionEnabled was not called");
            Assert.IsTrue(mediumEncryptionEnabledPropertyListener.WasCalled, "RaisePropertyChanged for MediumEncryptionEnabled was not called");
            Assert.IsTrue(highEncryptionEnabledPropertyListener.WasCalled, "RaisePropertyChanged for HighEncryptionEnabled was not called");
            Assert.IsTrue(extendedPermissonsEnabledPropertyListener.WasCalled, "RaisePropertyChanged for Settings was not called");
            Assert.IsTrue(restrictLowQualityPrintingEnabledPropertyListener.WasCalled, "RaisePropertyChanged for Settings was not called");
            Assert.IsTrue(allowFillFormsEnabledPropertyListener.WasCalled, "RaisePropertyChanged for Settings was not called");
            Assert.IsTrue(allowScreenReadersEnabledPropertyListener.WasCalled, "RaisePropertyChanged for Settings was not called");
            Assert.IsTrue(allowEditingAssemblyEnabledPropertyListener.WasCalled, "RaisePropertyChanged for Settings was not called");
            Assert.IsTrue(pdfVersionPropertyListener.WasCalled, "RaisePropertyChanged for PdfVersion was not called");
        }

        [Test]
        public void Rc128BitEncryption_EnumToBooleanPropertiesTest()
        {
            var pdfTabViewModel = new PdfTabViewModel();
            pdfTabViewModel.CurrentProfile = new ConversionProfile();

            pdfTabViewModel.CurrentProfile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Rc128Bit;

            Assert.IsFalse(pdfTabViewModel.LowEncryptionEnabled,
                "LowEncryptionEnabled true for 128Bit encryption");
            Assert.IsTrue(pdfTabViewModel.MediumEncryptionEnabled,
                "MediumEncryptionEnabled false for 128Bit encryption");
            Assert.IsFalse(pdfTabViewModel.HighEncryptionEnabled,
                "HighEncryptionEnabled true for 128Bit encryption");
        }

        [Test]
        public void Rc128BitEncryption_BooleanPropertiesToEnumTest()
        {
            var pdfTabViewModel = new PdfTabViewModel();
            pdfTabViewModel.CurrentProfile = new ConversionProfile();

            pdfTabViewModel.MediumEncryptionEnabled = true;

            Assert.AreEqual(EncryptionLevel.Rc128Bit, pdfTabViewModel.CurrentProfile.PdfSettings.Security.EncryptionLevel, "MediumEncryptionEnabled but EncryptionLevel is not Medium128Bit");
            Assert.IsFalse(pdfTabViewModel.LowEncryptionEnabled, "LowEncryptionEnabled true for 128Bit encryption");
            Assert.IsFalse(pdfTabViewModel.HighEncryptionEnabled, "HighEncryptionEnabled true for 128Bit encryption");
        }

        [Test]
        public void Rc128BitEncryption_ExtendedPermissionsEnabled()
        {
            var pdfTabViewModel = new PdfTabViewModel();
            pdfTabViewModel.CurrentProfile = new ConversionProfile();

            pdfTabViewModel.CurrentProfile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Rc128Bit;

            Assert.IsTrue(pdfTabViewModel.ExtendedPermissonsEnabled,
                "Extended permissions disabled for 128Bit encryption");
        }

        [Test]
        public void Rc128BitEncryption_CheckExtendedPermissionValues()
        {
            var pdfTabViewModel = new PdfTabViewModel();
            pdfTabViewModel.CurrentProfile = new ConversionProfile();

            pdfTabViewModel.CurrentProfile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Rc128Bit;

            pdfTabViewModel.CurrentProfile.PdfSettings.Security.RestrictPrintingToLowQuality = true;
            pdfTabViewModel.CurrentProfile.PdfSettings.Security.AllowToFillForms = true;
            pdfTabViewModel.CurrentProfile.PdfSettings.Security.AllowScreenReader = true;
            pdfTabViewModel.CurrentProfile.PdfSettings.Security.AllowToEditAssembly = true;
            Assert.IsTrue(pdfTabViewModel.RestrictLowQualityPrintingEnabled,
                "Enabled RestrictLowQualityPrintingEnabled is false for 128Bit encryption");
            Assert.IsTrue(pdfTabViewModel.AllowFillFormsEnabled,
                "Enabled AllowFillFormsEnabled is false for 128Bit encryption");
            Assert.IsTrue(pdfTabViewModel.AllowScreenReadersEnabled,
                "Enabled AllowScreenReadersEnabled is false for 128Bit encryption");
            Assert.IsTrue(pdfTabViewModel.AllowEditingAssemblyEnabled,
                "Enabled AllowEditingAssemblyEnabled is false for 128Bit encryption");

            pdfTabViewModel.CurrentProfile.PdfSettings.Security.RestrictPrintingToLowQuality = false;
            pdfTabViewModel.CurrentProfile.PdfSettings.Security.AllowToFillForms = false;
            pdfTabViewModel.CurrentProfile.PdfSettings.Security.AllowScreenReader = false;
            pdfTabViewModel.CurrentProfile.PdfSettings.Security.AllowToEditAssembly = false;
            Assert.IsFalse(pdfTabViewModel.RestrictLowQualityPrintingEnabled,
                "Disabled RestrictLowQualityPrintingEnabled is true for 128Bit encryption");
            Assert.IsFalse(pdfTabViewModel.AllowFillFormsEnabled,
                "Disabled  AllowFillFormsEnabled is true for 128Bit encryption");
            Assert.IsFalse(pdfTabViewModel.AllowScreenReadersEnabled,
                "Disabled  AllowScreenReadersEnabled is true for 128Bit encryption");
            Assert.IsFalse(pdfTabViewModel.AllowEditingAssemblyEnabled,
                "Disabled AllowEditingAssemblyEnabled is true for 128Bit encryption");
        }

        [Test]
        public void Aes128BitEncryptionEnabled_RaisePropertyChanged()
        {
            var pdfTabViewModel = new PdfTabViewModel();
            pdfTabViewModel.CurrentProfile = new ConversionProfile();

            var eventStub = MockRepository.GenerateStub<IEventHandler<PropertyChangedEventArgs>>();

            pdfTabViewModel.PropertyChanged += eventStub.OnEventRaised;

            var lowEncryptionEnabledPropertyListener = new PropertyChangedListenerMock(pdfTabViewModel, "LowEncryptionEnabled");
            var mediumEncryptionEnabledPropertyListener = new PropertyChangedListenerMock(pdfTabViewModel, "MediumEncryptionEnabled");
            var highEncryptionEnabledPropertyListener = new PropertyChangedListenerMock(pdfTabViewModel, "HighEncryptionEnabled");
            var extendedPermissonsEnabledPropertyListener = new PropertyChangedListenerMock(pdfTabViewModel, "ExtendedPermissonsEnabled");
            var restrictLowQualityPrintingEnabledPropertyListener = new PropertyChangedListenerMock(pdfTabViewModel, "RestrictLowQualityPrintingEnabled");
            var allowFillFormsEnabledPropertyListener = new PropertyChangedListenerMock(pdfTabViewModel, "AllowFillFormsEnabled");
            var allowScreenReadersEnabledPropertyListener = new PropertyChangedListenerMock(pdfTabViewModel, "AllowScreenReadersEnabled");
            var allowEditingAssemblyEnabledPropertyListener = new PropertyChangedListenerMock(pdfTabViewModel, "AllowEditingAssemblyEnabled");
            var pdfVersionPropertyListener = new PropertyChangedListenerMock(pdfTabViewModel, "PdfVersion");

            pdfTabViewModel.HighEncryptionEnabled = true;

            Assert.IsTrue(lowEncryptionEnabledPropertyListener.WasCalled, "RaisePropertyChanged for LowEncryptionEnabled was not called");
            Assert.IsTrue(mediumEncryptionEnabledPropertyListener.WasCalled, "RaisePropertyChanged for MediumEncryptionEnabled was not called");
            Assert.IsTrue(highEncryptionEnabledPropertyListener.WasCalled, "RaisePropertyChanged for HighEncryptionEnabled was not called");
            Assert.IsTrue(extendedPermissonsEnabledPropertyListener.WasCalled, "RaisePropertyChanged for Settings was not called");
            Assert.IsTrue(restrictLowQualityPrintingEnabledPropertyListener.WasCalled, "RaisePropertyChanged for Settings was not called");
            Assert.IsTrue(allowFillFormsEnabledPropertyListener.WasCalled, "RaisePropertyChanged for Settings was not called");
            Assert.IsTrue(allowScreenReadersEnabledPropertyListener.WasCalled, "RaisePropertyChanged for Settings was not called");
            Assert.IsTrue(allowEditingAssemblyEnabledPropertyListener.WasCalled, "RaisePropertyChanged for Settings was not called");
            Assert.IsTrue(pdfVersionPropertyListener.WasCalled, "RaisePropertyChanged for PdfVersion was not called");
        }

        [Test]
        public void Aes128BitEncryption_EnumToBooleanPropertiesTest()
        {
            var pdfTabViewModel = new PdfTabViewModel();
            pdfTabViewModel.CurrentProfile = new ConversionProfile();

            pdfTabViewModel.CurrentProfile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Aes128Bit;

            Assert.IsFalse(pdfTabViewModel.LowEncryptionEnabled,
                "LowEncryptionEnabled true for 128BitAes encryption");
            Assert.IsFalse(pdfTabViewModel.MediumEncryptionEnabled,
                "MediumEncryptionEnabled true for 128BitAes encryption");
            Assert.IsTrue(pdfTabViewModel.HighEncryptionEnabled,
                "HighEncryptionEnabled false for 128BitAes encryption");
        }

        [Test]
        public void Aes128BitEncryption_BooleanPropertiesToEnumTest()
        {
            var pdfTabViewModel = new PdfTabViewModel();
            pdfTabViewModel.CurrentProfile = new ConversionProfile();

            pdfTabViewModel.HighEncryptionEnabled = true;

            Assert.AreEqual(EncryptionLevel.Aes128Bit, pdfTabViewModel.CurrentProfile.PdfSettings.Security.EncryptionLevel, "HighEncryptionEnabled but EncryptionLevel is not High128BitAes");
            Assert.IsFalse(pdfTabViewModel.LowEncryptionEnabled, "LowEncryptionEnabled true for 128BitAes encryption");
            Assert.IsFalse(pdfTabViewModel.MediumEncryptionEnabled, "MediumEncryptionEnabled true for 128BitAes encryption");
        }

        [Test]
        public void Aes128BitEncryption_ExtendedPermissionsEnabled()
        {
            var pdfTabViewModel = new PdfTabViewModel();
            pdfTabViewModel.CurrentProfile = new ConversionProfile();

            pdfTabViewModel.CurrentProfile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Aes128Bit;

            Assert.IsTrue(pdfTabViewModel.ExtendedPermissonsEnabled,
                "Extended permissions disabled for 128BitAes encryption");
        }

        [Test]
        public void Aes128BitEncryption_CheckExtendedPermissionValues()
        {
            var pdfTabViewModel = new PdfTabViewModel();
            pdfTabViewModel.CurrentProfile = new ConversionProfile();

            pdfTabViewModel.CurrentProfile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Rc128Bit;

            pdfTabViewModel.CurrentProfile.PdfSettings.Security.RestrictPrintingToLowQuality = true;
            pdfTabViewModel.CurrentProfile.PdfSettings.Security.AllowToFillForms = true;
            pdfTabViewModel.CurrentProfile.PdfSettings.Security.AllowScreenReader = true;
            pdfTabViewModel.CurrentProfile.PdfSettings.Security.AllowToEditAssembly = true;
            Assert.IsTrue(pdfTabViewModel.RestrictLowQualityPrintingEnabled,
                "Enabled RestrictLowQualityPrintingEnabled is false for 128BitAes encryption");
            Assert.IsTrue(pdfTabViewModel.AllowFillFormsEnabled,
                "Enabled AllowFillFormsEnabled is false for 128BitAes encryption");
            Assert.IsTrue(pdfTabViewModel.AllowScreenReadersEnabled,
                "Enabled AllowScreenReadersEnabled is false for 128BitAes encryption");
            Assert.IsTrue(pdfTabViewModel.AllowEditingAssemblyEnabled,
                "Enabled AllowEditingAssemblyEnabled is false for 128BitAes encryption");

            pdfTabViewModel.CurrentProfile.PdfSettings.Security.RestrictPrintingToLowQuality = false;
            pdfTabViewModel.CurrentProfile.PdfSettings.Security.AllowToFillForms = false;
            pdfTabViewModel.CurrentProfile.PdfSettings.Security.AllowScreenReader = false;
            pdfTabViewModel.CurrentProfile.PdfSettings.Security.AllowToEditAssembly = false;
            Assert.IsFalse(pdfTabViewModel.RestrictLowQualityPrintingEnabled,
                "Disabled RestrictLowQualityPrintingEnabled is true for 128BitAes encryption");
            Assert.IsFalse(pdfTabViewModel.AllowFillFormsEnabled,
                "Disabled  AllowFillFormsEnabled is true for 128BitAes encryption");
            Assert.IsFalse(pdfTabViewModel.AllowScreenReadersEnabled,
                "Disabled  AllowScreenReadersEnabled is true for 128BitAes encryption");
            Assert.IsFalse(pdfTabViewModel.AllowEditingAssemblyEnabled,
                "Disabled AllowEditingAssemblyEnabled is true for 128BitAes encryption");
        }
    }
}
