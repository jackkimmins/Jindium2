using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jindium
{
    public static partial class Utilities
    {
        public static string BytesToFormattedString(long bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
            int i = 0;
            double dblSByte = bytes;
            while (bytes >= 1024)
            {
                bytes /= 1024;
                i++;
            }
            return String.Format("{0:0.##} {1}", bytes, suffixes[i]);
        }
    }
}
