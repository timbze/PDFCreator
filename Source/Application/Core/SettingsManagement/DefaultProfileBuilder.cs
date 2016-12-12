using System.Collections.Generic;
using pdfforge.DataStorage.Storage;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;

namespace pdfforge.PDFCreator.Core.SettingsManagement
{
    public class DefaultProfileBuilder
    {
        /// <summary>
        ///     Create an empty settings class with the proper registry storage attached
        /// </summary>
        /// <returns>An empty settings object</returns>
        public PdfCreatorSettings CreateEmptySettings(IStorage storage)
        {
            var settings = new PdfCreatorSettings(storage);
            return settings;
        }

        public PdfCreatorSettings CreateDefaultSettings(PdfCreatorSettings currentSettings)
        {
            var primaryPrinter = currentSettings.ApplicationSettings.PrimaryPrinter;
            var storage = currentSettings.GetStorage();
            var defaultLanguage = currentSettings.ApplicationSettings.Language;

            return CreateDefaultSettings(primaryPrinter, storage, defaultLanguage);
        }

        /// <summary>
        ///     Creates a settings object with default settings and profiles
        /// </summary>
        /// <returns>The initialized settings object</returns>
        public PdfCreatorSettings CreateDefaultSettings(string primaryPrinter, IStorage storage, string defaultLanguage)
        {
            var settings = CreateEmptySettings(storage);

            settings.ApplicationSettings.PrimaryPrinter = primaryPrinter;

            #region add default profiles

            settings.ConversionProfiles.Add(CreateDefaultProfile());
            settings.ConversionProfiles.Add(CreateHighCompressionProfile());
            settings.ConversionProfiles.Add(CreateHighQualityProfile());
            settings.ConversionProfiles.Add(CreateJpegProfile());
            settings.ConversionProfiles.Add(CreatePdfaProfile());
            settings.ConversionProfiles.Add(CreatePngProfile());
            settings.ConversionProfiles.Add(CreatePrintProfile());
            settings.ConversionProfiles.Add(CreateTiffProfile());

            #endregion

            var startReplacements = new[]
            {
                "Microsoft Word - ",
                "Microsoft PowerPoint - ",
                "Microsoft Excel - "
            };

            var endReplacements = new[]
            {
                ".xps",
                ".xml",
                ".xltx",
                ".xltm",
                ".xlt",
                ".xlsx",
                ".xlsm",
                ".xlsb",
                ".xls",
                ".xlam",
                ".xla",
                ".wmf",
                ".txt - Editor",
                ".txt - Notepad",
                ".txt",
                ".tiff",
                ".tif",
                ".thmx",
                ".slk",
                ".rtf",
                ".prn",
                ".pptx",
                ".pptm",
                ".ppt",
                ".ppsx",
                ".ppsm",
                ".pps",
                ".ppam",
                ".ppa",
                ".potx",
                ".potm",
                ".pot",
                ".png",
                ".pdf",
                ".odt",
                ".ods",
                ".odp",
                ".mhtml",
                ".mht",
                ".jpg",
                ".jpeg",
                ".html",
                ".htm",
                ".emf",
                ".dotx",
                ".dotm",
                ".dot",
                ".docx",
                ".docm",
                ".doc",
                ".dif",
                ".csv",
                ".bmp",
                " - Editor",
                " - Notepad"
            };

            var titleReplacement = new List<TitleReplacement>();

            foreach (var replacement in startReplacements)
            {
                titleReplacement.Add(new TitleReplacement(ReplacementType.Start, replacement, ""));
            }

            foreach (var replacement in endReplacements)
            {
                titleReplacement.Add(new TitleReplacement(ReplacementType.End, replacement, ""));
            }

            settings.ApplicationSettings.TitleReplacement = titleReplacement;

            settings.ApplicationSettings.Language = defaultLanguage;

            settings.SortConversionProfiles();

            if (string.IsNullOrWhiteSpace(settings.ApplicationSettings.LastUsedProfileGuid))
                settings.ApplicationSettings.LastUsedProfileGuid = ProfileGuids.DEFAULT_PROFILE_GUID;

            return settings;
        }

        public ConversionProfile CreateDefaultProfile()
        {
            var defaultProfile = new ConversionProfile();
            defaultProfile.Name = "<Default Profile>";
            defaultProfile.Guid = ProfileGuids.DEFAULT_PROFILE_GUID;

            SetDefaultProperties(defaultProfile, false);
            return defaultProfile;
        }

        private ConversionProfile CreateTiffProfile()
        {
            var tiffProfile = new ConversionProfile();
            tiffProfile.Name = "TIFF (multipage graphic file)";
            tiffProfile.Guid = ProfileGuids.TIFF_PROFILE_GUID;

            tiffProfile.OutputFormat = OutputFormat.Tif;
            tiffProfile.TiffSettings.Dpi = 150;
            tiffProfile.TiffSettings.Color = TiffColor.Color24Bit;

            SetDefaultProperties(tiffProfile, true);
            return tiffProfile;
        }

