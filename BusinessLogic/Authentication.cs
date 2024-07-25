using BusinessLogic.Utilities;
using DataAccess.Repository;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NLog;
using PetaPoco;
using PetaPoco.Providers;
using System;
using System.Dynamic;
using System.Threading.Tasks;
using DataAccess;
using DataAccess.DTO;
using System.Diagnostics;
using Novell.Directory.Ldap;
using Common;

namespace BusinessLogic
{
    public partial class Authentication : IDisposable
    {
        private static readonly NLog.Logger logger = LogManager.GetCurrentClassLogger();
        private readonly IDatabase db;
        private readonly IConfiguration config;
        private readonly bool localDatabaseInstance;

        public Authentication(IDatabase Database, IConfiguration Configuration)
        {
            db = Database;
            localDatabaseInstance = false;

            config = Configuration;
        }

        public Authentication(string ConnectionString, IConfiguration Configuration)
        {
            db = DatabaseConfiguration
                .Build()
                .UsingConnectionString(ConnectionString)
                .UsingProvider<PostgreSQLDatabaseProvider>()
                .Create();
            localDatabaseInstance = true;

            config = Configuration;
        }

        public async Task<StandardResult> Authenticate(string Username, string Password, 
            bool? IsSystemAdministration = null, SysAdmin SystemAdministration = null,
            LdapConfiguration ldapConfiguration = null, string OrganizationId = null)
        {
            StandardResult result = new StandardResult();
            application_user applicationUser = null;
            bool authenticated = false;

            try
            {
                if (string.IsNullOrEmpty(Username))
                {
                    logger.Debug("Username is empty");
                    return result;
                }
                
                try
                {
                    var sql = Sql.Builder.Append("SELECT * FROM application_user");
                    sql.Append("WHERE is_active = TRUE AND application_username = @0", Username);
                    if (!string.IsNullOrEmpty(OrganizationId))
                    {
                        sql.Append("AND organization_id = @0", OrganizationId);
                    }

                    applicationUser = await db.FirstOrDefaultAsync<application_user>(sql);
                    if (applicationUser != null)
                    {
                        logger.Debug($"User {Username} exists.");

                        if ((applicationUser.use_ldap ?? false) 
                            && (ldapConfiguration?.Enabled ?? false))
                        {
                            logger.Debug("Authenticating user using LDAP ...");
                            using (var cn = new LdapConnection())
                            {
                                try
                                {
                                    cn.Connect(ldapConfiguration.Host, ldapConfiguration.Port);
                                    cn.Bind(ldapConfiguration.BindDn, ldapConfiguration.BindCredentials);

                                    using (var cn2 = new LdapConnection())
                                    {
                                        cn2.Connect(ldapConfiguration.Host, ldapConfiguration.Port);
                                        cn2.Bind(Username, Password);
                                        authenticated = true;
                                    }

                                }
                                catch (Exception ex)
                                {
                                    logger.Error(ex);
                                    result.Message = ex.Message;
                                }
                            }
                        }
                        else
                        {
                            logger.Debug("Authenticating user using hashed password ...");
                            authenticated = StringHash.ValidateHash(Password, applicationUser.application_password);
                        }

                        if (authenticated)
                        {
                            logger.Debug($"User {Username} credential is valid");

                            var _organization = await db.FirstOrDefaultAsync<organization>(
                                "WHERE is_active = TRUE AND id = @0", applicationUser.organization_id);
                            if (_organization != null)
                            {
                                var dt = DateTime.Now.AddDays(10);
                                applicationUser.token_expiry = new DateTime(dt.Year, dt.Month, dt.Day,
                                    dt.Hour, dt.Minute, dt.Second);

                                result.Data = new ExpandoObject();
                                result.Data.OrganizationId = applicationUser.organization_id;
                                result.Data.AppUserId = applicationUser.id;
                                result.Data.AppUsername = applicationUser.application_username;
                                result.Data.AppFullname = applicationUser.fullname;
                                result.Data.AppEmail = applicationUser.email;
                                result.Data.IsSysAdmin = applicationUser.is_sysadmin;
                                result.Data.TokenExpiry = applicationUser.token_expiry;
                                result.Data.BusinessUnitId = applicationUser.business_unit_id;

                                //result.Data.ConnectionString = Encryptor.EncryptString(
                                //    _organization?.connection_string ?? db.ConnectionString);
                                result.Data.ConnectionString = Encryptor.EncryptString(db.ConnectionString);

                                applicationUser.access_token = StringHash.CreateHash(Username, 
                                    (int)(1000 + DateTime.Now.Ticks % 1000));
                                result.Data.AccessToken = applicationUser.access_token;

                                db.Update(applicationUser);
                                logger.Trace(db.LastCommand);

                                result.Success = true;

                                logger.Debug("User token created");
                            }
                            else
                            {
                                logger.Debug("User's organization does not exist");
                            }
                        }
                        else
                        {
                            logger.Debug("Invalid password");
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                }

                if(applicationUser == null && SystemAdministration != null)
                {
                    if (SystemAdministration.Username == Username && SystemAdministration.Password == Password)
                    {
                        if (SystemAdministration.IsEnabled && (IsSystemAdministration ?? false))
                        {
                            result.Data = new ExpandoObject();
                            result.Data.OrganizationId = Guid.Empty.ToString("N");
                            result.Data.AppUserId = Guid.Empty.ToString("N");
                            result.Data.AppUsername = Username;
                            result.Data.AppFullname = Username;
                            result.Data.AppEmail = "";
                            result.Data.IsSysAdmin = true;
                            result.Data.BusinessUnitId = "";

                            var dt = DateTime.Now.AddMinutes(10);
                            result.Data.TokenExpiry = new DateTime(dt.Year, dt.Month, dt.Day,
                                dt.Hour, dt.Minute, dt.Second);                            
                            result.Data.AccessToken = StringHash.CreateHash(Username, 
                                (int)(1000 + DateTime.Now.Ticks % 1000));
                            result.Data.ConnectionString = Encryptor.EncryptString(db.ConnectionString);

                            result.Success = true;
                            logger.Debug("User token for sysadmin created");
                        }
                        else
                        {
                            logger.Debug("Sysadmin is not enabled");
                        }
                    }
                    else
                    {
                        logger.Debug("Invalid username or password");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug(db.LastCommand);
                logger.Error(ex.ToString());

                result.Message = ex.Message;
            }

            logger.Debug($"{JsonConvert.SerializeObject(result)}");
            return result;
        }

        public StandardResult FindLDAPUser(string Username, LdapConfiguration ldapConfiguration)
        {
            StandardResult result = new StandardResult();

            using (var cn = new LdapConnection())
            {
                try
                {
                    cn.Connect(ldapConfiguration.Host, ldapConfiguration.Port);
                    cn.Bind(ldapConfiguration.BindDn, ldapConfiguration.BindCredentials);

                    var lsc = (LdapSearchResults)cn.Search(ldapConfiguration.SearchBase, LdapConnection.ScopeSub, 
                        string.Format(ldapConfiguration.SearchFilter, Username), null, false);
                    if (lsc == null)
                    {
                        logger.Error("Search result is null");
                        result.Message = "Search result is null";
                        return result;
                    }

                    while (lsc.HasMore())
                    {
                        LdapEntry ldapEntry = null;
                        try
                        {
                            ldapEntry = lsc.Next();
                        }
                        catch (LdapException ex)
                        {
                            logger.Debug(ex.Message);
                            continue;
                        }

                        if (ldapEntry != null)
                        {
                            result.Success = true;
                            result.Data = new ExpandoObject();

                            var attributeSet = ldapEntry.GetAttributeSet();
                            if (attributeSet.TryGetValue("distinguishedName", out LdapAttribute dn))
                            {
                                logger.Debug($"distinguishedName = {dn.StringValue}");
                                result.Data.distinguishedName = dn.StringValue;
                            }
                            if (attributeSet.TryGetValue("userPrincipalName", out LdapAttribute pn))
                            {
                                logger.Debug($"userPrincipalName = {pn.StringValue}");
                                result.Data.userPrincipalName = pn.StringValue;
                            }
                            if (attributeSet.TryGetValue("sAMAccountName", out LdapAttribute sam))
                            {
                                logger.Debug($"sAMAccountName = {sam.StringValue}");
                                result.Data.sAMAccountName = sam.StringValue;
                            }
                            if (attributeSet.TryGetValue("mail", out LdapAttribute mail))
                            {
                                logger.Debug($"mail = {mail.StringValue}");
                                result.Data.mail = mail.StringValue;
                            }

                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    result.Message = ex.Message;
                }
            }

            logger.Debug($"{JsonConvert.SerializeObject(result)}");
            return result;
        }

        public async Task<StandardResult> Authenticate(string ApiKey)
        {
            StandardResult result = new StandardResult();            

            try
            {
                if (string.IsNullOrEmpty(ApiKey))
                {
                    logger.Debug("API key is empty");
                    return result;
                }

                try
                {
                    var applicationUser = await db.FirstOrDefaultAsync<application_user>(
                        "WHERE is_active = TRUE AND api_key = @0 AND expired_date > @1", ApiKey, DateTime.Now.Date);
                    if (applicationUser != null)
                    {
                        logger.Trace("User's API key is valid");

                        var _organization = await db.FirstOrDefaultAsync<organization>(
                            "WHERE is_active = TRUE AND id = @0", applicationUser.organization_id);
                        if (_organization != null)
                        {
                            var dt = DateTime.Now.AddDays(10);
                            applicationUser.token_expiry = new DateTime(dt.Year, dt.Month, dt.Day,
                                dt.Hour, dt.Minute, dt.Second);

                            result.Data = new ExpandoObject();
                            result.Data.OrganizationId = applicationUser.organization_id;
                            result.Data.AppUserId = applicationUser.id;
                            result.Data.AppUsername = applicationUser.application_username;
                            result.Data.AppFullname = applicationUser.fullname;
                            result.Data.AppEmail = applicationUser.email;
                            result.Data.IsSysAdmin = applicationUser.is_sysadmin;
                            result.Data.TokenExpiry = applicationUser.token_expiry;

                            //result.Data.ConnectionString = Encryptor.EncryptString(
                            //    _organization?.connection_string ?? db.ConnectionString);
                            result.Data.ConnectionString = Encryptor.EncryptString(db.ConnectionString);

                            applicationUser.access_token = StringHash.CreateHash(ApiKey,
                                (int)(1000 + DateTime.Now.Ticks % 1000));
                            result.Data.AccessToken = applicationUser.access_token;

                            db.Update(applicationUser);
                            logger.Trace(db.LastCommand);

                            result.Success = true;

                            logger.Debug("User token created");
                        }
                        else
                        {
                            logger.Debug("User's organization does not exist");
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                }
            }
            catch (Exception ex)
            {
                logger.Debug(db.LastCommand);
                logger.Error(ex.ToString());

                result.Message = ex.Message;
            }

            logger.Debug($"{JsonConvert.SerializeObject(result)}");
            return result;
        }

        public StandardResult ForgotPassword(string LostAccount)
        {
            var result = new StandardResult();
            
            var sw = new Stopwatch();
            sw.Start();
            logger.Debug($"Start: {DateTime.Now}");

            try
            {
                if (string.IsNullOrEmpty(LostAccount))
                {
                    logger.Debug("LostAccount is empty");
                    return result;
                }

                var applicationUser = db.FirstOrDefault<application_user>(
                    "WHERE is_active = TRUE AND (application_username = @0 OR email = @0)", LostAccount);
                logger.Debug(db.LastCommand);

                if (applicationUser != null)
                {
                    var rnd = new Random();
                    var accessToken = StringHash.CreateHash(applicationUser.id, rnd.Next(10000, int.MaxValue));

                    result.Data = new ExpandoObject();
                    result.Data.AccessToken = accessToken;
                    result.Data.Email = applicationUser.email;

                    applicationUser.access_token = accessToken;
                    applicationUser.token_expiry = DateTime.Now.AddDays(1);

                    var ar = db.Update(applicationUser);
                    logger.Debug(db.LastCommand);

                    result.Success = ar > 0;
                }
                else
                {
                    logger.Debug("Application user is not found");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }


            sw.Stop();
            logger.Debug($"Elapsed: {sw.Elapsed}");

            return result;
        }

        public async Task<StandardResult> ResetPassword(string Id, string NewPassword)
        {
            var result = new StandardResult();

            try
            {
                if (string.IsNullOrEmpty(Id))
                {
                    logger.Debug("Id is empty");
                    return result;
                }

                if (string.IsNullOrEmpty(NewPassword))
                {
                    logger.Debug("Id is empty");
                    return result;
                }

                var applicationUser = await db.FirstOrDefaultAsync<application_user>(
                    "WHERE is_active = TRUE AND access_token = @0 AND token_expiry > @1", Id, DateTime.Now);
                logger.Debug(db.LastCommand);

                if (applicationUser != null)
                {
                    applicationUser.application_password = StringHash.CreateHash(NewPassword);
                    applicationUser.access_token = Guid.NewGuid().ToString("N");
                    applicationUser.token_expiry = DateTime.Now;

                    var ar = await db.UpdateAsync(applicationUser);
                    logger.Debug(db.LastCommand);

                    result.Success = ar > 0;
                }
                else
                {
                    logger.Debug("Invalid attempt");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                result.Message = ex.Message;
            }

            return result;
        }

        public void Dispose()
        {
            if (localDatabaseInstance && db != null)
            {
                db.Dispose();
            }
        }
    }
}