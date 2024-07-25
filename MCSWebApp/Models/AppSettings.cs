using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MCSWebApp.Models
{
    public class AppSettings
    {
        public string BaseUrl { get; set; }
        public string ApiBaseUrl { get; set; }
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
    }
}
