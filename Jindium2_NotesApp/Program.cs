using System;
using Jindium;
using System.Threading;

class Program
{
    static void Main()
    {
        JinServer server = new JinServer("http://localhost:5000/");

        server.ServerRoutes.OutputOverwriteWarnings = false;

        server.ServerRoutes.AddStaticRoute("/hello", Method.GET, async (ctx) =>
        {
            await ctx.Send("Hello, world!");
        });

        server.ServerRoutes.AddContentRoute("/", "/");
        server.ServerRoutes.AddContentRoute("/", "/dashboard", true);


        server.ServerRoutes.AddStaticRoute("/auth", Method.POST, async (ctx) =>
        {
            var data = await ctx.GetRequestPostData();

            if (data.ContainsKey("username") && data.ContainsKey("password"))
            {
                if (data["username"] == "jack" && data["password"] == "pass")
                {
                    ctx.Session.AddKeyValue("username", data["username"]);
                    ctx.Session.IsAuthenticated = true;

                    await ctx.Send("success");
                }
                else
                {
                    await ctx.Send("Incorrect username or password!");
                }
            }
            else
            {
                await ctx.Send("You must provide a username and password!");
            }
        });

        server.ServerReplacelets.AddReplacelet("FullName", (ctx, args) =>
        {
            return ctx.Session.GetValue("username");
        });


        server.Start();
    }
}