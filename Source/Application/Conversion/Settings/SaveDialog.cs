using pdfforge.DataStorage.Storage;
using pdfforge.DataStorage;
using PropertyChanged;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System;

// ! This file is generated automatically.
// ! Do not edit it outside the sections for custom code.
// ! These changes will be deleted during the next generation run

namespace pdfforge.PDFCreator.Conversion.Settings
{
	/// <summary>
	/// Settings to control the behaviour of the save dialog
	/// </summary>
	[ImplementPropertyChanged]
	public partial class SaveDialog : INotifyPropertyChanged {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		
		/// <summary>
		/// If true, the save dialog will open in the given folder instead of the last used folder.
		/// </summary>
		public bool SetDirectory { get; set; } = false;
		
		
		public void ReadValues(Data data, string path)
		{
			try { SetDirectory = bool.Parse(data.GetValue(@"" + path + @"SetDirectory")); } catch { SetDirectory = false;}
		}
		
		public void StoreValues(Data data, string path)
		{
			data.SetValue(@"" + path + @"SetDirectory", SetDirectory.ToString());
		}
		
		public SaveDialog Copy()
		{
			SaveDialog copy = new SaveDialog();
			
			copy.SetDirectory = SetDirectory;
			
			return copy;
		}
		
		public override bool Equals(object o)
		{
			if (!(o is SaveDialog)) return false;
			SaveDialog v = o as SaveDialog;
			
			if (!SetDirectory.Equals(v.SetDirectory)) return false;
			
			return true;
		}
		
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			
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
