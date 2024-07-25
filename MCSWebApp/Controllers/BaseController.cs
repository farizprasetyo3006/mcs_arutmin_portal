using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using NLog;
using BusinessLogic;
using Microsoft.AspNetCore.Http;
using MCSWebApp.Extensions;
using BusinessLogic.Utilities;
using Microsoft.EntityFrameworkCore;
using DataAccess.EFCore.Repository;
using DataAccess.EFCore;

namespace MCSWebApp.Controllers
{
    public class BaseController : Controller
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        protected readonly IConfiguration configuration;

        public const int MaxSessionDurationInMinutes = 1440;
        public string WebAppName { get; private set; }
        public string RootBreadcrumb { get; private set; }
        public string DefaultConnectionString { get; private set; }
        public UserContext CurrentUserContext { get; private set; }
        protected DbContextOptionsBuilder<mcsContext> DbOptionBuilder { get; private set; }

        public BaseController(IConfiguration Configuration)
        {
            configuration = Configuration;
            WebAppName = "AIMS";
            RootBreadcrumb = "AIMS";

            DefaultConnectionString = configuration.GetConnectionString("MCS");
            DbOptionBuilder = new DbContextOptionsBuilder<mcsContext>();
            DbOptionBuilder.UseNpgsql(Configuration.GetConnectionString("MCS"))
                .UseLoggerFactory(MyEFCoreLogger.MyLoggerFactory);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                CurrentUserContext = HttpContext.Session.Get<UserContext>("UserContext");
                if (CurrentUserContext != null)
                {
                    logger.Debug("Context is not null");
                    logger.Debug($"Context.ConnectionString = {CurrentUserContext.ConnectionString}");
                }

                var returnUrl = HttpContext.Request.Path.ToString();
                if (HttpContext.Request.QueryString.HasValue)
                    returnUrl += HttpContext.Request.QueryString.ToString();

                if (!string.IsNullOrEmpty(CurrentUserContext?.AppUsername) && 
                    !string.IsNullOrEmpty(CurrentUserContext?.AccessToken))
                {
                    if(!StringHash.ValidateHash(CurrentUserContext?.AppUsername, CurrentUserContext?.AccessToken))
                    {
                        logger.Debug("AccessToken is invalid");
                        Response.Redirect($"/Authentication/Login/Index?ReturnUrl={returnUrl}");
                    }
                }
                else
                {
                    logger.Debug("AccessToken is empty");
                    Response.Redirect($"/Authentication/Login/Index?ReturnUrl={returnUrl}");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                Response.Redirect("/Authentication/Login/Index");
            }

            ViewBag.RootBreadcrumb = RootBreadcrumb;
            ViewBag.OrganizationId = CurrentUserContext?.OrganizationId;
            ViewBag.AppUserId = CurrentUserContext?.AppUserId;
            ViewBag.AppUsername = CurrentUserContext?.AppUsername;
            ViewBag.AppFullname = CurrentUserContext?.AppFullname;
            ViewBag.IsSysAdmin = CurrentUserContext?.IsSysAdmin;

            base.OnActionExecuting(context);
        }
    }
}
