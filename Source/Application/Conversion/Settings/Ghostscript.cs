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
	/// Ghostscript settings
	/// </summary>
	public class Ghostscript {
		
		/// <summary>
		/// These parameters will be provided to Ghostscript in addition to the PDFCreator parameters
		/// </summary>
		public string AdditionalGsParameters { get; set; }
		
		
		private void Init() {
			AdditionalGsParameters = "";
		}
		
		public Ghostscript()
		{
			Init();
		}
		
		public void ReadValues(Data data, string path)
		{
			try { AdditionalGsParameters = Data.UnescapeString(data.GetValue(@"" + path + @"AdditionalGsParameters")); } catch { AdditionalGsParameters = "";}
		}
		
		public void StoreValues(Data data, string path)
		{
			data.SetValue(@"" + path + @"AdditionalGsParameters", Data.EscapeString(AdditionalGsParameters));
		}
		
		public Ghostscript Copy()
		{
			Ghostscript copy = new Ghostscript();
			
			copy.AdditionalGsParameters = AdditionalGsParameters;
			
			return copy;
		}
		
		public override bool Equals(object o)
		{
			if (!(o is Ghostscript)) return false;
			Ghostscript v = o as Ghostscript;
			
			if (!AdditionalGsParameters.Equals(v.AdditionalGsParameters)) return false;
			
			return true;
		}
		
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			
			sb.AppendLine("AdditionalGsParameters=" + AdditionalGsParameters.ToString());
			
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
