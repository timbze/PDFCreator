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
	public partial class ParameterSettings : INotifyPropertyChanged {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		private Data data = Data.CreateDataStorage();
		private IStorage _storage = null;
		
		public Parameters Parameters { get; set; } = new Parameters();
		
		public ParameterSettings(IStorage storage)
		{
			_storage = storage;
		}
		
		public bool LoadData(IStorage storage, string path)
		{
			try {
				data.Clear();
				storage.Data = data;
				storage.ReadData(path);
				ReadValues(data, "");
				return true;
			} catch { return false; }
			
		}
		
		public bool LoadData(string path)
		{
			return LoadData(_storage, path);
			
		}
		
		public bool SaveData(IStorage storage, string path)
		{
			try {
				data.Clear();
				StoreValues(data, "");
				storage.Data = data;
				storage.WriteData(path);
				return true;
			} catch { return false; }
			
		}
		
		public bool SaveData(string path)
		{
			return SaveData(_storage, path);
			
		}
		
		public void ReadValues(Data data, string path)
		{
			Parameters.ReadValues(data, path + @"Parameters\");
		}
		
		public void StoreValues(Data data, string path)
		{
			Parameters.StoreValues(data, path + @"Parameters\");
		}
		
		public ParameterSettings Copy()
		{
			ParameterSettings copy = new ParameterSettings(_storage);
			
			copy.Parameters = Parameters.Copy();
			
			return copy;
		}
		
		public override bool Equals(object o)
		{
			if (!(o is ParameterSettings)) return false;
			ParameterSettings v = o as ParameterSettings;
			
			if (!Parameters.Equals(v.Parameters)) return false;
			
			return true;
		}
		
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			
			sb.AppendLine("[Parameters]");
			sb.AppendLine(Parameters.ToString());
			
			return sb.ToString();
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
	}
}
