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
using Microsoft.AspNetCore.Http;

namespace MCSWebApp.Areas.Report.Controllers
{
    [Area("Report")]
    public class PBIController : BaseController
    {
        private readonly mcsContext dbContext;

        public PBIController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Reports];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ReportSmartMining];

            ViewBag.RoleAccessList = HttpContext.Session.GetString("RoleAccessList");

            string url = HttpContext.Request.Path;
            string ReportName = url.Substring(url.LastIndexOf('/') + 1);
            ViewBag.Breadcrumb = ReportName;
            ReportName = ReportName.ToLower();

            if (ReportName == "shipment")
            {
                ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.RShipment];
                ViewBag.BreadcrumbCode = WebAppMenu.RShipment;
                //ViewBag.UrlReport = "https://app.powerbi.com/view?r=eyJrIjoiNjAyYWNkOTMtNGEzMS00ZWFlLThhODktMWVmYzY4YzVlMGZkIiwidCI6IjFiNzYwNDc2LWMyZGMtNDY5NS04MjUwLWQzN2VjZTZiYzk0MiIsImMiOjEwfQ%3D%3D&pageName=ReportSection";

                ViewBag.UrlReport = "https://app.powerbi.com/reportEmbed?reportId=b25e934f-837a-4cc1-a788-0368941efb24&autoAuth=true&ctid=406ad0e2-e137-4c92-8d7d-58c1e2ad067a&config=eyJjbHVzdGVyVXJsIjoiaHR0cHM6Ly93YWJpLXNvdXRoLWVhc3QtYXNpYS1yZWRpcmVjdC5hbmFseXNpcy53aW5kb3dzLm5ldC8ifQ%3D%3D";
            }
            else if (ReportName == "barging")
            {
                ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.RBarging];
                ViewBag.BreadcrumbCode = WebAppMenu.RBarging;
                ViewBag.UrlReport = "https://app.powerbi.com/view?r=eyJrIjoiMmQzZjg3ZDctMTM1YS00NWQ1LThiNTQtMDA1YjEyZWJmYmUzIiwidCI6IjFiNzYwNDc2LWMyZGMtNDY5NS04MjUwLWQzN2VjZTZiYzk0MiIsImMiOjEwfQ%3D%3D";
            }
            else if (ReportName == "sof")
            {
                ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.RSof];
                ViewBag.BreadcrumbCode = WebAppMenu.RSof;
                ViewBag.UrlReport = "https://app.powerbi.com/view?r=eyJrIjoiYjIxNjdlMmYtYjM3NS00MDA0LTk2ZTMtYWQ4NWYyYWE5NmZhIiwidCI6IjFiNzYwNDc2LWMyZGMtNDY5NS04MjUwLWQzN2VjZTZiYzk0MiIsImMiOjEwfQ%3D%3D";
            }

            return View();
        }
    }
}
