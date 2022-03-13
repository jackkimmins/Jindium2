using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jindium;

class cText
{
    //Output the text to the console.
    public static void WriteLine(string value, string threadName = "INFO", ConsoleColor consoleColor = ConsoleColor.Gray)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write($"[{DateTime.Now.ToString("HH:mm:ss")}] [Thread/{threadName}]: ");
        Console.ResetColor();

        Console.ForegroundColor = consoleColor;
        Console.WriteLine(value);
        Console.ResetColor();
    }
}
