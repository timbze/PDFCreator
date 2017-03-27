using pdfforge.DataStorage;
using System.Text;
using System;

// ! This file is generated automatically.
// ! Do not edit it outside the sections for custom code.
// ! These changes will be deleted during the next generation run

namespace pdfforge.PDFCreator.Conversion.Settings
{
	public class ApplicationProperties {
		
		public DateTime NextUpdate { get; set; }
		
		/// <summary>
		/// Version of the settings classes. This is used for internal purposes, i.e. to match properties when they were renamed
		/// </summary>
		public int SettingsVersion { get; set; }
		
		
		private void Init() {
			NextUpdate = DateTime.Now;
			SettingsVersion = 6;
		}
		
		public ApplicationProperties()
		{
			Init();
		}
		
		public void ReadValues(Data data, string path)
		{
			try { NextUpdate = DateTime.Parse(data.GetValue(@"" + path + @"NextUpdate"), System.Globalization.CultureInfo.InvariantCulture); } catch { NextUpdate = DateTime.Now;}
			try { SettingsVersion = int.Parse(data.GetValue(@"" + path + @"SettingsVersion"), System.Globalization.CultureInfo.InvariantCulture); } catch { SettingsVersion = 6;}
		}
		
		public void StoreValues(Data data, string path)
		{
			data.SetValue(@"" + path + @"NextUpdate", NextUpdate.ToString("yyyy-MM-dd HH:mm:ss"));
			data.SetValue(@"" + path + @"SettingsVersion", SettingsVersion.ToString(System.Globalization.CultureInfo.InvariantCulture));
		}
		
		public ApplicationProperties Copy()
		{
			ApplicationProperties copy = new ApplicationProperties();
			
			copy.NextUpdate = NextUpdate;
			copy.SettingsVersion = SettingsVersion;
			
			return copy;
		}
		
		public override bool Equals(object o)
		{
			if (!(o is ApplicationProperties)) return false;
			ApplicationProperties v = o as ApplicationProperties;
			
			if (!NextUpdate.Equals(v.NextUpdate)) return false;
			if (!SettingsVersion.Equals(v.SettingsVersion)) return false;
			
			return true;
		}
		
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			
			sb.AppendLine("NextUpdate=" + NextUpdate.ToString());
			sb.AppendLine("SettingsVersion=" + SettingsVersion.ToString());
			
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
