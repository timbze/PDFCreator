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
	public partial class Notifications : INotifyPropertyChanged {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		
		/// <summary>
		/// Defines the notification level.
		/// </summary>
		public NotificationsLevel NotificationsLevel { get; set; } = NotificationsLevel.ShowAll;
		
		
		public void ReadValues(Data data, string path)
		{
			try { NotificationsLevel = (NotificationsLevel) Enum.Parse(typeof(NotificationsLevel), data.GetValue(@"" + path + @"NotificationsLevel")); } catch { NotificationsLevel = NotificationsLevel.ShowAll;}
		}
		
		public void StoreValues(Data data, string path)
		{
			data.SetValue(@"" + path + @"NotificationsLevel", NotificationsLevel.ToString());
		}
		
		public Notifications Copy()
		{
			Notifications copy = new Notifications();
			
			copy.NotificationsLevel = NotificationsLevel;
			
			return copy;
		}
		
		public override bool Equals(object o)
		{
			if (!(o is Notifications)) return false;
			Notifications v = o as Notifications;
			
			if (!NotificationsLevel.Equals(v.NotificationsLevel)) return false;
			
			return true;
		}
		
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			
			sb.AppendLine("NotificationsLevel=" + NotificationsLevel.ToString());
			
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
