using System.Collections.ObjectModel;
using System.IO;
using pdfforge.PDFCreator.Conversion.Settings.Enums;

namespace pdfforge.PDFCreator.Conversion.Settings
{
    public partial class ApplicationSettings
    {

        public DefaultViewer GetDefaultViewerByOutputFormat(OutputFormat format)
        {
            //Same default viewer for all pdf types
            if (format == OutputFormat.PdfA1B || format == OutputFormat.PdfA2B || format == OutputFormat.PdfX)
                format = OutputFormat.Pdf;

            foreach (var defaultViewer in DefaultViewers)
            {
                if (defaultViewer.OutputFormat == format)
                {
                    return defaultViewer;
                }
            }

            var defaultViewerByOutputFormat = new DefaultViewer();
            defaultViewerByOutputFormat.IsActive = false;
            defaultViewerByOutputFormat.OutputFormat = format;
            defaultViewerByOutputFormat.Path = "";
            defaultViewerByOutputFormat.Parameters = "";
            DefaultViewers.Add(defaultViewerByOutputFormat);

            return defaultViewerByOutputFormat;
        }
        
        public ObservableCollection<DefaultViewer> DefaultViewerList()
        {
            var list = new ObservableCollection<DefaultViewer>
            {
                GetDefaultViewerByOutputFormat(OutputFormat.Pdf),
                GetDefaultViewerByOutputFormat(OutputFormat.Png),
                GetDefaultViewerByOutputFormat(OutputFormat.Jpeg),
                GetDefaultViewerByOutputFormat(OutputFormat.Tif),
                GetDefaultViewerByOutputFormat(OutputFormat.Txt)
            };
            return list;
        }
    }
}

