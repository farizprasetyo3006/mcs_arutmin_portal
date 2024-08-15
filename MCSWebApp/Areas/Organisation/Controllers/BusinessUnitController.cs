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

namespace MCSWebApp.Areas.Organisation.Controllers
{
    [Area("Organisation")]
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

            ViewBag.RoleAccessList = HttpContext.Session.GetString("RoleAccessList");

            return View();
        }

        public async Task<IActionResult> Detail(string Id)
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.AreaBreadcrumb = "Organisation";
            ViewBag.Breadcrumb = "Business Unit";

            ViewBag.RoleAccessList = HttpContext.Session.GetString("RoleAccessList");

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
