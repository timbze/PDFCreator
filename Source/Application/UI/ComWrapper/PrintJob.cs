namespace pdfforge.PDFCreator.UI.ComWrapper
{
	public class PrintJob
	{
		private readonly dynamic _printJob;
		
		internal PrintJob(dynamic printJob)
		{
			_printJob = printJob;
		}
		public bool IsFinished
		{
			 get { return _printJob.IsFinished; }
		}
		
		public bool IsSuccessful
		{
			 get { return _printJob.IsSuccessful; }
		}
		
		public OutputFiles GetOutputFiles
		{
			 get { return new OutputFiles(_printJob.GetOutputFiles); }
		}
		
		public PrintJobInfo PrintJobInfo
		{
			 get { return new PrintJobInfo(_printJob.PrintJobInfo); }
		}
		
		public void SetProfileByGuid(string profileGuid)
		{
			_printJob.SetProfileByGuid(profileGuid);
		}
		
		public void ConvertTo(string fullFileName)
		{
			_printJob.ConvertTo(fullFileName);
		}
		
		public void ConvertToAsync(string fullFileName)
		{
			_printJob.ConvertToAsync(fullFileName);
		}
		
		public void SetProfileSetting(string name, string value)
		{
			_printJob.SetProfileSetting(name, value);
		}
		
		public string GetProfileSetting(string propertyName)
		{
			return _printJob.GetProfileSetting(propertyName);
		}
		
	}
}
