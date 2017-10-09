using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace JamesWright.SimpleHttp
{
    public class Request
    {
        public HttpListenerRequest HttpRequest;
        private string body;

        internal Request(HttpListenerRequest httpRequest)
        {
            this.HttpRequest = httpRequest;
        }

        public string[] Parameters { get; private set; }

        public string Endpoint
        {
            get { return this.HttpRequest.RawUrl; }
        }

        public string Method
        {
            get { return this.HttpRequest.HttpMethod; }
        }

        public async Task<string> GetBodyAsync()
        {
            if (Method == WebRequestMethods.Http.Get || !this.HttpRequest.HasEntityBody)
                return null;

            if (this.body == null)
            {
                byte[] buffer = new byte[this.HttpRequest.ContentLength64];
                using (Stream inputStream = this.HttpRequest.InputStream)
                {
                    await inputStream.ReadAsync(buffer, 0, buffer.Length);
                }

                this.body = Encoding.UTF8.GetString(buffer);
            }

            return this.body;
        }
    }
}
