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
using Npoi.Mapper;
using Npoi.Mapper.Attributes;
using Microsoft.EntityFrameworkCore;

namespace MCSWebApp.Areas.ContractManagement.Controllers
{
    [Area("ContractManagement")]
    public class AdvanceContractChargeController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public AdvanceContractChargeController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ContractManagement];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.AdvanceContract];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.AdvanceContractCharge];
            ViewBag.BreadcrumbCode = WebAppMenu.AdvanceContractCharge;

            return View();
        }

        public async Task<IActionResult> Detail(String Id)
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.AdvanceContract];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.AdvanceContractCharge];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.AdvanceContractChargeDetail];
            ViewBag.BreadcrumbCode = WebAppMenu.AdvanceContractChargeDetail;

            try
            {
                if (!string.IsNullOrEmpty(Id))
                {
                    var advanceContractChargeRecord = await dbContext.vw_advance_contract_charge
                    .Where(o => o.id == Id).FirstOrDefaultAsync();

                    if (advanceContractChargeRecord != null)
                    {
                        ViewBag.AdvanceContractCharge = advanceContractChargeRecord;
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
