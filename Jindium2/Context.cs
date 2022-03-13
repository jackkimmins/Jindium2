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

        public async Task Send(string data)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(data);
            await res.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        }

        public Context(HttpListenerRequest req, HttpListenerResponse res)
        {
            this.req = req;
            this.res = res;
        }
    }
}
