using System.Net;
using System.Linq;

namespace Jindium;

public class JinServer
{
    private HttpListener listener = new HttpListener();
    public string Address { get; private set; }
    public Routes ServerRoutes { get; private set; } = new Routes();
    public Replacelets ServerReplacelets { get; private set; } = new Replacelets();

    public JinServer(string address)
    {
        if (String.IsNullOrEmpty(address)) address = "http://localhost:5000/";

        Address = address;

        ServerRoutes.AddStaticRoute("/", Method.GET, (ctx) =>
        {
            return ctx.Send(StaticResp.WelcomeTemplate("Jindium"));
        });
    }

    private async Task HandleIncomingConnections()
    {
        while (true)
        {
            HttpListenerContext ctx = await listener.GetContextAsync();

            Context context = new Context(ctx.Request, ctx.Response, ServerReplacelets);

            string path = ctx.Request.Url.AbsolutePath;
            string method = ctx.Request.HttpMethod;

            if (!Enum.IsDefined(typeof(Method), method))
            {
                await context.ErrorPage("HTTP Method is not allowed", 405);
                continue;
            }

            var route = ServerRoutes.RoutesDictionary.FirstOrDefault(x => x.Key.Path == path && x.Key.Method == (Method)Enum.Parse(typeof(Method), method));

            if (route.Key.Path == path && route.Key.Method == (Method)Enum.Parse(typeof(Method), method))
            {
                await route.Value(context);
                cText.WriteLine($"{method} {path}", "REQ", ConsoleColor.Green);
            }
            else
            {
                await context.ErrorPage("This page does not exist.", 404);
                cText.WriteLine($"{method} {path}", "REQ", ConsoleColor.Red);
            }

            context.res.Close();
        }
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

        //Handle incoming connections.
        try
        {
            Task listenTask = HandleIncomingConnections();
            listenTask.GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            cText.WriteLine($"An error occured while handling incoming connections: {ex.Message}", "ERR", ConsoleColor.Red);
        }
        
        listener.Close();
    }
}