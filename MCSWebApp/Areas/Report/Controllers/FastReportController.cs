using MCSWebApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using DataAccess.EFCore.Repository;
using Microsoft.Extensions.Configuration;
using System.IO;
using MCSWebApp.Models;
using FastReport;
using FastReport.Export.Html;
using System.Text;
using FastReport.Utils;
using FastReport.Web;

namespace MCSWebApp.Areas.Report.Controllers
{
    [Area("Report")]
    public class FastReportController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public FastReportController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            var model = new FastReportModel()
            {
                WebReport = new WebReport(),
                ReportsList = new[]
                {
                    "Indexim - Shipping"
                }
            };

            model.WebReport.Report.Load("C:\\temp\\Reports\\Indexim - Shipping.frx");
            model.WebReport.Report.SetParameterValue("StartDateTime", new DateTime(2022, 1, 1));
            model.WebReport.Report.SetParameterValue("EndDateTime", new DateTime(9999, 1, 1));

            return View(model);
        }
    }
}
