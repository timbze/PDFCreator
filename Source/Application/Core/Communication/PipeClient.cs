using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;

namespace pdfforge.PDFCreator.Core.Communication
{
    public class PipeClient
    {
        //private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly string _pipeName;

        public PipeClient(string pipeName)
        {
            _pipeName = pipeName;
            Timeout = 10000;
        }

        public int Timeout { get; set; }

        public bool SendMessage(string message)
        {
            return SendMessage(message, Timeout);
        }

        public bool SendMessage(string message, int timeout)
        {
            var answer = "";
            //_logger.Debug("Pipe: " + _pipeName + "; Message: " + message);

            try
            {
                using (var pipeClient = new NamedPipeClientStream(".", _pipeName, PipeDirection.InOut))
                using (var sw = new StreamWriter(pipeClient))
                using (var sr = new StreamReader(pipeClient))
                {
                    pipeClient.Connect(timeout);
                    sw.AutoFlush = true;

                    var greeting = sr.ReadLine();
                    // Verify that this is the "true server"
                    if ((greeting != null) &&
                        greeting.StartsWith("HELLO", StringComparison.OrdinalIgnoreCase))
                    {
                        // The client security token is sent with the first write.
                        sw.WriteLine(message);

                        answer = sr.ReadLine();
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (ObjectDisposedException)
            {
            }
            catch (InvalidOperationException)
            {
            }
            catch (TimeoutException)
            {
            }
            return answer != null && answer.Equals("OK", StringComparison.OrdinalIgnoreCase);
        }
    }
}