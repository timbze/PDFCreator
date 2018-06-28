namespace pdfforge.PDFCreator.Core.DirectConversion
{
    public interface IDirectConversion
    {
        /// <summary>
        ///     Create unique job folder in spool folder and copy ps file to it.
        ///     Create inf file from ps file.
        /// </summary>
        /// <returns>inf file in spool folder</returns>
        string TransformToInfFile(string file, string printerName = "PDFCreator", string profileParameter = "", string outputFileParameter = "");
    }
}
