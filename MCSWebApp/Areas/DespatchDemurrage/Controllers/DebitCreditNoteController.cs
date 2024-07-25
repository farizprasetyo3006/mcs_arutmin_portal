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
using Microsoft.AspNetCore.Hosting;
using System.Text;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace MCSWebApp.Areas.DespatchDemurrage.Controllers
{
    [Area("DespatchDemurrage")]
    public class DebitCreditNoteController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public DebitCreditNoteController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.DesDemDebitCreditNote];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.DesDemDebitCreditNote];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.DesDemDebitCreditNote];
            ViewBag.BreadcrumbCode = WebAppMenu.DesDemDebitCreditNote;

            return View();
        }

        public async Task<IActionResult> Detail(String Id)
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.DesDemDebitCreditNote];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.DesDemDebitCreditNote];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.DesDemDebitCreditNote];
            ViewBag.BreadcrumbCode = WebAppMenu.DesDemDebitCreditNote;

            try
            {
                if (!string.IsNullOrEmpty(Id))
                {
                    var record = await dbContext.vw_despatch_demurrage_debit_credit_note
                    .Where(o => o.id == Id).FirstOrDefaultAsync();

                    if (record != null)
                    {
                        ViewBag.DesDemInvoice = record;
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
