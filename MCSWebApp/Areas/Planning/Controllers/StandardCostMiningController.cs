using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Entity;
using MCSWebApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NLog;
using Microsoft.AspNetCore.Http;

namespace MCSWebApp.Areas.Planning.Controllers
{
    [Area("Planning")]
    public class StandardCostMiningController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public StandardCostMiningController(IConfiguration Configuration)
            : base(Configuration)
        {
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.AreaBreadcrumb = "Planning";
            ViewBag.Breadcrumb = "Standard Cost Mining";

            ViewBag.RoleAccessList = HttpContext.Session.GetString("RoleAccessList");

            return View();
        }
    }
}
