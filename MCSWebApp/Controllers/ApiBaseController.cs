using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BusinessLogic;
using BusinessLogic.Utilities;
using DataAccess;
using DataAccess.DTO;
using DataAccess.EFCore;
using DataAccess.EFCore.Repository;
using MCSWebApp.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using NLog;
using PetaPoco;
using PetaPoco.Providers;

namespace MCSWebApp.Controllers
{
    [ApiController]
    public class ApiBaseController : ControllerBase, IActionFilter
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly string bearer = "Bearer ";
        protected readonly IConfiguration configuration;
        protected readonly IOptions<SysAdmin> sysAdminOption;
        protected string DefaultConnectionString { get; private set; }
        protected UserContext CurrentUserContext { get; private set; }
        protected DbContextOptionsBuilder<mcsContext> DbOptionBuilder { get; private set; }

        public ApiBaseController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
        {
            configuration = Configuration;
            sysAdminOption = SysAdminOption;
            DefaultConnectionString = configuration.GetConnectionString("MCS");
            DbOptionBuilder = new DbContextOptionsBuilder<mcsContext>();
            DbOptionBuilder.UseNpgsql(Configuration.GetConnectionString("MCS"))
                .UseLoggerFactory(MyEFCoreLogger.MyLoggerFactory);
        }

        [NonAction]
        public void OnActionExecuted(ActionExecutedContext context)
        {
            //throw new NotImplementedException();
        }

        [NonAction]
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ActionDescriptor.EndpointMetadata
                .Where(o => o.GetType() == typeof(AllowAnonymousAttribute))
                .Count() > 0)
            {
                using (var auth = new Authentication(DefaultConnectionString, configuration))
                {
                    try
                    {
                        var r = auth.Authenticate(sysAdminOption.Value.Username, sysAdminOption.Value.Password,
                            true, sysAdminOption.Value).GetAwaiter().GetResult();
                        CurrentUserContext = new UserContext()
                        {
                            OrganizationId = (string)r.Data?.OrganizationId,
                            AppUserId = (string)r.Data?.AppUserId,
                            AppUsername = (string)r.Data?.AppUsername,
                            AppFullname = (string)r.Data?.AppFullname,
                            IsSysAdmin = r.Data?.IsSysAdmin ?? false,
                            SystemAdministrator = sysAdminOption.Value,
                            TokenExpiry = r.Data?.TokenExpiry ?? DateTime.Now.AddHours(1),
                            AccessToken = (string)r.Data?.AccessToken,
                            ConnectionString = (string)r.Data?.ConnectionString,
                            BusinessUnitId = (string)r.Data?.BusinessUnitId

                        };

                        return;
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.ToString());
                        context.Result = new StatusCodeResult(401);
                    }
                }
            }

            if (context.HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues TokenHeader))
            {
                try
                {
                    var tokenData = JwtManager.GetPrincipal(TokenHeader.ToString().Substring(bearer.Length));
                    if (tokenData != null)
                    {
                        var claimUserData = tokenData.Claims.Where(o => o.Type == ClaimTypes.UserData).FirstOrDefault();
                        if (claimUserData != null)
                        {
                            CurrentUserContext = JsonConvert.DeserializeObject<UserContext>(claimUserData.Value);
                            logger.Debug($"CurrentUserContext = {JsonConvert.SerializeObject(CurrentUserContext)}");

                            // Extended validation
                            if (!StringHash.ValidateHash(CurrentUserContext.AppUsername, CurrentUserContext.AccessToken))
                            {
                                logger.Debug("UserContext AccessToken is not valid");
                                context.Result = new StatusCodeResult(401);
                                return;
                            }
                        }
                        else
                        {
                            logger.Debug("Jwt claimUserData is not valid");
                            context.Result = new StatusCodeResult(401);
                            return;
                        }
                    }
                    else
                    {
                        logger.Debug("Authorization TokenData does not exist");
                        context.Result = new StatusCodeResult(401);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    context.Result = new StatusCodeResult(401);
                }
            }
            else if (context.HttpContext.Request.Headers.TryGetValue("X-Api-Key", out StringValues ApiKey))
            {
                var apiKey = ApiKey.ToString();
                if (!string.IsNullOrEmpty(apiKey))
                {
                    using (var auth = new Authentication(DefaultConnectionString, configuration))
                    {
                        try
                        {
                            var r = auth.Authenticate(apiKey).GetAwaiter().GetResult();
                            if (r.Success)
                            {
                                CurrentUserContext = new UserContext()
                                {
                                    OrganizationId = (string)r.Data?.OrganizationId,
                                    AppUserId = (string)r.Data?.AppUserId,
                                    AppUsername = (string)r.Data?.AppUsername,
                                    AppFullname = (string)r.Data?.AppFullname,
                                    IsSysAdmin = r.Data?.IsSysAdmin ?? false,
                                    SystemAdministrator = sysAdminOption.Value,
                                    TokenExpiry = r.Data?.TokenExpiry ?? DateTime.Now.AddHours(1),
                                    AccessToken = (string)r.Data?.AccessToken,
                                    ConnectionString = (string)r.Data?.ConnectionString
                                };
                            }
                            else context.Result = new StatusCodeResult(401);

                            return;
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex.ToString());
                            context.Result = new StatusCodeResult(500);
                        }
                    }
                }
                else context.Result = new StatusCodeResult(401);

                return;
            }
            else
            {
                logger.Debug("Authorization or X-API-Key header does not exist");
                context.Result = new StatusCodeResult(401);
                return;
            }
        }
    }
}
