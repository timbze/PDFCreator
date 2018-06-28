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
	public partial class JobHistory : INotifyPropertyChanged {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		
		/// <summary>
		/// Max number of jobs tracked in history
		/// </summary>
		public int Capacity { get; set; } = 30;
		
		/// <summary>
		/// If enabled PDFCreator tracks latest converted files in history
		/// </summary>
		public bool Enabled { get; set; } = true;
		
		
		public void ReadValues(Data data, string path)
		{
			try { Capacity = int.Parse(data.GetValue(@"" + path + @"Capacity"), System.Globalization.CultureInfo.InvariantCulture); } catch { Capacity = 30;}
			try { Enabled = bool.Parse(data.GetValue(@"" + path + @"Enabled")); } catch { Enabled = true;}
		}
		
		public void StoreValues(Data data, string path)
		{
			data.SetValue(@"" + path + @"Capacity", Capacity.ToString(System.Globalization.CultureInfo.InvariantCulture));
			data.SetValue(@"" + path + @"Enabled", Enabled.ToString());
		}
		
		public JobHistory Copy()
		{
			JobHistory copy = new JobHistory();
			
			copy.Capacity = Capacity;
			copy.Enabled = Enabled;
			
			return copy;
		}
		
		public override bool Equals(object o)
		{
			if (!(o is JobHistory)) return false;
			JobHistory v = o as JobHistory;
			
			if (!Capacity.Equals(v.Capacity)) return false;
			if (!Enabled.Equals(v.Enabled)) return false;
			
			return true;
		}
		
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			
			sb.AppendLine("Capacity=" + Capacity.ToString());
			sb.AppendLine("Enabled=" + Enabled.ToString());
			
			return sb.ToString();
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
	}
}
