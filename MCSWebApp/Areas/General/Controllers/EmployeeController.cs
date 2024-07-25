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
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Npoi.Mapper;
using Npoi.Mapper.Attributes;

namespace MCSWebApp.Areas.General.Controllers
{
    [Area("General")]
    public class EmployeeController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public EmployeeController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.MasterData];
            ViewBag.AreaBreadcrumb = "General";
            ViewBag.Breadcrumb = "Employee";
            ViewBag.BreadcrumbCode = "Employee";

            return View();
        }

        public async Task<IActionResult> ExcelExport()
        {
            string sFileName = "Employee.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("Employee");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Employee Number");
                row.CreateCell(1).SetCellValue("Employee Name");
                row.CreateCell(2).SetCellValue("Address");
                row.CreateCell(3).SetCellValue("Join Date");
                row.CreateCell(4).SetCellValue("Gender");
                row.CreateCell(5).SetCellValue("Phone");
                row.CreateCell(6).SetCellValue("Is Operator");
                row.CreateCell(7).SetCellValue("Is Supervisor");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);

                var tabledata = dbContext.employee.Where(o => o.organization_id == CurrentUserContext.OrganizationId);
                // Inserting values to table
                foreach (var isi in tabledata)
                {
                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(isi.employee_number);
                    row.CreateCell(1).SetCellValue(isi.employee_name);
                    row.CreateCell(2).SetCellValue(isi.address);
                    row.CreateCell(3).SetCellValue(" " + Convert.ToDateTime(isi.join_date).ToString("yyyy-MM-dd"));
                    row.CreateCell(4).SetCellValue(Convert.ToBoolean(isi.gender));
                    row.CreateCell(5).SetCellValue(isi.phone);
                    row.CreateCell(6).SetCellValue(Convert.ToBoolean(isi.is_operator));
                    row.CreateCell(7).SetCellValue(Convert.ToBoolean(isi.is_supervisor));
                    RowCount++;
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

                finally
                {
                    var path = Path.Combine(FilePath, sFileName);
                    if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                }
            }
        }

    }
}
