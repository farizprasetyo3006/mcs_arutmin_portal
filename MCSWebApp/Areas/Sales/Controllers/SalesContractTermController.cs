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
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace MCSWebApp.Areas.Sales.Controllers
{
    [Area("Sales")]
    public class SalesContractTermController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public SalesContractTermController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public async Task<IActionResult> Detail(String Id)
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.SalesMarketing];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.SalesContract];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.SalesContract];
            ViewBag.BreadcrumbCode = WebAppMenu.SalesContract;

            ViewBag.RoleAccessList = HttpContext.Session.GetString("RoleAccessList");

            try
            {
                if (!string.IsNullOrEmpty(Id))
                {
                    var contractTerm = await dbContext.vw_sales_contract_term
                    .Where(o => o.id == Id).FirstOrDefaultAsync();

                    if (contractTerm != null)
                    {
                        ViewBag.ContractTerm = contractTerm;

                        // Get sales contract product
                        var contractProduct = await dbContext.vw_sales_contract_product
                        .Where(o => o.sales_contract_term_id == contractTerm.id).FirstOrDefaultAsync();
                        
                        if(contractProduct != null)
                        {
                            ViewBag.ContractProduct = contractProduct;
                        }

                        // Get sales contract despatch demurrage term
                        var contractDespatchDemurrageTerm = await dbContext.vw_sales_contract_despatch_demurrage_term
                        .Where(o => o.sales_contract_term_id == contractTerm.id).FirstOrDefaultAsync();

                        if (contractDespatchDemurrageTerm != null)
                        {
                            ViewBag.ContractDespatchDemurrageTerm = contractDespatchDemurrageTerm;
                        }
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
