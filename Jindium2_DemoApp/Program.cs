using System;
using Jindium;
using System.Threading;

namespace JindiumDemo;

class Program
{
    static void Main()
    {
        JinServer server = new JinServer("http://localhost:5000/", DefaultRoute);

        server.ServerRoutes.AddStaticRoute("/test", Method.GET, DefaultRoute);

        server.Start();
    }

    static async Task DefaultRoute(Context ctx)
    {
        await ctx.Send("Hello, world!");
    }
}