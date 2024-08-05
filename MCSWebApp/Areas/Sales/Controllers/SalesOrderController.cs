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
using Microsoft.AspNetCore.Http;

namespace MCSWebApp.Areas.Sales.Controllers
{
    [Area("Sales")]
    public class SalesOrderController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public SalesOrderController(IConfiguration Configuration)
            : base(Configuration)
        {
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.SalesMarketing];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.SalesOrder];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.SalesOrder];
            ViewBag.BreadcrumbCode = WebAppMenu.SalesOrder;

            ViewBag.RoleAccessList = HttpContext.Session.GetString("RoleAccessList");

            return View();
        }
    }
}
