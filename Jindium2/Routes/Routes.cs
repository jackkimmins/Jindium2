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
    private Dictionary<string, byte[]> ContentCache = new Dictionary<string, byte[]>();
    public void ClearContentCache()
    {
        ContentCache.Clear();
    }
    public List<string> ListContentCache()
    {
        return ContentCache.Keys.ToList();
    }

    //Get the total size of the content cache in bytes
    public long GetContentCacheSize()
    {
        long size = 0;

        foreach (KeyValuePair<string, byte[]> entry in ContentCache)
        {
            size += entry.Value.Length;
        }

        return size;
    }

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
        //contentPath = contentPath.Replace("/", "\\");
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
                filePath = (path == "/" ? "/" : path + "/") + filePath.Substring(filePath.Replace("\\", "/").IndexOf("#=#/") + 4).Replace("\\", "/");

                if (filePath.EndsWith("/index.html"))
                {
                    filePath = filePath.Substring(0, filePath.Length - "/index.html".Length);
                }

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

                    if (!ContentCache.ContainsKey(fullFilePath))
                    {
                        ContentCache.Add(fullFilePath, StaticResp.GetFileContent(fullFilePath, ctx.ContentType, true));
                    }

                    return ctx.SendFile(ContentCache[fullFilePath]);
                });
            }
        }
        else
        {
            AddRoute(new Route(path, Method.GET, RouteType.Static), (ctx) =>
            {
                if (!System.IO.File.Exists(contentPath))
                {
                    cText.WriteLine(contentPath + " does not exist!", "ERR", ConsoleColor.Red);
                    return ctx.ErrorPage(path + " does not exist.", 404);
                }

                if (!ContentCache.ContainsKey(contentPath))
                {
                    ContentCache.Add(contentPath, StaticResp.GetFileContent(contentPath, Utilities.GetContentType(System.IO.Path.GetExtension(contentPath)), true));
                }

                return ctx.SendFile(ContentCache[contentPath]);
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