using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jindium;

public class Replacelets
{
    public List<Replacelets> replacelets = new List<Replacelets>();

    public static void CreateReplacelet(string name, Func<Replacelets, Task> action)
    {
        Replacelets replacelet = new Replacelets();
        replacelet.name = name;
        replacelet.action = action;
        replacelets.Add(replacelet);
    }
}
