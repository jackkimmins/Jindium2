using System.Net;
using System.Linq;
using System.Threading.Tasks;

namespace Jindium;

public partial class JinServer
{
    private HttpListener listener = new HttpListener();
    public string Address { get; private set; }
    public Routes ServerRoutes { get; private set; } = new Routes("JindiumSite");
    public Replacelets ServerReplacelets { get; private set; } = new Replacelets();
    public Sessions Sessions { get; set; } = new Sessions();
    public bool IsServerRunning { get; private set; } = false;
    private bool CompletedShutdown = false;
    public bool Logging { get; private set; } = false;
    public int RequestsCount { get; private set; }

    public JinServer(string address)
    {
        if (String.IsNullOrEmpty(address)) address = "http://localhost:5000/";

        Address = address;

        ServerRoutes.AddStaticRoute("/", Method.GET, (ctx) =>
        {
            return ctx.Send(StaticResp.WelcomeTemplate("Jindium"));
        });
    }

    //Adds the Jindium PreFabs to the JinServer. /logout etc
    public void AddPreFabs()
    {
        ServerRoutes.AddStaticRoute("/logout", Method.GET, PreFabs.Logout);
    }

    private async Task HandleIncomingConnections()
    {
        while (IsServerRunning)
        {
            HttpListenerContext ctx = await listener.GetContextAsync();

            RequestsCount++;

            Context context = new Context(ctx.Request, ctx.Response, ServerReplacelets);

            string path = ctx.Request.Url.AbsolutePath;
            string method = ctx.Request.HttpMethod;

            if (Sessions.SessionsActive)
            {
                //Get session id from the SESSION cookie
                string? sessionId = ctx.Request.Cookies.Where(c => c.Name == "SESSION").FirstOrDefault()?.Value;

                if (sessionId == null)
                {
                    //No session cookie found, create a new session
                    sessionId = Sessions.StartSession();
                    ctx.Response.Cookies.Add(new Cookie("SESSION", sessionId));
                }
                else
                {
                    //Session cookie found, check if session exists
                    if (!Sessions.SessionsData.ContainsKey(sessionId))
                    {
                        //Session does not exist, create a new session
                        sessionId = Sessions.StartSession();
                        ctx.Response.Cookies.Add(new Cookie("SESSION", sessionId));
                    }
                }

                context.Session = Sessions.GetSession(sessionId);
            }

            if (!Enum.IsDefined(typeof(Method), method))
            {
                await context.ErrorPage("HTTP Method is not allowed", 405);
                continue;
            }

            var route = ServerRoutes.RoutesDictionary.FirstOrDefault(x => x.Key.Path == path && x.Key.Method == (Method)Enum.Parse(typeof(Method), method));

            if (route.Key.Path == path && route.Key.Method == (Method)Enum.Parse(typeof(Method), method))
            {
                await route.Value(context);

                if (Logging)
                    cText.WriteLine($"{method} {path}", "REQ", ConsoleColor.Green);
            }
            else
            {
                await context.ErrorPage("This page does not exist.", 404);

                if (Logging)
                    cText.WriteLine($"{method} {path}", "REQ", ConsoleColor.Red);
            }

            context.res.Close();
        }

        CompletedShutdown = true;
    }

    public void Start()
    {
        listener.Prefixes.Add(Address);

        //Start the listener.
        try
        {
            listener.Start();
        }
        catch (HttpListenerException ex)
        {
            cText.WriteLine($"Could not start the Jindimum on the following URI: {Address}", "ERR", ConsoleColor.Red);
            cText.WriteLine("Reason: " + ex.Message, "ERR", ConsoleColor.Red);
            return;
        }

        cText.WriteLine($"Jindium is Online! ({Address})", "INFO", ConsoleColor.Green);
        IsServerRunning = true;

        //Handle incoming connections.
        try
        {
            System.Threading.ThreadPool.QueueUserWorkItem(async (o) =>
            {
                await HandleIncomingConnections();
            });
        }
        catch (Exception ex)
        {
            cText.WriteLine($"An error occured while handling incoming connections: {ex.Message}", "ERR", ConsoleColor.Red);
        }

        while (true)
        {
            if (ExecuteCommand(Console.ReadLine() ?? ""))
                break;
        }

        IsServerRunning = false;

        cText.WriteLine($"Jindium is shutting down...", "INFO", ConsoleColor.Green);

        while (!CompletedShutdown)
        {
            System.Threading.Thread.Sleep(100);
        }

        cText.WriteLine($"Jindium is Offline!", "INFO", ConsoleColor.Red);

        listener.Close();
    }
}