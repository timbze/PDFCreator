using pdfforge.DataStorage;
using System.Text;

// ! This file is generated automatically.
// ! Do not edit it outside the sections for custom code.
// ! These changes will be deleted during the next generation run

namespace pdfforge.PDFCreator.Conversion.Settings
{
	public class TextSettings {
		
		/// <summary>
		/// Text Format (0 outputs XML-escaped Unicode along with information regarding the format of the text | 1  same XML output format, but attempts similar processing to MuPDF, and will output blocks of text | 2 outputs Unicode (UCS2) text (with a Byte Order Mark) which approximates the original text layout | 3 same as 2 encoded in UTF-8)
		/// </summary>
		public int Format { get; set; }
		
		
		private void Init() {
			Format = 2;
		}
		
		public TextSettings()
		{
			Init();
		}
		
		public void ReadValues(Data data, string path)
		{
			try { Format = int.Parse(data.GetValue(@"" + path + @"Format"), System.Globalization.CultureInfo.InvariantCulture); } catch { Format = 2;}
		}
		
		public void StoreValues(Data data, string path)
		{
			data.SetValue(@"" + path + @"Format", Format.ToString(System.Globalization.CultureInfo.InvariantCulture));
		}
		
		public TextSettings Copy()
		{
			TextSettings copy = new TextSettings();
			
			copy.Format = Format;
			
			return copy;
		}
		
		public override bool Equals(object o)
		{
			if (!(o is TextSettings)) return false;
			TextSettings v = o as TextSettings;
			
			if (!Format.Equals(v.Format)) return false;
			
			return true;
		}
		
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			
			sb.AppendLine("Format=" + Format.ToString());
			
			return sb.ToString();
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
// Custom Code starts here
// START_CUSTOM_SECTION:GENERAL
// END_CUSTOM_SECTION:GENERAL
// Custom Code ends here. Do not edit below
		
	}
}
