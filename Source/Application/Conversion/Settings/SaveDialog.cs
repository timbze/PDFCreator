using pdfforge.DataStorage;
using System.Text;

// ! This file is generated automatically.
// ! Do not edit it outside the sections for custom code.
// ! These changes will be deleted during the next generation run

namespace pdfforge.PDFCreator.Conversion.Settings
{
	/// <summary>
	/// Settings to control the behaviour of the save dialog
	/// </summary>
	public class SaveDialog {
		
		/// <summary>
		/// Folder in which the save dialog will be opened (if SetDirectory is true)
		/// </summary>
		public string Folder { get; set; }
		
		/// <summary>
		/// If true, the save dialog will open in the given folder instead of the last used folder.
		/// </summary>
		public bool SetDirectory { get; set; }
		
		
		private void Init() {
			Folder = "";
			SetDirectory = false;
		}
		
		public SaveDialog()
		{
			Init();
		}
		
		public void ReadValues(Data data, string path)
		{
			try { Folder = Data.UnescapeString(data.GetValue(@"" + path + @"Folder")); } catch { Folder = "";}
			try { SetDirectory = bool.Parse(data.GetValue(@"" + path + @"SetDirectory")); } catch { SetDirectory = false;}
		}
		
		public void StoreValues(Data data, string path)
		{
			data.SetValue(@"" + path + @"Folder", Data.EscapeString(Folder));
			data.SetValue(@"" + path + @"SetDirectory", SetDirectory.ToString());
		}
		
		public SaveDialog Copy()
		{
			SaveDialog copy = new SaveDialog();
			
			copy.Folder = Folder;
			copy.SetDirectory = SetDirectory;
			
			return copy;
		}
		
		public override bool Equals(object o)
		{
			if (!(o is SaveDialog)) return false;
			SaveDialog v = o as SaveDialog;
			
			if (!Folder.Equals(v.Folder)) return false;
			if (!SetDirectory.Equals(v.SetDirectory)) return false;
			
			return true;
		}
		
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			
			sb.AppendLine("Folder=" + Folder.ToString());
			sb.AppendLine("SetDirectory=" + SetDirectory.ToString());
			
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
		
// Custom Code starts here
// START_CUSTOM_SECTION:INCLUDES
// END_CUSTOM_SECTION:INCLUDES
// Custom Code ends here. Do not edit below
		
	}
}
