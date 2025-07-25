using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortener.Infrastructure.Exceptions
{
    public class AboutInfoNotFoundException : Exception
    {
        public AboutInfoNotFoundException() : base("About information not found.") { }
    }
}
