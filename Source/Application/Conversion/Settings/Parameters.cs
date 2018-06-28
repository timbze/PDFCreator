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
	[ImplementPropertyChanged]
	public partial class Parameters : INotifyPropertyChanged {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		
		/// <summary>
		/// The outputfile path
		/// </summary>
		public string Outputfile { get; set; } = "";
		
		/// <summary>
		/// Parameter for profile
		/// </summary>
		public string Profile { get; set; } = "";
		
		
		public void ReadValues(Data data, string path)
		{
			try { Outputfile = Data.UnescapeString(data.GetValue(@"" + path + @"Outputfile")); } catch { Outputfile = "";}
			try { Profile = Data.UnescapeString(data.GetValue(@"" + path + @"Profile")); } catch { Profile = "";}
		}
		
		public void StoreValues(Data data, string path)
		{
			data.SetValue(@"" + path + @"Outputfile", Data.EscapeString(Outputfile));
			data.SetValue(@"" + path + @"Profile", Data.EscapeString(Profile));
		}
		
		public Parameters Copy()
		{
			Parameters copy = new Parameters();
			
			copy.Outputfile = Outputfile;
			copy.Profile = Profile;
			
			return copy;
		}
		
		public override bool Equals(object o)
		{
			if (!(o is Parameters)) return false;
			Parameters v = o as Parameters;
			
			if (!Outputfile.Equals(v.Outputfile)) return false;
			if (!Profile.Equals(v.Profile)) return false;
			
			return true;
		}
		
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			
			sb.AppendLine("Outputfile=" + Outputfile.ToString());
			sb.AppendLine("Profile=" + Profile.ToString());
			
			return sb.ToString();
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
	}
}
