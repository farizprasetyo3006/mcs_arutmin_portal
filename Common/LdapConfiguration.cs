using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class LdapConfiguration
    {
        public bool Enabled { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string BindDn { get; set; }
        public string BindCredentials { get; set; }
        public string SearchBase { get; set; }
        public string SearchFilter { get; set; }
    }
}
