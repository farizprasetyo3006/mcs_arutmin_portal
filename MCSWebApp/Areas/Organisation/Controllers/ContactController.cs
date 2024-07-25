using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Entity;
using MCSWebApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NLog;

namespace MCSWebApp.Areas.Organisation.Controllers
{
    [Area("Organisation")]
    public class ContactController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public ContactController(IConfiguration Configuration)
            : base(Configuration)
        {
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.AreaBreadcrumb = "Organisation";
            ViewBag.Breadcrumb = "Contact";

            return View();
        }
    }
}
