using Prism;
using System;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.Controls.Tab
{
    public class SubTabItem : TabItem, IActiveAware
    {
        private bool _isActive;

        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                _isActive = value;
                IsActiveChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler IsActiveChanged;
    }
}
