using System;
using Jindium;
using System.Threading;

namespace JindiumDemo;

class Program
{
    static void Main()
    {
        JinServer server = new JinServer("http://localhost:5000/");

        server.ServerRoutes.AddStaticRoute("/", Method.GET, DefaultRoute);
        server.ServerRoutes.AddStaticRoute("/test", Method.GET, DefaultRoute);

        server.ServerRoutes.AddStaticRoute("/auth", Method.POST, AuthRoute);

        server.ServerRoutes.AddContentRoute("/jack", "/test.html");
        server.ServerRoutes.AddContentRoute("/login", "/login.html");
        server.ServerRoutes.AddContentRoute("/george", "/george");

        server.ServerReplacelets.AddReplacelet("HelloWorld", (args) =>
        {
            return "Hello, how are you doing today " + args["name"] + "?";
        });

        server.ServerReplacelets.AddReplacelet("DateTime", (args) =>
        {
            return DateTime.Now.ToString();
        });

        server.Start();
    }

    static async Task DefaultRoute(Context ctx)
    {
        await ctx.Send("Hello, world! <HelloWorld name=\"jack\"></REPL> <DateTime null></REPL>");
    }

    static async Task AuthRoute(Context ctx)
    {
        var data = await ctx.GetRequestPostData();

        //Check if the keys username and password exist in the data dictionary
        if (data.ContainsKey("username") && data.ContainsKey("password"))
        {
            //If they do, check if the username and password are correct
            if (data["username"] == "jack" && data["password"] == "pass")
            {
                //If they are, send a success message
                await ctx.Send("You are logged in!");
            }
            else
            {
                //If they are not, send an error message
                await ctx.Send("Incorrect username or password!");
            }
        }
        else
        {
            //If they are not, send an error message
            await ctx.Send("You must provide a username and password!");
        }
    }
}