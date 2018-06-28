using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.IntegrationTest.Conversion.Jobs
{
    internal class ImageExtractor : IRenderListener
    {
        private int _currentPage = 1;

        private List<ImageData> _imagesSizes = new List<ImageData>();

        public IList<ImageData> ExtractImagesSizes(string pdfPath)
        {
            _imagesSizes = new List<ImageData>();

            using (var pdfReader = new PdfReader(pdfPath))
            {
                if (pdfReader.IsEncrypted())
                    throw new ApplicationException(pdfPath + " is encrypted.");

                var pdfParser = new PdfReaderContentParser(pdfReader);

                while (_currentPage <= pdfReader.NumberOfPages)
                {
                    pdfParser.ProcessContent(_currentPage, this);

                    _currentPage++;
                }
            }

            // we extracted them in reverse order previously
            _imagesSizes.Reverse();
            return _imagesSizes;
        }

        public void BeginTextBlock()
        {
        }

        public void EndTextBlock()
        {
        }

        public void RenderText(TextRenderInfo renderInfo)
        {
        }

        public void RenderImage(ImageRenderInfo renderInfo)
        {
            var imageObject = renderInfo.GetImage();
            var filter = imageObject.Get(PdfName.FILTER);
            var height = (PdfNumber)imageObject.Get(PdfName.HEIGHT);
            var width = (PdfNumber)imageObject.Get(PdfName.WIDTH);
            var imageRawBytes = imageObject.GetImageAsBytes();

            var byteSize = imageRawBytes.Length;
            _imagesSizes.Add(new ImageData
            {
                FileSize = byteSize,
                Filter = filter?.ToString(),
                Height = height.IntValue,
                Width = width.IntValue
            });
        }
    }
}
