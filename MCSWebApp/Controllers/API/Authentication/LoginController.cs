using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using MCSWebApp.Controllers;
using Microsoft.Extensions.Configuration;
using Common;
using BusinessLogic;
using NLog;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using MCSWebApp.Middleware;
using Microsoft.AspNetCore.Authorization;
using DataAccess.DTO;
using System.Net.Mail;
using WebApp.Services;
using System.Dynamic;
using PetaPoco;
using PetaPoco.Providers;
using DataAccess.Repository;
using BusinessLogic.Utilities;
using Microsoft.AspNetCore.Http.Extensions;

namespace MCSWebApp.API.Authentication.Controllers
{
    [Route("api/Authentication/[controller]")]
    [ApiController]
    public class LoginController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly IHttpContextAccessor hca;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly SmtpClient smtpClient;
        private readonly LdapConfiguration ldapConfiguration;

        public LoginController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption,
            IHttpContextAccessor hca, IWebHostEnvironment webHostEnvironment, SmtpClient smtpClient) 
            : base(Configuration, SysAdminOption)
        {
            this.hca = hca;
            this.webHostEnvironment = webHostEnvironment;
            this.smtpClient = smtpClient;

            var ldapSection = configuration.GetSection("LdapConfiguration");
            ldapConfiguration = ldapSection?.Get<LdapConfiguration>();
        }

        [HttpPost("Submit")]
        [AllowAnonymous]
        public async Task<IActionResult> Submit([FromBody] UserCredential userCredential)
        {
            var result = new StandardResult();

            try
            {
                var auth = new BusinessLogic.Authentication(DefaultConnectionString, configuration);
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
                        IsSysAdmin = (bool)r.Data?.IsSysAdmin,
                        TokenExpiry = (DateTime)r.Data?.TokenExpiry,
                        AccessToken = (string)r.Data?.AccessToken,
                        ConnectionString = (string)r.Data?.ConnectionString,
                    };

                    var role = "User";
                    if (userCredential.SystemAdministration ?? false)
                    {
                        role = "System Administrator";
                        userContext.SystemAdministrator = sysAdminOption.Value;
                    }
                    else if (userContext.IsSysAdmin)
                    {
                        role = "Administrator";
                    }

                    var token = JwtManager.GenerateToken(userContext.AppUsername, role, userContext);
                    result.Data = new 
                    { 
                        id = userContext.AppUserId,
                        token
                    };
                    result.Success = true;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                result.Message = ex.Message;
            }

            return new JsonResult(result);
        }

        public class RoleList
        {
            public string id { get; set; }
            public string role_name { get; set; }
        }

        [HttpGet("RoleByUserName/{UserName}")]
        [AllowAnonymous]
        public async Task<IActionResult> RoleByUserName(string UserName)
        {
            var result = new StandardResult();

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
                        var sql = Sql.Builder.Append("select ar.id, ar.role_name from application_user au join user_role ur " +
                            "on au.id = ur.application_user_id join application_role ar on ar.id = ur.application_role_id " +
                            "where au.application_username = @0 order by ar.role_name", UserName);

                        var lostRole = await db.FetchAsync<RoleList>(sql);
                        if (lostRole != null)
                        {
                            result.Data = lostRole.ToList();
                            result.Success = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        result.Success = false;

                        logger.Debug(db.LastCommand);
                        logger.Error(ex.ToString());
                    }
                }
            }

            return new JsonResult(result);
        }

        [HttpGet("ForgotPassword")]
        [AllowAnonymous]
        public IActionResult ForgotPassword([FromQuery] string Id)
        {
            var result = new StandardResult();

            try
            {
                if (!string.IsNullOrEmpty(Id))
                {
                    logger.Debug($"DefaultConnectionString = {DefaultConnectionString}");
                    using (var db = DatabaseConfiguration
                            .Build()
                            .UsingConnectionString(DefaultConnectionString)
                            .UsingProvider<PostgreSQLDatabaseProvider>()
                            .Create()) 
                    {
                        try
                        {
                            var appUser = db.FirstOrDefault<application_user>(
                                "WHERE application_username = @0 OR email = @1", Id, Id);
                            logger.Debug(db.LastCommand);
                            if(appUser != null)
                            {
                                if(!string.IsNullOrEmpty(appUser.email))
                                {
                                    var rnd = new Random();
                                    var accessToken = StringHash.CreateHash(appUser.id, rnd.Next(10000, 100000));

                                    var sql = Sql.Builder.Append("UPDATE application_user");
                                    sql.Append("SET access_token = @0", accessToken);
                                    sql.Append(", token_expiry = @0", DateTime.Now.AddDays(7));
                                    sql.Append("WHERE id = @0", appUser.id);
                                    var ar = db.Execute(sql);
                                    logger.Debug(db.LastCommand);
                                    logger.Debug($"Affected row = {ar}");

                                    if (ar > 0)
                                    {
                                        var qb = new QueryBuilder();
                                        qb.Add("AccessToken", accessToken);
                                        var url = $"{hca.HttpContext.Request.Scheme}://{hca.HttpContext.Request.Host}"
                                            + $"/Authentication/Login/ResetPassword{qb.ToQueryString()}";

                                        #region Send email

                                        using (var mailMessage = new MailMessage(
                                            from: configuration["Email:EmailAddress:Default"],
                                            to: appUser.email))
                                        {
                                            mailMessage.Subject = "Reset Password - Smart Mining";

                                            var msg = "";
                                            var fileTemplate = configuration["Email:ResetPassword:Template"];
                                            fileTemplate = $"{webHostEnvironment.WebRootPath}\\media\\template\\{fileTemplate}";
                                            logger.Debug($"fileTemplate = {fileTemplate}");

                                            if (System.IO.File.Exists(fileTemplate))
                                            {
                                                msg = System.IO.File.ReadAllText(fileTemplate);
                                                msg = msg?.Replace("{{reset-password-url}}", url);
                                            }
                                            else
                                            {
                                                logger.Error($"File template {fileTemplate} does not exist");
                                                result.Message = "Contact your administrator";
                                            }

                                            // Send email
                                            try
                                            {
                                                if (!string.IsNullOrEmpty(msg))
                                                {
                                                    mailMessage.Body = msg;
                                                    mailMessage.IsBodyHtml = true;

                                                    smtpClient.Send(mailMessage);
                                                    result.Success = true;
                                                }
                                                else
                                                {
                                                    logger.Debug("Email template is empty");
                                                    result.Message = "Contact your administrator";
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                logger.Error(ex.ToString());
                                                result.Message = ex.Message;
                                            }
                                        }

                                        #endregion
                                    }
                                }
                                else
                                {
                                    logger.Debug("User's email is empty");
                                    result.Message = "Contact your administrator";
                                }
                            }
                            else
                            {
                                logger.Debug("User does not exist");
                                result.Message = "Contact your administrator";
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Debug(db.LastCommand);
                            logger.Error(ex.ToString());
                            result.Message = ex.Message;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                result.Message = ex.Message;
                result.Success = false;
            }

            return new JsonResult(result);
        }

        [HttpGet("ResetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromQuery] string Id, [FromQuery] string NewPassword)
        {
            var result = new StandardResult();

            try
            {
                if (!string.IsNullOrEmpty(Id))
                {
                    var auth = new BusinessLogic.Authentication(DefaultConnectionString, configuration);
                    result = await auth.ResetPassword(Id, NewPassword);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                result.Message = ex.Message;
            }

            return new JsonResult(result);
        }

    }
}
