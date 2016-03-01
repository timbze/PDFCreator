using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using NUnit.Framework;
using pdfforge.PDFCreator.Core.Ghostscript;
using pdfforge.PDFCreator.Core.Ghostscript.OutputDevices;
using pdfforge.PDFCreator.Core.Settings.Enums;

namespace PDFCreator.Core.UnitTest.Ghostscript.OutputDevices
{
    class PdfDeviceParametersTest
    {

        private OutputDevice _pdfDevice;
        private Collection<string> _parameterStrings;
        private GhostscriptVersion _ghostscriptVersion;

        [SetUp]
        public void SetUp()
        {
            _pdfDevice = ParametersTestHelper.GenerateDevice(OutputFormat.Pdf);
            _ghostscriptVersion = ParametersTestHelper.GhostscriptVersionDummie;
        }

        [Test]
        public void CheckDeviceIndependentDefaultParameters()
        {
            _parameterStrings = new Collection<string>(_pdfDevice.GetGhostScriptParameters(_ghostscriptVersion));
            ParametersTestHelper.CheckDefaultParameters(_parameterStrings);
        }

        [Test]
        public void CheckDeviceSpecificDefaultParameters()
        {
            _parameterStrings = new Collection<string>(_pdfDevice.GetGhostScriptParameters(_ghostscriptVersion));
            
            Assert.Contains("-sDEVICE=pdfwrite", _parameterStrings, "Missing default device parameter.");
            Assert.Contains("-dCompatibilityLevel=1.4", _parameterStrings, "Missing CompatibilityLevel 1.4");
            Assert.Contains("-dPDFSETTINGS=/default", _parameterStrings, "Missing default device parameter.");
            Assert.Contains("-dEmbedAllFonts=true", _parameterStrings, "Missing default device parameter.");

            var outputFileParameter = _parameterStrings.First(x => x.StartsWith("-sOutputFile="));
            Assert.IsNotNull(outputFileParameter, "Missing -sOutputFile parameter.");
            outputFileParameter = outputFileParameter.Replace("-sOutputFile=", "");
            var requiredOutput = Path.Combine(_pdfDevice.Job.JobTempOutputFolder, _pdfDevice.Job.JobTempFileName) + ".pdf";
            Assert.AreEqual(requiredOutput, outputFileParameter, "Failure in output");
        }

        [Test]
        public void ParametersTest_PdfX()
        {
            _pdfDevice.Job.Profile.OutputFormat = OutputFormat.PdfX;

            _parameterStrings = new Collection<string>(_pdfDevice.GetGhostScriptParameters(_ghostscriptVersion));

            Assert.Contains("-dPDFX", _parameterStrings, "Missing parameter.");
            var iccProfile = _parameterStrings.FirstOrDefault(x => x.StartsWith("-sOutputICCProfile=\""));
            Assert.IsNotNull(iccProfile, "Missing Parameter for ICC Profile.");            
            var defFile = _parameterStrings.FirstOrDefault(x => x.EndsWith("pdfx_def.ps"));
            Assert.IsNotNull(defFile, "Missing DefFile.");
        }

        [Test]
        public void ParametersTest_PdfX_DefinitonFile_Behind_ProcessColorModel()
        {
            _pdfDevice.Job.Profile.OutputFormat = OutputFormat.PdfX;
            _parameterStrings = new Collection<string>(_pdfDevice.GetGhostScriptParameters(_ghostscriptVersion));
            
            var defFile = _parameterStrings.FirstOrDefault(x => x.EndsWith("pdfx_def.ps"));
            var defFileIndex = _parameterStrings.IndexOf(defFile);

            var processColorModel = _parameterStrings.FirstOrDefault(x => x.StartsWith("-dProcessColorModel"));
            var processColorModelIndex = _parameterStrings.IndexOf(processColorModel);

            Assert.Greater(defFileIndex, processColorModelIndex);
        }

        [Test]
        public void ParametersTest_PdfX_DefinitonFile_Behind_ColorConversionStrategyIndex()
        {
            _pdfDevice.Job.Profile.OutputFormat = OutputFormat.PdfX;
            _parameterStrings = new Collection<string>(_pdfDevice.GetGhostScriptParameters(_ghostscriptVersion));

            var defFile = _parameterStrings.FirstOrDefault(x => x.EndsWith("pdfx_def.ps"));
            var defFileIndex = _parameterStrings.IndexOf(defFile);

            var sColorConversionStrategy = _parameterStrings.FirstOrDefault(x => x.StartsWith("-sColorConversionStrategy"));
            var sColorConversionStrategyIndex = _parameterStrings.IndexOf(sColorConversionStrategy);

            Assert.Greater(defFileIndex, sColorConversionStrategyIndex);
        }

        [Test]
        public void ParametersTest_PdfX_DefinitonFile_Before_GrayImageAutoFilterStrategy()
        {
            _pdfDevice.Job.Profile.OutputFormat = OutputFormat.PdfX;
            _parameterStrings = new Collection<string>(_pdfDevice.GetGhostScriptParameters(_ghostscriptVersion));

            var grayImageAutoFilterStrategy = _parameterStrings.FirstOrDefault(x => x.StartsWith("-dGrayImageAutoFilterStrategy"));
            var grayImageAutoFilterStrategyIndex = _parameterStrings.IndexOf(grayImageAutoFilterStrategy);
            
            var defFile = _parameterStrings.FirstOrDefault(x => x.EndsWith("pdfx_def.ps"));
            var defFileIndex = _parameterStrings.IndexOf(defFile);

            Assert.Greater(grayImageAutoFilterStrategyIndex, defFileIndex);
        }

