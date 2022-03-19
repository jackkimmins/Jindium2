using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jindium
{
    public static class PreFabs
    {
        public static async Task Logout(Context ctx)
        {
            ctx.Session.IsAuthenticated = false;

            //Clear the session data
            ctx.Session.Clear();

            await ctx.Redirect("/");
        }
    }
}
