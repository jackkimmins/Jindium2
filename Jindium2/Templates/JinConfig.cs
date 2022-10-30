using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Jindium;

public class JinConfig
{
    public string ServerName { get; set; } = "Jindium";
    public string Address { get; set; } = "http://localhost:5000/";
    public bool Logging { get; set; } = false;
}