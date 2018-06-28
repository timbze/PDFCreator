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
	/// Settings for the JPEG output format
	/// </summary>
	[ImplementPropertyChanged]
	public partial class JpegSettings : INotifyPropertyChanged {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		
		/// <summary>
		/// Number of colors.
		/// </summary>
		public JpegColor Color { get; set; } = JpegColor.Color24Bit;
		
		/// <summary>
		/// Resolution of the JPEG files
		/// </summary>
		public int Dpi { get; set; } = 150;
		
		/// <summary>
		/// Quality factor of the resulting JPEG (100 is best, 0 is worst)
		/// </summary>
		public int Quality { get; set; } = 75;
		
		
		public void ReadValues(Data data, string path)
		{
			try { Color = (JpegColor) Enum.Parse(typeof(JpegColor), data.GetValue(@"" + path + @"Color")); } catch { Color = JpegColor.Color24Bit;}
			try { Dpi = int.Parse(data.GetValue(@"" + path + @"Dpi"), System.Globalization.CultureInfo.InvariantCulture); } catch { Dpi = 150;}
			try { Quality = int.Parse(data.GetValue(@"" + path + @"Quality"), System.Globalization.CultureInfo.InvariantCulture); } catch { Quality = 75;}
		}
		
		public void StoreValues(Data data, string path)
		{
			data.SetValue(@"" + path + @"Color", Color.ToString());
			data.SetValue(@"" + path + @"Dpi", Dpi.ToString(System.Globalization.CultureInfo.InvariantCulture));
			data.SetValue(@"" + path + @"Quality", Quality.ToString(System.Globalization.CultureInfo.InvariantCulture));
		}
		
		public JpegSettings Copy()
		{
			JpegSettings copy = new JpegSettings();
			
			copy.Color = Color;
			copy.Dpi = Dpi;
			copy.Quality = Quality;
			
			return copy;
		}
		
		public override bool Equals(object o)
		{
			if (!(o is JpegSettings)) return false;
			JpegSettings v = o as JpegSettings;
			
			if (!Color.Equals(v.Color)) return false;
			if (!Dpi.Equals(v.Dpi)) return false;
			if (!Quality.Equals(v.Quality)) return false;
			
			return true;
		}
		
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			
			sb.AppendLine("Color=" + Color.ToString());
			sb.AppendLine("Dpi=" + Dpi.ToString());
			sb.AppendLine("Quality=" + Quality.ToString());
			
			return sb.ToString();
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
	}
}
