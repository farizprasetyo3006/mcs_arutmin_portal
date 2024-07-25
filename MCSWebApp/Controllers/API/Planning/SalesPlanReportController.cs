using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using DataAccess.EFCore;
using Microsoft.EntityFrameworkCore;
using NLog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using DataAccess.DTO;
using Omu.ValueInjecter;
using DataAccess.EFCore.Repository;
using Microsoft.AspNetCore.Hosting;
using BusinessLogic;
using System.Text;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using MCSWebApp.Models;
using Common;

namespace MCSWebApp.Controllers.API.Sales
{
    [Route("api/Planning/[controller]")]
    [ApiController]
    public class SalesPlanReportController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public SalesPlanReportController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("GenerateReport/{Id}")]
        public object GenerateReport(string Id)
        {
            try
            {
                SalesPlanReportModel result = new SalesPlanReportModel();
                var selectedPlan = dbContext.sales_plan.Where(x => x.id == Id)
                    .Include(x => x.sales_plan_detail).FirstOrDefault();
                if(selectedPlan == null)
                {
                    return result;
                }

                var plan_name = "";
                var master_list = dbContext.master_list.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                    && o.id == selectedPlan.plan_year_id).FirstOrDefault();
                if (master_list != null) plan_name = master_list.item_name.ToString();

                result.SalesPlanName = plan_name;
                result.Version = selectedPlan.revision_number.ToString();
                result.DateCreate = DateTime.Now;
                result.Remark = selectedPlan.notes;
                result.Site = "";

                var salesContracts = dbContext.sales_contract.Where(x => x.created_on.HasValue && x.created_on.Value.Year == DateTime.Now.Year)
                    .Include(x => x.sales_contract_term)
                    .ThenInclude(y => y.sales_contract_despatch_plan)
                    //.ThenInclude(z => z.despatch_order)
                    .ToList();
                result.SalesPlanReportMonths = new List<SalesPlanReportMonth>();
                result.SalesPlanReportSalesContractTotals = new List<SalesPlanReportSalesContractTotalYearly>();
                List<SalesPlanReportMonth> listSalesPlanPerMonth = new List<SalesPlanReportMonth>();
                List<SalesPlanReportSalesContractTotalYearly> listTotalPerContractYearly = new List<SalesPlanReportSalesContractTotalYearly>();
                decimal tempCarryOver = 0;
                for (int i=1; i<13; i++)
                {
                    var selectedPlanDetail = selectedPlan.sales_plan_detail.FirstOrDefault(x => x.month_id == i);

                    SalesPlanReportMonth contractPerMonth = new SalesPlanReportMonth();
                    contractPerMonth.Month = i;
                    DateTime tempDatetime = new DateTime(2021, i, 12);
                    contractPerMonth.MonthName = tempDatetime.ToString("MMMM");
                    contractPerMonth.SalesContracts = new List<SalesPlanReportSalesContract>();
                    contractPerMonth.SalesActualMonthTotal = 0;
                    contractPerMonth.SalesPlanMonthTotal = 0;
                    contractPerMonth.CarryOver = tempCarryOver;
                    contractPerMonth.SalesPlanTarget = selectedPlanDetail != null ? (decimal)selectedPlanDetail.quantity : 0;
                    List<SalesPlanReportSalesContract> listSalesContractPerMonth = new List<SalesPlanReportSalesContract>();
                    foreach(sales_contract item in salesContracts)
                    {
                        SalesPlanReportSalesContract objSalesContract = new SalesPlanReportSalesContract();
                        objSalesContract.SalesContractId = item.id;
                        objSalesContract.SalesContractName = item.sales_contract_name;
                        objSalesContract.SalesContractType = "Domestic";
                        objSalesContract.SalesContractsActualTotal = 0;
                        objSalesContract.SalesContractsPlanTotal = 0;
                        var objSalesContractTerm = item.sales_contract_term.FirstOrDefault(x => x.start_date.Value.Month == i);
                        //ambil dari despatch plan ??
                        if (objSalesContractTerm != null)
                        {
                            var despatchPlan = objSalesContractTerm.sales_contract_despatch_plan;

                            //ambil dari Shipping order ??
                            if (despatchPlan != null && despatchPlan.Count() > 0)
                            {
                                objSalesContract.SalesContractsPlanTotal = despatchPlan != null && despatchPlan.Count() > 0 ? (decimal)despatchPlan.Sum(x => x.quantity) : 0;
                                contractPerMonth.SalesPlanMonthTotal += objSalesContract.SalesContractsPlanTotal;

                                foreach (sales_contract_despatch_plan itemDespatchPlan in despatchPlan)
                                {
                                    //var despatchOrder = itemDespatchPlan.despatch_order.FirstOrDefault(x => x.despatch_order_date.Value.Month == i);
                                    //var despatchOrder = itemDespatchPlan.despatch_order;
                                    //objSalesContract.SalesContractsActualTotal += despatchOrder != null ? (decimal)despatchOrder.Sum(x => x.quantity) : 0;
                                }
                                contractPerMonth.SalesActualMonthTotal += objSalesContract.SalesContractsActualTotal;
                            }
                            else
                            {
                                objSalesContract.SalesContractsActualTotal = 0;
                            }

                        }
                        
                        listSalesContractPerMonth.Add(objSalesContract);
                    }
                    contractPerMonth.SalesContracts = listSalesContractPerMonth;
                    contractPerMonth.SalesPlanMonthTotal += (tempCarryOver * -1);
                    contractPerMonth.CarryForward = contractPerMonth.SalesActualMonthTotal - contractPerMonth.SalesPlanMonthTotal;
                    tempCarryOver = contractPerMonth.CarryForward;
                    listSalesPlanPerMonth.Add(contractPerMonth);                                        
                }

                foreach (sales_contract item in salesContracts)
                {
                    SalesPlanReportSalesContractTotalYearly obj = new SalesPlanReportSalesContractTotalYearly();
                    obj.SalesContractId = item.id;
                    obj.SalesContractName = item.sales_contract_name;
                    obj.SalesContractsActualTotal = listSalesPlanPerMonth.Sum(x => x.SalesContracts.FirstOrDefault(y => y.SalesContractId == item.id).SalesContractsActualTotal);
                    obj.SalesContractsPlanTotal = listSalesPlanPerMonth.Sum(x => x.SalesContracts.FirstOrDefault(y => y.SalesContractId == item.id).SalesContractsPlanTotal);
                    listTotalPerContractYearly.Add(obj);
                }
                 
                result.SalesPlanReportMonths = listSalesPlanPerMonth;
                result.SalesPlanReportSalesContractTotals = listTotalPerContractYearly;
                result.TotalPlanYearly = listTotalPerContractYearly.Sum(x => x.SalesContractsPlanTotal);
                result.TotalActualYearly = listTotalPerContractYearly.Sum(x => x.SalesContractsActualTotal);
                result.TotalTargetYearly = (decimal)selectedPlan.sales_plan_detail.Sum(x => x.quantity);
                return result;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        

        

    }
}
