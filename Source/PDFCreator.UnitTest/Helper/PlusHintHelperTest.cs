using System;
using System.Linq;
using SystemInterface.Microsoft.Win32;
using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Core.Printer;
using pdfforge.PDFCreator.Helper;
using pdfforge.PDFCreator.Shared.Licensing;

namespace PDFCreator.UnitTest.Helper
{
    [TestFixture]
    public class PlusHintHelperTest
    {
        private IRegistry _registry;
        private IPrinterPortReader _portReader;
        private PlusHintHelper _plusHintHelper;

        private const string RegistryKeyForHintSettings = @"HKEY_CURRENT_USER\Software\pdfforge\PDFCreator";
        private const string RegistryKeyForCounter = "LastPlusHintCounter";
        private const string RegistryKeyForDate = "LastPlusHintDate";

        [SetUp]
        public void Setup()
        {
            _registry = Substitute.For<IRegistry>();
            _portReader = Substitute.For<IPrinterPortReader>(); 
            var edition = new Edition();
            edition.ShowPlusHint = true;
            _plusHintHelper = new PlusHintHelper(_portReader, _registry, edition);
        }

        [Test]
        public void DisplayHint_DisabledShowPlusHintInEdition_ReturnsFalse()
        {
            var edition = new Edition();
            edition.ShowPlusHint = false;
            _plusHintHelper = new PlusHintHelper(_portReader, _registry, edition);
            var result = _plusHintHelper.DisplayHint();

            Assert.IsFalse(result);
        }

        [TestCase(11, 10, -1)]
        [TestCase(200, 10, -1)]
        [TestCase(11, 10, -30)]
        public void DisplayHint_IfCounterAndDateDifferencesAreNotEnough_ReturnsFalse(int currentCounter, int lastCounter, int days)
        {
            _portReader.ReadPrinterPort("pdfcmon").Returns(new PrinterPort {JobCounter = lastCounter});

            _registry.GetValue(RegistryKeyForHintSettings, RegistryKeyForCounter, 0).Returns(lastCounter);
            _registry.GetValue(RegistryKeyForHintSettings, RegistryKeyForDate, "").Returns(DateTime.Now.AddDays(days));

            Assert.IsFalse(_plusHintHelper.DisplayHint());
        }

        [Test]
        public void DisplayHint_IfCounterAndDateAreNotSet_ReturnsFalseAndSetsValues()
        {
            var currentCount = 1;
            _portReader.ReadPrinterPort("pdfcmon").Returns(new PrinterPort { JobCounter = currentCount });
            _registry.GetValue(RegistryKeyForHintSettings, RegistryKeyForCounter, 0).Returns(0);
            _registry.GetValue(RegistryKeyForHintSettings, RegistryKeyForDate, "").Returns("");

            var result = _plusHintHelper.DisplayHint();

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

            Assert.IsTrue(_plusHintHelper.DisplayHint());

            _registry.Received().SetValue(RegistryKeyForHintSettings, RegistryKeyForCounter, 300);
            _registry.Received().SetValue(RegistryKeyForHintSettings, RegistryKeyForDate, Arg.Is<DateTime>(dateTime => dateTime > DateTime.Now.AddMinutes(-1)));
        }
    }
}