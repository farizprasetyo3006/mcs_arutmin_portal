using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore.Mvc;
using MCSWebApp.Controllers;
using NLog;
using Microsoft.EntityFrameworkCore;
using DataAccess.EFCore.Repository;
using Microsoft.Extensions.Configuration;
using MCSWebApp.Models;
using System.Drawing;

namespace MCSWebApp.Areas.Data
{
    [Area("Data")]
    public class DashboardController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public DashboardController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }
        public async Task<IActionResult> Index()
        {
            /*ViewBag.WebAppName = "AIMS";
            //ViewBag.RootBreadcrumb = "Smart Mining"; ob,coalmine,haul,produce
            //ViewBag.AreaBreadcrumb = "Data";
            ViewBag.Breadcrumb = "Dashboard";*/
            DateTime today = DateTime.Today;
            DateTime todayWithSevenHours = today.AddHours(7);
            DateTime tomorrowWithSevenHours = today.AddDays(1).AddHours(7);
            string query = @"
                            select round(sum(pt.loading_quantity),3) as quantity, 'processing' as module_name 
                            from processing_transaction pt 
	                            where pt.loading_datetime >= current_date::date + interval '7 hours' 
	                            and pt.loading_datetime < current_date::date +interval '1 day 7 hours'
                            union all 
                            select round(sum(pt.loading_quantity),3) as quantity, 'waste_removal' as module_name 
                            from waste_removal pt 
	                            where pt.unloading_datetime >= current_date::date + interval '7 hours' 
	                            --and pt.unloading_datetime < current_date::date +interval '1 day 7 hours' 
                            union all 
                            select round(sum(pt.loading_quantity),3) as quantity, 'hauling' as module_name
                            from hauling_transaction pt 
	                            where pt.loading_datetime >= current_date::date + interval '7 hours' 
	                            and pt.loading_datetime < current_date::date +interval '1 day 7 hours'
                            union all 
                            select round(sum(pt.loading_quantity),3) as quantity, 'production' as module_name 
                            from production_transaction pt 
	                            where pt.loading_datetime >= current_date::date + interval '7 hours' 
	                            and pt.loading_datetime < current_date::date +interval '1 day 7 hours'";

            /*var conn = dbContext.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open)
            {
                await conn.OpenAsync();
            }

            List<DashboardViewModel> results = new List<DashboardViewModel>();

            if (conn.State == System.Data.ConnectionState.Open)
            {
                using (var cmd = conn.CreateCommand())
                {
                    try
                    {
                        cmd.CommandText = query;
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                results.Add(new DashboardViewModel
                                {
                                    quantity = reader.IsDBNull(0) ? (decimal?)null : reader.GetDecimal(0),
                                    module_name = reader.GetString(1)
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.ToString());
                        return BadRequest(ex.Message);
                    }
                }
            }
            var dashboardData = new DashboardViewModel();

            foreach (var result in results)
            {
                switch (result.module_name)
                {
                    case "processing":
                        dashboardData.quantity_processing = result.quantity;
                        break;
                    case "waste_removal":
                        dashboardData.quantity_waste_removal = result.quantity;
                        break;
                    case "hauling":
                        dashboardData.quantity_hauling = result.quantity;
                        break;
                    case "production":
                        dashboardData.quantity_production = result.quantity;
                        break;
                }
            }*/
            ViewBag.WebAppName = WebAppName;/*
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProductionLogistics];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Planning];*/
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.AIDashboard];
            ViewBag.BreadcrumbCode = WebAppMenu.AIDashboard;

            return View();
            //return View(dashboardData);
        }
    }
}
