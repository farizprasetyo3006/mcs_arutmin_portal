using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using DataAccess.EFCore.Repository;
using System;

namespace MCSWebApp.Models
{
    /// <summary>Provides easy-access to building the PPDPP Budget Navigation using JSON text data.</summary>
    /// <remarks>These classes are solely created for Demo purposes, please do not use them in Production.</remarks>
    public class SalesPlanReportModel
    {
        public string SavedId { get; set; }
        public string SalesPlanName { get; set; }
        public string Version { get; set; }
        public DateTime DateCreate { get; set; }
        public string Remark { get; set; }
        public string Site { get; set; }

        public List<SalesPlanReportMonth> SalesPlanReportMonths { get; set; }
        public List<SalesPlanReportSalesContractTotalYearly> SalesPlanReportSalesContractTotals { get; set; }
        public decimal TotalPlanYearly { get; set; }
        public decimal TotalActualYearly { get; set; }
        public decimal TotalTargetYearly { get; set; }

    }

    public class SalesPlanReportMonth
    {
        public string MonthName { get; set; }
        public int Month { get; set; }
        public List<SalesPlanReportSalesContract> SalesContracts { get; set; }
        public decimal SalesPlanTarget { get; set; }
        public decimal SalesPlanMonthTotal { get; set; }
        public decimal SalesActualMonthTotal { get; set; }
        public decimal CarryOver { get; set; }
        public decimal CarryForward { get; set; }

    }

    public class SalesPlanReportSalesContract
    {
        public string SalesContractId { get; set; }
        public string SalesContractName { get; set; }
        public string SalesContractType { get; set; }
        public decimal SalesContractsPlanTotal { get; set; }
        public decimal SalesContractsActualTotal { get; set; }
    }

    public class SalesPlanReportSalesContractTotalYearly
    {
        public string SalesContractId { get; set; }
        public string SalesContractName { get; set; }
        public decimal SalesContractsPlanTotal { get; set; }
        public decimal SalesContractsActualTotal { get; set; }

    }

}