using System;
using System.Text;
using System.Threading;
using NUnit.Framework;
using pdfforge.PDFCreator.Core.Communication;

namespace PDFCreator.Utilities.IntegrationTest.Communication
{
    [TestFixture]
    public class PipeCommunicationTest
    {
        [TearDown]
        public void TearDown()
        {
            _pipeServer?.Stop();
        }

        private string _pipeName;
        private PipeServer _pipeServer;

        private void CreateServerPipe()
        {
            _pipeName = "TestPipe" + RandomString(8);
            _pipeServer = new PipeServer(_pipeName, _pipeName);
        }

        private void CreateSessionPipe()
        {
            _pipeName = "TestPipe" + RandomString(8);
            _pipeServer = new PipeServer(_pipeName, _pipeName);
        }

        private static string RandomString(long length)
        {
            var rnd = RandomHelper.Instance;
            var temp = new StringBuilder();
            for (long i = 0; i < length; i++)
            {
                temp.Append(rnd.Next(9));
            }
            return temp.ToString();
        }

        [Test]
        public void PipeClient_SendsMessageWithoutServerRunning_GetsTimeout()
        {
            var pipeClient = new PipeClient("nonexistingpipe");
            pipeClient.Timeout = 200;

            Assert.IsFalse(pipeClient.SendMessage("test"));
        }

        [Test]
        public void PipeServer_DoubleInstance_ThrowsException()
        {
            CreateServerPipe();
            _pipeServer.Start();

            var pipeServer = new PipeServer(_pipeName, _pipeName);

            try
            {
                Assert.Throws<InvalidOperationException>(() => pipeServer.Start());
            }
            finally
            {
                pipeServer.Stop();
            }
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

        [Test]
        public void PipeServer_PipeClientSendsMessage_MessageIsReceived()
        {
            CreateServerPipe();
            const string message = "test";

            var reset = new ManualResetEvent(false);
            _pipeServer.OnNewMessage += (sender, e) => { if (e.Message == message) reset.Set(); };
            _pipeServer.Start();

            var pipeClient = new PipeClient(_pipeName);
            pipeClient.SendMessage(message);
            Assert.IsTrue(reset.WaitOne(200));
        }
        
        [Test]
        public void PipeServer_SessionPipeClientSendsMessage_MessageIsReceived()
        {
            CreateSessionPipe();
            const string message = "test";

            var reset = new ManualResetEvent(false);
            _pipeServer.OnNewMessage += (sender, e) => { if (e.Message == message) reset.Set(); };
            _pipeServer.Start();

            var pipeClient = new PipeClient(_pipeName);
            pipeClient.SendMessage(message);
            Assert.IsTrue(reset.WaitOne(200));
        }

        [Test]
        public void PipeServer_SingleInstance_DetectsRunningInstance()
        {
            CreateServerPipe();
            _pipeServer.Start();

            Assert.IsTrue(_pipeServer.IsServerRunning(), "No server pipe found");
        }

        [Test]
        public void PipeServer_WhenPreparingShutdown_MessagesAreRejected()
        {
            CreateSessionPipe();
            const string message = "test";

            _pipeServer.Start();

            var pipeClient = new PipeClient(_pipeName);

            _pipeServer.PrepareShutdown();

            var success = pipeClient.SendMessage(message);

            Assert.IsFalse(success);
        }
    }
}