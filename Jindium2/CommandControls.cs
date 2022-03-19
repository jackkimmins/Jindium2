using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jindium
{
    public partial class JinServer
    {
        public bool ExecuteCommand(string? commandInput)
        {
            if (!string.IsNullOrEmpty(commandInput))
                cText.WriteLine("Please enter a valid command. Type 'help' for help.", "CMD", ConsoleColor.Blue);

            switch (commandInput)
            {
                case "help":
                    cText.WriteLine("Available Jindium Commands:", "CMD", ConsoleColor.Blue);
                    cText.WriteLine("help - Shows this help", "CMD", ConsoleColor.Blue);
                    cText.WriteLine("clear - Clears the console", "CMD", ConsoleColor.Blue);
                    cText.WriteLine("stop - Stops the server", "CMD", ConsoleColor.Blue);
                    cText.WriteLine("status - Shows the server status", "CMD", ConsoleColor.Blue);
                    cText.WriteLine("sessions - Shows the sessions", "CMD", ConsoleColor.Blue);
                    break;
                case "stop":
                    IsServerRunning = false;
                    return true;
                case "clear":
                    Console.Clear();
                    break;
                case "status":
                    break;
                case "sessions":
                    if (Sessions.SessionsActive == false)
                    {
                        cText.WriteLine("Sessions are not active!", "CMD", ConsoleColor.Red);
                        break;
                    }

                    cText.WriteLine("Active Jindium Sessions:", "CMD", ConsoleColor.Blue);
                    if (Sessions.SessionsData.Count == 0)
                    {
                        cText.WriteLine("- No Active Sessions", "CMD", ConsoleColor.Blue);
                    }
                    else
                    {
                        foreach (var session in Sessions.SessionsData)
                        {
                            cText.WriteLine("- Session ID: " + session.Key, "CMD", ConsoleColor.Blue);
                            cText.WriteLine("- Session Data: ", "CMD", ConsoleColor.Blue);

                            if (session.Value.Count > 0)
                            {
                                foreach (var data in session.Value)
                                {
                                    cText.WriteLine("  - " + data.Key + ": " + data.Value, "CMD", ConsoleColor.Blue);
                                }
                            }
                            else
                            {
                                cText.WriteLine("  - No data", "CMD", ConsoleColor.Blue);
                            }

                            Console.WriteLine();
                        }
                    }
                    break;
                default:
                    cText.WriteLine("Please enter a valid command. Type 'help' for help.", "CMD", ConsoleColor.Blue);
                    break;
            }

            return false;
        }
    }
}
