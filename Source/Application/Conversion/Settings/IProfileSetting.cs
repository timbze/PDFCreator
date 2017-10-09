using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.Conversion.Settings
{
    public interface IProfileSetting
    {
        bool Enabled { get; set; }
    }
}