        [Test]
        public void ParametersTest_PdfA1b()
        {
            _pdfDevice.Job.Profile.OutputFormat = OutputFormat.PdfA1B;

            _parameterStrings = new Collection<string>(_pdfDevice.GetGhostScriptParameters(_ghostscriptVersion));

            Assert.Contains("-dPDFA=1", _parameterStrings, "Missing parameter.");
            var iccProfile = _parameterStrings.FirstOrDefault(x => x.StartsWith("-sOutputICCProfile=\""));
            Assert.IsNotNull(iccProfile, "Missing Parameter for ICC Profile.");
            var defFile = _parameterStrings.FirstOrDefault(x => x.EndsWith("pdfa_def.ps"));
            Assert.IsNotNull(defFile, "Missing DefFile.");
        }

        [Test]
        public void ParametersTest_PdfA2b()
        {
            _pdfDevice.Job.Profile.OutputFormat = OutputFormat.PdfA2B;

            _parameterStrings = new Collection<string>(_pdfDevice.GetGhostScriptParameters(_ghostscriptVersion));

            Assert.Contains("-dPDFA=2", _parameterStrings, "Missing parameter.");
            var iccProfile = _parameterStrings.FirstOrDefault(x => x.StartsWith("-sOutputICCProfile=\""));
            Assert.IsNotNull(iccProfile, "Missing Parameter for ICC Profile.");
            var defFile = _parameterStrings.FirstOrDefault(x => x.EndsWith("pdfa_def.ps"));
            Assert.IsNotNull(defFile, "Missing DefFile.");
        }

        [Test]
        public void ParametersTest_FastWebView()
        {
            _pdfDevice.Job.Profile.OutputFormat = OutputFormat.Pdf;
            _pdfDevice.Job.Profile.PdfSettings.FastWebView = true;
            _parameterStrings = new Collection<string>(_pdfDevice.GetGhostScriptParameters(_ghostscriptVersion));
            Assert.Contains("-dFastWebView=true", _parameterStrings, "Missing parameter.");

            _pdfDevice.Job.Profile.OutputFormat = OutputFormat.Pdf;
            _pdfDevice.Job.Profile.PdfSettings.FastWebView = false;
            _parameterStrings = new Collection<string>(_pdfDevice.GetGhostScriptParameters(_ghostscriptVersion));
            Assert.False(_parameterStrings.Contains("-dFastWebView=true"), "Fast web view parameter falsely set.");

            _pdfDevice.Job.Profile.OutputFormat = OutputFormat.PdfA1B;
            _pdfDevice.Job.Profile.PdfSettings.FastWebView = true;
            _parameterStrings = new Collection<string>(_pdfDevice.GetGhostScriptParameters(_ghostscriptVersion));
            Assert.False(_parameterStrings.Contains("-dFastWebView=true"), "PdfA-1b must not contain fast web view parameter.");

            _pdfDevice.Job.Profile.OutputFormat = OutputFormat.PdfA2B;
            _pdfDevice.Job.Profile.PdfSettings.FastWebView = true;
            _parameterStrings = new Collection<string>(_pdfDevice.GetGhostScriptParameters(_ghostscriptVersion));
            Assert.False(_parameterStrings.Contains("-dFastWebView=true"), "PdfA-2b must not contain fast web view parameter.");
        }

        [Test]
        public void ParametersTest_PageOrientation_Landscape()
        {
            _pdfDevice.Job.Profile.OutputFormat = OutputFormat.Pdf;
            _pdfDevice.Job.Profile.PdfSettings.PageOrientation = PageOrientation.Landscape;
            _parameterStrings = new Collection<string>(_pdfDevice.GetGhostScriptParameters(_ghostscriptVersion));
            Assert.Contains("-dAutoRotatePages=/None", _parameterStrings, "Missing parameter.");
            
            Assert.Contains("-c", _parameterStrings, "Missing parameter.");
            Assert.Contains("<</Orientation 3>> setpagedevice", _parameterStrings, "Missing parameter.");
            var cIndex = _parameterStrings.IndexOf("-c");
            Assert.Less(cIndex, _parameterStrings.IndexOf("<</Orientation 3>> setpagedevice"), "Destiller parameter before -c");
        }

