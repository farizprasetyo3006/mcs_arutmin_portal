using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Entity;
using MCSWebApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NLog;

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

            return View();
        }
    }
}
