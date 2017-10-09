using pdfforge.PDFCreator.Editions.EditionBase;
using System;

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
