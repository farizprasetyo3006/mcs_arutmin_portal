using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Entity;
using Common;
using MCSWebApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NLog;

namespace MCSWebApp.Areas.Sales.Controllers
{
    [Area("Sales")]
    public class ShippingInstructionApprovalController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public ShippingInstructionApprovalController(IConfiguration Configuration)
            : base(Configuration)
        {
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ContractManagement];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.SalesContract];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ShippingInstruction];
            ViewBag.BreadcrumbCode = WebAppMenu.SalesInvoice;

            return View();
        }
    }
}
