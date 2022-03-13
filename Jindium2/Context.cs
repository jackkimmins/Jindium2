using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Jindium
{
    public class Context
    {
        public HttpListenerRequest req { get; set; } = null;
        public HttpListenerResponse res { get; set; } = null;

        public async Task Send(string data, int statusCode = 200, string contentType = "text/html")
        {
            await CreateResponse(Encoding.UTF8.GetBytes(data), statusCode, contentType);
        }

        public async Task ErrorPage(string message, int statusCode = 500)
        {
            await Send(StaticResp.ErrorTemplate("test", statusCode.ToString()), statusCode, "text/html");
        }

        private Task AddCustomHeaders()
        {
            res.Headers.Add("Server", "Jindium");

            return Task.CompletedTask;
        }

        private async Task CreateResponse(byte[] data, int statusCode = 200, string contentType = "text/html")
        {
            res.StatusCode = statusCode;
            res.ContentType = contentType;

            AddCustomHeaders();

            await res.OutputStream.WriteAsync(data, 0, data.Length);
        }

        public Context(HttpListenerRequest req, HttpListenerResponse res)
        {
            this.req = req;
            this.res = res;
        }
    }
}
