﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic;
using BusinessLogic.Utilities;
using MCSWebApp.Extensions;
using MCSWebApp.Middleware;
using MCSWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NLog;
using DataAccess;
using DataAccess.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using PetaPoco;
using PetaPoco.Providers;
using DataAccess.Repository;
using Common;

namespace MCSWebApp.Areas.Authentication.Controllers
{
    [Area("Authentication")]
    public class LoginController : Controller
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly IConfiguration configuration;
        private readonly IOptions<SysAdmin> sysAdminOption;
        private readonly LdapConfiguration ldapConfiguration;

        public LoginController(IConfiguration configuration, IOptions<SysAdmin> sysAdminOption)
        {
            this.configuration = configuration;
            this.sysAdminOption = sysAdminOption;

            var ldapSection = configuration.GetSection("LdapConfiguration");
            ldapConfiguration = ldapSection?.Get<LdapConfiguration>();
        }

        [HttpGet]
        public IActionResult Index([FromQuery] string ReturnUrl)
        {
            //ViewBag.ListRole = new List<string>();

            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpGet]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult ResetPassword([FromQuery] string AccessToken)
        {
            if (!string.IsNullOrEmpty(AccessToken))
            {
                var defaultConnectionString = configuration.GetConnectionString("MCS");
                if (!string.IsNullOrEmpty(defaultConnectionString))
                {
                    using (var db = DatabaseConfiguration
                            .Build()
                            .UsingConnectionString(defaultConnectionString)
                            .UsingProvider<PostgreSQLDatabaseProvider>()
                            .Create())
                    {
                        try
                        {
                            var appUser = db.FirstOrDefault<application_user>("WHERE access_token = @0 AND token_expiry > @1",
                                AccessToken, DateTime.Now);
                            if (appUser != null)
                            {
                                if(StringHash.ValidateHash(appUser.id, AccessToken))
                                {
                                    ViewBag.Id = AccessToken;
                                    return View();                                
                                }
                                else
                                {
                                    logger.Debug($"User's access_token {AccessToken} is not valid");
                                }
                            }
                            else
                            {
                                logger.Debug($"User with access_token {AccessToken} is not found");
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Debug(db.LastCommand);
                            logger.Error(ex.ToString());                            
                        }
                    }
                }                
            }

            return Redirect("/Home/Index");
        }

        [HttpPost]
        public async Task<IActionResult> Submit(UserCredential userCredential)
        {
            var business_unit_id = "";
            var redirectionUrl = userCredential?.ReturnUrl ?? "/Authentication/Login/Index";

            try
            {
                var defaultConnectionString = configuration.GetConnectionString("MCS");
                if (!string.IsNullOrEmpty(defaultConnectionString))
                {
                    using (var auth = new BusinessLogic.Authentication(defaultConnectionString, configuration))
                    {
                        try
                        {
                            logger.Debug($"userCredential = {JsonConvert.SerializeObject(userCredential)}");
                            logger.Debug($"sysAdminOption.Value = {JsonConvert.SerializeObject(sysAdminOption.Value)}");

                            using (var db = DatabaseConfiguration
                                    .Build()
                                    .UsingConnectionString(defaultConnectionString)
                                    .UsingProvider<PostgreSQLDatabaseProvider>()
                                    .Create())
                            {
                                if (!string.IsNullOrEmpty(userCredential.Username))
                                {
                                    var sql = $"select uro.application_role_id from application_user au join user_role uro on " +
                                        $"uro.application_user_id = au.id where au.application_username = @0" ;

                                    var recRole = db.FirstOrDefault<dynamic>(sql, userCredential.Username);
                                    if (recRole != null)
                                    {
                                        GlobalVars.ROLE_ID = recRole.application_role_id;
                                        userCredential.RoleId = recRole.application_role_id;

                                        var businessUnit = db.FirstOrDefault<role_business_unit>("WHERE application_role_id = @0 ", userCredential.RoleId);
                                        if (businessUnit != null)
                                        {
                                            GlobalVars.BUSINESS_UNIT_ID = businessUnit.business_unit_id;
                                            business_unit_id = businessUnit.business_unit_id;

                                            HttpContext.Session.SetString("BUSINESS_UNIT_ID",businessUnit.business_unit_id);
                                        }
                                    }
                                }
                            }

                            var r = await auth.Authenticate(userCredential.Username, userCredential.Password, 
                                userCredential.SystemAdministration, sysAdminOption.Value, ldapConfiguration,
                                userCredential.OrganizationId);
                            if (r.Success)
                            {
                                var userContext = new UserContext
                                {
                                    OrganizationId = (string)r.Data?.OrganizationId,
                                    AppUserId = (string)r.Data?.AppUserId,
                                    AppUsername = (string)r.Data?.AppUsername,
                                    AppFullname = (string)r.Data?.AppFullname,
                                    IsSysAdmin = (bool?)r.Data?.IsSysAdmin ?? false,
                                    TokenExpiry = (DateTime?)r.Data?.TokenExpiry ?? DateTime.Now,
                                    AccessToken = (string)r.Data?.AccessToken,
                                    ConnectionString = (string)r.Data?.ConnectionString,
                                    RoleId = userCredential.RoleId,
                                    BusinessUnitId = business_unit_id
                                };

                                var role = "User";
                                if(userCredential.SystemAdministration ?? false)
                                {
                                    role = "System Administrator";
                                    userContext.SystemAdministrator = sysAdminOption.Value;
                                }
                                else if (userContext.IsSysAdmin)
                                {
                                    role = "Administrator";
                                }

                                logger.Debug($"Role = {role}");

                                HttpContext.Session.Set<UserContext>("UserContext", userContext);
                                var token = JwtManager.GenerateToken(userContext.AppUsername, role, userContext);
                                HttpContext.Response.Cookies.Append("Token", token);

                                ViewBag.AppUsername = userCredential.Username;

                                using (var db = DatabaseConfiguration
                                        .Build()
                                        .UsingConnectionString(defaultConnectionString)
                                        .UsingProvider<PostgreSQLDatabaseProvider>()
                                        .Create())
                                {
                                    try
                                    {
                                        var sql = $"select id, role_name from application_role where organization_id = @0";

                                        List <dynamic> RoleList = db.Fetch<dynamic>(sql, userContext.OrganizationId);
                                        if (RoleList != null)
                                        {
                                            ViewBag.RoleList = RoleList.ToList();
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        logger.Debug(db.LastCommand);
                                        logger.Error(ex.ToString());
                                    }
                                }

                                redirectionUrl = userCredential?.ReturnUrl ?? "/Data/Dashboard/Index";
                            }
                            else
                            {
                                ViewBag.LoginStatus = "Wrong user name or password.";
                                return View("Index");
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex.ToString());                            
                        }
                    }
                }
                else
                {
                    logger.Error("Default connection string MCS is empty");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());                
            }

            return Redirect(redirectionUrl);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            var redirectionUrl = "/Authentication/Login/Index";

            try
            {
                HttpContext.Response.Cookies.Append("Token", "");
                HttpContext.Session.Clear();
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());                
            }

            return Redirect(redirectionUrl);
        }
    }
}