        private ConversionProfile CreatePrintProfile()
        {
            var printProfile = new ConversionProfile();
            printProfile.Name = "Print after saving";
            printProfile.Guid = ProfileGuids.PRINT_PROFILE_GUID;

            printProfile.Printing.Enabled = true;
            printProfile.Printing.SelectPrinter = SelectPrinter.ShowDialog;

            SetDefaultProperties(printProfile, true);
            return printProfile;
        }

        private ConversionProfile CreatePngProfile()
        {
            var pngProfile = new ConversionProfile();
            pngProfile.Name = "PNG (graphic file)";
            pngProfile.Guid = ProfileGuids.PNG_PROFILE_GUID;

            pngProfile.OutputFormat = OutputFormat.Png;
            pngProfile.PngSettings.Dpi = 150;
            pngProfile.PngSettings.Color = PngColor.Color24Bit;

            SetDefaultProperties(pngProfile, true);
            return pngProfile;
        }

        private ConversionProfile CreatePdfaProfile()
        {
            var pdfaProfile = new ConversionProfile();
            pdfaProfile.Name = "PDF/A (long term preservation)";
            pdfaProfile.Guid = ProfileGuids.PDFA_PROFILE_GUID;

            pdfaProfile.OutputFormat = OutputFormat.PdfA2B;
            pdfaProfile.PdfSettings.CompressColorAndGray.Enabled = true;
            pdfaProfile.PdfSettings.CompressColorAndGray.Compression = CompressionColorAndGray.Automatic;
            pdfaProfile.PdfSettings.CompressMonochrome.Enabled = true;
            pdfaProfile.PdfSettings.CompressMonochrome.Compression = CompressionMonochrome.CcittFaxEncoding;

            SetDefaultProperties(pdfaProfile, true);
            return pdfaProfile;
        }

        private ConversionProfile CreateJpegProfile()
        {
            var jpegProfile = new ConversionProfile();
            jpegProfile.Name = "JPEG (graphic file)";
            jpegProfile.Guid = ProfileGuids.JPEG_PROFILE_GUID;

            jpegProfile.OutputFormat = OutputFormat.Jpeg;
            jpegProfile.JpegSettings.Dpi = 150;
            jpegProfile.JpegSettings.Color = JpegColor.Color24Bit;
            jpegProfile.JpegSettings.Quality = 75;

            SetDefaultProperties(jpegProfile, true);
            return jpegProfile;
        }

        private ConversionProfile CreateHighQualityProfile()
        {
            var highQualityProfile = new ConversionProfile();
            highQualityProfile.Name = "High Quality (large files)";
            highQualityProfile.Guid = ProfileGuids.HIGH_QUALITY_PROFILE_GUID;

            highQualityProfile.OutputFormat = OutputFormat.Pdf;
            highQualityProfile.PdfSettings.CompressColorAndGray.Enabled = true;
            highQualityProfile.PdfSettings.CompressColorAndGray.Compression = CompressionColorAndGray.Zip;
            highQualityProfile.PdfSettings.CompressMonochrome.Enabled = true;
            highQualityProfile.PdfSettings.CompressMonochrome.Compression = CompressionMonochrome.Zip;

            highQualityProfile.JpegSettings.Dpi = 300;
            highQualityProfile.JpegSettings.Quality = 100;
            highQualityProfile.JpegSettings.Color = JpegColor.Color24Bit;

            highQualityProfile.PngSettings.Dpi = 300;
            highQualityProfile.PngSettings.Color = PngColor.Color32BitTransp;

            highQualityProfile.TiffSettings.Dpi = 300;
            highQualityProfile.TiffSettings.Color = TiffColor.Color24Bit;

            SetDefaultProperties(highQualityProfile, true);
            return highQualityProfile;
        }

        private ConversionProfile CreateHighCompressionProfile()
        {
            var highCompressionProfile = new ConversionProfile();
            highCompressionProfile.Name = "High Compression (small files)";
            highCompressionProfile.Guid = ProfileGuids.HIGH_COMPRESSION_PROFILE_GUID;

            highCompressionProfile.OutputFormat = OutputFormat.Pdf;
            highCompressionProfile.PdfSettings.CompressColorAndGray.Enabled = true;
            highCompressionProfile.PdfSettings.CompressColorAndGray.Compression = CompressionColorAndGray.JpegMaximum;

            highCompressionProfile.PdfSettings.CompressMonochrome.Enabled = true;
            highCompressionProfile.PdfSettings.CompressMonochrome.Compression = CompressionMonochrome.RunLengthEncoding;

            highCompressionProfile.JpegSettings.Dpi = 100;
            highCompressionProfile.JpegSettings.Color = JpegColor.Color24Bit;
            highCompressionProfile.JpegSettings.Quality = 50;

            highCompressionProfile.PngSettings.Dpi = 100;
            highCompressionProfile.PngSettings.Color = PngColor.Color24Bit;

            highCompressionProfile.TiffSettings.Dpi = 100;
            highCompressionProfile.TiffSettings.Color = TiffColor.Color24Bit;

            SetDefaultProperties(highCompressionProfile, true);
            return highCompressionProfile;
        }

        private void SetDefaultProperties(ConversionProfile profile, bool isDeletable)
        {
            profile.Properties.Renamable = false;
            profile.Properties.Deletable = isDeletable;
            profile.Properties.Editable = true;
        }
    }
}