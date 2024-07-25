using Microsoft.Extensions.Configuration;
using System;

namespace MCSTest
{
    class Program
    {
        public static IConfigurationRoot config;

        static void Main(string[] args)
        {
            config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();
            try
            {
                var test1 = new LdapTest();
                var section = config.GetSection(nameof(LdapConfiguration));
                var ldapConfig = section.Get<LdapConfiguration>();
                test1.Run(ldapConfig);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
