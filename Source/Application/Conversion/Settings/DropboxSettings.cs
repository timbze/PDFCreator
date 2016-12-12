using pdfforge.DataStorage.Storage;
using pdfforge.DataStorage;
using System.Collections.Generic;
using System.Text;
using System;

// ! This file is generated automatically.
// ! Do not edit it outside the sections for custom code.
// ! These changes will be deleted during the next generation run

namespace pdfforge.PDFCreator.Conversion.Settings
{
	/// <summary>
	/// Dropbox settings for currently logged user
	/// </summary>
	public class DropboxSettings {
		
		public string AccountId { get; set; }
		
		public bool CreateShareLink { get; set; }
		
		public bool Enabled { get; set; }
		
		/// <summary>
		/// If true, files with the same name will not be overwritten on the server. A counter will be appended instead (i.e. document_2.pdf)
		/// </summary>
		public bool EnsureUniqueFilenames { get; set; }
		
		public string SharedFolder { get; set; }
		
		
		private void Init() {
			AccountId = "";
			CreateShareLink = false;
			Enabled = false;
			EnsureUniqueFilenames = false;
			SharedFolder = "PDFCreator";
		}
		
		public DropboxSettings()
		{
			Init();
		}
		
		public void ReadValues(Data data, string path)
		{
			try { AccountId = Data.UnescapeString(data.GetValue(@"" + path + @"AccountId")); } catch { AccountId = "";}
			try { CreateShareLink = bool.Parse(data.GetValue(@"" + path + @"CreateShareLink")); } catch { CreateShareLink = false;}
			try { Enabled = bool.Parse(data.GetValue(@"" + path + @"Enabled")); } catch { Enabled = false;}
			try { EnsureUniqueFilenames = bool.Parse(data.GetValue(@"" + path + @"EnsureUniqueFilenames")); } catch { EnsureUniqueFilenames = false;}
			try { SharedFolder = Data.UnescapeString(data.GetValue(@"" + path + @"SharedFolder")); } catch { SharedFolder = "PDFCreator";}
		}
		
		public void StoreValues(Data data, string path)
		{
			data.SetValue(@"" + path + @"AccountId", Data.EscapeString(AccountId));
			data.SetValue(@"" + path + @"CreateShareLink", CreateShareLink.ToString());
			data.SetValue(@"" + path + @"Enabled", Enabled.ToString());
			data.SetValue(@"" + path + @"EnsureUniqueFilenames", EnsureUniqueFilenames.ToString());
			data.SetValue(@"" + path + @"SharedFolder", Data.EscapeString(SharedFolder));
		}
		
		public DropboxSettings Copy()
		{
			DropboxSettings copy = new DropboxSettings();
			
			copy.AccountId = AccountId;
			copy.CreateShareLink = CreateShareLink;
			copy.Enabled = Enabled;
			copy.EnsureUniqueFilenames = EnsureUniqueFilenames;
			copy.SharedFolder = SharedFolder;
			
			return copy;
		}
		
		public override bool Equals(object o)
		{
			if (!(o is DropboxSettings)) return false;
			DropboxSettings v = o as DropboxSettings;
			
			if (!AccountId.Equals(v.AccountId)) return false;
			if (!CreateShareLink.Equals(v.CreateShareLink)) return false;
			if (!Enabled.Equals(v.Enabled)) return false;
			if (!EnsureUniqueFilenames.Equals(v.EnsureUniqueFilenames)) return false;
			if (!SharedFolder.Equals(v.SharedFolder)) return false;
			
			return true;
		}
		
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			
			sb.AppendLine("AccountId=" + AccountId.ToString());
			sb.AppendLine("CreateShareLink=" + CreateShareLink.ToString());
			sb.AppendLine("Enabled=" + Enabled.ToString());
			sb.AppendLine("EnsureUniqueFilenames=" + EnsureUniqueFilenames.ToString());
			sb.AppendLine("SharedFolder=" + SharedFolder.ToString());
			
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
