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
	/// Action to upload files to a HTTP server
	/// </summary>
	[ImplementPropertyChanged]
	public partial class HttpSettings : INotifyPropertyChanged {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		
		public string AccountId { get; set; } = "";
		
		/// <summary>
		/// If true, this action will be executed
		/// </summary>
		public bool Enabled { get; set; } = false;
		
		
		public void ReadValues(Data data, string path)
		{
			try { AccountId = Data.UnescapeString(data.GetValue(@"" + path + @"AccountId")); } catch { AccountId = "";}
			try { Enabled = bool.Parse(data.GetValue(@"" + path + @"Enabled")); } catch { Enabled = false;}
		}
		
		public void StoreValues(Data data, string path)
		{
			data.SetValue(@"" + path + @"AccountId", Data.EscapeString(AccountId));
			data.SetValue(@"" + path + @"Enabled", Enabled.ToString());
		}
		
		public HttpSettings Copy()
		{
			HttpSettings copy = new HttpSettings();
			
			copy.AccountId = AccountId;
			copy.Enabled = Enabled;
			
			return copy;
		}
		
		public override bool Equals(object o)
		{
			if (!(o is HttpSettings)) return false;
			HttpSettings v = o as HttpSettings;
			
			if (!AccountId.Equals(v.AccountId)) return false;
			if (!Enabled.Equals(v.Enabled)) return false;
			
			return true;
		}
		
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			
			sb.AppendLine("AccountId=" + AccountId.ToString());
			sb.AppendLine("Enabled=" + Enabled.ToString());
			
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
