using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public partial class Constants
    {
        public enum BargeStatus { Empty = 0, Barge_Loading = 10, Cargo_On_Water = 20, Barge_Unloading = 30 };
        public static readonly Dictionary<int, string> BargeStatuses =
            new Dictionary<int, string>
            {
                { (int)BargeStatus.Empty, Enum.GetName(typeof(BargeStatus), (int)BargeStatus.Empty).Replace("_", "") },
                { (int)BargeStatus.Barge_Loading, Enum.GetName(typeof(BargeStatus), (int)BargeStatus.Barge_Loading).Replace("_", "") },
                { (int)BargeStatus.Cargo_On_Water, Enum.GetName(typeof(BargeStatus), (int)BargeStatus.Cargo_On_Water).Replace("_", "") },
                { (int)BargeStatus.Barge_Unloading, Enum.GetName(typeof(BargeStatus), (int)BargeStatus.Barge_Unloading).Replace("_", "") }
            };

        public enum ContractType { AR, AP };
        public static readonly List<string> ContractTypes =
            new List<string>
            {
                { ContractType.AR.ToString() },
                { ContractType.AP.ToString() },
            };

        public static readonly List<string> DMOList =
            new List<string>
            {
                { "DMO" },
                { "EXPORT" },
            };

        public static readonly List<string> ElectricityList =
            new List<string>
            {
                { "EL" },
                { "NO" },
            };

        public static readonly List<string> PrintStatus =
            new List<string>
            {
                { "ORIGINAL" }, { "FIRST ORIGINAL" }, { "SECOND ORIGINAL" }, { "THIRD ORIGINAL" }, { "COPY" },
            };

        public enum AddressType { Main_Address = 0, Billing_Address = 10, Shipping_Address = 20 }
        public static readonly Dictionary<int, string> AddressTypes =
            new Dictionary<int, string>
            {
                { (int)AddressType.Main_Address, Enum.GetName(typeof(AddressType), (int)AddressType.Main_Address).Replace("_", "") },
                { (int)AddressType.Billing_Address, Enum.GetName(typeof(AddressType), (int)AddressType.Billing_Address).Replace("_", "") },
                { (int)AddressType.Shipping_Address, Enum.GetName(typeof(AddressType), (int)AddressType.Shipping_Address).Replace("_", "") }
            };

        public static Dictionary<string, string> ApplicationEntities =
            new Dictionary<string, string> 
            {
                { "accounting_period", "Accounting Period" },
                { "analyte", "Analyte" },
                { "application_entity", "Application Entity" },
                { "application_role", "Application Role" },
                { "application_user", "Application User" },
                { "audit_trail", "Audit Trail" },
                { "bank", "Bank" },
                { "bank_account", "Bank Account" },
                { "barge", "Barge" },
                { "barging_transaction", "Barging Transaction" },
                { "business_area", "Business Area" },
                { "business_partner", "Business Partner" },
                { "business_unit", "Business Unit" },
                { "city", "City" },
                { "coa", "Chart of Account" },
                { "coa_subaccount", "Sub Account" },
                { "contact", "Contact" },
                { "contractor", "Contractor" },
                { "contractor_document", "Contractor Document" },
                { "cost_rate", "Cost Rate" },
                { "country", "Country" },
                { "currency", "Currency" },
                { "currency_exchange", "Currency Exchange" },
                { "customer", "Customer" },
                { "daily_record", "Daily Record" },
                { "data_extension", "Data Extension" },
                { "despatch_order", "Despatch Order" },
                { "despatch_order_delay", "Despatch Order Delay" },
                { "document_type", "Document Type" },
                // { "entity", "Entity" },
                { "equipment", "Equipment" },
                { "equipment_category", "Equipment Category" },
                { "equipment_cost_rate", "Equipment Cost Rate" },
                { "equipment_type", "Equipment Type" },
                { "equipment_usage_transaction", "Equipment Usage Transaction" },
                { "hauling_transaction", "Hauling Transaction" },
                { "incident", "Incident" },
                { "incident_category", "Incident Category" },
                { "inventory_transaction", "Inventory Transaction" },
                { "inventory_transaction_detail", "Inventory Transaction Detail" },
                { "list_key", "List Key" },
                { "list_value", "List Value" },
                { "material_type", "Material Type" },
                { "material_type_analyte", "Material Type Analyte" },
                { "mine_location", "Mine Location" },
                { "organization", "Organization" },
                { "port_location", "Port Location" },
                { "process_flow", "Process Flow" },
                { "process_flow_analyte", "Process Flow Analyte" },
                { "processing_category", "Processing Category" },
                { "processing_transaction", "Processing Transaction" },
                { "product", "Product" },
                { "product_category", "Product Category" },
                { "product_specification", "Product Specification" },
                { "production_plan", "Production Plan" },
                { "production_plan_detail", "Production Plan Detail" },
                { "production_transaction", "Production Transaction" },
                { "province", "Province" },
                { "railing_transaction", "Railing Transaction" },
                { "railing_transaction_detail", "Railing Transaction Detail" },
                { "role_access", "Role Access" },
                { "sales_demurrage_rate", "Sales Demurrage Rate" },
                { "sales_invoice", "Sales Invoice" },
                { "sales_invoice_detail", "Sales Invoice Detail" },
                { "sales_order", "Sales Order" },
                { "sales_order_detail", "Sales Order Detail" },
                { "sales_plan", "Sales Plan" },
                { "sales_plan_detail", "Sales Plan Detail" },
                { "sales_product_specification", "Sales Product Specification" },
                { "sampling_template", "Sampling Template" },
                { "sampling_template_detail", "Sampling Template Detail" },
                { "shared_record", "Shared Record" },
                { "shift", "Shift" },
                { "shift_category", "Shift Category" },
                { "shipping_delay", "Shipping Delay" },
                { "shipping_document", "Shipping Document" },
                { "shipping_transaction", "Shipping Transaction" },
                { "standard_cost_mining", "Standard Cost Mining" },
                { "standard_cost_mining_detail", "Standard Cost Mining Detail" },
                { "stock_location", "Stock Location" },
                { "stockpile_location", "Stockpile Location" },
                { "stockpile_state", "Stockpile State" },
                { "survey", "Survey" },
                { "survey_analyte", "Survey Analyte" },
                { "survey_detail", "Survey Detail" },
                { "team", "Team" },
                { "team_member", "Team Member" },
                { "team_role", "Team Role" },
                { "train", "Train" },
                { "transport", "Transport" },
                { "truck", "Truck" },
                { "truck_cost_rate", "Truck Cost Rate" },
                { "tug", "Tug" },
                { "uom", "UoM" },
                { "uom_category", "UoM Category" },
                { "uom_conversion", "UoM Conversion" },
                { "user_role", "User Role" },
                { "vessel", "Vessel" },
                { "wagon", "Wagon" },
                { "waste", "Waste" },
                { "waste_category", "Waste Category" },
                { "waste_location", "Waste Location" },
                { "waste_removal", "Waste Removal" },
                { "waste_specification", "Waste Specification" },
                { "wip_location", "WIP Location" }
            };

        //public static long MAX_ROLE_COUNT = 10000;
        public static long ACCESS_NO_ACCESS = 0;
        public static long ACCESS_OWN_RECORD = 1;
        //public static long ACCESS_BUSINESS_UNIT = MAX_ROLE_COUNT;
        //public static long ACCESS_DEEP_BUSINESS_UNIT = ACCESS_BUSINESS_UNIT * MAX_ROLE_COUNT;
        //public static long ACCESS_ORGANIZATION = ACCESS_DEEP_BUSINESS_UNIT * MAX_ROLE_COUNT;
        public static long ACCESS_BUSINESS_UNIT = 10;
        public static long ACCESS_DEEP_BUSINESS_UNIT = 100;
        public static long ACCESS_ORGANIZATION = 1000;
    }
}
