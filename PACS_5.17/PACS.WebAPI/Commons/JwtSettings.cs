using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PACS.WebAPI
{
    public class JwtSettings
    {
        public string SecurityKey { get; set; }
        public TimeSpan ExpiresIn { get; set; }
    }
}
