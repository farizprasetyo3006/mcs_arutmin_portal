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
    public class SalesInvoiceApprovalController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public SalesInvoiceApprovalController(IConfiguration Configuration)
            : base(Configuration)
        {
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ContractManagement];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Invoice];
            //ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.SalesInvoice];
            ViewBag.Breadcrumb = "Sales Invoice Approval";
            ViewBag.BreadcrumbCode = WebAppMenu.SalesInvoice;

            return View();
        }
    }
}