        [Test]
        public void ParametersTest_PageOrientation_Automatic()
        {
            _pdfDevice.Job.Profile.OutputFormat = OutputFormat.PdfA2B;
            _pdfDevice.Job.Profile.PdfSettings.PageOrientation = PageOrientation.Automatic;
            _parameterStrings = new Collection<string>(_pdfDevice.GetGhostScriptParameters(_ghostscriptVersion));
            Assert.Contains("-dAutoRotatePages=/PageByPage", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dParseDSCComments=false", _parameterStrings, "Missing parameter.");
        }

        [Test]
        public void ParametersTest_PageOrientation_Portrait()
        {
            _pdfDevice.Job.Profile.OutputFormat = OutputFormat.PdfX;
            _pdfDevice.Job.Profile.PdfSettings.PageOrientation = PageOrientation.Portrait;
            
            _parameterStrings = new Collection<string>(_pdfDevice.GetGhostScriptParameters(_ghostscriptVersion));
            
            Assert.Contains("-dAutoRotatePages=/None", _parameterStrings, "Missing parameter.");

            Assert.Contains("-c", _parameterStrings, "Missing parameter.");
            Assert.Contains("<</Orientation 0>> setpagedevice", _parameterStrings, "Missing parameter.");
            var cIndex = _parameterStrings.IndexOf("-c");
            Assert.Less(cIndex, _parameterStrings.IndexOf("<</Orientation 0>> setpagedevice"), "Destiller parameter before -c");
        }

        [Test]
        public void ParametersTest_ColorSchemes_Cmyk()
        {
            _pdfDevice.Job.Profile.OutputFormat = OutputFormat.Pdf;
            _pdfDevice.Job.Profile.PdfSettings.ColorModel = ColorModel.Cmyk;
            _parameterStrings = new Collection<string>(_pdfDevice.GetGhostScriptParameters(_ghostscriptVersion));
            Assert.Contains("-sColorConversionStrategy=CMYK", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dProcessColorModel=/DeviceCMYK", _parameterStrings, "Missing parameter.");
        }

        [Test]
        public void ParametersTest_ColorSchemes_Gray()
        {
            _pdfDevice.Job.Profile.OutputFormat = OutputFormat.Pdf;
            _pdfDevice.Job.Profile.PdfSettings.ColorModel = ColorModel.Gray;
            _parameterStrings = new Collection<string>(_pdfDevice.GetGhostScriptParameters(_ghostscriptVersion));
            Assert.Contains("-sColorConversionStrategy=Gray", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dProcessColorModel=/DeviceGray", _parameterStrings, "Missing parameter.");
        }

        [Test]
        public void ParametersTest_ColorSchemes_Rgb()
        {
            _pdfDevice.Job.Profile.OutputFormat = OutputFormat.Pdf;
            _pdfDevice.Job.Profile.PdfSettings.ColorModel = ColorModel.Rgb;
            _parameterStrings = new Collection<string>(_pdfDevice.GetGhostScriptParameters(_ghostscriptVersion));
            Assert.Contains("-sColorConversionStrategy=RGB", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dProcessColorModel=/DeviceRGB", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dConvertCMYKImagesToRGB=true", _parameterStrings, "Missing parameter.");
        }

        [Test]
        [Ignore]
        public void ParametersTest_ColorSchemes_Rgb_PdfA_1b_RequiresDeviceIndependentColor()
        {
            _pdfDevice.Job.Profile.OutputFormat = OutputFormat.PdfA1B;
            _pdfDevice.Job.Profile.PdfSettings.ColorModel = ColorModel.Rgb;
            _parameterStrings = new Collection<string>(_pdfDevice.GetGhostScriptParameters(_ghostscriptVersion));
            Assert.Contains("-sColorConversionStrategy=/UseDeviceIndependentColor", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dProcessColorModel=/DeviceRGB", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dConvertCMYKImagesToRGB=true", _parameterStrings, "Missing parameter.");
        }

        [Test]
        [Ignore]
        public void ParametersTest_ColorSchemes_Rgb_PdfA_2b_RequiresDeviceIndependentColor()
        {
            _pdfDevice.Job.Profile.OutputFormat = OutputFormat.PdfA2B;
            _pdfDevice.Job.Profile.PdfSettings.ColorModel = ColorModel.Rgb;
            _parameterStrings = new Collection<string>(_pdfDevice.GetGhostScriptParameters(_ghostscriptVersion));
            Assert.Contains("-sColorConversionStrategy=/UseDeviceIndependentColor", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dProcessColorModel=/DeviceRGB", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dConvertCMYKImagesToRGB=true", _parameterStrings, "Missing parameter.");
        }

        [Test]
        public void ParametersTest_ColorSchemes_Rgb_PdfA_2b_WillBeForcedToCmyk()
        {
            _pdfDevice.Job.Profile.OutputFormat = OutputFormat.PdfA2B;
            _pdfDevice.Job.Profile.PdfSettings.ColorModel = ColorModel.Rgb;
            _parameterStrings = new Collection<string>(_pdfDevice.GetGhostScriptParameters(_ghostscriptVersion));
            Assert.Contains("-sColorConversionStrategy=CMYK", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dProcessColorModel=/DeviceCMYK", _parameterStrings, "Missing parameter.");
        }

        [Test]
        public void ParametersTest_PdfX_RgbBecomesCmyk()
        {
            _pdfDevice.Job.Profile.OutputFormat = OutputFormat.PdfX;
            _pdfDevice.Job.Profile.PdfSettings.ColorModel = ColorModel.Rgb;
            _parameterStrings = new Collection<string>(_pdfDevice.GetGhostScriptParameters(_ghostscriptVersion));

            Assert.Contains("-sColorConversionStrategy=CMYK", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dProcessColorModel=/DeviceCMYK", _parameterStrings, "Missing parameter.");
        }

        [Test]
        public void ParametersTest_GrayAndColorImagesCompressionDisabled()
        {
            _pdfDevice.Job.Profile.OutputFormat = OutputFormat.Pdf;
            _pdfDevice.Job.Profile.PdfSettings.CompressColorAndGray.Enabled = false;

            _parameterStrings = new Collection<string>(_pdfDevice.GetGhostScriptParameters(_ghostscriptVersion));

            Assert.Contains("-dAutoFilterColorImages=false", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dAutoFilterGrayImages=false", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dEncodeColorImages=false", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dEncodeGrayImages=false", _parameterStrings, "Missing parameter.");
        }

        [Test]
        public void ParametersTest_GrayAndColorImagesCompressionJpegMaximum_ResamplingDisabled()
        {
            _pdfDevice.Job.Profile.OutputFormat = OutputFormat.Pdf;
            _pdfDevice.Job.Profile.PdfSettings.CompressColorAndGray.Enabled = true;
            _pdfDevice.Job.Profile.PdfSettings.CompressColorAndGray.Compression = CompressionColorAndGray.JpegMaximum;
            _pdfDevice.Job.Profile.PdfSettings.CompressColorAndGray.Resampling = false;
            
            _parameterStrings = new Collection<string>(_pdfDevice.GetGhostScriptParameters(_ghostscriptVersion));
            
            Assert.Contains("-dAutoFilterColorImages=false", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dAutoFilterGrayImages=false", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dEncodeColorImages=true", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dEncodeGrayImages=true", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dColorImageFilter=/DCTEncode", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dGrayImageFilter=/DCTEncode", _parameterStrings, "Missing parameter.");

            Assert.Contains("-c", _parameterStrings, "Missing parameter.");
            Assert.Contains(".setpdfwrite << /ColorImageDict <</QFactor 2.4 /Blend 1 /HSample [2 1 1 2] /VSample [2 1 1 2]>> >> setdistillerparams", _parameterStrings, "Missing parameter.");
            Assert.Contains(".setpdfwrite << /GrayImageDict <</QFactor 2.4 /Blend 1 /HSample [2 1 1 2] /VSample [2 1 1 2]>> >> setdistillerparams", _parameterStrings, "Missing parameter.");
            var cIndex = _parameterStrings.IndexOf("-c");
            Assert.Less(cIndex, _parameterStrings.IndexOf(".setpdfwrite << /ColorImageDict <</QFactor 2.4 /Blend 1 /HSample [2 1 1 2] /VSample [2 1 1 2]>> >> setdistillerparams"), "Destiller parameter before -c");
            Assert.Less(cIndex, _parameterStrings.IndexOf(".setpdfwrite << /GrayImageDict <</QFactor 2.4 /Blend 1 /HSample [2 1 1 2] /VSample [2 1 1 2]>> >> setdistillerparams"), "Destiller parameter before -c");
        }

        [Test]
        public void ParametersTest_GrayAndColorImagesCompressionJpegHigh_Resampling3Dpi_RaisedTo4()
        {
            _pdfDevice.Job.Profile.OutputFormat = OutputFormat.PdfA2B;
            _pdfDevice.Job.Profile.PdfSettings.CompressColorAndGray.Enabled = true;
            _pdfDevice.Job.Profile.PdfSettings.CompressColorAndGray.Compression = CompressionColorAndGray.JpegHigh;
            _pdfDevice.Job.Profile.PdfSettings.CompressColorAndGray.Resampling = true;
            _pdfDevice.Job.Profile.PdfSettings.CompressColorAndGray.Dpi = 3;

            _parameterStrings = new Collection<string>(_pdfDevice.GetGhostScriptParameters(_ghostscriptVersion));

            Assert.Contains("-dAutoFilterColorImages=false", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dAutoFilterGrayImages=false", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dEncodeColorImages=true", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dEncodeGrayImages=true", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dColorImageFilter=/DCTEncode", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dGrayImageFilter=/DCTEncode", _parameterStrings, "Missing parameter.");

            Assert.Contains("-c", _parameterStrings, "Missing parameter.");
            Assert.Contains(".setpdfwrite << /ColorImageDict <</QFactor 1.3 /Blend 1 /HSample [2 1 1 2] /VSample [2 1 1 2]>> >> setdistillerparams", _parameterStrings, "Missing parameter.");
            Assert.Contains(".setpdfwrite << /GrayImageDict <</QFactor 1.3 /Blend 1 /HSample [2 1 1 2] /VSample [2 1 1 2]>> >> setdistillerparams", _parameterStrings, "Missing parameter.");
            var cIndex = _parameterStrings.IndexOf("-c");
            Assert.Less(cIndex, _parameterStrings.IndexOf(".setpdfwrite << /ColorImageDict <</QFactor 1.3 /Blend 1 /HSample [2 1 1 2] /VSample [2 1 1 2]>> >> setdistillerparams"), "Destiller parameter before -c");
            Assert.Less(cIndex, _parameterStrings.IndexOf(".setpdfwrite << /GrayImageDict <</QFactor 1.3 /Blend 1 /HSample [2 1 1 2] /VSample [2 1 1 2]>> >> setdistillerparams"), "Destiller parameter before -c");

            Assert.Contains("-dDownsampleColorImages=true", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dColorImageResolution=" + 4, _parameterStrings, "Missing parameter.");
            Assert.Contains("-dDownsampleGrayImages=true", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dGrayImageResolution=" + 4, _parameterStrings, "Missing parameter.");
        }

        [Test]
        public void ParametersTest_GrayAndColorImagesCompressionJpegMedium_Resampling4Dpi()
        {
            _pdfDevice.Job.Profile.OutputFormat = OutputFormat.PdfX;
            _pdfDevice.Job.Profile.PdfSettings.CompressColorAndGray.Enabled = true;
            _pdfDevice.Job.Profile.PdfSettings.CompressColorAndGray.Compression = CompressionColorAndGray.JpegMedium;
            _pdfDevice.Job.Profile.PdfSettings.CompressColorAndGray.Resampling = true;
            _pdfDevice.Job.Profile.PdfSettings.CompressColorAndGray.Dpi = 4;

            _parameterStrings = new Collection<string>(_pdfDevice.GetGhostScriptParameters(_ghostscriptVersion));

            Assert.Contains("-dAutoFilterColorImages=false", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dAutoFilterGrayImages=false", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dEncodeColorImages=true", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dEncodeGrayImages=true", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dColorImageFilter=/DCTEncode", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dGrayImageFilter=/DCTEncode", _parameterStrings, "Missing parameter.");

            Assert.Contains("-c", _parameterStrings, "Missing parameter.");
            Assert.Contains(".setpdfwrite << /ColorImageDict <</QFactor 0.76 /Blend 1 /HSample [2 1 1 2] /VSample [2 1 1 2]>> >> setdistillerparams", _parameterStrings, "Missing parameter.");
            Assert.Contains(".setpdfwrite << /GrayImageDict <</QFactor 0.76 /Blend 1 /HSample [2 1 1 2] /VSample [2 1 1 2]>> >> setdistillerparams", _parameterStrings, "Missing parameter.");
            var cIndex = _parameterStrings.IndexOf("-c");
            Assert.Less(cIndex, _parameterStrings.IndexOf(".setpdfwrite << /ColorImageDict <</QFactor 0.76 /Blend 1 /HSample [2 1 1 2] /VSample [2 1 1 2]>> >> setdistillerparams"), "Destiller parameter before -c");
            Assert.Less(cIndex, _parameterStrings.IndexOf(".setpdfwrite << /GrayImageDict <</QFactor 0.76 /Blend 1 /HSample [2 1 1 2] /VSample [2 1 1 2]>> >> setdistillerparams"), "Destiller parameter before -c");

            Assert.Contains("-dDownsampleColorImages=true", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dColorImageResolution=" + 4, _parameterStrings, "Missing parameter.");
            Assert.Contains("-dDownsampleGrayImages=true", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dGrayImageResolution=" + 4, _parameterStrings, "Missing parameter.");
        }

        [Test]
        public void ParametersTest_GrayAndColorImagesCompressionJpegLow_Resampling150Dpi()
        {
            _pdfDevice.Job.Profile.OutputFormat = OutputFormat.Pdf;
            _pdfDevice.Job.Profile.PdfSettings.CompressColorAndGray.Enabled = true;
            _pdfDevice.Job.Profile.PdfSettings.CompressColorAndGray.Compression = CompressionColorAndGray.JpegLow;
            _pdfDevice.Job.Profile.PdfSettings.CompressColorAndGray.Resampling = true;
            _pdfDevice.Job.Profile.PdfSettings.CompressColorAndGray.Dpi = 150;

            _parameterStrings = new Collection<string>(_pdfDevice.GetGhostScriptParameters(_ghostscriptVersion));
            
            Assert.Contains("-dAutoFilterColorImages=false", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dAutoFilterGrayImages=false", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dEncodeColorImages=true", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dEncodeGrayImages=true", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dColorImageFilter=/DCTEncode", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dGrayImageFilter=/DCTEncode", _parameterStrings, "Missing parameter.");

            Assert.Contains("-c", _parameterStrings, "Missing parameter.");
            Assert.Contains(".setpdfwrite << /ColorImageDict <</QFactor 0.40 /Blend 1 /HSample [2 1 1 2] /VSample [2 1 1 2]>> >> setdistillerparams", _parameterStrings, "Missing parameter.");
            Assert.Contains(".setpdfwrite << /GrayImageDict <</QFactor 0.40 /Blend 1 /HSample [2 1 1 2] /VSample [2 1 1 2]>> >> setdistillerparams", _parameterStrings, "Missing parameter.");
            var cIndex = _parameterStrings.IndexOf("-c");
            Assert.Less(cIndex, _parameterStrings.IndexOf(".setpdfwrite << /ColorImageDict <</QFactor 0.40 /Blend 1 /HSample [2 1 1 2] /VSample [2 1 1 2]>> >> setdistillerparams"), "Destiller parameter before -c");
            Assert.Less(cIndex, _parameterStrings.IndexOf(".setpdfwrite << /GrayImageDict <</QFactor 0.40 /Blend 1 /HSample [2 1 1 2] /VSample [2 1 1 2]>> >> setdistillerparams"), "Destiller parameter before -c");

            Assert.Contains("-dDownsampleColorImages=true", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dColorImageResolution=" + 150, _parameterStrings, "Missing parameter.");
            Assert.Contains("-dDownsampleGrayImages=true", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dGrayImageResolution=" + 150, _parameterStrings, "Missing parameter.");
        }

        [Test]
        public void ParametersTest_GrayAndColorImagesCompressionJpegMinimum_Resampling1200Dpi()
        {
            _pdfDevice.Job.Profile.OutputFormat = OutputFormat.PdfA2B;
            _pdfDevice.Job.Profile.PdfSettings.CompressColorAndGray.Enabled = true;
            _pdfDevice.Job.Profile.PdfSettings.CompressColorAndGray.Compression = CompressionColorAndGray.JpegMinimum;
            _pdfDevice.Job.Profile.PdfSettings.CompressColorAndGray.Resampling = true;
            _pdfDevice.Job.Profile.PdfSettings.CompressColorAndGray.Dpi = 1200;

            _parameterStrings = new Collection<string>(_pdfDevice.GetGhostScriptParameters(_ghostscriptVersion));

            Assert.Contains("-dAutoFilterColorImages=false", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dAutoFilterGrayImages=false", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dEncodeColorImages=true", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dEncodeGrayImages=true", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dColorImageFilter=/DCTEncode", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dGrayImageFilter=/DCTEncode", _parameterStrings, "Missing parameter.");

            Assert.Contains("-c", _parameterStrings, "Missing parameter.");
            Assert.Contains(".setpdfwrite << /ColorImageDict <</QFactor 0.15 /Blend 1 /HSample [2 1 1 2] /VSample [2 1 1 2]>> >> setdistillerparams", _parameterStrings, "Missing parameter.");
            Assert.Contains(".setpdfwrite << /GrayImageDict <</QFactor 0.15 /Blend 1 /HSample [2 1 1 2] /VSample [2 1 1 2]>> >> setdistillerparams", _parameterStrings, "Missing parameter.");
            var cIndex = _parameterStrings.IndexOf("-c");
            Assert.Less(cIndex, _parameterStrings.IndexOf(".setpdfwrite << /ColorImageDict <</QFactor 0.15 /Blend 1 /HSample [2 1 1 2] /VSample [2 1 1 2]>> >> setdistillerparams"), "Destiller parameter before -c");
            Assert.Less(cIndex, _parameterStrings.IndexOf(".setpdfwrite << /GrayImageDict <</QFactor 0.15 /Blend 1 /HSample [2 1 1 2] /VSample [2 1 1 2]>> >> setdistillerparams"), "Destiller parameter before -c");

            Assert.Contains("-dDownsampleColorImages=true", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dColorImageResolution=" + 1200, _parameterStrings, "Missing parameter.");
            Assert.Contains("-dDownsampleGrayImages=true", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dGrayImageResolution=" + 1200, _parameterStrings, "Missing parameter.");
        }

        [Test]
        public void ParametersTest_GrayAndColorImagesCompressionJpegZip_Resampling2400Dpi()
        {
            _pdfDevice.Job.Profile.OutputFormat = OutputFormat.PdfX;
            _pdfDevice.Job.Profile.PdfSettings.CompressColorAndGray.Enabled = true;
            _pdfDevice.Job.Profile.PdfSettings.CompressColorAndGray.Compression = CompressionColorAndGray.Zip;
            _pdfDevice.Job.Profile.PdfSettings.CompressColorAndGray.Resampling = true;
            _pdfDevice.Job.Profile.PdfSettings.CompressColorAndGray.Dpi = 2400;

            _parameterStrings = new Collection<string>(_pdfDevice.GetGhostScriptParameters(_ghostscriptVersion));

            Assert.Contains("-dAutoFilterColorImages=false", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dAutoFilterGrayImages=false", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dEncodeColorImages=true", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dEncodeGrayImages=true", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dColorImageFilter=/FlateEncode", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dGrayImageFilter=/FlateEncode", _parameterStrings, "Missing parameter.");

            Assert.Contains("-dDownsampleColorImages=true", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dColorImageResolution=" + 2400, _parameterStrings, "Missing parameter.");
            Assert.Contains("-dDownsampleGrayImages=true", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dGrayImageResolution=" + 2400, _parameterStrings, "Missing parameter.");
        }

        [Test]
        public void ParametersTest_GrayAndColorImagesCompressionJpegManual_Resampling2401Dpi_LoweredTo2400Dpi()
        {
            _pdfDevice.Job.Profile.OutputFormat = OutputFormat.Pdf;
            _pdfDevice.Job.Profile.PdfSettings.CompressColorAndGray.Enabled = true;
            _pdfDevice.Job.Profile.PdfSettings.CompressColorAndGray.Compression = CompressionColorAndGray.JpegManual;
            _pdfDevice.Job.Profile.PdfSettings.CompressColorAndGray.JpegCompressionFactor = 1.23;
            _pdfDevice.Job.Profile.PdfSettings.CompressColorAndGray.Resampling = true;
            _pdfDevice.Job.Profile.PdfSettings.CompressColorAndGray.Dpi = 2401;

            _parameterStrings = new Collection<string>(_pdfDevice.GetGhostScriptParameters(_ghostscriptVersion));
            
            Assert.Contains("-dAutoFilterColorImages=false", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dAutoFilterGrayImages=false", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dEncodeColorImages=true", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dEncodeGrayImages=true", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dColorImageFilter=/DCTEncode", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dGrayImageFilter=/DCTEncode", _parameterStrings, "Missing parameter.");

            Assert.Contains("-c", _parameterStrings, "Missing parameter.");
            Assert.Contains(".setpdfwrite << /ColorImageDict <</QFactor " +
                                                1.23.ToString(CultureInfo.InvariantCulture) +
                                                " /Blend 1 /HSample [2 1 1 2] /VSample [2 1 1 2]>> >> setdistillerparams", _parameterStrings, "Missing parameter.");
            Assert.Contains(".setpdfwrite << /GrayImageDict <</QFactor " +
                                                1.23.ToString(CultureInfo.InvariantCulture) +
                                                " /Blend 1 /HSample [2 1 1 2] /VSample [2 1 1 2]>> >> setdistillerparams", _parameterStrings, "Missing parameter.");
            var cIndex = _parameterStrings.IndexOf("-c");
            Assert.Less(cIndex, _parameterStrings.IndexOf(".setpdfwrite << /ColorImageDict <</QFactor " +
                                                1.23.ToString(CultureInfo.InvariantCulture) +
                                                " /Blend 1 /HSample [2 1 1 2] /VSample [2 1 1 2]>> >> setdistillerparams"), "Destiller parameter before -c");
            Assert.Less(cIndex, _parameterStrings.IndexOf(".setpdfwrite << /GrayImageDict <</QFactor " +
                                                1.23.ToString(CultureInfo.InvariantCulture) +
                                                " /Blend 1 /HSample [2 1 1 2] /VSample [2 1 1 2]>> >> setdistillerparams"), "Destiller parameter before -c");

            Assert.Contains("-dDownsampleColorImages=true", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dColorImageResolution=" + 2400, _parameterStrings, "Missing parameter.");
            Assert.Contains("-dDownsampleGrayImages=true", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dGrayImageResolution=" + 2400, _parameterStrings, "Missing parameter.");
        }

        [Test]
        public void ParametersTest_GrayAndColorImagesCompressionJpegAutomatic_Resampling300Dpi_ResamplingGetsDisabled()
        {
            _pdfDevice.Job.Profile.OutputFormat = OutputFormat.PdfA2B;
            _pdfDevice.Job.Profile.PdfSettings.CompressColorAndGray.Enabled = true;
            _pdfDevice.Job.Profile.PdfSettings.CompressColorAndGray.Compression = CompressionColorAndGray.Automatic;
            _pdfDevice.Job.Profile.PdfSettings.CompressColorAndGray.Resampling = true;
            _pdfDevice.Job.Profile.PdfSettings.CompressColorAndGray.Dpi = 300;

            _pdfDevice.Job.Profile.PdfSettings.CompressColorAndGray.Resampling = true;

            _parameterStrings = new Collection<string>(_pdfDevice.GetGhostScriptParameters(_ghostscriptVersion));

            Assert.Contains("-dAutoFilterColorImages=true", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dAutoFilterGrayImages=true", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dEncodeColorImages=true", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dEncodeGrayImages=true", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dColorImageAutoFilterStrategy=/JPEG", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dGrayImageAutoFilterStrategy=/JPEG", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dColorImageFilter=/DCTEncode", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dGrayImageFilter=/DCTEncode", _parameterStrings, "Missing parameter.");

            Assert.IsFalse(_parameterStrings.Contains("-dDownsampleColorImages=true"), "Falsely set downsample parameter for colored images");
            Assert.IsFalse(_parameterStrings.Contains("-dDownsampleGrayImages=true"), "Falsely set downsample parameter for gray images");
        }

        [Test]
        public void ParametersTest_MonoImagesCompressionDisabled_ResamplingEnabled_ResamplingGetsBlocked()
        {
            _pdfDevice.Job.Profile.OutputFormat = OutputFormat.Pdf;
            _pdfDevice.Job.Profile.PdfSettings.CompressMonochrome.Enabled = false;
            _pdfDevice.Job.Profile.PdfSettings.CompressMonochrome.Resampling = true;
            _pdfDevice.Job.Profile.PdfSettings.CompressMonochrome.Dpi = 123;
            _parameterStrings = new Collection<string>(_pdfDevice.GetGhostScriptParameters(_ghostscriptVersion));

            Assert.Contains("-dEncodeMonoImages=false", _parameterStrings, "Missing parameter.");

            Assert.IsFalse(_parameterStrings.Contains("-dDownsampleMonoImages=true"), "Falsely set resample parameter");
            Assert.IsFalse(_parameterStrings.Contains("-dMonoImageDownsampleType=/Bicubic"), "Falsely set resample parameter");
            Assert.IsFalse(_parameterStrings.Contains("-dMonoImageResolution=" + 123), "Falsely set resample parameter");
        }

        [Test]
        public void ParametersTest_MonoImagesCcittFaxEncoding_ResamplingDisabled()
        {
            _pdfDevice.Job.Profile.OutputFormat = OutputFormat.PdfA2B;
            _pdfDevice.Job.Profile.PdfSettings.CompressMonochrome.Enabled = true;
            _pdfDevice.Job.Profile.PdfSettings.CompressMonochrome.Compression = CompressionMonochrome.CcittFaxEncoding;
            _pdfDevice.Job.Profile.PdfSettings.CompressMonochrome.Resampling = false;
            _pdfDevice.Job.Profile.PdfSettings.CompressMonochrome.Dpi = 123;

            _parameterStrings = new Collection<string>(_pdfDevice.GetGhostScriptParameters(_ghostscriptVersion));

            Assert.Contains("-dEncodeMonoImages=true", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dMonoImageFilter=/CCITTFaxEncode", _parameterStrings, "Missing parameter.");

            Assert.IsFalse(_parameterStrings.Contains("-dDownsampleMonoImages=true"), "Falsely set resample parameter");
            Assert.IsFalse(_parameterStrings.Contains("-dMonoImageDownsampleType=/Bicubic"), "Falsely set resample parameter");
            Assert.IsFalse(_parameterStrings.Contains("-dMonoImageResolution=" + 123), "Falsely set resample parameter");
        }

        [Test]
        public void ParametersTest_MonoImagesRunLengthEncoding_Resampling300Dpi()
        {
            _pdfDevice.Job.Profile.OutputFormat = OutputFormat.PdfX;
            _pdfDevice.Job.Profile.PdfSettings.CompressMonochrome.Enabled = true;
            _pdfDevice.Job.Profile.PdfSettings.CompressMonochrome.Compression = CompressionMonochrome.RunLengthEncoding;
            _pdfDevice.Job.Profile.PdfSettings.CompressMonochrome.Resampling = true;
            _pdfDevice.Job.Profile.PdfSettings.CompressMonochrome.Dpi = 300;

            _parameterStrings = new Collection<string>(_pdfDevice.GetGhostScriptParameters(_ghostscriptVersion));

            Assert.Contains("-dEncodeMonoImages=true", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dMonoImageFilter=/RunLengthEncode", _parameterStrings, "Missing parameter.");

            Assert.Contains("-dMonoImageDownsampleType=/Bicubic", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dMonoImageResolution=" + 300, _parameterStrings, "Missing parameter.");
        }

        [Test]
        public void ParametersTest_MonoImagesZipResampling3Dpi_RaisedTo4()
        {
            _pdfDevice.Job.Profile.OutputFormat = OutputFormat.Pdf;
            _pdfDevice.Job.Profile.PdfSettings.CompressMonochrome.Enabled = true;
            _pdfDevice.Job.Profile.PdfSettings.CompressMonochrome.Compression = CompressionMonochrome.Zip;
            _pdfDevice.Job.Profile.PdfSettings.CompressMonochrome.Resampling = true;
            _pdfDevice.Job.Profile.PdfSettings.CompressMonochrome.Dpi = 3;

            _parameterStrings = new Collection<string>(_pdfDevice.GetGhostScriptParameters(_ghostscriptVersion));

            Assert.Contains("-dEncodeMonoImages=true", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dMonoImageFilter=/FlateEncode", _parameterStrings, "Missing parameter.");

            Assert.Contains("-dMonoImageDownsampleType=/Bicubic", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dMonoImageResolution=" + 4, _parameterStrings, "Missing parameter.");
        }

        [Test]
        public void ParametersTest_MonoImagesZipResampling2401Dpi_LoweredTo2400()
        {
            _pdfDevice.Job.Profile.OutputFormat = OutputFormat.Pdf;
            _pdfDevice.Job.Profile.PdfSettings.CompressMonochrome.Enabled = true;
            _pdfDevice.Job.Profile.PdfSettings.CompressMonochrome.Compression = CompressionMonochrome.Zip;
            _pdfDevice.Job.Profile.PdfSettings.CompressMonochrome.Resampling = true;
            _pdfDevice.Job.Profile.PdfSettings.CompressMonochrome.Dpi = 2401;

            _parameterStrings = new Collection<string>(_pdfDevice.GetGhostScriptParameters(_ghostscriptVersion));

            Assert.Contains("-dEncodeMonoImages=true", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dMonoImageFilter=/FlateEncode", _parameterStrings, "Missing parameter.");

            Assert.Contains("-dMonoImageDownsampleType=/Bicubic", _parameterStrings, "Missing parameter.");
            Assert.Contains("-dMonoImageResolution=" + 2400, _parameterStrings, "Missing parameter.");
        }

        [Test]
        public void ParametersTest_CoverPage()
        {
            _pdfDevice.Job.Profile.OutputFormat = OutputFormat.Pdf;
            _pdfDevice.Job.Profile.CoverPage.Enabled = true;
            const string coverFile = "CoverFile.pdf";
            _pdfDevice.Job.Profile.CoverPage.File = coverFile;

            _parameterStrings = new Collection<string>(_pdfDevice.GetGhostScriptParameters(_ghostscriptVersion));

            Assert.Contains(coverFile, _parameterStrings, "Missing parameter.");
            var fIndex = _parameterStrings.IndexOf("-f");
            Assert.Less(fIndex, _parameterStrings.IndexOf(coverFile), "CoverFile not behind -f parameter.");

            _pdfDevice.Job.Profile.CoverPage.Enabled = false;
            _parameterStrings = new Collection<string>(_pdfDevice.GetGhostScriptParameters(_ghostscriptVersion));
            Assert.AreEqual(-1, _parameterStrings.IndexOf(coverFile), "Falsely set CoverFile.");
        }

        [Test]
        public void ParametersTest_AttachmentPage()
        {
            _pdfDevice.Job.Profile.OutputFormat = OutputFormat.Pdf;
            _pdfDevice.Job.Profile.AttachmentPage.Enabled = true;
            const string attachmentFile = "AttachmentFile.pdf";
            _pdfDevice.Job.Profile.AttachmentPage.File = attachmentFile;

            _parameterStrings = new Collection<string>(_pdfDevice.GetGhostScriptParameters(_ghostscriptVersion));

            Assert.Contains(attachmentFile, _parameterStrings, "Missing parameter.");
            var fIndex = _parameterStrings.IndexOf("-f");
            Assert.Less(fIndex, _parameterStrings.IndexOf(attachmentFile), "AttachmentFile not behind -f parameter.");

            _pdfDevice.Job.Profile.AttachmentPage.Enabled = false;
            _parameterStrings = new Collection<string>(_pdfDevice.GetGhostScriptParameters(_ghostscriptVersion));
            Assert.AreEqual(-1, _parameterStrings.IndexOf(attachmentFile), "Falsely set AttachmentFile.");
        }

        [Test]
        public void ParametersTest_CoverAndAttachmentPage()
        {
            _pdfDevice.Job.Profile.OutputFormat = OutputFormat.Pdf;
            _pdfDevice.Job.Profile.CoverPage.Enabled = true;
            const string coverFile = "CoverFile.pdf";
            _pdfDevice.Job.Profile.CoverPage.File = coverFile;
            _pdfDevice.Job.Profile.AttachmentPage.Enabled = true;
            const string attachmentFile = "AttachmentFile.pdf";
            _pdfDevice.Job.Profile.AttachmentPage.File = attachmentFile;

            _parameterStrings = new Collection<string>(_pdfDevice.GetGhostScriptParameters(_ghostscriptVersion));

            var coverIndex = _parameterStrings.IndexOf(coverFile);
            var attachmentIndex = _parameterStrings.IndexOf(attachmentFile);

            Assert.LessOrEqual(coverIndex - attachmentIndex, -2, "No further (file)parameter between cover and attachment file.");
        }

    }
}

