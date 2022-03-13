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
        public Replacelets LocalReplacelets { get; private set; }

        private string ApplyReplacelets(string content)
        {
            foreach (var replacelet in LocalReplacelets.ReplaceletDictionary)
            {
                content = content.Replace(replacelet.Key, replacelet.Value(""));
            }

            return content;
        }

        public async Task Send(string data, int statusCode = 200, string contentType = "text/html")
        {
            data = ApplyReplacelets(data);
            await CreateResponse(Encoding.UTF8.GetBytes(data), statusCode, contentType);
        }

        public async Task ErrorPage(string message, int statusCode = 500)
        {
            await Send(StaticResp.ErrorTemplate("test", statusCode.ToString()), statusCode, "text/html");
        }

        public async Task<Dictionary<string, string>> GetRequestPostData()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            if (req.HasEntityBody)
            {
                byte[] buffer = new byte[req.ContentLength64];
                await req.InputStream.ReadAsync(buffer, 0, buffer.Length);
                string body = Encoding.UTF8.GetString(buffer);

                string[] pairs = body.Split('&');

                foreach (string pair in pairs)
                {
                    string[] keyValue = pair.Split('=');
                    data.Add(keyValue[0], keyValue[1]);
                }
            }

            return data;
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

        public Context(HttpListenerRequest req, HttpListenerResponse res, Replacelets replacelets)
        {
            this.req = req;
            this.res = res;
            LocalReplacelets = replacelets;
        }
    }
}