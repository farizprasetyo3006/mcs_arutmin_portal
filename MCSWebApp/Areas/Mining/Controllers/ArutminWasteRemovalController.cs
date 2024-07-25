using Common;
using DataAccess.EFCore.Repository;
using MCSWebApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NLog;

namespace MCSWebApp.Areas.Mining.Controllers
{
    [Area("Mining")]
    public class ArutminWasteRemovalController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public ArutminWasteRemovalController(IConfiguration Configuration) : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProductionLogistics];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Mining];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.WasteRemovalByFleet];
            ViewBag.BreadcrumbCode = WebAppMenu.WasteRemovalByFleet;

            return View();
        }
    }
}
