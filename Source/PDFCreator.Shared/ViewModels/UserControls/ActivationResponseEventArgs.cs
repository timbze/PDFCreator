using System;
using pdfforge.LicenseValidator;
using pdfforge.PDFCreator.Shared.Licensing;

namespace pdfforge.PDFCreator.Shared.ViewModels.UserControls
{
    public class ActivationResponseEventArgs : EventArgs
    {
        public ActivationResponseEventArgs(Edition edition)
        {
            Edition = edition;
        }

        public Edition Edition { get; }
    }
}