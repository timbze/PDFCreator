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
	public class Accounts {
		
		public IList<DropboxAccount> DropboxAccounts { get; set; }
		
		private void Init() {
			DropboxAccounts = new List<DropboxAccount>();
		}
		
		public Accounts()
		{
			Init();
		}
		
		public void ReadValues(Data data, string path)
		{
			
			try
			{
				int numClasses = int.Parse(data.GetValue(@"" + path + @"DropboxAccounts\numClasses"));
				for (int i = 0; i < numClasses; i++)
				{
					DropboxAccount tmp = new DropboxAccount();
					tmp.ReadValues(data, @"" + path + @"DropboxAccounts\" + i + @"\");
					DropboxAccounts.Add(tmp);
				}
			} catch {}
			
		}
		
		public void StoreValues(Data data, string path)
		{
			
			for (int i = 0; i < DropboxAccounts.Count; i++)
			{
				DropboxAccount tmp = DropboxAccounts[i];
				tmp.StoreValues(data, @"" + path + @"DropboxAccounts\" + i + @"\");
			}
			data.SetValue(@"" + path + @"DropboxAccounts\numClasses", DropboxAccounts.Count.ToString());
			
		}
		
		public Accounts Copy()
		{
			Accounts copy = new Accounts();
			
			
			copy.DropboxAccounts = new List<DropboxAccount>();
			for (int i = 0; i < DropboxAccounts.Count; i++)
			{
				copy.DropboxAccounts.Add(DropboxAccounts[i].Copy());
			}
			
			
			return copy;
		}
		
		public override bool Equals(object o)
		{
			if (!(o is Accounts)) return false;
			Accounts v = o as Accounts;
			
			
			if (DropboxAccounts.Count != v.DropboxAccounts.Count) return false;
			for (int i = 0; i < DropboxAccounts.Count; i++)
			{
				if (!DropboxAccounts[i].Equals(v.DropboxAccounts[i])) return false;
			}
			
			
			return true;
		}
		
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			
			
			for (int i = 0; i < DropboxAccounts.Count; i++)
			{
				sb.AppendLine(DropboxAccounts.ToString());
			}
			
			
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
