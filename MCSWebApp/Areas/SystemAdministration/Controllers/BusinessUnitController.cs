using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Entity;
using MCSWebApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NLog;

namespace MCSWebApp.Areas.SystemAdministration.Controllers
{
    [Area("SystemAdministration")]
    public class BusinessUnitController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public BusinessUnitController(IConfiguration Configuration)
            : base(Configuration)
        {
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.AreaBreadcrumb = "System Administration";
            ViewBag.Breadcrumb = "Business Unit";

            return View();
        }

        public async Task<IActionResult> Detail(string Id)
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.AreaBreadcrumb = "SystemAdministration";
            ViewBag.Breadcrumb = "Business Unit";

            try
            {
                if (!string.IsNullOrEmpty(Id))
                {
                    var svc = new BusinessUnit(CurrentUserContext);
                    var record = await svc.GetByIdAsync(Id);
                    if(record != null)
                    {
                        ViewBag.Id = record.id;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());                
            }

            return View();
        }

    }
}
