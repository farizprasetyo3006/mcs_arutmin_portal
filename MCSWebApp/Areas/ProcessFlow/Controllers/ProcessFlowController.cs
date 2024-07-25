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
using DataAccess.EFCore.Repository;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using System.IO;
using Npoi.Mapper;
using Npoi.Mapper.Attributes;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace MCSWebApp.Areas.ProcessFlow.Controllers
{
    [Area("ProcessFlow")]
    public class ProcessFlowController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public ProcessFlowController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProductionLogistics];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Modelling];
            ViewBag.SubAreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProcessFlow];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProcessingFlow];
            ViewBag.BreadcrumbCode = WebAppMenu.ProcessingFlow;

            return View();
        }

        public async Task<IActionResult> ExcelExport1()
        {
            string sFileName = "ProcessFlow.xlsx";
            sFileName = sFileName.Insert(sFileName.LastIndexOf("."), string.Format("_{0}", DateTime.Now.ToString("yyyyMMdd_HHmmss_ffff")));

            string FilePath = configuration.GetSection("Path").GetSection("UploadBasePath").Value + PublicFunctions.ExcelFolder;
            if (!Directory.Exists(FilePath))  Directory.CreateDirectory(FilePath);
            FilePath = Path.Combine(FilePath, sFileName);

            var mapper = new Npoi.Mapper.Mapper();
            mapper.Ignore<process_flow>(o => o.organization_)
                .Ignore<process_flow>(o => o.destination_location_)
                .Ignore<process_flow>(o => o.source_location_);
            mapper.Put(dbContext.process_flow, "ProcessFlow", true);
            mapper.Save(FilePath);

            var memory = new MemoryStream();
            using (var stream = new FileStream(FilePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            //Throws Generated file to Browser
            try
            {
                return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
            }
            // Deletes the generated file
            finally
            {
                var path = Path.Combine(FilePath, sFileName);
                if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
            }
        }

        public async Task<IActionResult> ExcelExport()
        {
            string sFileName = "ProcessFlow.xlsx";
            sFileName = sFileName.Insert(sFileName.LastIndexOf("."), string.Format("_{0}", DateTime.Now.ToString("yyyyMMdd_HHmmss_ffff")));

            string FilePath = configuration.GetSection("Path").GetSection("UploadBasePath").Value + PublicFunctions.ExcelFolder;
            if (!Directory.Exists(FilePath))  Directory.CreateDirectory(FilePath);

            FileInfo file = new FileInfo(Path.Combine(FilePath, sFileName));
            var memory = new MemoryStream();
            using (var fs = new FileStream(Path.Combine(FilePath, sFileName), FileMode.Create, FileAccess.Write))
            {
                int RowCount = 1;
                IWorkbook workbook;
                workbook = new XSSFWorkbook();
                ISheet excelSheet = workbook.CreateSheet("ProcessFlow");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Process Flow Name");
                row.CreateCell(1).SetCellValue("Process Flow Code");
                row.CreateCell(2).SetCellValue("Process Flow Category");
                row.CreateCell(3).SetCellValue("Source");
                row.CreateCell(4).SetCellValue("Destination");
                row.CreateCell(5).SetCellValue("Sampling Template");
                row.CreateCell(6).SetCellValue("Assume Source Quality");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);

                var tabledata = dbContext.process_flow.Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .OrderByDescending(o => o.created_on);
                // Inserting values to table
                foreach (var Value in tabledata)
                {
                    var source_code = "";
                    var source = dbFind.business_area
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId 
                            && o.id == Value.source_location_id)
                        .FirstOrDefault();
                    if (source != null) source_code = source.business_area_code.ToString();

                    var destination_code = "";
                    var destination = dbFind.business_area
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            && o.id == Value.destination_location_id)
                        .FirstOrDefault();
                    if (destination != null) destination_code = destination.business_area_code.ToString();

                    var sampling_template_code = "";
                    var sampling_template = dbFind.sampling_template
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            && o.id == Value.sampling_template_id)
                        .FirstOrDefault();
                    if (sampling_template != null) sampling_template_code = sampling_template.sampling_template_code.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(Value.process_flow_name);
                    row.CreateCell(1).SetCellValue(Value.process_flow_code);
                    row.CreateCell(2).SetCellValue(Value.process_flow_category);
                    row.CreateCell(3).SetCellValue(source_code);
                    row.CreateCell(4).SetCellValue(destination_code);
                    row.CreateCell(5).SetCellValue(sampling_template_code);
                    row.CreateCell(6).SetCellValue(PublicFunctions.BenarSalah(Value.assume_source_quality));

                    RowCount++;
                    if (RowCount > 50) break;
                }
                workbook.Write(fs);
                using (var stream = new FileStream(Path.Combine(FilePath, sFileName), FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                //Throws Generated file to Browser
                try
                {
                    return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
                }
                // Deletes the generated file from /wwwroot folder
                finally
                {
                    var path = Path.Combine(FilePath, sFileName);
                    if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                }
            }
        }

    }
}
