using pdfforge.PDFCreator.Conversion.Processing.PdfToolsProcessing;
using PDFCreator.TestUtilities;

namespace pdfforge.PDFCreator.IntegrationTest.Conversion.PDFProcessing.PdfTools
{
    public class PdfToolsTestLicensing : PdfToolsLicensing
    {
        public string _PdfToolboxKey
        { 
            set { PdfToolboxKey = value; }
        }
        public string _PdfAConverterKey
        { 
            set { PdfAConverterKey = value; }
        }
        public string _PdfSecureKey
        {
            set { PdfSecureKey = value; }
        }
        public PdfToolsTestLicensing() : base(ParameterHelper.GetPassword("pdftoolbox_key"), ParameterHelper.GetPassword("pdfa_converter_key"), ParameterHelper.GetPassword("pdfsecure_key"))
        {   }
    }
}
