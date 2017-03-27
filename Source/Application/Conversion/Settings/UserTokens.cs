using pdfforge.DataStorage;
using System.Text;

// ! This file is generated automatically.
// ! Do not edit it outside the sections for custom code.
// ! These changes will be deleted during the next generation run

namespace pdfforge.PDFCreator.Conversion.Settings
{
	/// <summary>
	/// Parse ps files for user definied tokens
	/// </summary>
	public class UserTokens {
		
		/// <summary>
		/// Activate parsing ps files for user tokens (Only available in PDFCreator Business)
		/// </summary>
		public bool Enabled { get; set; }
		
		
		private void Init() {
			Enabled = false;
		}
		
		public UserTokens()
		{
			Init();
		}
		
		public void ReadValues(Data data, string path)
		{
			try { Enabled = bool.Parse(data.GetValue(@"" + path + @"Enabled")); } catch { Enabled = false;}
		}
		
		public void StoreValues(Data data, string path)
		{
			data.SetValue(@"" + path + @"Enabled", Enabled.ToString());
		}
		
		public UserTokens Copy()
		{
			UserTokens copy = new UserTokens();
			
			copy.Enabled = Enabled;
			
			return copy;
		}
		
		public override bool Equals(object o)
		{
			if (!(o is UserTokens)) return false;
			UserTokens v = o as UserTokens;
			
			if (!Enabled.Equals(v.Enabled)) return false;
			
			return true;
		}
		
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			
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
