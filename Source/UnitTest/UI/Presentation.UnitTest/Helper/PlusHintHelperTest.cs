using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Core.Printing.Port;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using System;
using System.Linq;
using SystemInterface.Microsoft.Win32;

namespace Presentation.UnitTest.Helper
{
    [TestFixture]
    public class PlusHintHelperTest
    {
        [SetUp]
        public void Setup()
        {
            _registry = Substitute.For<IRegistry>();
            _registry.GetValue(RegistryKeyForHintSettings, RegistryKeyForCounter, 0).Returns(0);
            _registry.GetValue(RegistryKeyForHintSettings, RegistryKeyForDate, "").Returns("");

            _portReader = Substitute.For<IPrinterPortReader>();

            var installationPathProvider = Substitute.For<IInstallationPathProvider>();
            installationPathProvider.ApplicationRegistryPath.Returns(RegistryKeyForHintSettings.Replace(@"HKEY_CURRENT_USER\", ""));

            _plusHintHelper = new PlusHintHelper(_portReader, _registry, installationPathProvider);
        }

        private IRegistry _registry;
        private IPrinterPortReader _portReader;
        private PlusHintHelper _plusHintHelper;

        private const string RegistryKeyForHintSettings = @"HKEY_CURRENT_USER\Software\pdfforge\PDFCreator";
        private const string RegistryKeyForCounter = "LastPlusHintCounter";
        private const string RegistryKeyForDate = "LastPlusHintDate";

        [TestCase(11, 10, -1)]
        [TestCase(200, 10, -1)]
        [TestCase(11, 10, -30)]
        public void DisplayHint_IfCounterAndDateDifferencesAreNotEnough_ReturnsFalse(int currentCounter, int lastCounter, int days)
        {
            _portReader.ReadPrinterPort("pdfcmon").Returns(new PrinterPort { JobCounter = lastCounter });

            _registry.GetValue(RegistryKeyForHintSettings, RegistryKeyForCounter, 0).Returns(lastCounter);
            _registry.GetValue(RegistryKeyForHintSettings, RegistryKeyForDate, "").Returns(DateTime.Now.AddDays(days));

            Assert.IsFalse(_plusHintHelper.QueryDisplayHint());
        }

        [Test]
        public void DisplayHint_IfCounterAndDateAreNotSet_ReturnsFalseAndSetsValues()
        {
            var currentCount = 1;
            _portReader.ReadPrinterPort("pdfcmon").Returns(new PrinterPort { JobCounter = currentCount });
            _registry.GetValue(RegistryKeyForHintSettings, RegistryKeyForCounter, 0).Returns(0);
            _registry.GetValue(RegistryKeyForHintSettings, RegistryKeyForDate, "").Returns("");

            var result = _plusHintHelper.QueryDisplayHint();

            Assert.IsFalse(result);
            try
            {
                _registry.Received().SetValue(RegistryKeyForHintSettings, RegistryKeyForCounter, 0);
                _registry.Received()
                    .SetValue(RegistryKeyForHintSettings, RegistryKeyForDate,
                        Arg.Is<DateTime>(dateTime => dateTime > DateTime.Now.AddMinutes(-1)));
            }
            catch
            {
                Console.WriteLine("Received calls:");
                _registry.ReceivedCalls().ToList().ForEach(Console.WriteLine);
                throw;
            }
        }

        [Test]
        public void DisplayHint_IfCounterAndDateDifferencesAreEnough_ReturnsTrueAndWritesCurrentValuesToRegistry()
        {
            _portReader.ReadPrinterPort("pdfcmon").Returns(new PrinterPort { JobCounter = 300 });

            _registry.GetValue(RegistryKeyForHintSettings, RegistryKeyForCounter, 0).Returns(100);
            _registry.GetValue(RegistryKeyForHintSettings, RegistryKeyForDate, "").Returns(DateTime.Now.AddMonths(-1));

            Assert.IsTrue(_plusHintHelper.QueryDisplayHint());

            _registry.Received().SetValue(RegistryKeyForHintSettings, RegistryKeyForCounter, 300);
            _registry.Received().SetValue(RegistryKeyForHintSettings, RegistryKeyForDate, Arg.Is<DateTime>(dateTime => dateTime > DateTime.Now.AddMinutes(-1)));
        }

        [Test]
        public void PlusHintHelperDisabled_AlwaysReturnsFalse()
        {
            var disabledHelper = new PlusHintHelperDisabled();

            Assert.IsFalse(disabledHelper.QueryDisplayHint());
        }

        [Test]
        public void PlusHintHelperDisabled_AlwaysHasZeroCounter()
        {
            var disabledHelper = new PlusHintHelperDisabled();

            Assert.AreEqual(0, disabledHelper.CurrentJobCounter);
        }
    }
}
