using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using WebMarkupMin.Core;

namespace Jindium;

public static class StaticResp
{
    public static string FileTopComment()
    {
            return $@"<!--
   _ _           _ _
  (_(_)         | (_)
   _ _ _ __   __| |_ _   _ _ __ ___
  | | | '_ \ / _` | | | | | '_ ` _ \
  | | | | | | (_| | | |_| | | | | | |
  | |_|_| |_|\__,_|_|\__,_|_| |_| |_|
 _/ |
|__/

 - Jindium Server - Version " + JinServer.JindiumFrameworkVersion + $@" -

-->
";
    }

    public static string WelcomeTemplate(string siteName)
    {
        return @"<!DOCTYPE html><html lang=""en""><head> <meta charset=""UTF-8""> <meta http-equiv=""X-UA-Compatible"" content=""IE=edge""> <meta name=""viewport"" content=""width=device-width, initial-scale=1.0""> <title>" + siteName + @"</title> <style>body{margin:0;color:#444;background-color:#475bce;font-family:'Segoe UI',Tahoma,Geneva,Verdana,sans-serif;font-size:80%;width:100vw;height:100vh;display:flex;justify-content:center;align-items:center}h2{font-size:1.2em}#header,#page{-webkit-box-shadow:0 0 4px 0 rgba(0,0,0,.6);-moz-box-shadow:0 0 4px 0 rgba(0,0,0,.6);box-shadow:0 0 4px 0 rgba(0,0,0,.6)}#page{background-color:#FFF;width:40%;padding:20px}#header{padding:2px;text-align:center;background-color:#4e9d26;color:#FFF}#content{padding:4px 0 34px 0;border-bottom:5px #c54f4277 solid}a{text-decoration:none}</style></head><body> <div id=""page""> <div id=""header""> <h1>Welcome to Jindium!</h1> </div><div id=""content""> <h2>Welcome to this instance of Jindium!</h2> <p>This Jindium Server is called '" + siteName + @"'.</p> </div></div></body></html>";
    }

    public static string ErrorTemplate(string code, string siteName)
    {
        return @"<!DOCTYPE html><html lang=""en""><head> <meta charset=""UTF-8""> <meta http-equiv=""X-UA-Compatible"" content=""IE=edge""> <meta name=""viewport"" content=""width=device-width, initial-scale=1.0""> <title>" + code + @"</title> <style>body{margin:0;color:#444;background-color:#475bce;font-family:'Segoe UI',Tahoma,Geneva,Verdana,sans-serif;font-size:80%;width:100vw;height:100vh;display:flex;justify-content:center;align-items:center}h2{font-size:1.2em}#header,#page{-webkit-box-shadow:0 0 4px 0 rgba(0,0,0,.6);-moz-box-shadow:0 0 4px 0 rgba(0,0,0,.6);box-shadow:0 0 4px 0 rgba(0,0,0,.6)}#page{background-color:#FFF;width:40%;padding:20px}#header{padding:2px;text-align:center;background-color:#C55042;color:#FFF}#content{padding:4px 0 34px 0;border-bottom:5px #c54f4277 solid}a{text-decoration:none}</style></head><body> <div id=""page""> <div id=""header""> <h1>ERROR: " + code + @"</h1> </div><div id=""content""> <h2>" + siteName + @"</h2> <p>The requested URL was not found on this Jindium Server.</p><p>Please ensure that the URL is correct and try again.</p><a href=""/"">Return to Homepage</a> </div></div></body></html>";
    }

    public static string RemoveWhiteSpaceFromStylesheets(string body)
    {
        body = Regex.Replace(body, @"[a-zA-Z]+#", "#");
        body = Regex.Replace(body, @"[\n\r]+\s*", string.Empty);
        body = Regex.Replace(body, @"\s+", " ");
        body = Regex.Replace(body, @"\s?([:,;{}])\s?", "$1");
        body = body.Replace(";}", "}");
        body = Regex.Replace(body, @"([\s:]0)(px|pt|%|em)", "$1");

        // Remove comments from CSS
        body = Regex.Replace(body, @"/\*[\d\D]*?\*/", string.Empty);

        return body;
    }

    public static byte[] GetFileContent(string path, string contentType, bool shouldMinify = false)
    {
        byte[] fileContent = Encoding.UTF8.GetBytes("Something went wrong.");

        try
        {
            fileContent = System.IO.File.ReadAllBytes(path);

            if (shouldMinify)
            {
                if (contentType != "text/html" && contentType != "text/css" && contentType != "text/javascript") return fileContent;

                string content = Encoding.UTF8.GetString(fileContent);

                HtmlMinificationSettings settings = new HtmlMinificationSettings();
                settings.PreserveCase = true;
                settings.EmptyTagRenderMode = HtmlEmptyTagRenderMode.SpaceAndSlash;

                if (contentType == "text/html")
                {
                    var htmlMinifier = new HtmlMinifier(settings);
                    return Encoding.UTF8.GetBytes(FileTopComment() + htmlMinifier.Minify(content).MinifiedContent);
                }
                else if (contentType == "text/css")
                {
                    return Encoding.UTF8.GetBytes(RemoveWhiteSpaceFromStylesheets(content));
                }
            }

            return fileContent;
        }
        catch (Exception e)
        {
            cText.WriteLine(e.Message, "ERR", ConsoleColor.Red);
        }

        return fileContent;
    }
}