using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore.Mvc;
using MCSWebApp.Controllers;
using NLog;
using Microsoft.EntityFrameworkCore;
using DataAccess.EFCore.Repository;
using Microsoft.Extensions.Configuration;



namespace MCSWebApp.Areas.Data
{
    [Area("Data")]
    public class HistoricalRainfallController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public HistoricalRainfallController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }
        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.HistoricalRainfall];
            ViewBag.BreadcrumbCode = WebAppMenu.HistoricalRainfall;

            return View();
        }
    }
}
