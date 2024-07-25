using MCSWebApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using DataAccess;
using DataAccess.EFCore.Repository;
using Microsoft.Extensions.Configuration;
using System.IO;
using MCSWebApp.Models;
using FastReport;
using FastReport.Export.Html;
using System.Text;
using FastReport.Utils;
using FastReport.Web;
using Common;
using Microsoft.EntityFrameworkCore;
using System.Xml;
using System.Dynamic;
using Newtonsoft.Json;

namespace MCSWebApp.Areas.Report.Controllers
{
    [Area("Report")]
    public class ReportTemplateController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public ReportTemplateController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Reports];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ReportTemplate];
            ViewBag.BreadcrumbCode = WebAppMenu.ReportTemplate;

            return View();
        }

        public async Task<IActionResult> Viewer(string Id)
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Reports];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ReportTemplate];
            ViewBag.BreadcrumbCode = WebAppMenu.ReportTemplate;

            try
            {
                var dataContext = CurrentUserContext.GetDataContext();

                var record = await dbContext.report_template
                    .Where(o => o.id == Id)
                    .FirstOrDefaultAsync();
                if (record != null)
                {
                    if (await mcsContext.CanRead(dbContext, record.id, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin)
                    {
                        ViewBag.Id = Id;

                        var model = new ReportViewerModel()
                        {
                            ReportName = record.report_name,
                            Parameters = new List<ReportParameterModel>()
                        };

                        using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(record.report_definition)))
                        {
                            try
                            {
                                var xmlDoc = new System.Xml.XmlDocument();
                                xmlDoc.Load(stream);

                                var xmlNodes = xmlDoc.SelectNodes("//Parameter");
                                foreach (XmlNode xmlNode in xmlNodes)
                                {
                                    var rpm = new ReportParameterModel()
                                    {
                                        Name = xmlNode.Attributes["Name"]?.Value,
                                        DataType = xmlNode.Attributes["DataType"]?.Value,
                                        Description = xmlNode.Attributes["Description"]?.Value,
                                        LookupValues = new Dictionary<string, string>()
                                    };
                                    model.Parameters.Add(rpm);

                                    if (!string.IsNullOrEmpty(rpm.Description) &&
                                        rpm.Description.Trim().ToLower().StartsWith("select"))
                                    {
                                        var qry = rpm.Description.Trim().ToLower();
                                        // Parse fields
                                        var start = qry.IndexOf("select") + "select".Length;
                                        var end = qry.IndexOf("from");
                                        var fieldNames = qry.Substring(start, end - start);
                                        if (!string.IsNullOrEmpty(fieldNames))
                                        {
                                            var fields = fieldNames.Split(",".ToCharArray());
                                            if (fields?.Length > 0)
                                            {
                                                var dynamicLookups = await dataContext.Database.FetchAsync<dynamic>(qry);
                                                if (dynamicLookups?.Count > 0)
                                                {
                                                    foreach (dynamic dynamicLookup in dynamicLookups)
                                                    {
                                                        string json = JsonConvert.SerializeObject(dynamicLookup);
                                                        Dictionary<string, object> dl = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                                                        var key = (string)dl[fields[0].Trim().ToLower()];
                                                        var text = fields?.Length > 1 ? (string)dl[fields[1].Trim().ToLower()]
                                                            : (string)dl[fields[0].Trim().ToLower()];
                                                        rpm.LookupValues.Add(key, text);
                                                    }
                                                }
                                            }

                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex);
                                return BadRequest(ex.Message);
                            }
                        }

                        return View(model);
                    }
                    else
                    {
                        logger.Debug("User is not authorized.");
                        return Unauthorized();
                    }
                }
                else
                {
                    logger.Debug("Record is not found.");
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<IActionResult> Renderer([FromQuery] string p)
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Reports];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ReportTemplate];
            ViewBag.BreadcrumbCode = WebAppMenu.ReportTemplate;

            try
            {
                if (string.IsNullOrEmpty(p))
                {
                    return BadRequest("Parameter is empty");
                }

                dynamic parameter = new ExpandoObject();
                parameter = JsonConvert.DeserializeObject<dynamic>(p);
                var id = (string)parameter.__id;

                var record = await dbContext.report_template
                    .Where(o => o.id == id)
                    .FirstOrDefaultAsync();
                if (record != null)
                {
                    if (await mcsContext.CanUpdate(dbContext, record.id, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin)
                    {
                        var model = new ReportViewerModel()
                        {
                            WebReport = new WebReport(),
                            ReportName = record.report_name,
                            Parameters = new List<ReportParameterModel>()
                        };

                        model.WebReport.Report.LoadFromString(record.report_definition);

                        string connString = configuration.GetConnectionString("MCS");
                        //model.WebReport.Report.Dictionary.Connections[0].ConnectionString = connString;
                        
                        for (int i = 0; i <= 20; i++)
                        {
                            try
                            {
                                model.WebReport.Report.Dictionary.Connections[i].ConnectionString = connString;
                            }
                            catch { }
                        }

                        using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(record.report_definition)))
                        {
                            var xmlDoc = new System.Xml.XmlDocument();
                            xmlDoc.Load(stream);

                            var xmlNodes = xmlDoc.SelectNodes("//Parameter");
                            foreach (XmlNode xmlNode in xmlNodes)
                            {
                                switch (xmlNode.Attributes["DataType"].Value)
                                {
                                    case "System.Date":
                                    case "System.DateTime":
                                        model.WebReport.Report.SetParameterValue(xmlNode.Attributes["Name"].Value,
                                            DateTime.Parse((string)parameter[xmlNode.Attributes["Name"].Value]));
                                        break;
                                    case "System.Int32":
                                        model.WebReport.Report.SetParameterValue(xmlNode.Attributes["Name"].Value,
                                            Int32.Parse((string)parameter[xmlNode.Attributes["Name"].Value]));
                                        break;
                                    case "System.Int64":
                                        model.WebReport.Report.SetParameterValue(xmlNode.Attributes["Name"].Value,
                                            Int64.Parse((string)parameter[xmlNode.Attributes["Name"].Value]));
                                        break;
                                    case "System.Decimal":
                                        model.WebReport.Report.SetParameterValue(xmlNode.Attributes["Name"].Value,
                                            Decimal.Parse((string)parameter[xmlNode.Attributes["Name"].Value]));
                                        break;
                                    case "System.Single":
                                        model.WebReport.Report.SetParameterValue(xmlNode.Attributes["Name"].Value,
                                            Single.Parse((string)parameter[xmlNode.Attributes["Name"].Value]));
                                        break;
                                    case "System.Double":
                                        model.WebReport.Report.SetParameterValue(xmlNode.Attributes["Name"].Value,
                                            Double.Parse((string)parameter[xmlNode.Attributes["Name"].Value]));
                                        break;
                                    case "System.TimeSpan":
                                        model.WebReport.Report.SetParameterValue(xmlNode.Attributes["Name"].Value,
                                            TimeSpan.Parse((string)parameter[xmlNode.Attributes["Name"].Value]));
                                        break;
                                    default:
                                        model.WebReport.Report.SetParameterValue(xmlNode.Attributes["Name"].Value,
                                            (string)parameter[xmlNode.Attributes["Name"].Value]);
                                        break;
                                }
                            }
                        }

                        return View(model);
                    }
                    else
                    {
                        logger.Debug("User is not authorized.");
                        return Unauthorized();
                    }
                }
                else
                {
                    logger.Debug("Record is not found.");
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }            
        }
    }
}
