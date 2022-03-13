using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jindium;

public class Routes
{
    public Dictionary<Route, Func<Context, Task>> RoutesDictionary { get; private set; } = null;

    public void AddStaticRoute(Route route, Func<Context, Task> action)
    {
        if (RoutesDictionary.ContainsKey(route))
        {
            if (route.Path != "/")
                cText.WriteLine("A route with the same path and method already exists! (" + route.Path + ") Overwriting...");

            RoutesDictionary[route] = action;
            return;
        }

        RoutesDictionary.Add(route, action);
    }

    public void AddStaticRoute(string path, Method method, Func<Context, Task> action)
    {
        AddStaticRoute(new Route(path, method), action);
    }

    public Routes()
    {
        RoutesDictionary = new Dictionary<Route, Func<Context, Task>>();
    }
}

public enum Method
{
    GET,
    POST
}

public struct Route
{
    public string Path;
    public Method Method;

    public Route(string path, Method method = Method.GET)
    {
        Path = path;
        Method = method;
    }
}