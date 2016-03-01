using System;
using NUnit.Framework;
using pdfforge.PDFCreator.COM;

namespace PDFCreator.IntegrationTest.COM
{
    [TestFixture]
    class PrinterDevicesTest
    {
        private Printers _printerDevices;

        [SetUp]
        public void SetUp()
        {
            _printerDevices = new Printers();    
        }

        [TearDown]
        public void TearDown()
        {}

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void GetPrinterByIndex_IfIndexTooBig_ThrowArgumentException()
        {
            int count = _printerDevices.Count;

            _printerDevices.GetPrinterByIndex(count + 1);
        }
        
        [Test]
        [ExpectedException(typeof (ArgumentException))]
        public void GetPrinterByIndex_IfIndexNegative_ThrowArgumentException()
        {
            _printerDevices.GetPrinterByIndex(-1);
        }
    }
}
