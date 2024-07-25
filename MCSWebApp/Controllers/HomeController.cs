using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MCSWebApp.Models;
using NLog;
using Microsoft.Extensions.Configuration;

namespace MCSWebApp.Controllers
{
    public class HomeController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public HomeController(IConfiguration Configuration)
            : base(Configuration)
        {
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Index()
        {
            return Redirect("/Authentication/Login/Index");
        }

        public IActionResult Dashboard()
        {
            ViewBag.RootBreadcrumb = "MCS";
            ViewBag.AreaBreadcrumb = "Home";
            ViewBag.Breadcrumb = "Dashboard";

            try
            {

            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
