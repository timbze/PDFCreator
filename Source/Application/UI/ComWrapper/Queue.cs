using System;

namespace pdfforge.PDFCreator.UI.ComWrapper
{
	public class Queue
	{
		private readonly dynamic _queue;
		
		public Queue()
		{
			Type queueType = Type.GetTypeFromProgID("PDFCreator.JobQueue");
			_queue = Activator.CreateInstance(queueType);
		}
		internal Queue(dynamic queue)
		{
			_queue = queue;
		}
		public int Count
		{
			 get { return _queue.Count; }
		}
		
		public PrintJob NextJob
		{
			 get { return new PrintJob(_queue.NextJob); }
		}
		
		public void Initialize()
		{
			_queue.Initialize();
		}
		
		public bool WaitForJob(int timeOut)
		{
			return _queue.WaitForJob(timeOut);
		}
		
		public bool WaitForJobs(int jobCount, int timeOut)
		{
			return _queue.WaitForJobs(jobCount, timeOut);
		}
		
		public PrintJob GetJobByIndex(int jobIndex)
		{
			return new PrintJob(_queue.GetJobByIndex(jobIndex));
		}
		
		public void MergeJobs(PrintJob job1, PrintJob job2)
		{
			_queue.MergeJobs(job1, job2);
		}
		
		public void MergeAllJobs()
		{
			_queue.MergeAllJobs();
		}
		
		public void Clear()
		{
			_queue.Clear();
		}
		
		public void DeleteJob(int index)
		{
			_queue.DeleteJob(index);
		}
		
		public void ReleaseCom()
		{
			_queue.ReleaseCom();
		}
		
	}
}
