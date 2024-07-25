using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastReport.Web;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using MCSWebApp.Controllers;
using NLog;
using DataAccess.EFCore.Repository;
using Microsoft.Extensions.Configuration;

namespace MCSWebApp.Areas.Report.Controllers
{
    [Area("Report")]
    public class DesignerController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public DesignerController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            WebReport WebReport = new WebReport(); // Create a Web Report Object
            WebReport.Width = "1000"; // Set the width of the report
            WebReport.Height = "1000"; // Set the height of the report

            // Get the path to wwwroot folder
            //string contentRootPath = _hostingEnvironment.ContentRootPath;
            //string webRootPath = _hostingEnvironment.WebRootPath;

            /*
            WebReport.Report.Load(webRootPath + "/reports/Simple List.frx"); // Load the report into a WebReport object
            System.Data.DataSet dataSet = new System.Data.DataSet(); // Create a data source
            dataSet.ReadXml(webRootPath + "/reports/nwind.xml"); // Open the xml database
            WebReport.Report.RegisterData(dataSet, "NorthWind"); //Register the data source in the report
            */

            WebReport.Mode = WebReportMode.Designer; // Set the mode of the web report object - display of the designer
            WebReport.DesignerPath = "/WebReportDesigner/index.html"; // Specify the URL of the online designer
            WebReport.DesignerSaveMethod = (string reportId, string reportFileName, string message) =>
            {
                return "Ok";
            };

            ViewBag.WebReport = WebReport; // pass report to View
            return View();
        }

        [HttpPost]
        // call-back for save the designed report
        public async Task<IActionResult> SaveDesignedReport(string reportID, string reportUUID)
        {
            string webRootPath = ""; //_hostingEnvironment.WebRootPath; // Get the path to wwwroot folder ViewBag.Message = String.Format("Confirmed {0} {1}", reportID, reportUUID); // Set a message for view

            Stream reportForSave = Request.Body; // Write the result of the Post query to the stream
            string pathToSave = webRootPath + "/DesignedReports/TestReport.frx"; // get the path to save the file
            using (FileStream file = new FileStream(pathToSave, FileMode.Create))// Create a file stream {
            {
                await reportForSave.CopyToAsync(file); // Save the result of the query to a file
            }

            return View();
        }
    }
}
