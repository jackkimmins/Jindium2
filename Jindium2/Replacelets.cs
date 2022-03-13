using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jindium;

public class Replacelets
{
    public Dictionary<string, Func<string, string>> ReplaceletDictionary { get; private set; } = new Dictionary<string, Func<string, string>>();

    public void AddReplacelet(string name, Func<string, string> action)
    {
        if (ReplaceletDictionary.ContainsKey(name))
        {
            if (name != "/")
                cText.WriteLine("A replacelet with the same name already exists! (" + name + ") Overwriting...");

            ReplaceletDictionary[name] = action;
            return;
        }

        ReplaceletDictionary.Add(name, action);
    }
}