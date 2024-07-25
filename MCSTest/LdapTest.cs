using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Novell.Directory.Ldap;

namespace MCSTest
{
    public class LdapConfiguration
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string BindDn { get; set; }
        public string BindCredentials { get; set; }
        public string SearchBase { get; set; }
        public string SearchFilter { get; set; }
    }

    public class LdapTest
    {
        public LdapTest()
        {
        }

        public void Run(LdapConfiguration ldapConfig)
        {
            Console.WriteLine("Running ldap test ...");
            Console.WriteLine($"Configuration = {JsonConvert.SerializeObject(ldapConfig)}");
            using (var cn = new LdapConnection())
            {
                try
                {
                    cn.Connect(ldapConfig.Host, ldapConfig.Port);
                    // bind with an username and password
                    // this how you can verify the password of an user
                    cn.Bind(ldapConfig.BindDn, ldapConfig.BindCredentials);
                    Console.WriteLine("User is authenticated.");

                    //var searchFilter = "(&(objectCategory=person)(objectClass=user)(userPrincipalName={0}))";
                    //cn.SearchUsingSimplePaging(new SearchOptions(ldapConfig.SearchBase,
                    //    LdapConnection.ScopeSub, null, null), 1000);
                    var testUser = "daniel@indexim.co.id";

                    var lsc = (LdapSearchResults)cn.Search(ldapConfig.SearchBase,
                        LdapConnection.ScopeSub, string.Format(ldapConfig.SearchFilter, testUser), null, false);
                    if (lsc == null)
                        Console.WriteLine("searchResult is null");

                    while (lsc.HasMore())
                    {
                        LdapEntry ldapEntry = null;
                        try
                        {
                            ldapEntry = lsc.Next();
                        }
                        catch (LdapException ex)
                        {
                            Console.WriteLine(ex.Message);
                            continue;
                        }

                        if (ldapEntry != null)
                        {
                            var attributeSet = ldapEntry.GetAttributeSet();
                            if(attributeSet.TryGetValue("distinguishedName", out LdapAttribute dn))
                            {
                                Console.WriteLine($"distinguishedName = {dn.StringValue}");
                            }
                            if (attributeSet.TryGetValue("userPrincipalName", out LdapAttribute spn))
                            {
                                Console.WriteLine($"userPrincipalName = {spn.StringValue}");
                            }
                            if (attributeSet.TryGetValue("sAMAccountName", out LdapAttribute sam))
                            {
                                Console.WriteLine($"sAMAccountName = {sam.StringValue}");
                            }
                            if (attributeSet.TryGetValue("mail", out LdapAttribute mail))
                            {
                                Console.WriteLine($"mail = {mail.StringValue}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            Console.WriteLine("Ldap test done.");
            Console.ReadKey();
        }
    }
}
