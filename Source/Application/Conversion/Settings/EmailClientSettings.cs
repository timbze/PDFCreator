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
	/// Opens the default E-mail client with the converted document as attachment
	/// </summary>
	public class EmailClientSettings {
		
		/// <summary>
		/// Add the PDFCreator signature to the mail
		/// </summary>
		public bool AddSignature { get; set; }
		
		/// <summary>
		/// Body text of the E-mail
		/// </summary>
		public string Content { get; set; }
		
		/// <summary>
		/// Enables the EmailClient action
		/// </summary>
		public bool Enabled { get; set; }
		
		/// <summary>
		/// The list of receipients of the E-mail, i.e. info@someone.com; me@mywebsite.org
		/// </summary>
		public string Recipients { get; set; }
		
		/// <summary>
		/// Subject line of the E-mail
		/// </summary>
		public string Subject { get; set; }
		
		
		private void Init() {
			AddSignature = true;
			Content = "";
			Enabled = false;
			Recipients = "";
			Subject = "";
		}
		
		public EmailClientSettings()
		{
			Init();
		}
		
		public void ReadValues(Data data, string path)
		{
			try { AddSignature = bool.Parse(data.GetValue(@"" + path + @"AddSignature")); } catch { AddSignature = true;}
			try { Content = Data.UnescapeString(data.GetValue(@"" + path + @"Content")); } catch { Content = "";}
			try { Enabled = bool.Parse(data.GetValue(@"" + path + @"Enabled")); } catch { Enabled = false;}
			try { Recipients = Data.UnescapeString(data.GetValue(@"" + path + @"Recipients")); } catch { Recipients = "";}
			try { Subject = Data.UnescapeString(data.GetValue(@"" + path + @"Subject")); } catch { Subject = "";}
		}
		
		public void StoreValues(Data data, string path)
		{
			data.SetValue(@"" + path + @"AddSignature", AddSignature.ToString());
			data.SetValue(@"" + path + @"Content", Data.EscapeString(Content));
			data.SetValue(@"" + path + @"Enabled", Enabled.ToString());
			data.SetValue(@"" + path + @"Recipients", Data.EscapeString(Recipients));
			data.SetValue(@"" + path + @"Subject", Data.EscapeString(Subject));
		}
		
		public EmailClientSettings Copy()
		{
			EmailClientSettings copy = new EmailClientSettings();
			
			copy.AddSignature = AddSignature;
			copy.Content = Content;
			copy.Enabled = Enabled;
			copy.Recipients = Recipients;
			copy.Subject = Subject;
			
			return copy;
		}
		
		public override bool Equals(object o)
		{
			if (!(o is EmailClientSettings)) return false;
			EmailClientSettings v = o as EmailClientSettings;
			
			if (!AddSignature.Equals(v.AddSignature)) return false;
			if (!Content.Equals(v.Content)) return false;
			if (!Enabled.Equals(v.Enabled)) return false;
			if (!Recipients.Equals(v.Recipients)) return false;
			if (!Subject.Equals(v.Subject)) return false;
			
			return true;
		}
		
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			
			sb.AppendLine("AddSignature=" + AddSignature.ToString());
			sb.AppendLine("Content=" + Content.ToString());
			sb.AppendLine("Enabled=" + Enabled.ToString());
			sb.AppendLine("Recipients=" + Recipients.ToString());
			sb.AppendLine("Subject=" + Subject.ToString());
			
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
