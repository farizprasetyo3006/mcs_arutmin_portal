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
using DataAccess.EFCore.Repository;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using System.IO;

namespace MCSWebApp.Areas.Report.Controllers
{
    [Area("Report")]
    public class EllipseController : BaseController
    {
        private readonly mcsContext dbContext;

        public EllipseController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Invoice()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Reports];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Ellipse];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.EllipseInvoice];
            ViewBag.BreadcrumbCode = WebAppMenu.EllipseInvoice;
            return View();
        }
    }
}
