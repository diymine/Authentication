using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server
{
    public static class Constans
    {
        public const string Issuer = Audiance;
        public const string Audiance = "https://localhost:44330/";
        public const string Secret = "not_too_short_othewise_might_errors";
    }
}
