using System;
using pdfforge.PDFCreator.Editions.EditionBase;

namespace pdfforge.PDFCreator.Editions.PDFCreatorPlus
{
    internal static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            ProgramBase.Main(args, () => new PDFCreatorPlusBootstrapper());
        }
    }
}
