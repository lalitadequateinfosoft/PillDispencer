using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PillDispencer.Helper
{
    public static class Urls
    {
        public static string BaseAddress = "https://api.adequatetravel.com/jupiterapi/";
        public static string Url
        {
            get
            {
                return $"{BaseAddress}/api/";
            }

        }
    }
}
