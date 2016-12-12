using System.Collections.Generic;
using SystemInterface.IO;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.Conversion.Ghostscript.OutputDevices
{
    /// <summary>
    ///     Extends OutputDevice to create PNG files
    /// </summary>
    public class JpegDevice : OutputDevice
    {
        public JpegDevice(Job job) : base(job)
        {
        }

        public JpegDevice(Job job, IFile file, IOsHelper osHelper, ICommandLineUtil commandLineUtil) : base(job, file, osHelper, commandLineUtil)
        {
        }

        protected override void AddDeviceSpecificParameters(IList<string> parameters)
        {
            switch (Job.Profile.JpegSettings.Color)
            {
                case JpegColor.Gray8Bit:
                    parameters.Add("-sDEVICE=jpeggray");
                    break;
                default:
                    parameters.Add("-sDEVICE=jpeg");
                    break;
            }
            parameters.Add("-dJPEGQ=" + Job.Profile.JpegSettings.Quality);
            parameters.Add("-r" + Job.Profile.JpegSettings.Dpi);
            parameters.Add("-dTextAlphaBits=4");
            parameters.Add("-dGraphicsAlphaBits=4");
        }

        protected override string ComposeOutputFilename()
        {
            //%d for multiple Pages
            return Job.JobTempFileName + "%d.jpg";
        }
    }
}