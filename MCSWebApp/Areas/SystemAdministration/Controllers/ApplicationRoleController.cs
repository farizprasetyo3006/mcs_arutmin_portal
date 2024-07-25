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

namespace MCSWebApp.Areas.SystemAdministration.Controllers
{
    [Area("SystemAdministration")]
    public class ApplicationRoleController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public ApplicationRoleController(IConfiguration Configuration)
            : base(Configuration)
        {
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.UserSecurityManagement];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.SystemAdministration];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ApplicationRole];
            ViewBag.BreadcrumbCode = WebAppMenu.ApplicationRole;

            return View();
        }

        public IActionResult RoleAccess(string Id)
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.UserSecurityManagement];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.SystemAdministration];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ApplicationRole];
            ViewBag.BreadcrumbCode = WebAppMenu.ApplicationRole;

            ViewBag.ApplicationRoleId = Id;

            return View();
        }
    }
}
