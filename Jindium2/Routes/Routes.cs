using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jindium;

public class Routes
{
    public string JindiumContentPath { get; set; }
    public Dictionary<Route, Func<Context, Task>> RoutesDictionary { get; private set; } = null;

    public void AddRoute(Route route, Func<Context, Task> action)
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
        AddRoute(new Route(path, method), action);
    }

    public void AddContentRoute(string path, string contentPath, bool mustBeAuth = false)
    {
        contentPath = contentPath.Replace("/", "\\");
        contentPath = System.IO.Path.GetFullPath(JindiumContentPath + contentPath);

        //Check if contentPath is a directory
        if (System.IO.Directory.Exists(contentPath))
        {
            //Add a route for each file in the directory and subdirectories
            foreach (string file in System.IO.Directory.GetFileSystemEntries(contentPath, "*", System.IO.SearchOption.AllDirectories))
            {
                string fileName = System.IO.Path.GetFileName(file);
                string filePath = path + "/" + fileName;

                AddRoute(new Route(filePath + "/" + fileName, Method.GET), (ctx) =>
                {
                    ctx.MustBeAuth = mustBeAuth;

                    if (!System.IO.File.Exists(fileName))
                    {
                        return ctx.ErrorPage(path + " does not exist.", 405);
                    }

                    Console.WriteLine("Serving " + fileName);

                    return ctx.Send(StaticResp.GetFileContent(file));
                });
            }
        }
        else
        {
            AddRoute(new Route(path, Method.GET), (ctx) =>
            {
                if (!System.IO.File.Exists(contentPath))
                {
                    return ctx.ErrorPage(path + " does not exist.", 404);
                }

                return ctx.Send(StaticResp.GetFileContent(contentPath));
            });
        }
        
    }

    private void CheckForJindiumContentPath()
    {
        //Check if the directory exists
        if (!System.IO.Directory.Exists(JindiumContentPath))
        {
            cText.WriteLine("JindiumContentPath does not exist! (" + JindiumContentPath + ")");
            cText.WriteLine("Creating JindiumContentPath...");

            System.IO.Directory.CreateDirectory(JindiumContentPath);
        }
    }

    public Routes(string contentPath = "JindiumSite")
    {
        JindiumContentPath = contentPath;

        CheckForJindiumContentPath();

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