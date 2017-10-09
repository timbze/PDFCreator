using System;
using System.Collections.Generic;
using System.Net;

namespace JamesWright.SimpleHttp
{
    internal class RouteRepository
    {
        public Dictionary<string, Action<Request, Response>> Get { get; private set; }
        public Dictionary<string, Action<Request, Response>> Post { get; private set; }
        public Dictionary<string, Action<Request, Response>> Put { get; private set; }

        public RouteRepository()
        {
            Get = new Dictionary<string, Action<Request, Response>>();
            Post = new Dictionary<string, Action<Request, Response>>();
            Put = new Dictionary<string, Action<Request, Response>>();
        }

        public Dictionary<string, Action<Request, Response>> GetRoutes(string method)
        {
            switch (method)
            {
                case WebRequestMethods.Http.Get:
                    return Get;

                case WebRequestMethods.Http.Post:
                    return Post;

                case WebRequestMethods.Http.Put:
                    return Put;

                default:
                    return null;
            }
        }
    }
}
