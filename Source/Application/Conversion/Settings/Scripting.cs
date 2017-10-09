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
	/// The scripting action allows to run a script after the conversion
	/// </summary>
	[ImplementPropertyChanged]
	public partial class Scripting : INotifyPropertyChanged {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		
		/// <summary>
		/// If true, the given script or application will be executed
		/// </summary>
		public bool Enabled { get; set; } = false;
		
		/// <summary>
		/// Parameters that will be passed to the script in addition to the output files
		/// </summary>
		public string ParameterString { get; set; } = "";
		
		/// <summary>
		/// Path to the script or application
		/// </summary>
		public string ScriptFile { get; set; } = "";
		
		/// <summary>
		/// Wait until the script excution has ended
		/// </summary>
		public bool WaitForScript { get; set; } = true;
		
		
		public void ReadValues(Data data, string path)
		{
			try { Enabled = bool.Parse(data.GetValue(@"" + path + @"Enabled")); } catch { Enabled = false;}
			try { ParameterString = Data.UnescapeString(data.GetValue(@"" + path + @"ParameterString")); } catch { ParameterString = "";}
			try { ScriptFile = Data.UnescapeString(data.GetValue(@"" + path + @"ScriptFile")); } catch { ScriptFile = "";}
			try { WaitForScript = bool.Parse(data.GetValue(@"" + path + @"WaitForScript")); } catch { WaitForScript = true;}
		}
		
		public void StoreValues(Data data, string path)
		{
			data.SetValue(@"" + path + @"Enabled", Enabled.ToString());
			data.SetValue(@"" + path + @"ParameterString", Data.EscapeString(ParameterString));
			data.SetValue(@"" + path + @"ScriptFile", Data.EscapeString(ScriptFile));
			data.SetValue(@"" + path + @"WaitForScript", WaitForScript.ToString());
		}
		
		public Scripting Copy()
		{
			Scripting copy = new Scripting();
			
			copy.Enabled = Enabled;
			copy.ParameterString = ParameterString;
			copy.ScriptFile = ScriptFile;
			copy.WaitForScript = WaitForScript;
			
			return copy;
		}
		
		public override bool Equals(object o)
		{
			if (!(o is Scripting)) return false;
			Scripting v = o as Scripting;
			
			if (!Enabled.Equals(v.Enabled)) return false;
			if (!ParameterString.Equals(v.ParameterString)) return false;
			if (!ScriptFile.Equals(v.ScriptFile)) return false;
			if (!WaitForScript.Equals(v.WaitForScript)) return false;
			
			return true;
		}
		
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			
			sb.AppendLine("Enabled=" + Enabled.ToString());
			sb.AppendLine("ParameterString=" + ParameterString.ToString());
			sb.AppendLine("ScriptFile=" + ScriptFile.ToString());
			sb.AppendLine("WaitForScript=" + WaitForScript.ToString());
			
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
