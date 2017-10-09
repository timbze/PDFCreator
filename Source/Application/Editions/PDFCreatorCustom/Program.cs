using pdfforge.PDFCreator.Editions.EditionBase;
using System;

namespace pdfforge.PDFCreator.Editions.PDFCreatorCustom
{
    internal static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            ProgramBase.Main(args, () => new PDFCreatorCustomBootstrapper());
        }
    }
}
