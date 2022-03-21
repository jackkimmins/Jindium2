using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Jindium
{
    public class Context
    {
        public HttpListenerRequest req { get; private set; } = null;
        public HttpListenerResponse res { get; private set; } = null;
        public Replacelets LocalReplacelets { get; private set; }
        public SessionData Session { get; set; }
        public string? ContentType { get; set; } = null;
        public bool MustBeAuth { get; set; } = false;

        private string ApplyReplacelets(string content)
        {
            foreach (KeyValuePair<string, Func<Context, Dictionary<string, string>, string>> replacelet in LocalReplacelets.ReplaceletDictionary)
            {
                foreach (Match replaceletMatch in new Regex("<" + replacelet.Key + "(?=\\s)(?!(?:[^>\"\\']|\"[^\"]*\"|\\'[^\\']*\\')*?(?<=\\s)(?:term|range)\\s*=)(?!\\s*/?>)\\s+(?:\".*?\"|\\'.*?\\'|[^>]*?)+>").Matches(content))
                {
                    Dictionary<string, string> ReplaceletArgs = new Dictionary<string, string>();

                    foreach (Match match in new Regex("(\\w+)=[\"']?((?:.(?![\"']?\\s+(?:\\S+)=|\\s*\\/?[>\"']))+.)[\"']?").Matches(replaceletMatch.Value))
                    {
                        ReplaceletArgs.Add(match.Groups[1].Value, match.Groups[2].Value);
                    }

                    content = content.Replace(replaceletMatch.Value, replacelet.Value(this, ReplaceletArgs));
                }
            }

            return content;
        }

        public async Task Send(string data, int statusCode = 200, string contentType = "text/html", bool AuthOverride = false)
        {
            if (MustBeAuth && Session.IsAuthenticated == false && AuthOverride == false)
            {
                await ErrorPage("You are not authorized to view this page.", 401);
                return;
            }

            data = ApplyReplacelets(data);
            await CreateResponse(Encoding.UTF8.GetBytes(data), statusCode, ContentType == null ? contentType : ContentType);
        }

        public async Task SendFile(byte[] data, int statusCode = 200, string contentType = "text/html", bool AuthOverride = false)
        {
            if (MustBeAuth && Session.IsAuthenticated == false && AuthOverride == false)
            {
                await ErrorPage("You are not authorized to view this page.", 401);
                return;
            }

            contentType = ContentType == null ? contentType : ContentType;

            if (contentType.StartsWith("text/"))
            {
                data = Encoding.UTF8.GetBytes(ApplyReplacelets(Encoding.UTF8.GetString(data)));
            }

            await CreateResponse(data, statusCode, contentType);
        }

        public async Task Redirect(string url)
        {
            res.RedirectLocation = url;
            await CreateResponse(Encoding.UTF8.GetBytes(""), 302, "text/html");
        }

        public async Task ErrorPage(string message, int statusCode = 500)
        {
            await CreateResponse(Encoding.UTF8.GetBytes(StaticResp.ErrorTemplate(message, "JindiumSiteNameChangeplease")), statusCode, "text/html");
        }

        public Task<Dictionary<string, string>> GetRequestPostData()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            if (req.HasEntityBody)
            {
                //Read the text using the StreamReader.
                using (System.IO.StreamReader reader = new System.IO.StreamReader(req.InputStream, req.ContentEncoding))
                {
                    string body = reader.ReadToEnd();

                    string[] pairs = body.Split('&');

                    foreach (string pair in pairs)
                    {
                        string[] keyValue = pair.Split('=');

                        if (keyValue.Length == 2)
                        {
                            data.Add(System.Web.HttpUtility.UrlDecode(keyValue[0]), System.Web.HttpUtility.UrlDecode(keyValue[1]));
                        }
                    }
                }
            }

            return Task.FromResult(data);
        }

        public async Task<bool> IsPostKeySet(params string[] keys)
        {
            Dictionary<string, string> data = await GetRequestPostData();

            foreach (string key in keys)
            {
                if (!data.ContainsKey(key))
                {
                    return false;
                }
            }

            return true;
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
            res.ContentEncoding = Encoding.UTF8;
            res.ContentLength64 = data.Length;

            await AddCustomHeaders();

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