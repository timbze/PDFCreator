using System;
using pdfforge.PDFCreator.Editions.EditionBase;

namespace pdfforge.PDFCreator.Editions.PDFCreatorBusiness
{
    internal static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            ProgramBase.Main(args, () => new PDFCreatorBusinessBootstrapper());
        }
    }
}
