using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Entity;
using MCSWebApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NLog;
using Common;
using PetaPoco;
using DataAccess.Repository;

namespace MCSWebApp.Areas.SystemAdministration.Controllers
{
    [Area("SystemAdministration")]
    public class ApplicationUserController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public ApplicationUserController(IConfiguration Configuration)
            : base(Configuration)
        {
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.UserSecurityManagement];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.SystemAdministration];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ApplicationUser];
            ViewBag.BreadcrumbCode = WebAppMenu.ApplicationUser;

            return View();
        }

        public async Task<IActionResult> Detail(string Id)
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.UserSecurityManagement];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.SystemAdministration];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ApplicationUser];
            ViewBag.BreadcrumbCode = WebAppMenu.ApplicationUser;

            if (!string.IsNullOrEmpty(Id))
            {
                try
                {
                    using (var db = CurrentUserContext.GetDataContext().Database)
                    {
                        try
                        {
                            var sql = Sql.Builder.Append("SELECT * FROM application_user WHERE id = @0", Id);
                            var appUser = await db.FirstOrDefaultAsync<application_user>(sql);
                            if(appUser != null)
                            {
                                ViewBag.Id = Id;                                
                                ViewBag.IsSysAdmin = CurrentUserContext.IsSysAdmin;
                                ViewBag.CurrentUserId = CurrentUserContext.AppUserId;
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Debug(db.LastCommand);
                            logger.Error(ex.ToString());                            
                        }
                    }                    
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                }
            }

            return View();
        }
    }
}
