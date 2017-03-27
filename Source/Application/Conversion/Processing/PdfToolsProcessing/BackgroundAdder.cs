using System;
using System.IO;
using NLog;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Conversion.Settings;
using PdfTools;
using PdfTools.Pdf;
using ErrorCode = pdfforge.PDFCreator.Conversion.Jobs.ErrorCode;

namespace pdfforge.PDFCreator.Conversion.Processing.PdfToolsProcessing
{
    internal class BackgroundAdder
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IPdfProcessor _pdfProcessor;

        public BackgroundAdder(IPdfProcessor pdfProcessor)
        {
            _pdfProcessor = pdfProcessor;
        }

        internal void AddBackground(string pdfFile, ConversionProfile conversionProfile)
        {
            if (!conversionProfile.BackgroundPage.Enabled)
                return;

            var tmpFile = _pdfProcessor.MoveFileToPreProcessFile(pdfFile, "PreBackground");

            try
            {
                DoAddBackground(pdfFile, tmpFile, conversionProfile);
            }
            catch (ProcessingException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ProcessingException(ex.GetType() + " while addding background:" + Environment.NewLine + ex.Message, ErrorCode.Background_GenericError);
            }
            finally
            {
                //delete copy of original file
                if (!string.IsNullOrEmpty(tmpFile))
                    if (File.Exists(tmpFile))
                        File.Delete(tmpFile);
            }
        }

        private void DoAddBackground(string pdfFile, string tmpFile,  ConversionProfile profile)
        {
            _logger.Debug("Start adding background.");

            var copyOptions = CopyOption.CopyLinks | CopyOption.CopyAnnotations | CopyOption.CopyFormFields | CopyOption.CopyOutlines | CopyOption.CopyLogicalStructure | CopyOption.CopyNamedDestinations | CopyOption.FlattenAnnotations | CopyOption.FlattenFormFields | CopyOption.FlattenSignatureAppearances | CopyOption.OptimizeResources;

            var conformance = DeterminePdfVersionConformance(profile);

            using (Stream backgroundStream = new FileStream(profile.BackgroundPage.File, FileMode.Open, FileAccess.Read))
            using (var backgroundDoc = Document.Open(backgroundStream, null))
            using (Stream docStream = new FileStream(tmpFile, FileMode.Open, FileAccess.Read))
            using (var doc = Document.Open(docStream, ""))
            using (Stream outStream = new FileStream(pdfFile, FileMode.Create, FileAccess.ReadWrite))
            using (var outDoc = Document.Create(outStream, conformance, new EncryptionParams()))
            {
                CopyMetadata(outDoc, doc.Metadata);

                var coverPages = GetNumberOfCoverPages(profile.CoverPage);
                var attachmentPages = GetNumberOfAttachmentPages(profile.AttachmentPage);

                var backgroundPageMapper = new BackgroundPageMapper(backgroundDoc.Pages.Count, profile.BackgroundPage, coverPages, attachmentPages);

                for (int i=0; i< doc.Pages.Count; i++)
                {
                    var page = doc.Pages[i];
                    var pageCopy = outDoc.CopyPage(page, copyOptions);

                    int backgroundPageNumber;
                    if (backgroundPageMapper.GetBackgroundPageNumber(i + 1, doc.Pages.Count, out backgroundPageNumber))
                    {
                        using (var gen = new ContentGenerator(pageCopy.Content, true))
                        {
                            var backgroundPage = backgroundDoc.Pages[backgroundPageNumber - 1];
                            var background = outDoc.CopyPageAsForm(backgroundPage, copyOptions);
                            var scaleWidth = pageCopy.Size.Width / backgroundPage.Size.Width;
                            var scaleHeight = pageCopy.Size.Height / backgroundPage.Size.Height;
                            var scale = scaleWidth < scaleHeight ? scaleWidth : scaleHeight;

                            var backgroundHeight = backgroundPage.Size.Height*scale;
                            var backgroundWidth = backgroundPage.Size.Width*scale;

                            var offsetHeight = (pageCopy.Size.Height - backgroundHeight)/2;
                            var offsetWidth = (pageCopy.Size.Width - backgroundWidth)/2;

                            var backgroundRect = new Rectangle()
                            {
                                Top = backgroundHeight + offsetHeight,
                                Bottom = offsetHeight,
                                Right = backgroundWidth + offsetWidth,
                                Left = offsetWidth,
                            };
                            
                            gen.PaintForm(background, backgroundRect);
                        }
                    }

                    outDoc.Pages.Add(pageCopy);
                }
            }
        }

        private Conformance DeterminePdfVersionConformance(ConversionProfile profile)
        {
            var pdfVersion = _pdfProcessor.DeterminePdfVersion(profile);
            if (pdfVersion == "1.7")
                return Conformance.Pdf17;
            if (pdfVersion == "1.6")
                return Conformance.Pdf16;
            if (pdfVersion == "1.5")
                return Conformance.Pdf15;

            return Conformance.Pdf14;
        }

        private void CopyMetadata(Document document, Metadata metadata)
        {
            document.Metadata.Author = metadata.Author;
            document.Metadata.CreationDate = metadata.CreationDate;
            document.Metadata.Creator = metadata.Creator;
            document.Metadata.Keywords = metadata.Keywords;
            document.Metadata.Producer = metadata.Producer;
            document.Metadata.Subject = metadata.Subject;
            document.Metadata.Title = metadata.Title;
        }

        private int GetNumberOfCoverPages(CoverPage coverPageSettings)
        {
            if (!coverPageSettings.Enabled)
                return 0;

            return _pdfProcessor.GetNumberOfPages(coverPageSettings.File);
        }

        private int GetNumberOfAttachmentPages(AttachmentPage attachmentPageSettings)
        {
            if (!attachmentPageSettings.Enabled)
                return 0;

            return _pdfProcessor.GetNumberOfPages(attachmentPageSettings.File);
        }
    }
}
