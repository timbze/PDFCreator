using pdfforge.DataStorage.Storage;
using pdfforge.DataStorage;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
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
	/// Compression settings for monochrome images
	/// </summary>
	[ImplementPropertyChanged]
	public partial class CompressMonochrome : INotifyPropertyChanged {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		
		/// <summary>
		/// Settings for the compression method.
		/// </summary>
		public CompressionMonochrome Compression { get; set; } = CompressionMonochrome.CcittFaxEncoding;
		
		/// <summary>
		/// Images will be resampled to this maximum resolution of the images, if resampling is enabled
		/// </summary>
		public int Dpi { get; set; } = 1200;
		
		/// <summary>
		/// If true, monochrome images will be processed according to the algorithm. If false, they will remain uncompressed
		/// </summary>
		public bool Enabled { get; set; } = true;
		
		/// <summary>
		/// If true, the images will be resampled to a maximum resolution
		/// </summary>
		public bool Resampling { get; set; } = false;
		
		
		public void ReadValues(Data data, string path)
		{
			try { Compression = (CompressionMonochrome) Enum.Parse(typeof(CompressionMonochrome), data.GetValue(@"" + path + @"Compression")); } catch { Compression = CompressionMonochrome.CcittFaxEncoding;}
			try { Dpi = int.Parse(data.GetValue(@"" + path + @"Dpi"), System.Globalization.CultureInfo.InvariantCulture); } catch { Dpi = 1200;}
			try { Enabled = bool.Parse(data.GetValue(@"" + path + @"Enabled")); } catch { Enabled = true;}
			try { Resampling = bool.Parse(data.GetValue(@"" + path + @"Resampling")); } catch { Resampling = false;}
		}
		
		public void StoreValues(Data data, string path)
		{
			data.SetValue(@"" + path + @"Compression", Compression.ToString());
			data.SetValue(@"" + path + @"Dpi", Dpi.ToString(System.Globalization.CultureInfo.InvariantCulture));
			data.SetValue(@"" + path + @"Enabled", Enabled.ToString());
			data.SetValue(@"" + path + @"Resampling", Resampling.ToString());
		}
		
		public CompressMonochrome Copy()
		{
			CompressMonochrome copy = new CompressMonochrome();
			
			copy.Compression = Compression;
			copy.Dpi = Dpi;
			copy.Enabled = Enabled;
			copy.Resampling = Resampling;
			
			return copy;
		}
		
		public override bool Equals(object o)
		{
			if (!(o is CompressMonochrome)) return false;
			CompressMonochrome v = o as CompressMonochrome;
			
			if (!Compression.Equals(v.Compression)) return false;
			if (!Dpi.Equals(v.Dpi)) return false;
			if (!Enabled.Equals(v.Enabled)) return false;
			if (!Resampling.Equals(v.Resampling)) return false;
			
			return true;
		}
		
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			
			sb.AppendLine("Compression=" + Compression.ToString());
			sb.AppendLine("Dpi=" + Dpi.ToString());
			sb.AppendLine("Enabled=" + Enabled.ToString());
			sb.AppendLine("Resampling=" + Resampling.ToString());
			
			return sb.ToString();
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
	}
}
