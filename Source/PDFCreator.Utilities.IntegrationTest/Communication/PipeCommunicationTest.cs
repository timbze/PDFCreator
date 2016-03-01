using System;
using System.Text;
using System.Threading;
using NUnit.Framework;
using pdfforge.PDFCreator.Utilities.Communication;

namespace PDFCreator.Utilities.IntegrationTest.Communication
{
    [TestFixture]
    public class PipeCommunicationTest
    {
        private string _pipeName;
        private PipeServer _pipeServer;

        private void CreateServerPipe()
        {
            _pipeName = "TestPipe" + RandomString(8);
            _pipeServer = PipeServer.CreatePipeServer(_pipeName, _pipeName);
        }

        private void CreateSessionPipe()
        {
            _pipeName = "TestPipe" + RandomString(8);
            _pipeServer = PipeServer.CreateSessionPipeServer(_pipeName);
        }

        [TearDown]
        public void TearDown()
        {
            if (_pipeServer != null)
                _pipeServer.Stop();
        }

        [Test]
        public void PipeServer_SingleInstance_DetectsRunningInstance()
        {
            CreateServerPipe();
            _pipeServer.Start();

            Assert.IsTrue(PipeServer.ServerInstanceRunning(_pipeName), "No server pipe found");
            Assert.IsFalse(PipeServer.SessionServerInstanceRunning(_pipeName), "Found session pipe server");
        }

        [Test]
        public void PipeServer_SingleSessionInstance_DetectsRunningSessionInstance()
        {
            CreateSessionPipe();
            
            Assert.IsTrue(PipeServer.SessionServerInstanceRunning(_pipeName), "No session pipe server found");
            Assert.IsFalse(PipeServer.ServerInstanceRunning(_pipeName), "Found server pipe");
        }
        
        [Test]
        public void PipeServer_DoubleInstance_ThrowsException()
        {
            CreateServerPipe();
            Assert.Throws<InvalidOperationException>(() => PipeServer.CreatePipeServer(_pipeName, _pipeName));
        }

        [Test]
        public void PipeServer_ReusingStoppedInstance_ThrowsException()
        {
            CreateServerPipe();
            _pipeServer.Start();
            _pipeServer.Stop();
            Assert.Throws<InvalidOperationException>(() => _pipeServer.Start());
        }

        [Test]
        public void PipeClient_SendsMessageWithoutServerRunning_GetsTimeout()
        {
            PipeClient pipeClient = PipeClient.CreatePipeClient("nonexistingpipe");
            pipeClient.Timeout = 200;
            
            Assert.IsFalse(pipeClient.SendMessage("test"));
        }

        [Test]
        public void PipeServer_PipeClientSendsMessage_MessageIsReceived()
        {
            CreateServerPipe();
            const string message = "test";

            var reset = new ManualResetEvent(false);
            _pipeServer.OnNewMessage += (sender, e) =>
            {
                if (e.Message == message) reset.Set();
            };
            _pipeServer.Start();

            var pipeClient = PipeClient.CreatePipeClient(_pipeName);
            pipeClient.SendMessage(message);
            Assert.IsTrue(reset.WaitOne(200));
        }

        [Test]
        public void PipeServer_SessionPipeClientSendsMessage_MessageIsReceived()
        {
            CreateSessionPipe();
            const string message = "test";

            var reset = new ManualResetEvent(false);
            _pipeServer.OnNewMessage += (sender, e) =>
            {
                if (e.Message == message) reset.Set();
            };
            _pipeServer.Start();

            var pipeClient = PipeClient.CreateSessionPipeClient(_pipeName);
            pipeClient.SendMessage(message);
            Assert.IsTrue(reset.WaitOne(200));
        }

        [Test]
        public void PipeServer_WhenPreparingShutdown_MessagesAreRejected()
        {
            CreateSessionPipe();
            const string message = "test";

            var reset = new ManualResetEvent(false);

            _pipeServer.Start();

            var pipeClient = PipeClient.CreateSessionPipeClient(_pipeName);

            _pipeServer.PrepareShutdown();

            var success = pipeClient.SendMessage(message);

            Assert.IsFalse(success);
        }

        [Test]
        public void PipeServer_OnClose_CloseEventIsFired()
        {
            CreateServerPipe();
            _pipeServer.Start();

            var reset = new ManualResetEvent(false);
            _pipeServer.OnServerClosed += (sender, e) => reset.Set();
            _pipeServer.Stop();

            Assert.IsTrue(reset.WaitOne(200));
        }

        private static string RandomString(Int64 length)
        {
            var rnd = RandomHelper.Instance;
            var temp = new StringBuilder();
            for (Int64 i = 0; i < length; i++)
            {
                temp.Append(rnd.Next(9));
            }
            return temp.ToString();
        }
    }
}
