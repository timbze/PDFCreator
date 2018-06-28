using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.Conversion.Settings
{
    public partial class ConversionProfile
    {
        public bool IsDefault
        {
            get { return Guid == ProfileGuids.DEFAULT_PROFILE_GUID; }
        }
    }
}
