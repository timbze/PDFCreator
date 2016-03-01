using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using NSubstitute;
using NUnit.Framework;
using pdfforge.LicenseValidator;
using pdfforge.PDFCreator.Shared.ViewModels;
using Rhino.Mocks;

namespace PDFCreator.Shared.Test.ViewModels
{
    [TestFixture]
    public class OfflineActivationViewModelTest
    {
        private ILicenseChecker _licenseChecker;
        private OfflineActivationViewModel _viewModel;
        private List<string> _propertiesChanged;

        [SetUp]
        public void BuildViewModel()
        {
            _licenseChecker = Substitute.For<ILicenseChecker>();
            _viewModel = new OfflineActivationViewModel(_licenseChecker);
            _propertiesChanged = new List<string>();
        }

        [Test]
        public void WhenSettingLicenseKey_OfflineActivationStringIsUpdated()
        {
            _viewModel.PropertyChanged += ViewModelOnPropertyChanged;
            _viewModel.LicenseKey = "123";

            Assert.Contains(nameof(OfflineActivationViewModel.LicenseKey), _propertiesChanged);
            Assert.Contains(nameof(OfflineActivationViewModel.OfflineActivationString), _propertiesChanged);
        }

        [Test]
        public void WhenQueryingOfflineActivationString_LicenseCheckerIsQueried()
        {
            _viewModel.PropertyChanged += ViewModelOnPropertyChanged;
            _viewModel.LicenseKey = "MY-LICENSE-KEY";

            var offlineActivation = _viewModel.OfflineActivationString;

            _licenseChecker.Received().GetOfflineActivationString(_viewModel.LicenseKey);
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            _propertiesChanged.Add(propertyChangedEventArgs.PropertyName);
        }
    }
}
