using System.Windows;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.TabHelper
{
    public class MasterIconTabHeader : FrameworkContentElement
    {
        public MasterIconTabHeader(string regionName)
        {
            RegionName = regionName;
        }

        public string RegionName { get; }
    }
}
