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
            List<string> AllFiles = new List<string>();

            void ParsePath(string path)
            {
                AllFiles.AddRange(Directory.GetFiles(path));

                foreach (string subdir in Directory.GetDirectories(path))
                {
                    ParsePath(subdir);
                }
            }

            ParsePath(contentPath);

            //Add a route for each file in the directory and subdirectories
            foreach (string file in AllFiles)
            {
                string filePath = file.Replace(JindiumContentPath, "#=#");
                filePath = path + "/" + filePath.Substring(filePath.IndexOf("#=#\\") + 4).Replace("\\", "/");

                AddRoute(new Route(filePath, Method.GET, RouteType.Content), (ctx) =>
                {
                    string fullFilePath = System.IO.Path.GetFullPath(file);

                    string fileExtension = System.IO.Path.GetExtension(file);
                    ctx.ContentType = Utilities.GetContentType(fileExtension);

                    ctx.MustBeAuth = mustBeAuth;

                    if (!System.IO.File.Exists(fullFilePath))
                    {
                        Console.WriteLine("Does not exitst: " + fullFilePath);
                        return ctx.ErrorPage(path + " does not exist.", 405);
                    }

                    return ctx.SendFile(StaticResp.GetFileContent(fullFilePath));
                });
            }
        }
        else
        {
            AddRoute(new Route(path, Method.GET, RouteType.Static), (ctx) =>
            {
                if (!System.IO.File.Exists(contentPath))
                {
                    return ctx.ErrorPage(path + " does not exist.", 404);
                }

                return ctx.SendFile(StaticResp.GetFileContent(contentPath));
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

public enum RouteType
{
    Static,
    Content
}

public struct Route
{
    public string Path { get; private set; }
    public Method Method { get; private set; }
    public RouteType Type { get; private set; }

    public Route(string path, Method method = Method.GET, RouteType type = RouteType.Static)
    {
        Path = path;
        Method = method;
        Type = type;
    }
}