using System;
using System.Threading;

namespace JamesWright.SimpleHttp
{
    public class HttpItegrationServer
    {
        private readonly Server _server;

        public HttpItegrationServer()
        {
            _server = new Server(new Listener(), new RouteRepository());
        }

        public void Start(string port = "8005")
        {
            AutoResetEvent keepAlive = new AutoResetEvent(false);
            _server.StartAsync(port).Wait();
            keepAlive.WaitOne();
        }

        public void Get(string endpoint, Action<Request, Response> handler)
        {
            _server.RouteRepository.Get.Add(endpoint, handler);
        }

        public void Post(string endpoint, Action<Request, Response> handler)
        {
            _server.RouteRepository.Post.Add(endpoint, handler);
        }

        public void Put(string endpoint, Action<Request, Response> handler)
        {
            _server.RouteRepository.Put.Add(endpoint, handler);
        }
    }
}
