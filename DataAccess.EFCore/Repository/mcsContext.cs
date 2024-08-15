using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace DataAccess.EFCore.Repository
{
    public partial class mcsContext : DbContext
    {
        public mcsContext()
        {
        }

        public mcsContext(DbContextOptions<mcsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<_operator> _operator { get; set; }
        public virtual DbSet<access_value> access_value { get; set; }
        public virtual DbSet<accounting_period> accounting_period { get; set; }
        public virtual DbSet<advance_contract> advance_contract { get; set; }
        public virtual DbSet<advance_contract_charge> advance_contract_charge { get; set; }
        public virtual DbSet<advance_contract_charge_detail> advance_contract_charge_detail { get; set; }
        public virtual DbSet<advance_contract_detail> advance_contract_detail { get; set; }
        public virtual DbSet<advance_contract_item> advance_contract_item { get; set; }
        public virtual DbSet<advance_contract_item_detail> advance_contract_item_detail { get; set; }
        public virtual DbSet<advance_contract_reference> advance_contract_reference { get; set; }
        public virtual DbSet<advance_contract_reference_detail> advance_contract_reference_detail { get; set; }
        public virtual DbSet<advance_contract_valuation> advance_contract_valuation { get; set; }
        public virtual DbSet<advance_contract_valuation_detail> advance_contract_valuation_detail { get; set; }
        public virtual DbSet<allsite_barging_pct> allsite_barging_pct { get; set; }
        public virtual DbSet<allsite_hauling_pct> allsite_hauling_pct { get; set; }
        public virtual DbSet<allsite_mine_pct> allsite_mine_pct { get; set; }
        public virtual DbSet<allsite_produce_pct> allsite_produce_pct { get; set; }
        public virtual DbSet<allsite_transfer_pct> allsite_transfer_pct { get; set; }
        public virtual DbSet<allsite_waste_pct> allsite_waste_pct { get; set; }
        public virtual DbSet<analyte> analyte { get; set; }
        public virtual DbSet<app_user> app_user { get; set; }
        public virtual DbSet<application_business_unit> application_business_unit { get; set; }
        public virtual DbSet<application_entity> application_entity { get; set; }
        public virtual DbSet<application_role> application_role { get; set; }
        public virtual DbSet<application_role_report> application_role_report { get; set; }
        public virtual DbSet<application_user> application_user { get; set; }
        public virtual DbSet<arutmin_waste_removal> arutmin_waste_removal { get; set; }
        public virtual DbSet<arutmin_waste_removal_item> arutmin_waste_removal_item { get; set; }
        public virtual DbSet<audit_trail> audit_trail { get; set; }
        public virtual DbSet<bank> bank { get; set; }
        public virtual DbSet<bank_account> bank_account { get; set; }
        public virtual DbSet<barge> barge { get; set; }
        public virtual DbSet<barge_rotation> barge_rotation { get; set; }
        public virtual DbSet<barging_closing> barging_closing { get; set; }
        public virtual DbSet<barging_closing_item> barging_closing_item { get; set; }
        public virtual DbSet<barging_load_unload_document> barging_load_unload_document { get; set; }
        public virtual DbSet<barging_plan> barging_plan { get; set; }
        public virtual DbSet<barging_plan_daily> barging_plan_daily { get; set; }
        public virtual DbSet<barging_plan_monthly> barging_plan_monthly { get; set; }
        public virtual DbSet<barging_plan_monthly_history> barging_plan_monthly_history { get; set; }
        public virtual DbSet<barging_plan_rawdata> barging_plan_rawdata { get; set; }
        public virtual DbSet<barging_schema> barging_schema { get; set; }
        public virtual DbSet<barging_transaction> barging_transaction { get; set; }
        public virtual DbSet<belt_scale> belt_scale { get; set; }
        public virtual DbSet<benchmark_price_brand> benchmark_price_brand { get; set; }
        public virtual DbSet<benchmark_price_series> benchmark_price_series { get; set; }
        public virtual DbSet<benchmark_price_series_detail> benchmark_price_series_detail { get; set; }
        public virtual DbSet<bi_monitoring> bi_monitoring { get; set; }
        public virtual DbSet<bi_timesheet_detail> bi_timesheet_detail { get; set; }
        public virtual DbSet<bi_timesheet_event> bi_timesheet_event { get; set; }
        public virtual DbSet<bill_of_lading> bill_of_lading { get; set; }
        public virtual DbSet<blending_plan> blending_plan { get; set; }
        public virtual DbSet<blending_plan_product> blending_plan_product { get; set; }
        public virtual DbSet<blending_plan_quality> blending_plan_quality { get; set; }
        public virtual DbSet<blending_plan_source> blending_plan_source { get; set; }
        public virtual DbSet<blending_plan_value> blending_plan_value { get; set; }
        public virtual DbSet<bs_logs> bs_logs { get; set; }
        public virtual DbSet<business_area> business_area { get; set; }
        public virtual DbSet<business_days> business_days { get; set; }
        public virtual DbSet<business_partner> business_partner { get; set; }
        public virtual DbSet<business_unit> business_unit { get; set; }
        public virtual DbSet<calendar> calendar { get; set; }
        public virtual DbSet<chls> chls { get; set; }
        public virtual DbSet<chls_additional_info> chls_additional_info { get; set; }
        public virtual DbSet<chls_cpp> chls_cpp { get; set; }
        public virtual DbSet<chls_hauling> chls_hauling { get; set; }
        public virtual DbSet<city> city { get; set; }
        public virtual DbSet<closing_schema> closing_schema { get; set; }
        public virtual DbSet<coa> coa { get; set; }
        public virtual DbSet<coa_subaccount> coa_subaccount { get; set; }
        public virtual DbSet<coal_barging_allsite_daily_pct> coal_barging_allsite_daily_pct { get; set; }
        public virtual DbSet<coal_barging_allsite_monthly_pct> coal_barging_allsite_monthly_pct { get; set; }
        public virtual DbSet<coal_barging_allsite_mtd_pct> coal_barging_allsite_mtd_pct { get; set; }
        public virtual DbSet<coal_barging_allsite_yearly_pct> coal_barging_allsite_yearly_pct { get; set; }
        public virtual DbSet<coal_barging_allsite_ytd_pct> coal_barging_allsite_ytd_pct { get; set; }
        public virtual DbSet<coal_barging_daily_details> coal_barging_daily_details { get; set; }
        public virtual DbSet<coal_barging_daily_pct> coal_barging_daily_pct { get; set; }
        public virtual DbSet<coal_barging_details_all> coal_barging_details_all { get; set; }
        public virtual DbSet<coal_barging_mtd_details> coal_barging_mtd_details { get; set; }
        public virtual DbSet<coal_barging_mtd_pct> coal_barging_mtd_pct { get; set; }
        public virtual DbSet<coal_barging_mtfull_details> coal_barging_mtfull_details { get; set; }
        public virtual DbSet<coal_barging_mtfull_details_trans> coal_barging_mtfull_details_trans { get; set; }
        public virtual DbSet<coal_barging_pct_all> coal_barging_pct_all { get; set; }
        public virtual DbSet<coal_barging_rawdata> coal_barging_rawdata { get; set; }
        public virtual DbSet<coal_barging_ytd_details> coal_barging_ytd_details { get; set; }
        public virtual DbSet<coal_barging_ytd_pct> coal_barging_ytd_pct { get; set; }
        public virtual DbSet<coal_barging_ytfull_details> coal_barging_ytfull_details { get; set; }
        public virtual DbSet<coal_barging_ytfull_details_trans> coal_barging_ytfull_details_trans { get; set; }
        public virtual DbSet<coal_hauling_allsite_daily_pct> coal_hauling_allsite_daily_pct { get; set; }
        public virtual DbSet<coal_hauling_allsite_monthly_pct> coal_hauling_allsite_monthly_pct { get; set; }
        public virtual DbSet<coal_hauling_allsite_mtd_pct> coal_hauling_allsite_mtd_pct { get; set; }
        public virtual DbSet<coal_hauling_allsite_yearly_pct> coal_hauling_allsite_yearly_pct { get; set; }
        public virtual DbSet<coal_hauling_allsite_ytd_pct> coal_hauling_allsite_ytd_pct { get; set; }
        public virtual DbSet<coal_hauling_daily_details> coal_hauling_daily_details { get; set; }
        public virtual DbSet<coal_hauling_daily_pct> coal_hauling_daily_pct { get; set; }
        public virtual DbSet<coal_hauling_details_all> coal_hauling_details_all { get; set; }
        public virtual DbSet<coal_hauling_mtd_details> coal_hauling_mtd_details { get; set; }
        public virtual DbSet<coal_hauling_mtd_pct> coal_hauling_mtd_pct { get; set; }
        public virtual DbSet<coal_hauling_mtfull_details> coal_hauling_mtfull_details { get; set; }
        public virtual DbSet<coal_hauling_mtfull_details_trans> coal_hauling_mtfull_details_trans { get; set; }
        public virtual DbSet<coal_hauling_pct_all> coal_hauling_pct_all { get; set; }
        public virtual DbSet<coal_hauling_rawdata> coal_hauling_rawdata { get; set; }
        public virtual DbSet<coal_hauling_ytd_details> coal_hauling_ytd_details { get; set; }
        public virtual DbSet<coal_hauling_ytd_pct> coal_hauling_ytd_pct { get; set; }
        public virtual DbSet<coal_hauling_ytfull_details> coal_hauling_ytfull_details { get; set; }
        public virtual DbSet<coal_hauling_ytfull_details_trans> coal_hauling_ytfull_details_trans { get; set; }
        public virtual DbSet<coal_mine_allsite_daily_pct> coal_mine_allsite_daily_pct { get; set; }
        public virtual DbSet<coal_mine_allsite_monthly_pct> coal_mine_allsite_monthly_pct { get; set; }
        public virtual DbSet<coal_mine_allsite_mtd_pct> coal_mine_allsite_mtd_pct { get; set; }
        public virtual DbSet<coal_mine_allsite_yearly_pct> coal_mine_allsite_yearly_pct { get; set; }
        public virtual DbSet<coal_mine_allsite_ytd_pct> coal_mine_allsite_ytd_pct { get; set; }
        public virtual DbSet<coal_mine_daily_details> coal_mine_daily_details { get; set; }
        public virtual DbSet<coal_mine_daily_pct> coal_mine_daily_pct { get; set; }
        public virtual DbSet<coal_mine_details_all> coal_mine_details_all { get; set; }
        public virtual DbSet<coal_mine_mtd_details> coal_mine_mtd_details { get; set; }
        public virtual DbSet<coal_mine_mtd_pct> coal_mine_mtd_pct { get; set; }
        public virtual DbSet<coal_mine_mtfull_details> coal_mine_mtfull_details { get; set; }
        public virtual DbSet<coal_mine_mtfull_details_trans> coal_mine_mtfull_details_trans { get; set; }
        public virtual DbSet<coal_mine_pct_all> coal_mine_pct_all { get; set; }
        public virtual DbSet<coal_mine_rawdata> coal_mine_rawdata { get; set; }
        public virtual DbSet<coal_mine_ytd_details> coal_mine_ytd_details { get; set; }
        public virtual DbSet<coal_mine_ytd_pct> coal_mine_ytd_pct { get; set; }
        public virtual DbSet<coal_mine_ytfull_details> coal_mine_ytfull_details { get; set; }
        public virtual DbSet<coal_mine_ytfull_details_trans> coal_mine_ytfull_details_trans { get; set; }
        public virtual DbSet<coal_movement> coal_movement { get; set; }
        public virtual DbSet<coal_price_index_aims> coal_price_index_aims { get; set; }
        public virtual DbSet<coal_price_index_mtd_aims> coal_price_index_mtd_aims { get; set; }
        public virtual DbSet<coal_price_index_week_mtd_aims> coal_price_index_week_mtd_aims { get; set; }
        public virtual DbSet<coal_produce_allsite_daily_pct> coal_produce_allsite_daily_pct { get; set; }
        public virtual DbSet<coal_produce_allsite_monthly_pct> coal_produce_allsite_monthly_pct { get; set; }
        public virtual DbSet<coal_produce_allsite_mtd_pct> coal_produce_allsite_mtd_pct { get; set; }
        public virtual DbSet<coal_produce_allsite_yearly_pct> coal_produce_allsite_yearly_pct { get; set; }
        public virtual DbSet<coal_produce_allsite_ytd_pct> coal_produce_allsite_ytd_pct { get; set; }
        public virtual DbSet<coal_produce_daily_details> coal_produce_daily_details { get; set; }
        public virtual DbSet<coal_produce_daily_pct> coal_produce_daily_pct { get; set; }
        public virtual DbSet<coal_produce_details_all> coal_produce_details_all { get; set; }
        public virtual DbSet<coal_produce_mtd_details> coal_produce_mtd_details { get; set; }
        public virtual DbSet<coal_produce_mtd_pct> coal_produce_mtd_pct { get; set; }
        public virtual DbSet<coal_produce_mtfull_details> coal_produce_mtfull_details { get; set; }
        public virtual DbSet<coal_produce_mtfull_details_trans> coal_produce_mtfull_details_trans { get; set; }
        public virtual DbSet<coal_produce_pct_all> coal_produce_pct_all { get; set; }
        public virtual DbSet<coal_produce_rawdata> coal_produce_rawdata { get; set; }
        public virtual DbSet<coal_produce_ytd_details> coal_produce_ytd_details { get; set; }
        public virtual DbSet<coal_produce_ytd_pct> coal_produce_ytd_pct { get; set; }
        public virtual DbSet<coal_produce_ytfull_details> coal_produce_ytfull_details { get; set; }
        public virtual DbSet<coal_produce_ytfull_details_trans> coal_produce_ytfull_details_trans { get; set; }
        public virtual DbSet<coal_produced_sp> coal_produced_sp { get; set; }
        public virtual DbSet<coal_transfer> coal_transfer { get; set; }
        public virtual DbSet<coal_transfer_allsite_daily_pct> coal_transfer_allsite_daily_pct { get; set; }
        public virtual DbSet<coal_transfer_allsite_monthly_pct> coal_transfer_allsite_monthly_pct { get; set; }
        public virtual DbSet<coal_transfer_allsite_mtd_pct> coal_transfer_allsite_mtd_pct { get; set; }
        public virtual DbSet<coal_transfer_allsite_yearly_pct> coal_transfer_allsite_yearly_pct { get; set; }
        public virtual DbSet<coal_transfer_allsite_ytd_pct> coal_transfer_allsite_ytd_pct { get; set; }
        public virtual DbSet<coal_transfer_closing> coal_transfer_closing { get; set; }
        public virtual DbSet<coal_transfer_closing_item> coal_transfer_closing_item { get; set; }
        public virtual DbSet<coal_transfer_daily_details> coal_transfer_daily_details { get; set; }
        public virtual DbSet<coal_transfer_daily_pct> coal_transfer_daily_pct { get; set; }
        public virtual DbSet<coal_transfer_details_all> coal_transfer_details_all { get; set; }
        public virtual DbSet<coal_transfer_mtd_details> coal_transfer_mtd_details { get; set; }
        public virtual DbSet<coal_transfer_mtd_pct> coal_transfer_mtd_pct { get; set; }
        public virtual DbSet<coal_transfer_mtfull_details> coal_transfer_mtfull_details { get; set; }
        public virtual DbSet<coal_transfer_mtfull_details_trans> coal_transfer_mtfull_details_trans { get; set; }
        public virtual DbSet<coal_transfer_pct_all> coal_transfer_pct_all { get; set; }
        public virtual DbSet<coal_transfer_rawdata> coal_transfer_rawdata { get; set; }
        public virtual DbSet<coal_transfer_ytd_details> coal_transfer_ytd_details { get; set; }
        public virtual DbSet<coal_transfer_ytd_pct> coal_transfer_ytd_pct { get; set; }
        public virtual DbSet<coal_transfer_ytfull_details> coal_transfer_ytfull_details { get; set; }
        public virtual DbSet<coal_transfer_ytfull_details_trans> coal_transfer_ytfull_details_trans { get; set; }
        public virtual DbSet<contact> contact { get; set; }
        public virtual DbSet<contractor> contractor { get; set; }
        public virtual DbSet<contractor_document> contractor_document { get; set; }
        public virtual DbSet<contractor_type> contractor_type { get; set; }
        public virtual DbSet<cost_rate> cost_rate { get; set; }
        public virtual DbSet<country> country { get; set; }
        public virtual DbSet<cpp_location> cpp_location { get; set; }
        public virtual DbSet<cpp_product> cpp_product { get; set; }
        public virtual DbSet<credit_limit_history> credit_limit_history { get; set; }
        public virtual DbSet<currency> currency { get; set; }
        public virtual DbSet<currency_exchange> currency_exchange { get; set; }
        public virtual DbSet<currency_exchange_api> currency_exchange_api { get; set; }
        public virtual DbSet<customer> customer { get; set; }
        public virtual DbSet<customer_attachment> customer_attachment { get; set; }
        public virtual DbSet<customer_ell> customer_ell { get; set; }
        public virtual DbSet<customer_type> customer_type { get; set; }
        public virtual DbSet<daily_record> daily_record { get; set; }
        public virtual DbSet<daily_stock_port> daily_stock_port { get; set; }
        public virtual DbSet<data_extension> data_extension { get; set; }
        public virtual DbSet<dates_aims> dates_aims { get; set; }
        public virtual DbSet<daywork> daywork { get; set; }
        public virtual DbSet<daywork_closing> daywork_closing { get; set; }
        public virtual DbSet<daywork_detail> daywork_detail { get; set; }
        public virtual DbSet<delay> delay { get; set; }
        public virtual DbSet<delay_category> delay_category { get; set; }
        public virtual DbSet<delay_details> delay_details { get; set; }
        public virtual DbSet<despatch_demurrage> despatch_demurrage { get; set; }
        public virtual DbSet<despatch_demurrage_debit_credit_note> despatch_demurrage_debit_credit_note { get; set; }
        public virtual DbSet<despatch_demurrage_delay> despatch_demurrage_delay { get; set; }
        public virtual DbSet<despatch_demurrage_detail> despatch_demurrage_detail { get; set; }
        public virtual DbSet<despatch_demurrage_invoice> despatch_demurrage_invoice { get; set; }
        public virtual DbSet<despatch_order> despatch_order { get; set; }
        public virtual DbSet<despatch_order_delay> despatch_order_delay { get; set; }
        public virtual DbSet<despatch_order_document> despatch_order_document { get; set; }
        public virtual DbSet<despatch_order_ell> despatch_order_ell { get; set; }
        public virtual DbSet<despatch_order_product_specifications> despatch_order_product_specifications { get; set; }
        public virtual DbSet<direct_nplct_details> direct_nplct_details { get; set; }
        public virtual DbSet<document_type> document_type { get; set; }
        public virtual DbSet<draft_survey> draft_survey { get; set; }
        public virtual DbSet<draft_survey_detail> draft_survey_detail { get; set; }
        public virtual DbSet<draft_survey_document> draft_survey_document { get; set; }
        public virtual DbSet<drill_blast_plan> drill_blast_plan { get; set; }
        public virtual DbSet<eight_week_forecast> eight_week_forecast { get; set; }
        public virtual DbSet<eight_week_forecast_item> eight_week_forecast_item { get; set; }
        public virtual DbSet<eight_week_forecast_item_detail> eight_week_forecast_item_detail { get; set; }
        public virtual DbSet<ell_sync> ell_sync { get; set; }
        public virtual DbSet<email_notification> email_notification { get; set; }
        public virtual DbSet<email_recipient> email_recipient { get; set; }
        public virtual DbSet<employee> employee { get; set; }
        public virtual DbSet<entity> entity { get; set; }
        public virtual DbSet<equipment> equipment { get; set; }
        public virtual DbSet<equipment_category> equipment_category { get; set; }
        public virtual DbSet<equipment_cost_rate> equipment_cost_rate { get; set; }
        public virtual DbSet<equipment_group> equipment_group { get; set; }
        public virtual DbSet<equipment_group_item> equipment_group_item { get; set; }
        public virtual DbSet<equipment_incident> equipment_incident { get; set; }
        public virtual DbSet<equipment_type> equipment_type { get; set; }
        public virtual DbSet<equipment_usage_transaction> equipment_usage_transaction { get; set; }
        public virtual DbSet<equipment_usage_transaction_detail> equipment_usage_transaction_detail { get; set; }
        public virtual DbSet<event_category> event_category { get; set; }
        public virtual DbSet<event_definition_category> event_definition_category { get; set; }
        public virtual DbSet<explosive_usage_plan> explosive_usage_plan { get; set; }
        public virtual DbSet<explosive_usage_plan_detail> explosive_usage_plan_detail { get; set; }
        public virtual DbSet<exposed_coal> exposed_coal { get; set; }
        public virtual DbSet<fuel_consumption> fuel_consumption { get; set; }
        public virtual DbSet<fuel_inventory> fuel_inventory { get; set; }
        public virtual DbSet<hauling_closing> hauling_closing { get; set; }
        public virtual DbSet<hauling_closing_item> hauling_closing_item { get; set; }
        public virtual DbSet<hauling_plan> hauling_plan { get; set; }
        public virtual DbSet<hauling_plan_daily> hauling_plan_daily { get; set; }
        public virtual DbSet<hauling_plan_history> hauling_plan_history { get; set; }
        public virtual DbSet<hauling_plan_monthly> hauling_plan_monthly { get; set; }
        public virtual DbSet<hauling_plan_monthly_history> hauling_plan_monthly_history { get; set; }
        public virtual DbSet<hauling_schema> hauling_schema { get; set; }
        public virtual DbSet<hauling_transaction> hauling_transaction { get; set; }
        public virtual DbSet<haze> haze { get; set; }
        public virtual DbSet<historical_stock_mutation> historical_stock_mutation { get; set; }
        public virtual DbSet<incident> incident { get; set; }
        public virtual DbSet<incident_category> incident_category { get; set; }
        public virtual DbSet<initial_information> initial_information { get; set; }
        public virtual DbSet<integrasi_ickm33> integrasi_ickm33 { get; set; }
        public virtual DbSet<integrasi_icport> integrasi_icport { get; set; }
        public virtual DbSet<integrasi_kmiashs> integrasi_kmiashs { get; set; }
        public virtual DbSet<integrasi_timesheet> integrasi_timesheet { get; set; }
        public virtual DbSet<integrasi_timesheetplan> integrasi_timesheetplan { get; set; }
        public virtual DbSet<inventory_transaction> inventory_transaction { get; set; }
        public virtual DbSet<inventory_transaction_detail> inventory_transaction_detail { get; set; }
        public virtual DbSet<item_class_view> item_class_view { get; set; }
        public virtual DbSet<item_group> item_group { get; set; }
        public virtual DbSet<item_list> item_list { get; set; }
        public virtual DbSet<joint_survey> joint_survey { get; set; }
        public virtual DbSet<joint_survey_analyte> joint_survey_analyte { get; set; }
        public virtual DbSet<joint_survey_attachment> joint_survey_attachment { get; set; }
        public virtual DbSet<joint_survey_detail> joint_survey_detail { get; set; }
        public virtual DbSet<land_clearing> land_clearing { get; set; }
        public virtual DbSet<land_clearing_detail> land_clearing_detail { get; set; }
        public virtual DbSet<list_key> list_key { get; set; }
        public virtual DbSet<list_value> list_value { get; set; }
        public virtual DbSet<lq_proportion> lq_proportion { get; set; }
        public virtual DbSet<master_list> master_list { get; set; }
        public virtual DbSet<master_month> master_month { get; set; }
        public virtual DbSet<master_month_historical_rain> master_month_historical_rain { get; set; }
        public virtual DbSet<material_type> material_type { get; set; }
        public virtual DbSet<material_type_analyte> material_type_analyte { get; set; }
        public virtual DbSet<mine_location> mine_location { get; set; }
        public virtual DbSet<mine_location_quality> mine_location_quality { get; set; }
        public virtual DbSet<mine_location_quality_pit> mine_location_quality_pit { get; set; }
        public virtual DbSet<mine_plan_geology> mine_plan_geology { get; set; }
        public virtual DbSet<mine_plan_ltp> mine_plan_ltp { get; set; }
        public virtual DbSet<mine_schema> mine_schema { get; set; }
        public virtual DbSet<mineplan_geology_history> mineplan_geology_history { get; set; }
        public virtual DbSet<mineplan_ltp_history> mineplan_ltp_history { get; set; }
        public virtual DbSet<model_geology> model_geology { get; set; }
        public virtual DbSet<months> months { get; set; }
        public virtual DbSet<mutation_product> mutation_product { get; set; }
        public virtual DbSet<mv_business_area_index> mv_business_area_index { get; set; }
        public virtual DbSet<mv_mutation> mv_mutation { get; set; }
        public virtual DbSet<mv_stockpile_mutation> mv_stockpile_mutation { get; set; }
        public virtual DbSet<mv_stockpile_mutation_arutmin> mv_stockpile_mutation_arutmin { get; set; }
        public virtual DbSet<mv_stockpile_mutation_daily> mv_stockpile_mutation_daily { get; set; }
        public virtual DbSet<mv_stockpile_mutation_kmia> mv_stockpile_mutation_kmia { get; set; }
        public virtual DbSet<mv_stockpile_mutation_mstr> mv_stockpile_mutation_mstr { get; set; }
        public virtual DbSet<mv_stockpile_mutation_quality> mv_stockpile_mutation_quality { get; set; }
        public virtual DbSet<mv_stockpile_mutation_quality_kmia> mv_stockpile_mutation_quality_kmia { get; set; }
        public virtual DbSet<mv_stockpile_mutation_quality_mstr> mv_stockpile_mutation_quality_mstr { get; set; }
        public virtual DbSet<national_holiday> national_holiday { get; set; }
        public virtual DbSet<nplct_stock_raw_data> nplct_stock_raw_data { get; set; }
        public virtual DbSet<organization> organization { get; set; }
        public virtual DbSet<parent_despatch_order> parent_despatch_order { get; set; }
        public virtual DbSet<port_location> port_location { get; set; }
        public virtual DbSet<price_adjustment_product_specification> price_adjustment_product_specification { get; set; }
        public virtual DbSet<price_analyst_arm> price_analyst_arm { get; set; }
        public virtual DbSet<price_analyst_invoice_arm> price_analyst_invoice_arm { get; set; }
        public virtual DbSet<price_analyst_uninvoice_arm> price_analyst_uninvoice_arm { get; set; }
        public virtual DbSet<price_index> price_index { get; set; }
        public virtual DbSet<price_index_history> price_index_history { get; set; }
        public virtual DbSet<price_index_map> price_index_map { get; set; }
        public virtual DbSet<price_index_map_detail> price_index_map_detail { get; set; }
        public virtual DbSet<price_index_product_specification> price_index_product_specification { get; set; }
        public virtual DbSet<price_series> price_series { get; set; }
        public virtual DbSet<price_series_detail> price_series_detail { get; set; }
        public virtual DbSet<process_flow> process_flow { get; set; }
        public virtual DbSet<process_flow_analyte> process_flow_analyte { get; set; }
        public virtual DbSet<processing_category> processing_category { get; set; }
        public virtual DbSet<processing_closing> processing_closing { get; set; }
        public virtual DbSet<processing_closing_item> processing_closing_item { get; set; }
        public virtual DbSet<processing_transaction> processing_transaction { get; set; }
        public virtual DbSet<produce_schema> produce_schema { get; set; }
        public virtual DbSet<product> product { get; set; }
        public virtual DbSet<product_category> product_category { get; set; }
        public virtual DbSet<product_specification> product_specification { get; set; }
        public virtual DbSet<product_standard> product_standard { get; set; }
        public virtual DbSet<production_closing> production_closing { get; set; }
        public virtual DbSet<production_closing_item> production_closing_item { get; set; }
        public virtual DbSet<production_plan> production_plan { get; set; }
        public virtual DbSet<production_plan_daily> production_plan_daily { get; set; }
        public virtual DbSet<production_plan_history> production_plan_history { get; set; }
        public virtual DbSet<production_plan_monthly> production_plan_monthly { get; set; }
        public virtual DbSet<production_plan_monthly_history> production_plan_monthly_history { get; set; }
        public virtual DbSet<production_plan_rawdata> production_plan_rawdata { get; set; }
        public virtual DbSet<production_plan_schema> production_plan_schema { get; set; }
        public virtual DbSet<production_transaction> production_transaction { get; set; }
        public virtual DbSet<production_transaction_item> production_transaction_item { get; set; }
        public virtual DbSet<progress_claim> progress_claim { get; set; }
        public virtual DbSet<progress_claim_charge> progress_claim_charge { get; set; }
        public virtual DbSet<province> province { get; set; }
        public virtual DbSet<quality_sampling> quality_sampling { get; set; }
        public virtual DbSet<quality_sampling_analyte> quality_sampling_analyte { get; set; }
        public virtual DbSet<quality_sampling_approval> quality_sampling_approval { get; set; }
        public virtual DbSet<quality_sampling_document> quality_sampling_document { get; set; }
        public virtual DbSet<railing_transaction> railing_transaction { get; set; }
        public virtual DbSet<railing_transaction_detail> railing_transaction_detail { get; set; }
        public virtual DbSet<raindelay_graph_aims> raindelay_graph_aims { get; set; }
        public virtual DbSet<raindelay_graph_ecocoal_aims> raindelay_graph_ecocoal_aims { get; set; }
        public virtual DbSet<raindelay_graph_ecoob_aims> raindelay_graph_ecoob_aims { get; set; }
        public virtual DbSet<raindelay_graph_ob_aims> raindelay_graph_ob_aims { get; set; }
        public virtual DbSet<rainfall> rainfall { get; set; }
        public virtual DbSet<rainfall_graph_aims> rainfall_graph_aims { get; set; }
        public virtual DbSet<rainfall_historical> rainfall_historical { get; set; }
        public virtual DbSet<rainfall_landingpage> rainfall_landingpage { get; set; }
        public virtual DbSet<rainfall_schema> rainfall_schema { get; set; }
        public virtual DbSet<raw_data_soa_export> raw_data_soa_export { get; set; }
        public virtual DbSet<raw_data_soa_pln> raw_data_soa_pln { get; set; }
        public virtual DbSet<raw_report_stockpile_summary_fp> raw_report_stockpile_summary_fp { get; set; }
        public virtual DbSet<ready_to_get> ready_to_get { get; set; }
        public virtual DbSet<reference_price_series> reference_price_series { get; set; }
        public virtual DbSet<rehandling_closing> rehandling_closing { get; set; }
        public virtual DbSet<rehandling_closing_item> rehandling_closing_item { get; set; }
        public virtual DbSet<rehandling_transaction> rehandling_transaction { get; set; }
        public virtual DbSet<report_template> report_template { get; set; }
        public virtual DbSet<role_access> role_access { get; set; }
        public virtual DbSet<role_business_unit> role_business_unit { get; set; }
        public virtual DbSet<role_exclusion> role_exclusion { get; set; }
        public virtual DbSet<royalty> royalty { get; set; }
        public virtual DbSet<royalty_cost> royalty_cost { get; set; }
        public virtual DbSet<royalty_payment> royalty_payment { get; set; }
        public virtual DbSet<royalty_pricing> royalty_pricing { get; set; }
        public virtual DbSet<royalty_quantity_quality> royalty_quantity_quality { get; set; }
        public virtual DbSet<royalty_valuation> royalty_valuation { get; set; }
        public virtual DbSet<rpt_join_survey_contract> rpt_join_survey_contract { get; set; }
        public virtual DbSet<rpt_joint_survey> rpt_joint_survey { get; set; }
        public virtual DbSet<rpt_production> rpt_production { get; set; }
        public virtual DbSet<rpt_timesheet> rpt_timesheet { get; set; }
        public virtual DbSet<rpt_timesheet_actplan> rpt_timesheet_actplan { get; set; }
        public virtual DbSet<rpt_timesheet_actplan_shift> rpt_timesheet_actplan_shift { get; set; }
        public virtual DbSet<rpt_timesheet_coal_day> rpt_timesheet_coal_day { get; set; }
        public virtual DbSet<rpt_timesheet_coal_day_event> rpt_timesheet_coal_day_event { get; set; }
        public virtual DbSet<rpt_timesheet_coal_day_plan> rpt_timesheet_coal_day_plan { get; set; }
        public virtual DbSet<rpt_timesheet_detail> rpt_timesheet_detail { get; set; }
        public virtual DbSet<rpt_timesheet_hmkaryawan> rpt_timesheet_hmkaryawan { get; set; }
        public virtual DbSet<rpt_timesheet_hourly> rpt_timesheet_hourly { get; set; }
        public virtual DbSet<rpt_timesheet_hourly_paua> rpt_timesheet_hourly_paua { get; set; }
        public virtual DbSet<rpt_timesheet_hourly_paua_byloader> rpt_timesheet_hourly_paua_byloader { get; set; }
        public virtual DbSet<rpt_timesheet_hourly_paua_byloader_performance> rpt_timesheet_hourly_paua_byloader_performance { get; set; }
        public virtual DbSet<rpt_timesheet_hourly_paua_byloader_plan> rpt_timesheet_hourly_paua_byloader_plan { get; set; }
        public virtual DbSet<rpt_timesheet_hourly_paua_old> rpt_timesheet_hourly_paua_old { get; set; }
        public virtual DbSet<rpt_timesheet_hourly_paua_plan> rpt_timesheet_hourly_paua_plan { get; set; }
        public virtual DbSet<rpt_timesheet_hourly_plan> rpt_timesheet_hourly_plan { get; set; }
        public virtual DbSet<rpt_timesheet_hr_only> rpt_timesheet_hr_only { get; set; }
        public virtual DbSet<rpt_timesheet_hr_only_ach> rpt_timesheet_hr_only_ach { get; set; }
        public virtual DbSet<rpt_timesheet_hr_only_ach_fuel> rpt_timesheet_hr_only_ach_fuel { get; set; }
        public virtual DbSet<rpt_timesheet_hr_only_achivement> rpt_timesheet_hr_only_achivement { get; set; }
        public virtual DbSet<rpt_timesheet_hr_only_bycnunit> rpt_timesheet_hr_only_bycnunit { get; set; }
        public virtual DbSet<rpt_timesheet_hr_only_bycnunit_obcoal> rpt_timesheet_hr_only_bycnunit_obcoal { get; set; }
        public virtual DbSet<rpt_timesheet_ob_day> rpt_timesheet_ob_day { get; set; }
        public virtual DbSet<rpt_timesheet_ob_day_event> rpt_timesheet_ob_day_event { get; set; }
        public virtual DbSet<rpt_timesheet_ob_day_plan> rpt_timesheet_ob_day_plan { get; set; }
        public virtual DbSet<rpt_timesheet_paua> rpt_timesheet_paua { get; set; }
        public virtual DbSet<rpt_timesheet_paua_old> rpt_timesheet_paua_old { get; set; }
        public virtual DbSet<rpt_timesheet_paua_performance> rpt_timesheet_paua_performance { get; set; }
        public virtual DbSet<rpt_timesheet_paua_productivity> rpt_timesheet_paua_productivity { get; set; }
        public virtual DbSet<rpt_timesheet_plan> rpt_timesheet_plan { get; set; }
        public virtual DbSet<rpt_timesheet_plan_detail> rpt_timesheet_plan_detail { get; set; }
        public virtual DbSet<rpt_timesheet_plan_detail_sh1> rpt_timesheet_plan_detail_sh1 { get; set; }
        public virtual DbSet<rpt_timesheet_plan_detail_sh2> rpt_timesheet_plan_detail_sh2 { get; set; }
        public virtual DbSet<rpt_timesheet_plan_hourly> rpt_timesheet_plan_hourly { get; set; }
        public virtual DbSet<rpt_timesheet_progressive> rpt_timesheet_progressive { get; set; }
        public virtual DbSet<rpt_timesheet_progressive_actplan> rpt_timesheet_progressive_actplan { get; set; }
        public virtual DbSet<rpt_timesheet_progressive_plan> rpt_timesheet_progressive_plan { get; set; }
        public virtual DbSet<rpt_timesheethr> rpt_timesheethr { get; set; }
        public virtual DbSet<sales_charge> sales_charge { get; set; }
        public virtual DbSet<sales_contract> sales_contract { get; set; }
        public virtual DbSet<sales_contract_attachment> sales_contract_attachment { get; set; }
        public virtual DbSet<sales_contract_charges> sales_contract_charges { get; set; }
        public virtual DbSet<sales_contract_despatch_demurrage_delay> sales_contract_despatch_demurrage_delay { get; set; }
        public virtual DbSet<sales_contract_despatch_demurrage_term> sales_contract_despatch_demurrage_term { get; set; }
        public virtual DbSet<sales_contract_despatch_plan> sales_contract_despatch_plan { get; set; }
        public virtual DbSet<sales_contract_detail> sales_contract_detail { get; set; }
        public virtual DbSet<sales_contract_end_user> sales_contract_end_user { get; set; }
        public virtual DbSet<sales_contract_group> sales_contract_group { get; set; }
        public virtual DbSet<sales_contract_invoice_target> sales_contract_invoice_target { get; set; }
        public virtual DbSet<sales_contract_payment_term> sales_contract_payment_term { get; set; }
        public virtual DbSet<sales_contract_product> sales_contract_product { get; set; }
        public virtual DbSet<sales_contract_product_specifications> sales_contract_product_specifications { get; set; }
        public virtual DbSet<sales_contract_quotation_price> sales_contract_quotation_price { get; set; }
        public virtual DbSet<sales_contract_taxes> sales_contract_taxes { get; set; }
        public virtual DbSet<sales_contract_term> sales_contract_term { get; set; }
        public virtual DbSet<sales_demurrage_rate> sales_demurrage_rate { get; set; }
        public virtual DbSet<sales_formula_variable> sales_formula_variable { get; set; }
        public virtual DbSet<sales_invoice> sales_invoice { get; set; }
        public virtual DbSet<sales_invoice_approval> sales_invoice_approval { get; set; }
        public virtual DbSet<sales_invoice_attachment> sales_invoice_attachment { get; set; }
        public virtual DbSet<sales_invoice_charges> sales_invoice_charges { get; set; }
        public virtual DbSet<sales_invoice_detail> sales_invoice_detail { get; set; }
        public virtual DbSet<sales_invoice_ell> sales_invoice_ell { get; set; }
        public virtual DbSet<sales_invoice_html_content> sales_invoice_html_content { get; set; }
        public virtual DbSet<sales_invoice_payment> sales_invoice_payment { get; set; }
        public virtual DbSet<sales_invoice_product_specifications> sales_invoice_product_specifications { get; set; }
        public virtual DbSet<sales_invoice_trace_element> sales_invoice_trace_element { get; set; }
        public virtual DbSet<sales_invoice_transhipment> sales_invoice_transhipment { get; set; }
        public virtual DbSet<sales_month_summary_aims> sales_month_summary_aims { get; set; }
        public virtual DbSet<sales_order> sales_order { get; set; }
        public virtual DbSet<sales_order_detail> sales_order_detail { get; set; }
        public virtual DbSet<sales_plan> sales_plan { get; set; }
        public virtual DbSet<sales_plan_customer> sales_plan_customer { get; set; }
        public virtual DbSet<sales_plan_detail> sales_plan_detail { get; set; }
        public virtual DbSet<sales_plan_snapshot> sales_plan_snapshot { get; set; }
        public virtual DbSet<sales_price> sales_price { get; set; }
        public virtual DbSet<sales_price_type> sales_price_type { get; set; }
        public virtual DbSet<sales_product_specification> sales_product_specification { get; set; }
        public virtual DbSet<sales_summary_aims> sales_summary_aims { get; set; }
        public virtual DbSet<sales_summary_aims_by_brand> sales_summary_aims_by_brand { get; set; }
        public virtual DbSet<sales_summary_aims_by_destination> sales_summary_aims_by_destination { get; set; }
        public virtual DbSet<sales_summary_aims_by_shipment> sales_summary_aims_by_shipment { get; set; }
        public virtual DbSet<sales_summary_aims_invoice> sales_summary_aims_invoice { get; set; }
        public virtual DbSet<sales_summary_aims_invoice_by_brand> sales_summary_aims_invoice_by_brand { get; set; }
        public virtual DbSet<sales_summary_aims_invoice_by_destination> sales_summary_aims_invoice_by_destination { get; set; }
        public virtual DbSet<sales_summary_aims_invoice_by_shipment> sales_summary_aims_invoice_by_shipment { get; set; }
        public virtual DbSet<sales_summary_aims_non_invoice> sales_summary_aims_non_invoice { get; set; }
        public virtual DbSet<sales_summary_aims_uninvoice_by_brand> sales_summary_aims_uninvoice_by_brand { get; set; }
        public virtual DbSet<sales_summary_aims_uninvoice_by_destination> sales_summary_aims_uninvoice_by_destination { get; set; }
        public virtual DbSet<sales_summary_aims_uninvoice_by_shipment> sales_summary_aims_uninvoice_by_shipment { get; set; }
        public virtual DbSet<sampling_template> sampling_template { get; set; }
        public virtual DbSet<sampling_template_detail> sampling_template_detail { get; set; }
        public virtual DbSet<sampling_type> sampling_type { get; set; }
        public virtual DbSet<shared_record> shared_record { get; set; }
        public virtual DbSet<shift> shift { get; set; }
        public virtual DbSet<shift_category> shift_category { get; set; }
        public virtual DbSet<shipment_forecast> shipment_forecast { get; set; }
        public virtual DbSet<shipment_plan> shipment_plan { get; set; }
        public virtual DbSet<shipping_closing> shipping_closing { get; set; }
        public virtual DbSet<shipping_closing_item> shipping_closing_item { get; set; }
        public virtual DbSet<shipping_cost> shipping_cost { get; set; }
        public virtual DbSet<shipping_cost_document> shipping_cost_document { get; set; }
        public virtual DbSet<shipping_delay> shipping_delay { get; set; }
        public virtual DbSet<shipping_document> shipping_document { get; set; }
        public virtual DbSet<shipping_instruction> shipping_instruction { get; set; }
        public virtual DbSet<shipping_instruction_asuransi> shipping_instruction_asuransi { get; set; }
        public virtual DbSet<shipping_instruction_attachment> shipping_instruction_attachment { get; set; }
        public virtual DbSet<shipping_instruction_detail_survey> shipping_instruction_detail_survey { get; set; }
        public virtual DbSet<shipping_instruction_detail_survey_document> shipping_instruction_detail_survey_document { get; set; }
        public virtual DbSet<shipping_instruction_document_agent> shipping_instruction_document_agent { get; set; }
        public virtual DbSet<shipping_instruction_pekerjaan_agent> shipping_instruction_pekerjaan_agent { get; set; }
        public virtual DbSet<shipping_instruction_stevedoring> shipping_instruction_stevedoring { get; set; }
        public virtual DbSet<shipping_instruction_to_company> shipping_instruction_to_company { get; set; }
        public virtual DbSet<shipping_instruction_tug_boat> shipping_instruction_tug_boat { get; set; }
        public virtual DbSet<shipping_load_unload_document> shipping_load_unload_document { get; set; }
        public virtual DbSet<shipping_loading_detail> shipping_loading_detail { get; set; }
        public virtual DbSet<shipping_program> shipping_program { get; set; }
        public virtual DbSet<shipping_program_detail> shipping_program_detail { get; set; }
        public virtual DbSet<shipping_transaction> shipping_transaction { get; set; }
        public virtual DbSet<shipping_transaction_detail> shipping_transaction_detail { get; set; }
        public virtual DbSet<shipping_transaction_lq> shipping_transaction_lq { get; set; }
        public virtual DbSet<sils> sils { get; set; }
        public virtual DbSet<sils_detail> sils_detail { get; set; }
        public virtual DbSet<sils_nplct> sils_nplct { get; set; }
        public virtual DbSet<sils_nplct_detail> sils_nplct_detail { get; set; }
        public virtual DbSet<sils_nplct_subdetail> sils_nplct_subdetail { get; set; }
        public virtual DbSet<sils_shipment_nplct_details> sils_shipment_nplct_details { get; set; }
        public virtual DbSet<sils_unloading> sils_unloading { get; set; }
        public virtual DbSet<sils_unloading_detail> sils_unloading_detail { get; set; }
        public virtual DbSet<sils_unloading_nplct_details> sils_unloading_nplct_details { get; set; }
        public virtual DbSet<sils_unloading_subdetail> sils_unloading_subdetail { get; set; }
        public virtual DbSet<sip_logs> sip_logs { get; set; }
        public virtual DbSet<slippery> slippery { get; set; }
        public virtual DbSet<slippery_schema> slippery_schema { get; set; }
        public virtual DbSet<sof> sof { get; set; }
        public virtual DbSet<sof_detail> sof_detail { get; set; }
        public virtual DbSet<sof_document> sof_document { get; set; }
        public virtual DbSet<sr_actual_all> sr_actual_all { get; set; }
        public virtual DbSet<sr_budget_all> sr_budget_all { get; set; }
        public virtual DbSet<sr_daily_actual> sr_daily_actual { get; set; }
        public virtual DbSet<sr_daily_budget> sr_daily_budget { get; set; }
        public virtual DbSet<sr_mtd_actual> sr_mtd_actual { get; set; }
        public virtual DbSet<sr_mtd_budget> sr_mtd_budget { get; set; }
        public virtual DbSet<sr_ytd_actual> sr_ytd_actual { get; set; }
        public virtual DbSet<sr_ytd_budget> sr_ytd_budget { get; set; }
        public virtual DbSet<standard_cost_mining> standard_cost_mining { get; set; }
        public virtual DbSet<standard_cost_mining_detail> standard_cost_mining_detail { get; set; }
        public virtual DbSet<stock_location> stock_location { get; set; }
        public virtual DbSet<stock_nplct> stock_nplct { get; set; }
        public virtual DbSet<stock_nplct_all_transaction> stock_nplct_all_transaction { get; set; }
        public virtual DbSet<stock_nplct_discharge> stock_nplct_discharge { get; set; }
        public virtual DbSet<stock_perunit_production_aims> stock_perunit_production_aims { get; set; }
        public virtual DbSet<stock_production_aims> stock_production_aims { get; set; }
        public virtual DbSet<stock_rom_hopper> stock_rom_hopper { get; set; }
        public virtual DbSet<stock_rom_pit> stock_rom_pit { get; set; }
        public virtual DbSet<stock_rom_port> stock_rom_port { get; set; }
        public virtual DbSet<stock_state> stock_state { get; set; }
        public virtual DbSet<stock_state_analyte> stock_state_analyte { get; set; }
        public virtual DbSet<stockpile_location> stockpile_location { get; set; }
        public virtual DbSet<stockpile_location_detail> stockpile_location_detail { get; set; }
        public virtual DbSet<stockpile_state> stockpile_state { get; set; }
        public virtual DbSet<survey> survey { get; set; }
        public virtual DbSet<survey_analyte> survey_analyte { get; set; }
        public virtual DbSet<survey_detail> survey_detail { get; set; }
        public virtual DbSet<sync_log> sync_log { get; set; }
        public virtual DbSet<tax> tax { get; set; }
        public virtual DbSet<team> team { get; set; }
        public virtual DbSet<team_member> team_member { get; set; }
        public virtual DbSet<team_role> team_role { get; set; }
        public virtual DbSet<temp_quality_mutation> temp_quality_mutation { get; set; }
        public virtual DbSet<temp_quality_mutation1> temp_quality_mutation1 { get; set; }
        public virtual DbSet<temp_quality_mutation10_kmia> temp_quality_mutation10_kmia { get; set; }
        public virtual DbSet<temp_quality_mutation11_kmia> temp_quality_mutation11_kmia { get; set; }
        public virtual DbSet<temp_quality_mutation1_kmia> temp_quality_mutation1_kmia { get; set; }
        public virtual DbSet<temp_quality_mutation2> temp_quality_mutation2 { get; set; }
        public virtual DbSet<temp_quality_mutation2_kmia> temp_quality_mutation2_kmia { get; set; }
        public virtual DbSet<temp_quality_mutation3> temp_quality_mutation3 { get; set; }
        public virtual DbSet<temp_quality_mutation3_kmia> temp_quality_mutation3_kmia { get; set; }
        public virtual DbSet<temp_quality_mutation4> temp_quality_mutation4 { get; set; }
        public virtual DbSet<temp_quality_mutation4_kmia> temp_quality_mutation4_kmia { get; set; }
        public virtual DbSet<temp_quality_mutation5> temp_quality_mutation5 { get; set; }
        public virtual DbSet<temp_quality_mutation5_kmia> temp_quality_mutation5_kmia { get; set; }
        public virtual DbSet<temp_quality_mutation6> temp_quality_mutation6 { get; set; }
        public virtual DbSet<temp_quality_mutation6_kmia> temp_quality_mutation6_kmia { get; set; }
        public virtual DbSet<temp_quality_mutation7> temp_quality_mutation7 { get; set; }
        public virtual DbSet<temp_quality_mutation7_kmia> temp_quality_mutation7_kmia { get; set; }
        public virtual DbSet<temp_quality_mutation8> temp_quality_mutation8 { get; set; }
        public virtual DbSet<temp_quality_mutation8_kmia> temp_quality_mutation8_kmia { get; set; }
        public virtual DbSet<temp_quality_mutation9_kmia> temp_quality_mutation9_kmia { get; set; }
        public virtual DbSet<tidalwave> tidalwave { get; set; }
        public virtual DbSet<timesheet> timesheet { get; set; }
        public virtual DbSet<timesheet_detail> timesheet_detail { get; set; }
        public virtual DbSet<timesheet_detail_event> timesheet_detail_event { get; set; }
        public virtual DbSet<timesheet_detail_event_del> timesheet_detail_event_del { get; set; }
        public virtual DbSet<timesheet_detail_event_plan> timesheet_detail_event_plan { get; set; }
        public virtual DbSet<timesheet_detail_plan> timesheet_detail_plan { get; set; }
        public virtual DbSet<timesheet_detail_productivity_problem> timesheet_detail_productivity_problem { get; set; }
        public virtual DbSet<timesheet_detail_productivity_problem_plan> timesheet_detail_productivity_problem_plan { get; set; }
        public virtual DbSet<timesheet_plan> timesheet_plan { get; set; }
        public virtual DbSet<train> train { get; set; }
        public virtual DbSet<transfer_schema> transfer_schema { get; set; }
        public virtual DbSet<transhipment_nplct_details> transhipment_nplct_details { get; set; }
        public virtual DbSet<transport> transport { get; set; }
        public virtual DbSet<truck> truck { get; set; }
        public virtual DbSet<truck_cost_rate> truck_cost_rate { get; set; }
        public virtual DbSet<trucking_nplct_details> trucking_nplct_details { get; set; }
        public virtual DbSet<tug> tug { get; set; }
        public virtual DbSet<uom> uom { get; set; }
        public virtual DbSet<uom_category> uom_category { get; set; }
        public virtual DbSet<uom_conversion> uom_conversion { get; set; }
        public virtual DbSet<user_role> user_role { get; set; }
        public virtual DbSet<vessel> vessel { get; set; }
        public virtual DbSet<vw_accounting_period> vw_accounting_period { get; set; }
        public virtual DbSet<vw_advance_contract> vw_advance_contract { get; set; }
        public virtual DbSet<vw_advance_contract_charge> vw_advance_contract_charge { get; set; }
        public virtual DbSet<vw_advance_contract_charge_detail> vw_advance_contract_charge_detail { get; set; }
        public virtual DbSet<vw_advance_contract_detail> vw_advance_contract_detail { get; set; }
        public virtual DbSet<vw_advance_contract_item> vw_advance_contract_item { get; set; }
        public virtual DbSet<vw_advance_contract_item_detail> vw_advance_contract_item_detail { get; set; }
        public virtual DbSet<vw_advance_contract_reference> vw_advance_contract_reference { get; set; }
        public virtual DbSet<vw_advance_contract_reference_detail> vw_advance_contract_reference_detail { get; set; }
        public virtual DbSet<vw_advance_contract_valuation> vw_advance_contract_valuation { get; set; }
        public virtual DbSet<vw_advance_contract_valuation_detail> vw_advance_contract_valuation_detail { get; set; }
        public virtual DbSet<vw_advance_contract_valuation_download> vw_advance_contract_valuation_download { get; set; }
        public virtual DbSet<vw_analyte> vw_analyte { get; set; }
        public virtual DbSet<vw_application_business_unit> vw_application_business_unit { get; set; }
        public virtual DbSet<vw_application_entity> vw_application_entity { get; set; }
        public virtual DbSet<vw_application_role> vw_application_role { get; set; }
        public virtual DbSet<vw_application_role_report> vw_application_role_report { get; set; }
        public virtual DbSet<vw_application_user> vw_application_user { get; set; }
        public virtual DbSet<vw_arutmin_waste_removal> vw_arutmin_waste_removal { get; set; }
        public virtual DbSet<vw_arutmin_waste_removal_item> vw_arutmin_waste_removal_item { get; set; }
        public virtual DbSet<vw_audit_trail> vw_audit_trail { get; set; }
        public virtual DbSet<vw_bank> vw_bank { get; set; }
        public virtual DbSet<vw_bank_account> vw_bank_account { get; set; }
        public virtual DbSet<vw_barge> vw_barge { get; set; }
        public virtual DbSet<vw_barge_rotation> vw_barge_rotation { get; set; }
        public virtual DbSet<vw_barging_load_unload_document> vw_barging_load_unload_document { get; set; }
        public virtual DbSet<vw_barging_plan> vw_barging_plan { get; set; }
        public virtual DbSet<vw_barging_plan_daily> vw_barging_plan_daily { get; set; }
        public virtual DbSet<vw_barging_plan_monthly> vw_barging_plan_monthly { get; set; }
        public virtual DbSet<vw_barging_plan_monthly_history> vw_barging_plan_monthly_history { get; set; }
        public virtual DbSet<vw_barging_plan_nplct> vw_barging_plan_nplct { get; set; }
        public virtual DbSet<vw_barging_transaction> vw_barging_transaction { get; set; }
        public virtual DbSet<vw_belt_scale> vw_belt_scale { get; set; }
        public virtual DbSet<vw_benchmark_price_series> vw_benchmark_price_series { get; set; }
        public virtual DbSet<vw_benchmark_price_series_detail> vw_benchmark_price_series_detail { get; set; }
        public virtual DbSet<vw_blending_plan> vw_blending_plan { get; set; }
        public virtual DbSet<vw_blending_plan_product> vw_blending_plan_product { get; set; }
        public virtual DbSet<vw_blending_plan_quality> vw_blending_plan_quality { get; set; }
        public virtual DbSet<vw_blending_plan_source> vw_blending_plan_source { get; set; }
        public virtual DbSet<vw_blending_plan_value> vw_blending_plan_value { get; set; }
        public virtual DbSet<vw_business_area> vw_business_area { get; set; }
        public virtual DbSet<vw_business_area_breakdown_structure> vw_business_area_breakdown_structure { get; set; }
        public virtual DbSet<vw_business_area_structure> vw_business_area_structure { get; set; }
        public virtual DbSet<vw_business_partner> vw_business_partner { get; set; }
        public virtual DbSet<vw_business_unit> vw_business_unit { get; set; }
        public virtual DbSet<vw_business_unit_structure> vw_business_unit_structure { get; set; }
        public virtual DbSet<vw_chls> vw_chls { get; set; }
        public virtual DbSet<vw_chls_additional_info> vw_chls_additional_info { get; set; }
        public virtual DbSet<vw_city> vw_city { get; set; }
        public virtual DbSet<vw_coa> vw_coa { get; set; }
        public virtual DbSet<vw_coa_subaccount> vw_coa_subaccount { get; set; }
        public virtual DbSet<vw_coal_barged_sp> vw_coal_barged_sp { get; set; }
        public virtual DbSet<vw_coal_mined_sp> vw_coal_mined_sp { get; set; }
        public virtual DbSet<vw_coal_movement_sp> vw_coal_movement_sp { get; set; }
        public virtual DbSet<vw_coal_transfer> vw_coal_transfer { get; set; }
        public virtual DbSet<vw_contact> vw_contact { get; set; }
        public virtual DbSet<vw_contractor> vw_contractor { get; set; }
        public virtual DbSet<vw_contractor_document> vw_contractor_document { get; set; }
        public virtual DbSet<vw_contractor_type> vw_contractor_type { get; set; }
        public virtual DbSet<vw_country> vw_country { get; set; }
        public virtual DbSet<vw_cpp_location> vw_cpp_location { get; set; }
        public virtual DbSet<vw_credit_limit_history> vw_credit_limit_history { get; set; }
        public virtual DbSet<vw_currency> vw_currency { get; set; }
        public virtual DbSet<vw_currency_exchange> vw_currency_exchange { get; set; }
        public virtual DbSet<vw_customer> vw_customer { get; set; }
        public virtual DbSet<vw_customer_ell> vw_customer_ell { get; set; }
        public virtual DbSet<vw_customer_transaction_history> vw_customer_transaction_history { get; set; }
        public virtual DbSet<vw_customer_type> vw_customer_type { get; set; }
        public virtual DbSet<vw_daily_stock_port> vw_daily_stock_port { get; set; }
        public virtual DbSet<vw_daywork> vw_daywork { get; set; }
        public virtual DbSet<vw_daywork_closing> vw_daywork_closing { get; set; }
        public virtual DbSet<vw_delay> vw_delay { get; set; }
        public virtual DbSet<vw_delay_details> vw_delay_details { get; set; }
        public virtual DbSet<vw_despatch_demurrage> vw_despatch_demurrage { get; set; }
        public virtual DbSet<vw_despatch_demurrage_debit_credit_note> vw_despatch_demurrage_debit_credit_note { get; set; }
        public virtual DbSet<vw_despatch_demurrage_delay> vw_despatch_demurrage_delay { get; set; }
        public virtual DbSet<vw_despatch_demurrage_detail> vw_despatch_demurrage_detail { get; set; }
        public virtual DbSet<vw_despatch_demurrage_invoice> vw_despatch_demurrage_invoice { get; set; }
        public virtual DbSet<vw_despatch_order> vw_despatch_order { get; set; }
        public virtual DbSet<vw_despatch_order_delay> vw_despatch_order_delay { get; set; }
        public virtual DbSet<vw_despatch_order_delivery_term> vw_despatch_order_delivery_term { get; set; }
        public virtual DbSet<vw_despatch_order_document> vw_despatch_order_document { get; set; }
        public virtual DbSet<vw_despatch_order_ell> vw_despatch_order_ell { get; set; }
        public virtual DbSet<vw_despatch_order_product_specifications> vw_despatch_order_product_specifications { get; set; }
        public virtual DbSet<vw_details_customer_invoice_history> vw_details_customer_invoice_history { get; set; }
        public virtual DbSet<vw_details_customer_payment_history> vw_details_customer_payment_history { get; set; }
        public virtual DbSet<vw_do_inv_currency_exchange> vw_do_inv_currency_exchange { get; set; }
        public virtual DbSet<vw_document_type> vw_document_type { get; set; }
        public virtual DbSet<vw_draft_survey> vw_draft_survey { get; set; }
        public virtual DbSet<vw_draft_survey_detail> vw_draft_survey_detail { get; set; }
        public virtual DbSet<vw_drill_blast_plan> vw_drill_blast_plan { get; set; }
        public virtual DbSet<vw_eight_week_forecast> vw_eight_week_forecast { get; set; }
        public virtual DbSet<vw_eight_week_forecast_item> vw_eight_week_forecast_item { get; set; }
        public virtual DbSet<vw_eight_week_forecast_simulation> vw_eight_week_forecast_simulation { get; set; }
        public virtual DbSet<vw_email_notification> vw_email_notification { get; set; }
        public virtual DbSet<vw_employee> vw_employee { get; set; }
        public virtual DbSet<vw_equipment> vw_equipment { get; set; }
        public virtual DbSet<vw_equipment_category> vw_equipment_category { get; set; }
        public virtual DbSet<vw_equipment_cost_rate> vw_equipment_cost_rate { get; set; }
        public virtual DbSet<vw_equipment_cost_rate_latest> vw_equipment_cost_rate_latest { get; set; }
        public virtual DbSet<vw_equipment_group> vw_equipment_group { get; set; }
        public virtual DbSet<vw_equipment_group_item> vw_equipment_group_item { get; set; }
        public virtual DbSet<vw_equipment_incident> vw_equipment_incident { get; set; }
        public virtual DbSet<vw_equipment_type> vw_equipment_type { get; set; }
        public virtual DbSet<vw_equipment_usage_transaction> vw_equipment_usage_transaction { get; set; }
        public virtual DbSet<vw_equipment_usage_transaction_detail> vw_equipment_usage_transaction_detail { get; set; }
        public virtual DbSet<vw_equipment_usage_transaction_detail_lookup> vw_equipment_usage_transaction_detail_lookup { get; set; }
        public virtual DbSet<vw_event_category> vw_event_category { get; set; }
        public virtual DbSet<vw_event_definition_category> vw_event_definition_category { get; set; }
        public virtual DbSet<vw_explosive_usage_plan> vw_explosive_usage_plan { get; set; }
        public virtual DbSet<vw_explosive_usage_plan_detail> vw_explosive_usage_plan_detail { get; set; }
        public virtual DbSet<vw_exposed_coal> vw_exposed_coal { get; set; }
        public virtual DbSet<vw_fuel_inventory> vw_fuel_inventory { get; set; }
        public virtual DbSet<vw_hauling_plan> vw_hauling_plan { get; set; }
        public virtual DbSet<vw_hauling_plan_daily> vw_hauling_plan_daily { get; set; }
        public virtual DbSet<vw_hauling_plan_history> vw_hauling_plan_history { get; set; }
        public virtual DbSet<vw_hauling_plan_monthly> vw_hauling_plan_monthly { get; set; }
        public virtual DbSet<vw_hauling_plan_monthly_history> vw_hauling_plan_monthly_history { get; set; }
        public virtual DbSet<vw_hauling_sp> vw_hauling_sp { get; set; }
        public virtual DbSet<vw_hauling_transaction> vw_hauling_transaction { get; set; }
        public virtual DbSet<vw_haze> vw_haze { get; set; }
        public virtual DbSet<vw_incident> vw_incident { get; set; }
        public virtual DbSet<vw_incident_category> vw_incident_category { get; set; }
        public virtual DbSet<vw_initial_information> vw_initial_information { get; set; }
        public virtual DbSet<vw_inventory_transaction> vw_inventory_transaction { get; set; }
        public virtual DbSet<vw_invoice_overdue> vw_invoice_overdue { get; set; }
        public virtual DbSet<vw_item_list> vw_item_list { get; set; }
        public virtual DbSet<vw_joint_survey> vw_joint_survey { get; set; }
        public virtual DbSet<vw_joint_survey_analyte> vw_joint_survey_analyte { get; set; }
        public virtual DbSet<vw_location> vw_location { get; set; }
        public virtual DbSet<vw_lookup_despatch_order_for_invoice> vw_lookup_despatch_order_for_invoice { get; set; }
        public virtual DbSet<vw_lookup_despatch_order_for_quotation> vw_lookup_despatch_order_for_quotation { get; set; }
        public virtual DbSet<vw_master_date> vw_master_date { get; set; }
        public virtual DbSet<vw_material_type> vw_material_type { get; set; }
        public virtual DbSet<vw_material_type_analyte> vw_material_type_analyte { get; set; }
        public virtual DbSet<vw_mine_location> vw_mine_location { get; set; }
        public virtual DbSet<vw_mine_location_breakdown_structure> vw_mine_location_breakdown_structure { get; set; }
        public virtual DbSet<vw_mine_location_quality> vw_mine_location_quality { get; set; }
        public virtual DbSet<vw_mine_location_quality_pit> vw_mine_location_quality_pit { get; set; }
        public virtual DbSet<vw_mine_plan_ltp> vw_mine_plan_ltp { get; set; }
        public virtual DbSet<vw_mineplan_geology_history> vw_mineplan_geology_history { get; set; }
        public virtual DbSet<vw_mineplan_ltp_history> vw_mineplan_ltp_history { get; set; }
        public virtual DbSet<vw_model_geology> vw_model_geology { get; set; }
        public virtual DbSet<vw_operator> vw_operator { get; set; }
        public virtual DbSet<vw_organization> vw_organization { get; set; }
        public virtual DbSet<vw_parent_despatch_order> vw_parent_despatch_order { get; set; }
        public virtual DbSet<vw_port_location> vw_port_location { get; set; }
        public virtual DbSet<vw_price_index> vw_price_index { get; set; }
        public virtual DbSet<vw_price_index_history> vw_price_index_history { get; set; }
        public virtual DbSet<vw_price_index_map> vw_price_index_map { get; set; }
        public virtual DbSet<vw_price_index_map_detail> vw_price_index_map_detail { get; set; }
        public virtual DbSet<vw_process_flow> vw_process_flow { get; set; }
        public virtual DbSet<vw_process_flow_analyte> vw_process_flow_analyte { get; set; }
        public virtual DbSet<vw_processing_category> vw_processing_category { get; set; }
        public virtual DbSet<vw_processing_closing_item> vw_processing_closing_item { get; set; }
        public virtual DbSet<vw_processing_transaction> vw_processing_transaction { get; set; }
        public virtual DbSet<vw_product> vw_product { get; set; }
        public virtual DbSet<vw_product_category> vw_product_category { get; set; }
        public virtual DbSet<vw_product_specification> vw_product_specification { get; set; }
        public virtual DbSet<vw_production_actual_plan> vw_production_actual_plan { get; set; }
        public virtual DbSet<vw_production_closing> vw_production_closing { get; set; }
        public virtual DbSet<vw_production_plan> vw_production_plan { get; set; }
        public virtual DbSet<vw_production_plan_daily> vw_production_plan_daily { get; set; }
        public virtual DbSet<vw_production_plan_monthly> vw_production_plan_monthly { get; set; }
        public virtual DbSet<vw_production_plan_monthly_history> vw_production_plan_monthly_history { get; set; }
        public virtual DbSet<vw_production_transaction> vw_production_transaction { get; set; }
        public virtual DbSet<vw_progress_claim> vw_progress_claim { get; set; }
        public virtual DbSet<vw_progress_claim_charge> vw_progress_claim_charge { get; set; }
        public virtual DbSet<vw_province> vw_province { get; set; }
        public virtual DbSet<vw_quality_sampling> vw_quality_sampling { get; set; }
        public virtual DbSet<vw_quality_sampling_analyte> vw_quality_sampling_analyte { get; set; }
        public virtual DbSet<vw_railing_transaction> vw_railing_transaction { get; set; }
        public virtual DbSet<vw_rainfall> vw_rainfall { get; set; }
        public virtual DbSet<vw_raw_data_lineup_all> vw_raw_data_lineup_all { get; set; }
        public virtual DbSet<vw_raw_data_lineup_db> vw_raw_data_lineup_db { get; set; }
        public virtual DbSet<vw_raw_data_lineup_nplct> vw_raw_data_lineup_nplct { get; set; }
        public virtual DbSet<vw_raw_data_lineup_transhipment> vw_raw_data_lineup_transhipment { get; set; }
        public virtual DbSet<vw_ready_to_get> vw_ready_to_get { get; set; }
        public virtual DbSet<vw_rehandling_transaction> vw_rehandling_transaction { get; set; }
        public virtual DbSet<vw_report_template> vw_report_template { get; set; }
        public virtual DbSet<vw_report_template2> vw_report_template2 { get; set; }
        public virtual DbSet<vw_report_viewer> vw_report_viewer { get; set; }
        public virtual DbSet<vw_role_access> vw_role_access { get; set; }
        public virtual DbSet<vw_role_business_unit> vw_role_business_unit { get; set; }
        public virtual DbSet<vw_royalty> vw_royalty { get; set; }
        public virtual DbSet<vw_royalty_cost> vw_royalty_cost { get; set; }
        public virtual DbSet<vw_royalty_payment> vw_royalty_payment { get; set; }
        public virtual DbSet<vw_royalty_pricing> vw_royalty_pricing { get; set; }
        public virtual DbSet<vw_royalty_quantity_quality> vw_royalty_quantity_quality { get; set; }
        public virtual DbSet<vw_royalty_valuation> vw_royalty_valuation { get; set; }
        public virtual DbSet<vw_royalty_valuation_recalc> vw_royalty_valuation_recalc { get; set; }
        public virtual DbSet<vw_sales_charge> vw_sales_charge { get; set; }
        public virtual DbSet<vw_sales_contract> vw_sales_contract { get; set; }
        public virtual DbSet<vw_sales_contract_charges> vw_sales_contract_charges { get; set; }
        public virtual DbSet<vw_sales_contract_despatch_demurrage_delay> vw_sales_contract_despatch_demurrage_delay { get; set; }
        public virtual DbSet<vw_sales_contract_despatch_demurrage_term> vw_sales_contract_despatch_demurrage_term { get; set; }
        public virtual DbSet<vw_sales_contract_despatch_plan> vw_sales_contract_despatch_plan { get; set; }
        public virtual DbSet<vw_sales_contract_detail> vw_sales_contract_detail { get; set; }
        public virtual DbSet<vw_sales_contract_end_user> vw_sales_contract_end_user { get; set; }
        public virtual DbSet<vw_sales_contract_invoice_target> vw_sales_contract_invoice_target { get; set; }
        public virtual DbSet<vw_sales_contract_payment_term> vw_sales_contract_payment_term { get; set; }
        public virtual DbSet<vw_sales_contract_product> vw_sales_contract_product { get; set; }
        public virtual DbSet<vw_sales_contract_product_specifications> vw_sales_contract_product_specifications { get; set; }
        public virtual DbSet<vw_sales_contract_qty> vw_sales_contract_qty { get; set; }
        public virtual DbSet<vw_sales_contract_quotation_price> vw_sales_contract_quotation_price { get; set; }
        public virtual DbSet<vw_sales_contract_taxes> vw_sales_contract_taxes { get; set; }
        public virtual DbSet<vw_sales_contract_term> vw_sales_contract_term { get; set; }
        public virtual DbSet<vw_sales_invoice> vw_sales_invoice { get; set; }
        public virtual DbSet<vw_sales_invoice_attachment> vw_sales_invoice_attachment { get; set; }
        public virtual DbSet<vw_sales_invoice_datagrid> vw_sales_invoice_datagrid { get; set; }
        public virtual DbSet<vw_sales_invoice_ell> vw_sales_invoice_ell { get; set; }
        public virtual DbSet<vw_sales_invoice_html_content> vw_sales_invoice_html_content { get; set; }
        public virtual DbSet<vw_sales_invoice_payment> vw_sales_invoice_payment { get; set; }
        public virtual DbSet<vw_sales_invoice_product_specification> vw_sales_invoice_product_specification { get; set; }
        public virtual DbSet<vw_sales_invoice_trace_element> vw_sales_invoice_trace_element { get; set; }
        public virtual DbSet<vw_sales_invoice_transhipment> vw_sales_invoice_transhipment { get; set; }
        public virtual DbSet<vw_sales_plan> vw_sales_plan { get; set; }
        public virtual DbSet<vw_sales_plan_customer> vw_sales_plan_customer { get; set; }
        public virtual DbSet<vw_sales_plan_customer_list> vw_sales_plan_customer_list { get; set; }
        public virtual DbSet<vw_sales_plan_detail> vw_sales_plan_detail { get; set; }
        public virtual DbSet<vw_sales_plan_snapshot> vw_sales_plan_snapshot { get; set; }
        public virtual DbSet<vw_sampling_template> vw_sampling_template { get; set; }
        public virtual DbSet<vw_sampling_template_detail> vw_sampling_template_detail { get; set; }
        public virtual DbSet<vw_sampling_type> vw_sampling_type { get; set; }
        public virtual DbSet<vw_shared_record> vw_shared_record { get; set; }
        public virtual DbSet<vw_shift> vw_shift { get; set; }
        public virtual DbSet<vw_shift_category> vw_shift_category { get; set; }
        public virtual DbSet<vw_shipment_plan> vw_shipment_plan { get; set; }
        public virtual DbSet<vw_shipping_cost> vw_shipping_cost { get; set; }
        public virtual DbSet<vw_shipping_delay> vw_shipping_delay { get; set; }
        public virtual DbSet<vw_shipping_instruction> vw_shipping_instruction { get; set; }
        public virtual DbSet<vw_shipping_instruction_asuransi> vw_shipping_instruction_asuransi { get; set; }
        public virtual DbSet<vw_shipping_instruction_detail_survey> vw_shipping_instruction_detail_survey { get; set; }
        public virtual DbSet<vw_shipping_instruction_detail_survey_document> vw_shipping_instruction_detail_survey_document { get; set; }
        public virtual DbSet<vw_shipping_instruction_document_agent> vw_shipping_instruction_document_agent { get; set; }
        public virtual DbSet<vw_shipping_instruction_pekerjaan_agent> vw_shipping_instruction_pekerjaan_agent { get; set; }
        public virtual DbSet<vw_shipping_instruction_stevedoring> vw_shipping_instruction_stevedoring { get; set; }
        public virtual DbSet<vw_shipping_instruction_to_company> vw_shipping_instruction_to_company { get; set; }
        public virtual DbSet<vw_shipping_instruction_tug_boat> vw_shipping_instruction_tug_boat { get; set; }
        public virtual DbSet<vw_shipping_load_unload_document> vw_shipping_load_unload_document { get; set; }
        public virtual DbSet<vw_shipping_program> vw_shipping_program { get; set; }
        public virtual DbSet<vw_shipping_program_detail> vw_shipping_program_detail { get; set; }
        public virtual DbSet<vw_shipping_progres> vw_shipping_progres { get; set; }
        public virtual DbSet<vw_shipping_transaction> vw_shipping_transaction { get; set; }
        public virtual DbSet<vw_shipping_transaction_detail> vw_shipping_transaction_detail { get; set; }
        public virtual DbSet<vw_sils> vw_sils { get; set; }
        public virtual DbSet<vw_sils_nplct_subdetail> vw_sils_nplct_subdetail { get; set; }
        public virtual DbSet<vw_sils_unloading> vw_sils_unloading { get; set; }
        public virtual DbSet<vw_sils_unloading_detail> vw_sils_unloading_detail { get; set; }
        public virtual DbSet<vw_slippery> vw_slippery { get; set; }
        public virtual DbSet<vw_sof> vw_sof { get; set; }
        public virtual DbSet<vw_sof_detail> vw_sof_detail { get; set; }
        public virtual DbSet<vw_stock_location> vw_stock_location { get; set; }
        public virtual DbSet<vw_stock_state> vw_stock_state { get; set; }
        public virtual DbSet<vw_stock_state_analyte> vw_stock_state_analyte { get; set; }
        public virtual DbSet<vw_stockpile_location> vw_stockpile_location { get; set; }
        public virtual DbSet<vw_stockpile_mutation_analyte> vw_stockpile_mutation_analyte { get; set; }
        public virtual DbSet<vw_stockpile_state> vw_stockpile_state { get; set; }
        public virtual DbSet<vw_stockpile_summary> vw_stockpile_summary { get; set; }
        public virtual DbSet<vw_stockpile_summary_daily> vw_stockpile_summary_daily { get; set; }
        public virtual DbSet<vw_stockpile_summary_monthly> vw_stockpile_summary_monthly { get; set; }
        public virtual DbSet<vw_survey> vw_survey { get; set; }
        public virtual DbSet<vw_survey_analyte> vw_survey_analyte { get; set; }
        public virtual DbSet<vw_survey_detail> vw_survey_detail { get; set; }
        public virtual DbSet<vw_sync_log> vw_sync_log { get; set; }
        public virtual DbSet<vw_team> vw_team { get; set; }
        public virtual DbSet<vw_team_member> vw_team_member { get; set; }
        public virtual DbSet<vw_team_role> vw_team_role { get; set; }
        public virtual DbSet<vw_tidalwave> vw_tidalwave { get; set; }
        public virtual DbSet<vw_timesheet> vw_timesheet { get; set; }
        public virtual DbSet<vw_timesheet_detail> vw_timesheet_detail { get; set; }
        public virtual DbSet<vw_timesheet_detail_event> vw_timesheet_detail_event { get; set; }
        public virtual DbSet<vw_timesheet_detail_event_plan> vw_timesheet_detail_event_plan { get; set; }
        public virtual DbSet<vw_timesheet_detail_plan> vw_timesheet_detail_plan { get; set; }
        public virtual DbSet<vw_timesheet_detail_productivity_problem> vw_timesheet_detail_productivity_problem { get; set; }
        public virtual DbSet<vw_timesheet_detail_productivity_problem_plan> vw_timesheet_detail_productivity_problem_plan { get; set; }
        public virtual DbSet<vw_timesheet_ell> vw_timesheet_ell { get; set; }
        public virtual DbSet<vw_timesheet_plan> vw_timesheet_plan { get; set; }
        public virtual DbSet<vw_train> vw_train { get; set; }
        public virtual DbSet<vw_transport> vw_transport { get; set; }
        public virtual DbSet<vw_truck> vw_truck { get; set; }
        public virtual DbSet<vw_truck_cost_rate> vw_truck_cost_rate { get; set; }
        public virtual DbSet<vw_tug> vw_tug { get; set; }
        public virtual DbSet<vw_uom> vw_uom { get; set; }
        public virtual DbSet<vw_uom_category> vw_uom_category { get; set; }
        public virtual DbSet<vw_uom_conversion> vw_uom_conversion { get; set; }
        public virtual DbSet<vw_user_role> vw_user_role { get; set; }
        public virtual DbSet<vw_vessel> vw_vessel { get; set; }
        public virtual DbSet<vw_wagon> vw_wagon { get; set; }
        public virtual DbSet<vw_waste> vw_waste { get; set; }
        public virtual DbSet<vw_waste_category> vw_waste_category { get; set; }
        public virtual DbSet<vw_waste_location> vw_waste_location { get; set; }
        public virtual DbSet<vw_waste_removal> vw_waste_removal { get; set; }
        public virtual DbSet<vw_waste_removal_closing_detail> vw_waste_removal_closing_detail { get; set; }
        public virtual DbSet<vw_waste_specification> vw_waste_specification { get; set; }
        public virtual DbSet<vw_weigh_bridge> vw_weigh_bridge { get; set; }
        public virtual DbSet<vw_wip_location> vw_wip_location { get; set; }
        public virtual DbSet<wagon> wagon { get; set; }
        public virtual DbSet<waste> waste { get; set; }
        public virtual DbSet<waste_allsite_daily_pct> waste_allsite_daily_pct { get; set; }
        public virtual DbSet<waste_allsite_monthly_pct> waste_allsite_monthly_pct { get; set; }
        public virtual DbSet<waste_allsite_mtd_pct> waste_allsite_mtd_pct { get; set; }
        public virtual DbSet<waste_allsite_rawdata> waste_allsite_rawdata { get; set; }
        public virtual DbSet<waste_allsite_yearly_pct> waste_allsite_yearly_pct { get; set; }
        public virtual DbSet<waste_allsite_ytd_pct> waste_allsite_ytd_pct { get; set; }
        public virtual DbSet<waste_category> waste_category { get; set; }
        public virtual DbSet<waste_daily_details> waste_daily_details { get; set; }
        public virtual DbSet<waste_daily_pct> waste_daily_pct { get; set; }
        public virtual DbSet<waste_details_all> waste_details_all { get; set; }
        public virtual DbSet<waste_location> waste_location { get; set; }
        public virtual DbSet<waste_monthly_details_trans> waste_monthly_details_trans { get; set; }
        public virtual DbSet<waste_monthly_pct> waste_monthly_pct { get; set; }
        public virtual DbSet<waste_mtd_details> waste_mtd_details { get; set; }
        public virtual DbSet<waste_mtd_pct> waste_mtd_pct { get; set; }
        public virtual DbSet<waste_mtfull_details> waste_mtfull_details { get; set; }
        public virtual DbSet<waste_pct_all> waste_pct_all { get; set; }
        public virtual DbSet<waste_removal> waste_removal { get; set; }
        public virtual DbSet<waste_removal_closing> waste_removal_closing { get; set; }
        public virtual DbSet<waste_removal_closing_item> waste_removal_closing_item { get; set; }
        public virtual DbSet<waste_removal_item> waste_removal_item { get; set; }
        public virtual DbSet<waste_schema> waste_schema { get; set; }
        public virtual DbSet<waste_specification> waste_specification { get; set; }
        public virtual DbSet<waste_yearly_details_trans> waste_yearly_details_trans { get; set; }
        public virtual DbSet<waste_yearly_pct> waste_yearly_pct { get; set; }
        public virtual DbSet<waste_ytd_details> waste_ytd_details { get; set; }
        public virtual DbSet<waste_ytd_pct> waste_ytd_pct { get; set; }
        public virtual DbSet<waste_ytfull_details> waste_ytfull_details { get; set; }
        public virtual DbSet<wb_logs> wb_logs { get; set; }
        public virtual DbSet<weigh_bridge> weigh_bridge { get; set; }
        public virtual DbSet<wip_location> wip_location { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseNpgsql("Host=10.129.129.130;Port=5432;Database=portal_arutmin;Username=admin;Password=@rutm1n2o23;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<_operator>(entity =>
            {
                entity.ToTable("operator");

                entity.HasIndex(e => e.nik)
                    .HasName("NIK")
                    .IsUnique();
            });

            modelBuilder.Entity<access_value>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<accounting_period>(entity =>
            {
                entity.HasIndex(e => e.organization_id)
                    .HasName("ix_accounting_period_organization_id");

                entity.HasIndex(e => new { e.organization_id, e.accounting_period_name })
                    .HasName("accounting_period_organization_id_accounting_period_name_key")
                    .IsUnique();

                entity.Property(e => e.accounting_period_name).IsRequired();

                entity.Property(e => e.end_date).HasColumnType("date");

                entity.Property(e => e.start_date).HasColumnType("date");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.accounting_period)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<advance_contract>(entity =>
            {
                entity.Property(e => e.advance_contract_number).IsRequired();

                entity.Property(e => e.contract_value).HasColumnType("numeric");

                entity.Property(e => e.contractor_id).IsRequired();

                entity.Property(e => e.end_date).HasColumnType("date");

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.start_date).HasColumnType("date");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.advance_contract)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<advance_contract_charge>(entity =>
            {
                entity.Property(e => e.advance_contract_id).IsRequired();

                entity.Property(e => e.charge_name).IsRequired();

                entity.Property(e => e.decimal_places).HasColumnType("numeric");

                entity.HasOne(d => d.advance_contract_)
                    .WithMany(p => p.advance_contract_charge)
                    .HasForeignKey(d => d.advance_contract_id)
                    .HasConstraintName("fk_advance_contract_advance_contract_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.advance_contract_charge)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<advance_contract_charge_detail>(entity =>
            {
                entity.Property(e => e.advance_contract_charge_id).IsRequired();

                entity.Property(e => e.advance_contract_detail_id).IsRequired();

                entity.Property(e => e.price_index_id).IsRequired();

                entity.HasOne(d => d.advance_contract_charge_)
                    .WithMany(p => p.advance_contract_charge_detail)
                    .HasForeignKey(d => d.advance_contract_charge_id)
                    .HasConstraintName("fk_advance_contract_charge_advance_contract_charge_id");

                entity.HasOne(d => d.joint_survey_)
                    .WithMany(p => p.advance_contract_charge_detail)
                    .HasForeignKey(d => d.joint_survey_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_joint_survey_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.advance_contract_charge_detail)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.price_index_)
                    .WithMany(p => p.advance_contract_charge_detail)
                    .HasForeignKey(d => d.price_index_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_price_index_id");
            });

            modelBuilder.Entity<advance_contract_detail>(entity =>
            {
                entity.Property(e => e.advance_contract_id).IsRequired();

                entity.Property(e => e.amount).HasColumnType("numeric");

                entity.Property(e => e.currency_id).IsRequired();

                entity.HasOne(d => d.advance_contract_)
                    .WithMany(p => p.advance_contract_detail)
                    .HasForeignKey(d => d.advance_contract_id)
                    .HasConstraintName("fk_advance_contract_advance_contract_id");

                entity.HasOne(d => d.currency_)
                    .WithMany(p => p.advance_contract_detail)
                    .HasForeignKey(d => d.currency_id)
                    .HasConstraintName("fk_currency_currency_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.advance_contract_detail)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<advance_contract_item>(entity =>
            {
                entity.Property(e => e.advance_contract_id).IsRequired();

                entity.Property(e => e.item_name).IsRequired();

                entity.HasOne(d => d.advance_contract_)
                    .WithMany(p => p.advance_contract_item)
                    .HasForeignKey(d => d.advance_contract_id)
                    .HasConstraintName("fk_advance_contract_item_advance_contract_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.advance_contract_item)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<advance_contract_item_detail>(entity =>
            {
                entity.Property(e => e.advance_contract_item_id).IsRequired();

                entity.Property(e => e.amount).HasColumnType("numeric");

                entity.HasOne(d => d.advance_contract_item_)
                    .WithMany(p => p.advance_contract_item_detail)
                    .HasForeignKey(d => d.advance_contract_item_id)
                    .HasConstraintName("fk_advance_contract_item_detail_advance_contract_item_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.advance_contract_item_detail)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<advance_contract_reference>(entity =>
            {
                entity.HasIndex(e => new { e.organization_id, e.progress_claim_name })
                    .HasName("uk_advance_contract_reference")
                    .IsUnique();

                entity.Property(e => e.actual_quantity).HasColumnType("numeric");

                entity.Property(e => e.end_date).HasColumnType("date");

                entity.Property(e => e.start_date).HasColumnType("date");

                entity.Property(e => e.target_quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<advance_contract_valuation>(entity =>
            {
                entity.Property(e => e.total_value).HasColumnType("numeric");

                entity.HasOne(d => d.advance_contract_reference_)
                    .WithMany(p => p.advance_contract_valuation)
                    .HasForeignKey(d => d.advance_contract_reference_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_advance_contract_reference_id");

                entity.HasOne(d => d.employee_)
                    .WithMany(p => p.advance_contract_valuation)
                    .HasForeignKey(d => d.employee_id)
                    .HasConstraintName("fk_advance_contract_employee_id");
            });

            modelBuilder.Entity<advance_contract_valuation_detail>(entity =>
            {
                entity.Property(e => e.convertion_amount).HasColumnType("numeric");

                entity.Property(e => e.value).HasColumnType("numeric");

                entity.HasOne(d => d.advance_contract_reference_detail_)
                    .WithMany(p => p.advance_contract_valuation_detail)
                    .HasForeignKey(d => d.advance_contract_reference_detail_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_advance_contract_reference_detail_id");

                entity.HasOne(d => d.advance_contract_valuation_)
                    .WithMany(p => p.advance_contract_valuation_detail)
                    .HasForeignKey(d => d.advance_contract_valuation_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_advance_contract_valuation_id");
            });

            modelBuilder.Entity<allsite_barging_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<allsite_hauling_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<allsite_mine_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<allsite_produce_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<allsite_transfer_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<allsite_waste_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<analyte>(entity =>
            {
                entity.HasIndex(e => new { e.organization_id, e.analyte_name })
                    .HasName("uk_analyte")
                    .IsUnique();

                entity.Property(e => e.analyte_name).IsRequired();

                entity.Property(e => e.uom_id).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.analyte)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.uom_)
                    .WithMany(p => p.analyte)
                    .HasForeignKey(d => d.uom_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_uom_uom_id");
            });

            modelBuilder.Entity<app_user>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<application_business_unit>(entity =>
            {
                entity.HasIndex(e => new { e.role_business_unit_id, e.application_entity_id })
                    .HasName("uk_application_business_unit")
                    .IsUnique();

                entity.Property(e => e.application_entity_id).IsRequired();

                entity.Property(e => e.role_business_unit_id).IsRequired();

                entity.HasOne(d => d.application_entity_)
                    .WithMany(p => p.application_business_unit)
                    .HasForeignKey(d => d.application_entity_id)
                    .HasConstraintName("fk_application_entity_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.application_business_unit)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<application_entity>(entity =>
            {
                entity.HasIndex(e => e.created_by)
                    .HasName("ix_application_entity_created_by")
                    .IncludeProperties(new[] { "created_on" });

                entity.HasIndex(e => e.entity_id)
                    .HasName("ix_application_entity_entity_id");

                entity.HasIndex(e => e.is_active)
                    .HasName("ix_application_entity_is_active")
                    .IncludeProperties(new[] { "owner_id", "organization_id" });

                entity.HasIndex(e => e.modified_by)
                    .HasName("ix_application_entity_modified_by")
                    .IncludeProperties(new[] { "modified_on" });

                entity.HasIndex(e => e.organization_id)
                    .HasName("ix_application_entity_organization_id")
                    .IncludeProperties(new[] { "is_active", "owner_id" });

                entity.HasIndex(e => e.owner_id)
                    .HasName("ix_application_entity_owner_id")
                    .IncludeProperties(new[] { "is_active", "organization_id" });

                entity.HasIndex(e => new { e.organization_id, e.entity_name })
                    .HasName("uk_application_entity")
                    .IsUnique();

                entity.Property(e => e.entity_name).IsRequired();

                entity.Property(e => e.organization_id).IsRequired();

                entity.Property(e => e.owner_id).IsRequired();
            });

            modelBuilder.Entity<application_role>(entity =>
            {
                entity.HasIndex(e => e.created_by)
                    .HasName("ix_application_role_created_by")
                    .IncludeProperties(new[] { "created_on" });

                entity.HasIndex(e => e.entity_id)
                    .HasName("ix_application_role_entity_id");

                entity.HasIndex(e => e.is_active)
                    .HasName("ix_application_role_is_active")
                    .IncludeProperties(new[] { "owner_id", "organization_id" });

                entity.HasIndex(e => e.modified_by)
                    .HasName("ix_application_role_modified_by")
                    .IncludeProperties(new[] { "modified_on" });

                entity.HasIndex(e => e.organization_id)
                    .HasName("ix_application_role_organization_id")
                    .IncludeProperties(new[] { "is_active", "owner_id" });

                entity.HasIndex(e => e.owner_id)
                    .HasName("ix_application_role_owner_id")
                    .IncludeProperties(new[] { "is_active", "organization_id" });

                entity.HasIndex(e => new { e.organization_id, e.role_name })
                    .HasName("uk_application_role")
                    .IsUnique();

                entity.Property(e => e.organization_id).IsRequired();

                entity.Property(e => e.owner_id).IsRequired();

                entity.Property(e => e.role_name).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.application_role)
                    .HasForeignKey(d => d.organization_id)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<application_user>(entity =>
            {
                entity.HasIndex(e => e.access_token)
                    .HasName("ix_application_user_access_token")
                    .IsUnique()
                    .IncludeProperties(new[] { "token_expiry" });

                entity.HasIndex(e => e.application_username)
                    .HasName("ix_application_user_application_username")
                    .IncludeProperties(new[] { "is_active", "owner_id", "organization_id" });

                entity.HasIndex(e => e.created_by)
                    .HasName("ix_application_user_created_by")
                    .IncludeProperties(new[] { "created_on" });

                entity.HasIndex(e => e.entity_id)
                    .HasName("ix_application_user_entity_id");

                entity.HasIndex(e => e.is_active)
                    .HasName("ix_application_user_is_active")
                    .IncludeProperties(new[] { "owner_id", "organization_id" });

                entity.HasIndex(e => e.is_sysadmin)
                    .HasName("ix_application_user_is_sysadmin")
                    .IncludeProperties(new[] { "is_active", "owner_id", "organization_id" });

                entity.HasIndex(e => e.modified_by)
                    .HasName("ix_application_user_modified_by")
                    .IncludeProperties(new[] { "modified_on" });

                entity.HasIndex(e => e.organization_id)
                    .HasName("ix_application_user_organization_id")
                    .IncludeProperties(new[] { "is_active", "owner_id" });

                entity.HasIndex(e => e.owner_id)
                    .HasName("ix_application_user_owner_id")
                    .IncludeProperties(new[] { "is_active", "organization_id" });

                entity.HasIndex(e => e.primary_team_id)
                    .HasName("ix_application_user_primary_team_id")
                    .IncludeProperties(new[] { "is_active", "owner_id", "organization_id" });

                entity.HasIndex(e => new { e.organization_id, e.application_username })
                    .HasName("uk_application_user")
                    .IsUnique();

                entity.Property(e => e.application_password).IsRequired();

                entity.Property(e => e.application_username).IsRequired();

                entity.Property(e => e.organization_id).IsRequired();

                entity.Property(e => e.owner_id).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.application_user)
                    .HasForeignKey(d => d.organization_id)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.primary_team_)
                    .WithMany(p => p.application_user)
                    .HasForeignKey(d => d.primary_team_id)
                    .HasConstraintName("fk_team_primary_team_id");
            });

            modelBuilder.Entity<arutmin_waste_removal>(entity =>
            {
                entity.HasIndex(e => e.organization_id)
                    .HasName("ix_arutmin_waste_removal_organization_id");

                entity.HasIndex(e => e.process_flow_id)
                    .HasName("ix_arutmin_waste_removal_process_flow_id");

                entity.HasIndex(e => e.waste_id)
                    .HasName("ix_arutmin_waste_removal_waste_id");

                entity.HasIndex(e => new { e.organization_id, e.transaction_id })
                    .HasName("uk_arutmin_waste_removal")
                    .IsUnique();

                entity.Property(e => e.density).HasColumnType("numeric");

                entity.Property(e => e.process_flow_id).IsRequired();

                entity.Property(e => e.ritase).HasColumnType("numeric");

                entity.Property(e => e.tonnage).HasColumnType("numeric");

                entity.Property(e => e.transaction_id).IsRequired();

                entity.Property(e => e.volume_bcm).HasColumnType("numeric");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.arutmin_waste_removal)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<arutmin_waste_removal_item>(entity =>
            {
                entity.HasIndex(e => e.organization_id)
                    .HasName("ix_arutmin_waste_removal_item_organization_id");

                entity.Property(e => e.ritase).HasColumnType("numeric");

                entity.Property(e => e.tonnage).HasColumnType("numeric");

                entity.Property(e => e.transaction_id).IsRequired();

                entity.Property(e => e.truck_factor).HasColumnType("numeric");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.arutmin_waste_removal_item)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<audit_trail>(entity =>
            {
                entity.HasIndex(e => e.created_by)
                    .HasName("ix_audit_trail_created_by")
                    .IncludeProperties(new[] { "created_on" });

                entity.HasIndex(e => e.created_on)
                    .HasName("ix_audit_trail_created_on")
                    .HasSortOrder(new[] { SortOrder.Descending });

                entity.HasIndex(e => e.entity_id)
                    .HasName("ix_audit_trail_entity_id");

                entity.HasIndex(e => e.modified_by)
                    .HasName("ix_audit_trail_modified_by")
                    .IncludeProperties(new[] { "modified_on" });

                entity.HasIndex(e => e.organization_id)
                    .HasName("ix_audit_trail_organization_id");

                entity.HasIndex(e => e.record_id)
                    .HasName("ix_audit_trail_record_id")
                    .IncludeProperties(new[] { "organization_id", "app_entity_id" });

                entity.HasIndex(e => e.table_name)
                    .HasName("ix_audit_trail_table_name");

                entity.Property(e => e.new_record).HasColumnType("jsonb");

                entity.Property(e => e.old_record).HasColumnType("jsonb");

                entity.Property(e => e.organization_id).IsRequired();

                entity.Property(e => e.owner_id).IsRequired();

                entity.Property(e => e.record_id).IsRequired();
            });

            modelBuilder.Entity<bank>(entity =>
            {
                entity.HasIndex(e => new { e.organization_id, e.bank_name })
                    .HasName("uk_bank")
                    .IsUnique();

                entity.Property(e => e.bank_name).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.bank)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<bank_account>(entity =>
            {
                entity.HasIndex(e => new { e.bank_id, e.account_number })
                    .HasName("uk_bank_account")
                    .IsUnique();

                entity.Property(e => e.account_holder).IsRequired();

                entity.Property(e => e.account_number).IsRequired();

                entity.Property(e => e.bank_id).IsRequired();

                entity.HasOne(d => d.bank_)
                    .WithMany(p => p.bank_account)
                    .HasForeignKey(d => d.bank_id)
                    .HasConstraintName("fk_bank_bank_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.bank_account)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<barge>(entity =>
            {
                entity.HasIndex(e => new { e.organization_id, e.vehicle_name })
                    .HasName("uk_barge")
                    .IsUnique();

                entity.Property(e => e.closing_date).HasColumnType("date");

                entity.Property(e => e.current_stock).HasColumnType("numeric");

                entity.Property(e => e.maximum_capacity).HasColumnType("numeric");

                entity.Property(e => e.minimum_capacity).HasColumnType("numeric");

                entity.Property(e => e.opening_date).HasColumnType("date");

                entity.Property(e => e.target_capacity).HasColumnType("numeric");

                entity.Property(e => e.trip_plan).HasColumnType("numeric");

                entity.Property(e => e.vehicle_name).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.barge)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<barge_rotation>(entity =>
            {
                entity.Property(e => e.net_quantity).HasColumnType("numeric");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.barge_rotation)
                    .HasForeignKey(d => d.organization_id)
                    .HasConstraintName("fk_organization_id");
            });

            modelBuilder.Entity<barging_closing>(entity =>
            {
                entity.Property(e => e.distance).HasColumnType("numeric");

                entity.Property(e => e.volume).HasColumnType("numeric");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.barging_closing)
                    .HasForeignKey(d => d.organization_id)
                    .HasConstraintName("barging_closing_organization_id_fkey");
            });

            modelBuilder.Entity<barging_closing_item>(entity =>
            {
                entity.Property(e => e.barging_closing_id).IsRequired();

                entity.Property(e => e.quantity_item).HasColumnType("numeric");

                entity.Property(e => e.transaction_item_date).HasColumnType("date");

                entity.HasOne(d => d.barging_closing_)
                    .WithMany(p => p.barging_closing_item)
                    .HasForeignKey(d => d.barging_closing_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("barging_closing_item_fk");
            });

            modelBuilder.Entity<barging_load_unload_document>(entity =>
            {
                entity.Property(e => e.barging_transaction_id).IsRequired();

                entity.HasOne(d => d.barging_transaction_)
                    .WithMany(p => p.barging_load_unload_document)
                    .HasForeignKey(d => d.barging_transaction_id)
                    .HasConstraintName("fk_barging_transaction_barging_transaction_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.barging_load_unload_document)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<barging_plan>(entity =>
            {
                entity.HasIndex(e => new { e.organization_id, e.barging_plan_number })
                    .HasName("uk_barging_plan")
                    .IsUnique();

                entity.Property(e => e.barging_plan_number).IsRequired();

                entity.HasOne(d => d.location_)
                    .WithMany(p => p.barging_planlocation_)
                    .HasForeignKey(d => d.location_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_business_area_location_id");

                entity.HasOne(d => d.master_list_)
                    .WithMany(p => p.barging_plan)
                    .HasForeignKey(d => d.master_list_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_master_list_master_list_id");

                entity.HasOne(d => d.mine_location_)
                    .WithMany(p => p.barging_plan)
                    .HasForeignKey(d => d.mine_location_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_mine_location_mine_location_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.barging_plan)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.pit_)
                    .WithMany(p => p.barging_planpit_)
                    .HasForeignKey(d => d.pit_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_business_area_pit_id");
            });

            modelBuilder.Entity<barging_plan_daily>(entity =>
            {
                entity.Property(e => e.loading_rate).HasColumnType("numeric");

                entity.Property(e => e.operational_hours).HasColumnType("numeric");

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.HasOne(d => d.barging_plan_monthly_)
                    .WithMany(p => p.barging_plan_daily)
                    .HasForeignKey(d => d.barging_plan_monthly_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_barging_plan_monthly_barging_plan_monthly_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.barging_plan_daily)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<barging_plan_monthly>(entity =>
            {
                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.HasOne(d => d.barging_plan_)
                    .WithMany(p => p.barging_plan_monthly)
                    .HasForeignKey(d => d.barging_plan_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_barging_plan_barging_plan_id");

                entity.HasOne(d => d.month_)
                    .WithMany(p => p.barging_plan_monthly)
                    .HasForeignKey(d => d.month_id)
                    .HasConstraintName("fk_months_month_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.barging_plan_monthly)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<barging_plan_monthly_history>(entity =>
            {
                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.HasOne(d => d.barging_plan_)
                    .WithMany(p => p.barging_plan_monthly_history)
                    .HasForeignKey(d => d.barging_plan_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_barging_plan_barging_plan_id");

                entity.HasOne(d => d.barging_plan_monthly_)
                    .WithMany(p => p.barging_plan_monthly_history)
                    .HasForeignKey(d => d.barging_plan_monthly_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_barging_plan_monthly_barging_plan_monthly_id");

                entity.HasOne(d => d.month_)
                    .WithMany(p => p.barging_plan_monthly_history)
                    .HasForeignKey(d => d.month_id)
                    .HasConstraintName("fk_months_month_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.barging_plan_monthly_history)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<barging_plan_rawdata>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.daily_qty).HasColumnType("numeric");

                entity.Property(e => e.monthly_qty).HasColumnType("numeric");
            });

            modelBuilder.Entity<barging_schema>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.qty).HasColumnType("numeric");
            });

            modelBuilder.Entity<barging_transaction>(entity =>
            {
                entity.HasIndex(e => e.accounting_period_id)
                    .HasName("ix_barging_transaction_accounting_period_id");

                entity.HasIndex(e => e.process_flow_id)
                    .HasName("ix_barging_transaction_process_flow_id");

                entity.HasIndex(e => e.product_id)
                    .HasName("ix_barging_transaction_product_id");

                entity.HasIndex(e => e.survey_id)
                    .HasName("ix_barging_transaction_survey_id");

                entity.HasIndex(e => e.uom_id)
                    .HasName("ix_barging_transaction_uom_id");

                entity.HasIndex(e => new { e.organization_id, e.transaction_number })
                    .HasName("uk_barging_transaction")
                    .IsUnique();

                entity.Property(e => e.beltscale_quantity).HasColumnType("numeric");

                entity.Property(e => e.distance).HasColumnType("numeric");

                entity.Property(e => e.hour_usage).HasColumnType("numeric");

                entity.Property(e => e.integration_status).HasDefaultValueSql("'NOT APPROVED'::text");

                entity.Property(e => e.intermediate_quantity).HasColumnType("numeric");

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.return_cargo).HasColumnType("numeric");

                entity.Property(e => e.source_location_id).IsRequired();

                entity.Property(e => e.transaction_number).IsRequired();

                entity.Property(e => e.uom_id).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.barging_transaction)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<belt_scale>(entity =>
            {
                entity.Property(e => e.loading_rate).HasColumnType("numeric");

                entity.Property(e => e.quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<benchmark_price_series>(entity =>
            {
                entity.Property(e => e.end_date).HasColumnType("date");

                entity.Property(e => e.start_date).HasColumnType("date");

                entity.HasOne(d => d.reference_price_)
                    .WithMany(p => p.benchmark_price_series)
                    .HasForeignKey(d => d.reference_price_id)
                    .HasConstraintName("benchmark_price_series_fk");
            });

            modelBuilder.Entity<benchmark_price_series_detail>(entity =>
            {
                entity.Property(e => e.ash).HasColumnType("numeric");

                entity.Property(e => e.calori).HasColumnType("numeric");

                entity.Property(e => e.price).HasColumnType("numeric");

                entity.Property(e => e.total_moisture).HasColumnType("numeric");

                entity.Property(e => e.total_sulphur).HasColumnType("numeric");

                entity.HasOne(d => d.benchmark_price_)
                    .WithMany(p => p.benchmark_price_series_detail)
                    .HasForeignKey(d => d.benchmark_price_id)
                    .HasConstraintName("benchmark_price_series_detail_fk");

                entity.HasOne(d => d.brand_)
                    .WithMany(p => p.benchmark_price_series_detail)
                    .HasForeignKey(d => d.brand_id)
                    .HasConstraintName("benchmark_price_series_detail_to_brancmark_price_brand_fk");

                entity.HasOne(d => d.currency_)
                    .WithMany(p => p.benchmark_price_series_detail)
                    .HasForeignKey(d => d.currency_id)
                    .HasConstraintName("benchmark_price_series_detail_fk_1");
            });

            modelBuilder.Entity<bi_monitoring>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.sumrec).HasColumnType("numeric");

                entity.Property(e => e.trans_date).HasColumnType("date");
            });

            modelBuilder.Entity<bi_timesheet_detail>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.achievement).HasColumnType("numeric");

                entity.Property(e => e.coal).HasColumnType("numeric");

                entity.Property(e => e.coalplan).HasColumnType("numeric");

                entity.Property(e => e.id).HasMaxLength(4000);

                entity.Property(e => e.ob).HasColumnType("numeric");

                entity.Property(e => e.obplan).HasColumnType("numeric");

                entity.Property(e => e.vol_distance).HasColumnType("numeric");

                entity.Property(e => e.vol_distance_plan).HasColumnType("numeric");

                entity.Property(e => e.volume).HasColumnType("numeric");

                entity.Property(e => e.volume_plan).HasColumnType("numeric");
            });

            modelBuilder.Entity<bi_timesheet_event>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.minute).HasColumnType("numeric");

                entity.Property(e => e.timesheet_hour).HasColumnType("numeric");

                entity.Property(e => e.timesheet_time).HasColumnType("time without time zone");
            });

            modelBuilder.Entity<bill_of_lading>(entity =>
            {
                entity.HasIndex(e => e.bill_of_lading_date)
                    .HasName("ix_bill_of_lading_bill_of_lading_date");

                entity.Property(e => e.bill_of_lading_date).HasColumnType("date");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.bill_of_lading)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<blending_plan>(entity =>
            {
                entity.HasIndex(e => e.accounting_period_id)
                    .HasName("ix_blending_plan_accounting_period_id");

                entity.HasIndex(e => e.process_flow_id)
                    .HasName("ix_blending_plan_process_flow_id");

                entity.HasIndex(e => e.product_id)
                    .HasName("ix_blending_plan_product_id");

                entity.HasIndex(e => e.survey_id)
                    .HasName("ix_blending_plan_survey_id");

                entity.HasIndex(e => e.uom_id)
                    .HasName("ix_blending_plan_uom_id");

                entity.HasIndex(e => new { e.organization_id, e.transaction_number })
                    .HasName("uk_blending_plan")
                    .IsUnique();

                entity.Property(e => e.hour_usage).HasColumnType("numeric");

                entity.Property(e => e.product_id).IsRequired();

                entity.Property(e => e.transaction_number).IsRequired();

                entity.Property(e => e.unloading_quantity).HasColumnType("numeric");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.blending_plan)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<blending_plan_product>(entity =>
            {
                entity.HasIndex(e => e.accounting_period_id)
                    .HasName("ix_blending_plan_source_accounting_period_id_1");

                entity.HasIndex(e => e.process_flow_id)
                    .HasName("ix_blending_plan_source_process_flow_id_1");

                entity.HasIndex(e => e.product_id)
                    .HasName("ix_blending_plan_source_product_id_1");

                entity.HasIndex(e => e.survey_id)
                    .HasName("ix_blending_plan_source_survey_id_1");

                entity.HasIndex(e => e.uom_id)
                    .HasName("ix_blending_plan_source_uom_id_1");

                entity.HasIndex(e => new { e.blending_plan_id, e.source_location_id })
                    .HasName("uk_blending_plan_source_1")
                    .IsUnique();

                entity.Property(e => e.analyte_1).HasColumnType("numeric");

                entity.Property(e => e.analyte_10).HasColumnType("numeric");

                entity.Property(e => e.analyte_11).HasColumnType("numeric");

                entity.Property(e => e.analyte_2).HasColumnType("numeric");

                entity.Property(e => e.analyte_3).HasColumnType("numeric");

                entity.Property(e => e.analyte_4).HasColumnType("numeric");

                entity.Property(e => e.analyte_5).HasColumnType("numeric");

                entity.Property(e => e.analyte_6).HasColumnType("numeric");

                entity.Property(e => e.analyte_7).HasColumnType("numeric");

                entity.Property(e => e.analyte_8).HasColumnType("numeric");

                entity.Property(e => e.analyte_9).HasColumnType("numeric");

                entity.Property(e => e.blending_plan_id).IsRequired();

                entity.Property(e => e.hour_usage).HasColumnType("numeric");

                entity.Property(e => e.loading_quantity).HasColumnType("numeric");

                entity.Property(e => e.spec_ts).HasColumnType("numeric");

                entity.Property(e => e.volume).HasColumnType("numeric");
            });

            modelBuilder.Entity<blending_plan_quality>(entity =>
            {
                entity.HasIndex(e => new { e.blending_plan_id, e.quality_sampling_id, e.survey_id })
                    .HasName("uk_blending_plan_quality")
                    .IsUnique();

                entity.Property(e => e.blending_plan_id).IsRequired();

                entity.HasOne(d => d.blending_plan_)
                    .WithMany(p => p.blending_plan_quality)
                    .HasForeignKey(d => d.blending_plan_id)
                    .HasConstraintName("fk_blending_plan_blending_plan_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.blending_plan_quality)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<blending_plan_source>(entity =>
            {
                entity.HasIndex(e => e.accounting_period_id)
                    .HasName("ix_blending_plan_source_accounting_period_id");

                entity.HasIndex(e => e.process_flow_id)
                    .HasName("ix_blending_plan_source_process_flow_id");

                entity.HasIndex(e => e.product_id)
                    .HasName("ix_blending_plan_source_product_id");

                entity.HasIndex(e => e.survey_id)
                    .HasName("ix_blending_plan_source_survey_id");

                entity.HasIndex(e => e.uom_id)
                    .HasName("ix_blending_plan_source_uom_id");

                entity.HasIndex(e => new { e.blending_plan_id, e.source_location_id })
                    .HasName("uk_blending_plan_source")
                    .IsUnique();

                entity.Property(e => e.analyte_1).HasColumnType("numeric");

                entity.Property(e => e.analyte_10).HasColumnType("numeric");

                entity.Property(e => e.analyte_11).HasColumnType("numeric");

                entity.Property(e => e.analyte_2).HasColumnType("numeric");

                entity.Property(e => e.analyte_3).HasColumnType("numeric");

                entity.Property(e => e.analyte_4).HasColumnType("numeric");

                entity.Property(e => e.analyte_5).HasColumnType("numeric");

                entity.Property(e => e.analyte_6).HasColumnType("numeric");

                entity.Property(e => e.analyte_7).HasColumnType("numeric");

                entity.Property(e => e.analyte_8).HasColumnType("numeric");

                entity.Property(e => e.analyte_9).HasColumnType("numeric");

                entity.Property(e => e.blending_plan_id).IsRequired();

                entity.Property(e => e.hour_usage).HasColumnType("numeric");

                entity.Property(e => e.loading_quantity).HasColumnType("numeric");

                entity.Property(e => e.spec_ts).HasColumnType("numeric");

                entity.Property(e => e.volume).HasColumnType("numeric");

                entity.HasOne(d => d.blending_plan_)
                    .WithMany(p => p.blending_plan_source)
                    .HasForeignKey(d => d.blending_plan_id)
                    .HasConstraintName("blending_plan_source_blending_plan_id_fkey");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.blending_plan_source)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<blending_plan_value>(entity =>
            {
                entity.HasIndex(e => new { e.blending_plan_source_id, e.analyte_id })
                    .HasName("uk_blending_plan_value")
                    .IsUnique();

                entity.Property(e => e.analyte_value).HasColumnType("numeric");

                entity.Property(e => e.blending_plan_source_id).IsRequired();

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.HasOne(d => d.blending_plan_source_)
                    .WithMany(p => p.blending_plan_value)
                    .HasForeignKey(d => d.blending_plan_source_id)
                    .HasConstraintName("fk_blending_plan_source_blending_plan_source_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.blending_plan_value)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<business_area>(entity =>
            {
                entity.Property(e => e.business_area_name).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.business_area)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization");
            });

            modelBuilder.Entity<business_days>(entity =>
            {
                entity.Property(e => e.calendar_id).IsRequired();

                entity.HasOne(d => d.calendar_)
                    .WithMany(p => p.business_days)
                    .HasForeignKey(d => d.calendar_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_calendar_calendar_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.business_days)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<business_partner>(entity =>
            {
                entity.HasIndex(e => e.organization_id)
                    .HasName("ix_business_partner_organization_id");

                entity.HasIndex(e => new { e.organization_id, e.business_partner_name })
                    .HasName("uk_business_partner")
                    .IsUnique();

                entity.Property(e => e.business_partner_name).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.business_partner)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<business_unit>(entity =>
            {
                entity.HasIndex(e => e.created_by)
                    .HasName("ix_business_unit_created_by")
                    .IncludeProperties(new[] { "created_on" });

                entity.HasIndex(e => e.default_team_id)
                    .HasName("ix_business_unit_default_team_id");

                entity.HasIndex(e => e.entity_id)
                    .HasName("ix_business_unit_entity_id");

                entity.HasIndex(e => e.is_active)
                    .HasName("ix_business_unit_is_active")
                    .IncludeProperties(new[] { "owner_id", "organization_id" });

                entity.HasIndex(e => e.modified_by)
                    .HasName("ix_business_unit_modified_by")
                    .IncludeProperties(new[] { "modified_on" });

                entity.HasIndex(e => e.organization_id)
                    .HasName("ix_business_unit_organization_id")
                    .IncludeProperties(new[] { "is_active", "owner_id" });

                entity.HasIndex(e => e.owner_id)
                    .HasName("ix_business_unit_owner_id")
                    .IncludeProperties(new[] { "is_active", "organization_id" });

                entity.HasIndex(e => e.parent_business_unit_id)
                    .HasName("ix_business_unit_parent_business_unit_id");

                entity.HasIndex(e => new { e.organization_id, e.business_unit_name })
                    .HasName("uk_business_unit")
                    .IsUnique();

                entity.Property(e => e.business_unit_name).IsRequired();

                entity.Property(e => e.organization_id).IsRequired();

                entity.Property(e => e.owner_id).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.business_unit)
                    .HasForeignKey(d => d.organization_id)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<calendar>(entity =>
            {
                entity.HasIndex(e => new { e.organization_id, e.calendar_name })
                    .HasName("uk_calendar")
                    .IsUnique();

                entity.Property(e => e.calendar_name).IsRequired();

                entity.Property(e => e.end_date).HasColumnType("date");

                entity.Property(e => e.start_date).HasColumnType("date");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.calendar)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<chls>(entity =>
            {
                entity.HasIndex(e => new { e.organization_id, e.transaction_number })
                    .HasName("uk_chls")
                    .IsUnique();

                entity.Property(e => e.date).HasColumnType("date");

                entity.Property(e => e.gross_loading_cpp).HasColumnType("numeric");

                entity.Property(e => e.gross_loading_olc).HasColumnType("numeric");

                entity.Property(e => e.net_loading_cpp).HasColumnType("numeric");

                entity.Property(e => e.net_loading_olc).HasColumnType("numeric");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.chls)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<chls_additional_info>(entity =>
            {
                entity.Property(e => e.chls_id).IsRequired();

                entity.Property(e => e.start_count).HasColumnType("numeric");

                entity.Property(e => e.stop_count).HasColumnType("numeric");

                entity.Property(e => e.total_count).HasColumnType("numeric");

                entity.HasOne(d => d.chls_)
                    .WithMany(p => p.chls_additional_info)
                    .HasForeignKey(d => d.chls_id)
                    .HasConstraintName("fk_chls_chls_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.chls_additional_info)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<chls_cpp>(entity =>
            {
                entity.HasIndex(e => new { e.organization_id, e.formula })
                    .HasName("uk_chls_cpp")
                    .IsUnique();

                entity.Property(e => e.duration).HasColumnType("numeric");

                entity.Property(e => e.formula).IsRequired();

                entity.Property(e => e.nett_loading_rate).HasColumnType("numeric");

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.chls_cpp)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<chls_hauling>(entity =>
            {
                entity.HasIndex(e => new { e.organization_id, e.formula })
                    .HasName("uk_chls_hauling")
                    .IsUnique();

                entity.Property(e => e.duration).HasColumnType("numeric");

                entity.Property(e => e.formula).IsRequired();

                entity.Property(e => e.nett_loading_rate).HasColumnType("numeric");

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.chls_hauling)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<city>(entity =>
            {
                entity.HasIndex(e => new { e.province_id, e.city_name })
                    .HasName("uk_city")
                    .IsUnique();

                entity.Property(e => e.city_name).IsRequired();

                entity.Property(e => e.province_id).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.city)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.province_)
                    .WithMany(p => p.city)
                    .HasForeignKey(d => d.province_id)
                    .HasConstraintName("fk_province_province_id");
            });

            modelBuilder.Entity<closing_schema>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.month_closing).HasColumnType("numeric");

                entity.Property(e => e.qty).HasColumnType("numeric");

                entity.Property(e => e.year_closing).HasColumnType("numeric");
            });

            modelBuilder.Entity<coa>(entity =>
            {
                entity.HasIndex(e => new { e.organization_id, e.account_code })
                    .HasName("uk_coa")
                    .IsUnique();

                entity.Property(e => e.account_code).IsRequired();

                entity.Property(e => e.account_name).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.coa)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<coa_subaccount>(entity =>
            {
                entity.HasIndex(e => new { e.coa_id, e.subaccount_code })
                    .HasName("uk_coa_subaccount")
                    .IsUnique();

                entity.Property(e => e.coa_id).IsRequired();

                entity.Property(e => e.subaccount_code).IsRequired();

                entity.Property(e => e.subaccount_name).IsRequired();

                entity.HasOne(d => d.coa_)
                    .WithMany(p => p.coa_subaccount)
                    .HasForeignKey(d => d.coa_id)
                    .HasConstraintName("fk_coa_coa_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.coa_subaccount)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<coal_barging_allsite_daily_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_barging_allsite_monthly_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_barging_allsite_mtd_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_barging_allsite_yearly_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_barging_allsite_ytd_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_barging_daily_details>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_barging_daily_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_barging_details_all>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_barging_mtd_details>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_barging_mtd_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_barging_mtfull_details>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_barging_mtfull_details_trans>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_barging_pct_all>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_barging_rawdata>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.beltscale_quantity).HasColumnType("numeric");

                entity.Property(e => e.distance).HasColumnType("numeric");

                entity.Property(e => e.draft_survey_date).HasColumnType("date");

                entity.Property(e => e.draftsurvey_qty).HasColumnType("numeric");

                entity.Property(e => e.hour_usage).HasColumnType("numeric");

                entity.Property(e => e.intermediate_quantity).HasColumnType("numeric");

                entity.Property(e => e.return_cargo).HasColumnType("numeric");

                entity.Property(e => e.split_qty).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_barging_ytd_details>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_barging_ytd_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_barging_ytfull_details>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_barging_ytfull_details_trans>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_hauling_allsite_daily_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_hauling_allsite_monthly_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_hauling_allsite_mtd_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_hauling_allsite_yearly_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_hauling_allsite_ytd_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_hauling_daily_details>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_hauling_daily_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_hauling_details_all>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_hauling_mtd_details>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_hauling_mtd_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_hauling_mtfull_details>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_hauling_mtfull_details_trans>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_hauling_pct_all>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_hauling_rawdata>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.distance).HasColumnType("numeric");

                entity.Property(e => e.gross).HasColumnType("numeric");

                entity.Property(e => e.hour_usage).HasColumnType("numeric");

                entity.Property(e => e.loading_quantity).HasColumnType("numeric");

                entity.Property(e => e.netto_rekon).HasColumnType("numeric");

                entity.Property(e => e.tare).HasColumnType("numeric");

                entity.Property(e => e.unloading_quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_hauling_ytd_details>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_hauling_ytd_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_hauling_ytfull_details>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_hauling_ytfull_details_trans>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_mine_allsite_daily_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_mine_allsite_monthly_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_mine_allsite_mtd_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_mine_allsite_yearly_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_mine_allsite_ytd_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_mine_daily_details>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_mine_daily_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_mine_details_all>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_mine_mtd_details>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_mine_mtd_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_mine_mtfull_details>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_mine_mtfull_details_trans>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_mine_pct_all>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_mine_rawdata>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.density).HasColumnType("numeric");

                entity.Property(e => e.distance).HasColumnType("numeric");

                entity.Property(e => e.duration).HasColumnType("time without time zone");

                entity.Property(e => e.elevation).HasColumnType("numeric");

                entity.Property(e => e.gross).HasColumnType("numeric");

                entity.Property(e => e.hour_usage).HasColumnType("numeric");

                entity.Property(e => e.loading_quantity).HasColumnType("numeric");

                entity.Property(e => e.ritase).HasColumnType("numeric");

                entity.Property(e => e.tare).HasColumnType("numeric");

                entity.Property(e => e.unloading_quantity).HasColumnType("numeric");

                entity.Property(e => e.volume).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_mine_ytd_details>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_mine_ytd_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_mine_ytfull_details>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_mine_ytfull_details_trans>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_movement>(entity =>
            {
                entity.HasIndex(e => e.accounting_period_id)
                    .HasName("ix_rehandling_transaction_accounting_period_id_1");

                entity.HasIndex(e => e.process_flow_id)
                    .HasName("ix_rehandling_transaction_process_flow_id_1");

                entity.HasIndex(e => e.product_id)
                    .HasName("ix_rehandling_transaction_product_id_1");

                entity.HasIndex(e => e.survey_id)
                    .HasName("ix_rehandling_transaction_survey_id_1");

                entity.HasIndex(e => e.uom_id)
                    .HasName("ix_rehandling_transaction_uom_id_1");

                entity.HasIndex(e => new { e.organization_id, e.transaction_number })
                    .HasName("uk_rehandling_transaction_1")
                    .IsUnique();

                entity.Property(e => e.hour_usage).HasColumnType("numeric");

                entity.Property(e => e.loading_quantity).HasColumnType("numeric");

                entity.Property(e => e.source_location_id).IsRequired();

                entity.Property(e => e.transaction_number).IsRequired();

                entity.Property(e => e.unloading_quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_price_index_aims>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.chg_val).HasColumnType("numeric");

                entity.Property(e => e.max_date).HasColumnType("date");

                entity.Property(e => e.max_val).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_price_index_mtd_aims>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.chg_val).HasColumnType("numeric");

                entity.Property(e => e.max_date).HasColumnType("date");

                entity.Property(e => e.max_val).HasColumnType("numeric");

                entity.Property(e => e.min_date).HasColumnType("date");

                entity.Property(e => e.min_val).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_price_index_week_mtd_aims>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.mtd_chg_val).HasColumnType("numeric");

                entity.Property(e => e.mtd_max_date).HasColumnType("date");

                entity.Property(e => e.mtd_max_val).HasColumnType("numeric");

                entity.Property(e => e.mtd_min_date).HasColumnType("date");

                entity.Property(e => e.mtd_min_val).HasColumnType("numeric");

                entity.Property(e => e.week_chg_val).HasColumnType("numeric");

                entity.Property(e => e.week_max_date).HasColumnType("date");

                entity.Property(e => e.week_max_val).HasColumnType("numeric");

                entity.Property(e => e.week_min_date).HasColumnType("date");

                entity.Property(e => e.week_min_val).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_produce_allsite_daily_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_produce_allsite_monthly_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_produce_allsite_mtd_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_produce_allsite_yearly_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_produce_allsite_ytd_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_produce_daily_details>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_produce_daily_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_produce_details_all>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_produce_mtd_details>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_produce_mtd_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_produce_mtfull_details>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_produce_mtfull_details_trans>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_produce_pct_all>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_produce_rawdata>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.hour_usage).HasColumnType("numeric");

                entity.Property(e => e.in_qty).HasColumnType("numeric");

                entity.Property(e => e.out_qty).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_produce_ytd_details>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_produce_ytd_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_produce_ytfull_details>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_produce_ytfull_details_trans>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_produced_sp>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.date).HasColumnType("date");

                entity.Property(e => e.quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_transfer>(entity =>
            {
                entity.HasIndex(e => e.accounting_period_id)
                    .HasName("ix_hauling_transaction_accounting_period_id_1");

                entity.HasIndex(e => e.process_flow_id)
                    .HasName("ix_hauling_transaction_process_flow_id_1");

                entity.HasIndex(e => e.product_id)
                    .HasName("ix_hauling_transaction_product_id_1");

                entity.HasIndex(e => e.survey_id)
                    .HasName("ix_hauling_transaction_survey_id_1");

                entity.HasIndex(e => e.uom_id)
                    .HasName("ix_hauling_transaction_uom_id_1");

                entity.HasIndex(e => new { e.organization_id, e.transaction_number })
                    .HasName("uk_hauling_transaction_1")
                    .IsUnique();

                entity.Property(e => e.destination_location_id).IsRequired();

                entity.Property(e => e.distance).HasColumnType("numeric");

                entity.Property(e => e.gross).HasColumnType("numeric");

                entity.Property(e => e.hour_usage).HasColumnType("numeric");

                entity.Property(e => e.integration_status).HasDefaultValueSql("'NOT APPROVED'::text");

                entity.Property(e => e.loading_quantity).HasColumnType("numeric");

                entity.Property(e => e.process_flow_id).IsRequired();

                entity.Property(e => e.tare).HasColumnType("numeric");

                entity.Property(e => e.transaction_number).IsRequired();

                entity.Property(e => e.unloading_quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_transfer_allsite_daily_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_transfer_allsite_monthly_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_transfer_allsite_mtd_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_transfer_allsite_yearly_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_transfer_allsite_ytd_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_transfer_closing>(entity =>
            {
                entity.Property(e => e.distance).HasColumnType("numeric");

                entity.Property(e => e.volume).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_transfer_closing_item>(entity =>
            {
                entity.Property(e => e.coal_transfer_closing_id).IsRequired();

                entity.Property(e => e.quantity_item).HasColumnType("numeric");

                entity.Property(e => e.transaction_item_date).HasColumnType("date");

                entity.HasOne(d => d.coal_transfer_closing_)
                    .WithMany(p => p.coal_transfer_closing_item)
                    .HasForeignKey(d => d.coal_transfer_closing_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("coal_transfer_closing_item_fk");
            });

            modelBuilder.Entity<coal_transfer_daily_details>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_transfer_daily_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_transfer_details_all>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_transfer_mtd_details>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_transfer_mtd_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_transfer_mtfull_details>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_transfer_mtfull_details_trans>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_transfer_pct_all>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_transfer_rawdata>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.distance).HasColumnType("numeric");

                entity.Property(e => e.gross).HasColumnType("numeric");

                entity.Property(e => e.hour_usage).HasColumnType("numeric");

                entity.Property(e => e.loading_quantity).HasColumnType("numeric");

                entity.Property(e => e.tare).HasColumnType("numeric");

                entity.Property(e => e.unloading_quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_transfer_ytd_details>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_transfer_ytd_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_transfer_ytfull_details>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<coal_transfer_ytfull_details_trans>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<contact>(entity =>
            {
                entity.HasIndex(e => e.business_partner_id)
                    .HasName("ix_contact_business_partner_id");

                entity.HasIndex(e => e.organization_id)
                    .HasName("ix_contact_organization_id");

                entity.HasIndex(e => new { e.business_partner_id, e.contact_name, e.contact_email, e.contact_phone })
                    .HasName("uk_contact")
                    .IsUnique();

                entity.Property(e => e.business_partner_id).IsRequired();

                entity.Property(e => e.contact_name).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.contact)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<contractor>(entity =>
            {
                entity.HasIndex(e => e.business_partner_code)
                    .HasName("uk_contractor")
                    .IsUnique();

                entity.Property(e => e.barge_trip_plan).HasColumnType("numeric");

                entity.Property(e => e.business_partner_name).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.contractor)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<contractor_document>(entity =>
            {
                entity.Property(e => e.contractor_id).IsRequired();

                entity.HasOne(d => d.contractor_)
                    .WithMany(p => p.contractor_document)
                    .HasForeignKey(d => d.contractor_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_contractor_contractor_id");

                entity.HasOne(d => d.document_type_)
                    .WithMany(p => p.contractor_document)
                    .HasForeignKey(d => d.document_type_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_document_type_document_type_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.contractor_document)
                    .HasForeignKey(d => d.organization_id)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<cost_rate>(entity =>
            {
                entity.Property(e => e.hourly_rate).HasColumnType("numeric");

                entity.Property(e => e.trip_rate).HasColumnType("numeric");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.cost_rate)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<country>(entity =>
            {
                entity.HasIndex(e => new { e.organization_id, e.country_name })
                    .HasName("uk_country")
                    .IsUnique();

                entity.Property(e => e.country_code).IsRequired();

                entity.Property(e => e.country_name).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.country)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_country_country_id");
            });

            modelBuilder.Entity<cpp_location>(entity =>
            {
                entity.HasIndex(e => new { e.business_area_id, e.parent_stock_location_id, e.stock_location_name })
                    .HasName("uk1_cpp_location")
                    .IsUnique();

                entity.Property(e => e.closing_date).HasColumnType("date");

                entity.Property(e => e.cpp_location_code).IsRequired();

                entity.Property(e => e.current_stock).HasColumnType("numeric");

                entity.Property(e => e.maximum_capacity).HasColumnType("numeric");

                entity.Property(e => e.minimum_capacity).HasColumnType("numeric");

                entity.Property(e => e.opening_date).HasColumnType("date");

                entity.Property(e => e.target_capacity).HasColumnType("numeric");

                entity.HasOne(d => d.business_area_)
                    .WithMany(p => p.cpp_location)
                    .HasForeignKey(d => d.business_area_id)
                    .HasConstraintName("cpp_location_business_area_id_fkey");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.cpp_location)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<cpp_product>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.qty).HasColumnType("numeric");
            });

            modelBuilder.Entity<credit_limit_history>(entity =>
            {
                entity.Property(e => e.credit_limit_value).HasColumnType("numeric");

                entity.Property(e => e.customer_id).IsRequired();
            });

            modelBuilder.Entity<currency>(entity =>
            {
                entity.HasIndex(e => new { e.organization_id, e.currency_name })
                    .HasName("uk_currency")
                    .IsUnique();

                entity.Property(e => e.currency_code).IsRequired();

                entity.Property(e => e.currency_name).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.currency)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<currency_exchange>(entity =>
            {
                entity.Property(e => e.buying_rate)
                    .HasColumnType("numeric")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.end_date).HasColumnType("date");

                entity.Property(e => e.exchange_rate).HasColumnType("numeric");

                entity.Property(e => e.selling_rate)
                    .HasColumnType("numeric")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.source_currency_id).IsRequired();

                entity.Property(e => e.start_date).HasColumnType("date");

                entity.Property(e => e.target_currency_id).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.currency_exchange)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.source_currency_)
                    .WithMany(p => p.currency_exchangesource_currency_)
                    .HasForeignKey(d => d.source_currency_id)
                    .HasConstraintName("fk_currency_source_currency_id");

                entity.HasOne(d => d.target_currency_)
                    .WithMany(p => p.currency_exchangetarget_currency_)
                    .HasForeignKey(d => d.target_currency_id)
                    .HasConstraintName("fk_currency_target_currency_id");
            });

            modelBuilder.Entity<currency_exchange_api>(entity =>
            {
                entity.Property(e => e.f_buy_rate).HasColumnType("numeric");

                entity.Property(e => e.f_middle_rate).HasColumnType("numeric");

                entity.Property(e => e.f_sell_rate).HasColumnType("numeric");
            });

            modelBuilder.Entity<customer>(entity =>
            {
                entity.HasIndex(e => new { e.organization_id, e.business_partner_code, e.alias_name })
                    .HasName("uk_customer")
                    .IsUnique();

                entity.Property(e => e.business_partner_name).IsRequired();

                entity.Property(e => e.credit_limit).HasColumnType("numeric");

                entity.Property(e => e.is_taxable).HasDefaultValueSql("true");

                entity.Property(e => e.remained_credit_limit).HasColumnType("numeric");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.customer)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<customer_attachment>(entity =>
            {
                entity.HasOne(d => d.customer_)
                    .WithMany(p => p.customer_attachment)
                    .HasForeignKey(d => d.customer_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_customer_attachment_customer");
            });

            modelBuilder.Entity<customer_ell>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.business_partner_name).IsRequired();

                entity.Property(e => e.credit_limit)
                    .HasColumnType("numeric")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.id).IsRequired();

                entity.Property(e => e.is_taxable).HasDefaultValueSql("true");

                entity.Property(e => e.remained_credit_limit)
                    .HasColumnType("numeric")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.sync_id).IsRequired();
            });

            modelBuilder.Entity<customer_type>(entity =>
            {
                entity.HasIndex(e => new { e.customer_type_code, e.organization_id })
                    .HasName("Code")
                    .IsUnique();
            });

            modelBuilder.Entity<daily_record>(entity =>
            {
                entity.HasIndex(e => new { e.data_category, e.data_date, e.business_area_id })
                    .HasName("uk_daily_record")
                    .IsUnique();

                entity.Property(e => e.data_date).HasColumnType("date");

                entity.Property(e => e.data_value).HasColumnType("numeric");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.daily_record)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<data_extension>(entity =>
            {
                entity.HasIndex(e => e.record_id)
                    .HasName("uk_data_extension")
                    .IsUnique();

                entity.Property(e => e.number0).HasColumnType("numeric");

                entity.Property(e => e.number1).HasColumnType("numeric");

                entity.Property(e => e.number2).HasColumnType("numeric");

                entity.Property(e => e.number3).HasColumnType("numeric");

                entity.Property(e => e.number4).HasColumnType("numeric");

                entity.Property(e => e.number5).HasColumnType("numeric");

                entity.Property(e => e.number6).HasColumnType("numeric");

                entity.Property(e => e.number7).HasColumnType("numeric");

                entity.Property(e => e.number8).HasColumnType("numeric");

                entity.Property(e => e.number9).HasColumnType("numeric");

                entity.Property(e => e.object0).HasColumnType("jsonb");

                entity.Property(e => e.object1).HasColumnType("jsonb");

                entity.Property(e => e.object2).HasColumnType("jsonb");

                entity.Property(e => e.object3).HasColumnType("jsonb");

                entity.Property(e => e.object4).HasColumnType("jsonb");

                entity.Property(e => e.object5).HasColumnType("jsonb");

                entity.Property(e => e.object6).HasColumnType("jsonb");

                entity.Property(e => e.object7).HasColumnType("jsonb");

                entity.Property(e => e.object8).HasColumnType("jsonb");

                entity.Property(e => e.object9).HasColumnType("jsonb");

                entity.Property(e => e.record_id).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.data_extension)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<dates_aims>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.dates).HasColumnType("date");
            });

            modelBuilder.Entity<daywork>(entity =>
            {
                entity.Property(e => e.hm_duration).HasColumnType("numeric");

                entity.Property(e => e.hm_end).HasColumnType("numeric");

                entity.Property(e => e.hm_start).HasColumnType("numeric");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.daywork)
                    .HasForeignKey(d => d.organization_id)
                    .HasConstraintName("daywork_organization_id_fkey");
            });

            modelBuilder.Entity<daywork_closing>(entity =>
            {
                entity.Property(e => e.total_hm).HasColumnType("numeric");

                entity.Property(e => e.total_value).HasColumnType("numeric");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.daywork_closing)
                    .HasForeignKey(d => d.organization_id)
                    .HasConstraintName("daywork_closing_organization_id_fkey");
            });

            modelBuilder.Entity<daywork_detail>(entity =>
            {
                entity.Property(e => e.hm_duration).HasColumnType("numeric");

                entity.Property(e => e.hm_end).HasColumnType("numeric");

                entity.Property(e => e.hm_start).HasColumnType("numeric");
            });

            modelBuilder.Entity<delay>(entity =>
            {
                entity.HasIndex(e => e.organization_id)
                    .HasName("ix_delay_organization_id");

                entity.Property(e => e.business_area_id).IsRequired();

                entity.Property(e => e.delay_date).HasColumnType("date");

                entity.Property(e => e.duration).HasColumnType("numeric");

                entity.HasOne(d => d.equipment_)
                    .WithMany(p => p.delay)
                    .HasForeignKey(d => d.equipment_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_delay_equipment");

                entity.HasOne(d => d.equipment_type_)
                    .WithMany(p => p.delay)
                    .HasForeignKey(d => d.equipment_type_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_delay_equipment_type");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.delay)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<delay_category>(entity =>
            {
                entity.HasIndex(e => e.organization_id)
                    .HasName("ix_event_category_organization_id");

                entity.HasIndex(e => new { e.organization_id, e.delay_category_name })
                    .HasName("uk_delay_category")
                    .IsUnique();

                entity.Property(e => e.delay_category_name).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.delay_category)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<delay_details>(entity =>
            {
                entity.HasIndex(e => e.organization_id)
                    .HasName("ix_delay_details_organization_id");

                entity.Property(e => e.delay_id).IsRequired();

                entity.Property(e => e.duration).HasColumnType("numeric");

                entity.HasOne(d => d.event_category_)
                    .WithMany(p => p.delay_details)
                    .HasForeignKey(d => d.event_category_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_delay_details_event_category");

                entity.HasOne(d => d.event_definition_category_)
                    .WithMany(p => p.delay_details)
                    .HasForeignKey(d => d.event_definition_category_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_delay_details_event_definition_category");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.delay_details)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<despatch_demurrage>(entity =>
            {
                entity.Property(e => e.contract_name).IsRequired();

                entity.Property(e => e.end_date).HasColumnType("date");

                entity.Property(e => e.start_date).HasColumnType("date");

                entity.HasOne(d => d.despatch_order_)
                    .WithMany(p => p.despatch_demurrage)
                    .HasForeignKey(d => d.despatch_order_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_despatch_order_despatch_order_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.despatch_demurrage)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<despatch_demurrage_debit_credit_note>(entity =>
            {
                entity.HasIndex(e => new { e.organization_id, e.despatch_demurrage_invoice_id })
                    .HasName("despatch_demurrage_debit_cred_organization_id_despatch_demu_key")
                    .IsUnique();

                entity.Property(e => e.debit_credit_date).HasColumnType("date");

                entity.Property(e => e.debit_credit_number).IsRequired();

                entity.Property(e => e.rate).HasColumnType("numeric");

                entity.Property(e => e.total_price).HasColumnType("numeric");

                entity.HasOne(d => d.bank_account_)
                    .WithMany(p => p.despatch_demurrage_debit_credit_notebank_account_)
                    .HasForeignKey(d => d.bank_account_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("despatch_demurrage_debit_credit_note_bank_account_id_fkey");

                entity.HasOne(d => d.correspondent_bank_)
                    .WithMany(p => p.despatch_demurrage_debit_credit_notecorrespondent_bank_)
                    .HasForeignKey(d => d.correspondent_bank_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("despatch_demurrage_debit_credit_note_correspondent_bank_id_fkey");

                entity.HasOne(d => d.currency_exchange_)
                    .WithMany(p => p.despatch_demurrage_debit_credit_note)
                    .HasForeignKey(d => d.currency_exchange_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("despatch_demurrage_debit_credit_note_currency_exchange_id_fkey");

                entity.HasOne(d => d.currency_)
                    .WithMany(p => p.despatch_demurrage_debit_credit_note)
                    .HasForeignKey(d => d.currency_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("despatch_demurrage_debit_credit_note_currency_id_fkey");

                entity.HasOne(d => d.despatch_demurrage_invoice_)
                    .WithMany(p => p.despatch_demurrage_debit_credit_note)
                    .HasForeignKey(d => d.despatch_demurrage_invoice_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("despatch_demurrage_debit_cred_despatch_demurrage_invoice_i_fkey");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.despatch_demurrage_debit_credit_note)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("despatch_demurrage_debit_credit_note_organization_id_fkey");
            });

            modelBuilder.Entity<despatch_demurrage_delay>(entity =>
            {
                entity.HasIndex(e => new { e.despatch_demurrage_id, e.event_category_id })
                    .HasName("uk_despatch_demurrage_sof")
                    .IsUnique();

                entity.Property(e => e.demurrage_percent).HasColumnType("numeric");

                entity.Property(e => e.despatch_demurrage_id).IsRequired();

                entity.Property(e => e.despatch_percent).HasColumnType("numeric");

                entity.Property(e => e.event_category_id).IsRequired();

                entity.HasOne(d => d.despatch_demurrage_)
                    .WithMany(p => p.despatch_demurrage_delay)
                    .HasForeignKey(d => d.despatch_demurrage_id)
                    .HasConstraintName("fk_despatch_demurrage_despatch_demurrage_id");

                entity.HasOne(d => d.despatch_order_)
                    .WithMany(p => p.despatch_demurrage_delay)
                    .HasForeignKey(d => d.despatch_order_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_despatch_order_despatch_order");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.despatch_demurrage_delay)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<despatch_demurrage_detail>(entity =>
            {
                entity.HasIndex(e => new { e.despatch_demurrage_id, e.sof_id })
                    .HasName("unique_despatch_demurrage_sof")
                    .IsUnique();

                entity.Property(e => e.despatch_demurrage_id).IsRequired();

                entity.Property(e => e.despatch_percent).HasColumnType("numeric");

                entity.Property(e => e.loading_rate).HasColumnType("numeric");

                entity.Property(e => e.rate).HasColumnType("numeric");

                entity.Property(e => e.turn_time).HasColumnType("numeric");

                entity.HasOne(d => d.despatch_demurrage_)
                    .WithMany(p => p.despatch_demurrage_detail)
                    .HasForeignKey(d => d.despatch_demurrage_id)
                    .HasConstraintName("fk_despatch_demurrage_despatch_demurrage_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.despatch_demurrage_detail)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<despatch_demurrage_invoice>(entity =>
            {
                entity.HasIndex(e => new { e.organization_id, e.despatch_order_id })
                    .HasName("uk_despatch_demurrage_invoice")
                    .IsUnique();

                entity.Property(e => e.invoice_date).HasColumnType("date");

                entity.Property(e => e.invoice_number).IsRequired();

                entity.Property(e => e.laytime_allowed_duration).HasColumnType("numeric");

                entity.Property(e => e.laytime_used_duration).HasColumnType("numeric");

                entity.Property(e => e.rate).HasColumnType("numeric");

                entity.Property(e => e.total_price).HasColumnType("numeric");

                entity.Property(e => e.total_price_final).HasColumnType("numeric");

                entity.HasOne(d => d.currency_)
                    .WithMany(p => p.despatch_demurrage_invoice)
                    .HasForeignKey(d => d.currency_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_despatch_demurrage_invoice_currency_id");

                entity.HasOne(d => d.despatch_order_)
                    .WithMany(p => p.despatch_demurrage_invoice)
                    .HasForeignKey(d => d.despatch_order_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_despatch_demurrage_invoice_despatch_order_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.despatch_demurrage_invoice)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.sof_)
                    .WithMany(p => p.despatch_demurrage_invoice)
                    .HasForeignKey(d => d.sof_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_despatch_demurrage_invoice_sof_id");
            });

            modelBuilder.Entity<despatch_order>(entity =>
            {
                entity.HasIndex(e => new { e.organization_id, e.despatch_order_number })
                    .HasName("uk_despatch_order")
                    .IsUnique();

                entity.Property(e => e.bill_lading_date).HasColumnType("date");

                entity.Property(e => e.bill_of_lading_date).HasColumnType("date");

                entity.Property(e => e.despatch_demurrage_rate).HasColumnType("numeric");

                entity.Property(e => e.despatch_order_date).HasColumnType("date");

                entity.Property(e => e.despatch_order_number).IsRequired();

                entity.Property(e => e.despatch_percentage).HasColumnType("numeric");

                entity.Property(e => e.final_quantity).HasColumnType("numeric");

                entity.Property(e => e.laycan_end).HasColumnType("date");

                entity.Property(e => e.laycan_start).HasColumnType("date");

                entity.Property(e => e.laytime_duration).HasColumnType("numeric");

                entity.Property(e => e.loading_rate).HasColumnType("numeric");

                entity.Property(e => e.order_reference_date).HasColumnType("date");

                entity.Property(e => e.planned_despatch_date).HasColumnType("date");

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.quantity_actual).HasColumnType("numeric");

                entity.Property(e => e.required_quantity).HasColumnType("numeric");

                entity.Property(e => e.turn_time).HasColumnType("numeric");

                entity.HasOne(d => d.contract_product_)
                    .WithMany(p => p.despatch_order)
                    .HasForeignKey(d => d.contract_product_id)
                    .HasConstraintName("fk_sales_contract_product_contract_product_id");

                entity.HasOne(d => d.contract_term_)
                    .WithMany(p => p.despatch_order)
                    .HasForeignKey(d => d.contract_term_id)
                    .HasConstraintName("despatch_order_fk");

                entity.HasOne(d => d.customer_)
                    .WithMany(p => p.despatch_order)
                    .HasForeignKey(d => d.customer_id)
                    .HasConstraintName("fk_customer_customer_id");

                entity.HasOne(d => d.document_reference_)
                    .WithMany(p => p.despatch_order)
                    .HasForeignKey(d => d.document_reference_id)
                    .HasConstraintName("fk_master_list_document_reference_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.despatch_orderorganization_)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.seller_)
                    .WithMany(p => p.despatch_orderseller_)
                    .HasForeignKey(d => d.seller_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_seller_id");
            });

            modelBuilder.Entity<despatch_order_delay>(entity =>
            {
                entity.Property(e => e.delay_category_id).IsRequired();

                entity.Property(e => e.demurrage_percent).HasColumnType("numeric");

                entity.Property(e => e.despatch_order_id).IsRequired();

                entity.Property(e => e.despatch_percent).HasColumnType("numeric");

                entity.HasOne(d => d.despatch_order_)
                    .WithMany(p => p.despatch_order_delay)
                    .HasForeignKey(d => d.despatch_order_id)
                    .HasConstraintName("fk_despatch_order_despatch_order_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.despatch_order_delay)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<despatch_order_document>(entity =>
            {
                entity.Property(e => e.despatch_order_id).IsRequired();
            });

            modelBuilder.Entity<despatch_order_ell>(entity =>
            {
                entity.HasKey(e => e.sync_id)
                    .HasName("despatch_order_ell_pkey");

                entity.Property(e => e.bill_lading_date).HasColumnType("date");

                entity.Property(e => e.bill_of_lading_date).HasColumnType("date");

                entity.Property(e => e.despatch_demurrage_rate).HasColumnType("numeric");

                entity.Property(e => e.despatch_order_date).HasColumnType("date");

                entity.Property(e => e.despatch_order_number).IsRequired();

                entity.Property(e => e.eta_plan).HasColumnType("date");

                entity.Property(e => e.final_quantity).HasColumnType("numeric");

                entity.Property(e => e.id).IsRequired();

                entity.Property(e => e.laycan_end).HasColumnType("date");

                entity.Property(e => e.laycan_start).HasColumnType("date");

                entity.Property(e => e.laytime_duration).HasColumnType("numeric");

                entity.Property(e => e.loading_rate).HasColumnType("numeric");

                entity.Property(e => e.order_reference_date).HasColumnType("date");

                entity.Property(e => e.planned_despatch_date).HasColumnType("date");

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.quantity_actual).HasColumnType("numeric");

                entity.Property(e => e.required_quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<despatch_order_product_specifications>(entity =>
            {
                entity.Property(e => e.target).HasColumnType("numeric");

                entity.Property(e => e.value).HasColumnType("numeric");
            });

            modelBuilder.Entity<direct_nplct_details>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<document_type>(entity =>
            {
                entity.HasIndex(e => new { e.organization_id, e.document_type_name })
                    .HasName("uk_document_type")
                    .IsUnique();

                entity.Property(e => e.document_type_name).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.document_type)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<draft_survey>(entity =>
            {
                entity.HasIndex(e => new { e.organization_id, e.survey_number })
                    .HasName("Survey Number")
                    .IsUnique();

                entity.Property(e => e.bill_lading_date).HasColumnType("date");

                entity.Property(e => e.draught_survey_issued).HasColumnType("date");

                entity.Property(e => e.ds_issued_date).HasColumnType("date");

                entity.Property(e => e.lc_amount).HasColumnType("numeric");

                entity.Property(e => e.lc_date).HasColumnType("date");

                entity.Property(e => e.lc_price).HasColumnType("numeric");

                entity.Property(e => e.peb_date).HasColumnType("date");

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.sof_issued_date).HasColumnType("date");

                entity.Property(e => e.survey_date).HasColumnType("date");

                entity.Property(e => e.wlcr).HasColumnType("date");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.draft_survey)
                    .HasForeignKey(d => d.organization_id)
                    .HasConstraintName("draft_survey_organization_id_fkey");
            });

            modelBuilder.Entity<draft_survey_detail>(entity =>
            {
                entity.Property(e => e.bill_lading_date).HasColumnType("date");

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.survey_date).HasColumnType("date");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.draft_survey_detail)
                    .HasForeignKey(d => d.organization_id)
                    .HasConstraintName("draft_survey_detail_organization_id_fkey");
            });

            modelBuilder.Entity<draft_survey_document>(entity =>
            {
                entity.Property(e => e.draft_survey_id).IsRequired();
            });

            modelBuilder.Entity<drill_blast_plan>(entity =>
            {
                entity.Property(e => e.blast_volume).HasColumnType("numeric");

                entity.Property(e => e.end_date).HasColumnType("date");

                entity.Property(e => e.plan_number).IsRequired();

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.request_level).HasColumnType("numeric");

                entity.Property(e => e.start_date).HasColumnType("date");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.drill_blast_plan)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.pit_)
                    .WithMany(p => p.drill_blast_plan)
                    .HasForeignKey(d => d.pit_id)
                    .HasConstraintName("fk_business_area_pit_id");

                entity.HasOne(d => d.uom_)
                    .WithMany(p => p.drill_blast_plan)
                    .HasForeignKey(d => d.uom_id)
                    .HasConstraintName("fk_uom_uom_id");

                entity.HasOne(d => d.vendor_)
                    .WithMany(p => p.drill_blast_plan)
                    .HasForeignKey(d => d.vendor_id)
                    .HasConstraintName("fk_contractor_vendor_id");
            });

            modelBuilder.Entity<eight_week_forecast>(entity =>
            {
                entity.HasIndex(e => new { e.planning_number, e.version, e.year_id })
                    .HasName("uk_eight_week_forecast")
                    .IsUnique();

                entity.Property(e => e.planning_number).IsRequired();

                entity.Property(e => e.total).HasColumnType("numeric");

                entity.HasOne(d => d.business_unit_)
                    .WithMany(p => p.eight_week_forecast)
                    .HasForeignKey(d => d.business_unit_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_business_unit_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.eight_week_forecast)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.uom_)
                    .WithMany(p => p.eight_week_forecast)
                    .HasForeignKey(d => d.uom_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_uom_id");

                entity.HasOne(d => d.year_)
                    .WithMany(p => p.eight_week_forecast)
                    .HasForeignKey(d => d.year_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_year");
            });

            modelBuilder.Entity<eight_week_forecast_item>(entity =>
            {
                entity.HasIndex(e => new { e.header_id, e.activity_plan_id, e.location_id, e.business_area_pit_id, e.product_category_id, e.product_id, e.contractor_id })
                    .HasName("fk_eight_week_forecast_item")
                    .IsUnique();

                entity.Property(e => e.header_id).IsRequired();

                entity.Property(e => e.total).HasColumnType("numeric");

                entity.HasOne(d => d.activity_plan_)
                    .WithMany(p => p.eight_week_forecast_item)
                    .HasForeignKey(d => d.activity_plan_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_activity_plan_id");

                entity.HasOne(d => d.header_)
                    .WithMany(p => p.eight_week_forecast_item)
                    .HasForeignKey(d => d.header_id)
                    .HasConstraintName("fk_eight_week_forecast_item_id");

                entity.HasOne(d => d.location_)
                    .WithMany(p => p.eight_week_forecast_item)
                    .HasForeignKey(d => d.location_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_location_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.eight_week_forecast_item)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<eight_week_forecast_item_detail>(entity =>
            {
                entity.Property(e => e.ash_adb).HasColumnType("numeric");

                entity.Property(e => e.gcv_gad).HasColumnType("numeric");

                entity.Property(e => e.gcv_gar).HasColumnType("numeric");

                entity.Property(e => e.header_id).IsRequired();

                entity.Property(e => e.im_adb).HasColumnType("numeric");

                entity.Property(e => e.item_id).IsRequired();

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.tm_arb).HasColumnType("numeric");

                entity.Property(e => e.ts_adb).HasColumnType("numeric");

                entity.Property(e => e.week_id).IsRequired();

                entity.HasOne(d => d.item_)
                    .WithMany(p => p.eight_week_forecast_item_detail)
                    .HasForeignKey(d => d.item_id)
                    .HasConstraintName("fk_eight_week_forecast_item_detail_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.eight_week_forecast_item_detail)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_id");
            });

            modelBuilder.Entity<email_notification>(entity =>
            {
                entity.Property(e => e.email_content).IsRequired();

                entity.Property(e => e.email_subject).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.email_notification)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<email_recipient>(entity =>
            {
                entity.Property(e => e.email_notification_id).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.email_recipient)
                    .HasForeignKey(d => d.organization_id)
                    .HasConstraintName("email_recipient_organization_id_fkey");
            });

            modelBuilder.Entity<employee>(entity =>
            {
                entity.HasIndex(e => e.employee_name)
                    .HasName("ix_employee_employee_name");

                entity.HasIndex(e => e.employee_number)
                    .HasName("ix_employee_employee_number");

                entity.HasIndex(e => e.is_operator)
                    .HasName("ix_employee_is_operator");

                entity.HasIndex(e => e.is_supervisor)
                    .HasName("ix_employee_is_supervisor");

                entity.HasIndex(e => e.organization_id)
                    .HasName("ix_employee_organization_id");

                entity.HasIndex(e => new { e.organization_id, e.employee_number })
                    .HasName("employee_organization_id_employee_number_key")
                    .IsUnique();

                entity.HasIndex(e => new { e.organization_id, e.employee_number, e.id })
                    .HasName("ix_employee");

                entity.Property(e => e.employee_number).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.employee)
                    .HasForeignKey(d => d.organization_id)
                    .HasConstraintName("employee_organization_id_fkey");
            });

            modelBuilder.Entity<entity>(entity =>
            {
                entity.HasIndex(e => e.entity_id)
                    .HasName("ix_entity_entity_id");

                entity.HasIndex(e => e.is_active)
                    .HasName("ix_entity_is_active")
                    .IncludeProperties(new[] { "owner_id", "organization_id" });

                entity.HasIndex(e => e.organization_id)
                    .HasName("ix_entity_organization_id")
                    .IncludeProperties(new[] { "is_active", "owner_id" });

                entity.HasIndex(e => e.owner_id)
                    .HasName("ix_entity_owner_id")
                    .IncludeProperties(new[] { "is_active", "organization_id" });
            });

            modelBuilder.Entity<equipment>(entity =>
            {
                entity.HasIndex(e => e.capacity_uom_id)
                    .HasName("ix_equipment_capacity_uom_id");

                entity.HasIndex(e => e.equipment_type_id)
                    .HasName("ix_equipment_equipment_type_id");

                entity.HasIndex(e => e.organization_id)
                    .HasName("ix_equipment_organization_id");

                entity.HasIndex(e => e.vendor_id)
                    .HasName("ix_equipment_vendor_id");

                entity.HasIndex(e => new { e.organization_id, e.equipment_code })
                    .HasName("Equipment")
                    .IsUnique();

                entity.HasIndex(e => new { e.id, e.organization_id, e.equipment_code })
                    .HasName("ix_equipment");

                entity.Property(e => e.capacity).HasColumnType("numeric(38,6)");

                entity.Property(e => e.equipment_code).IsRequired();

                entity.Property(e => e.equipment_type_id).IsRequired();

                entity.Property(e => e.plan).HasColumnType("numeric");

                entity.HasOne(d => d.equipment_type_)
                    .WithMany(p => p.equipment)
                    .HasForeignKey(d => d.equipment_type_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("equipment_equipment_type_id_fkey");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.equipment)
                    .HasForeignKey(d => d.organization_id)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<equipment_category>(entity =>
            {
                entity.HasIndex(e => new { e.organization_id, e.equipment_category_name })
                    .HasName("uk_equipment_category")
                    .IsUnique();

                entity.Property(e => e.equipment_category_name).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.equipment_category)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<equipment_cost_rate>(entity =>
            {
                entity.HasIndex(e => e.equipment_id)
                    .HasName("ix_equipment_cost_rate_equipment_id");

                entity.HasIndex(e => e.organization_id)
                    .HasName("ix_equipment_cost_rate_organization_id");

                entity.HasIndex(e => new { e.equipment_id, e.accounting_period_id })
                    .HasName("uk_equipment_cost_rate")
                    .IsUnique();

                entity.Property(e => e.equipment_id).IsRequired();

                entity.Property(e => e.fuel_per_hour).HasColumnType("numeric");

                entity.Property(e => e.hourly_rate).HasColumnType("numeric");

                entity.Property(e => e.monthly_rate).HasColumnType("numeric");

                entity.Property(e => e.trip_rate).HasColumnType("numeric");

                entity.HasOne(d => d.equipment_)
                    .WithMany(p => p.equipment_cost_rate)
                    .HasForeignKey(d => d.equipment_id)
                    .HasConstraintName("fk_equipment_equipment_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.equipment_cost_rate)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<equipment_group>(entity =>
            {
                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.equipment_group)
                    .HasForeignKey(d => d.organization_id)
                    .HasConstraintName("fk_organization_id");
            });

            modelBuilder.Entity<equipment_group_item>(entity =>
            {
                entity.Property(e => e.density).HasColumnType("numeric");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.equipment_group_item)
                    .HasForeignKey(d => d.organization_id)
                    .HasConstraintName("fk_organization_id");
            });

            modelBuilder.Entity<equipment_incident>(entity =>
            {
                entity.Property(e => e.accounting_period_id).IsRequired();

                entity.Property(e => e.equipment_id).IsRequired();

                entity.Property(e => e.event_category_id).IsRequired();

                entity.HasOne(d => d.accounting_period_)
                    .WithMany(p => p.equipment_incident)
                    .HasForeignKey(d => d.accounting_period_id)
                    .HasConstraintName("fk_accounting_period_accounting_period_id");

                entity.HasOne(d => d.equipment_)
                    .WithMany(p => p.equipment_incident)
                    .HasForeignKey(d => d.equipment_id)
                    .HasConstraintName("fk_equipment_equipment_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.equipment_incident)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<equipment_type>(entity =>
            {
                entity.HasIndex(e => new { e.organization_id, e.equipment_type_code })
                    .HasName("EquipmentType")
                    .IsUnique();

                entity.Property(e => e.equipment_type_code).IsRequired();

                entity.Property(e => e.equipment_type_name).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.equipment_type)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<equipment_usage_transaction>(entity =>
            {
                entity.HasIndex(e => e.organization_id)
                    .HasName("ix_equipment_usage_transaction_organization_id");

                entity.Property(e => e.advance_contract_reference_id).IsRequired();

                entity.Property(e => e.equipment_usage_number).IsRequired();

                entity.HasOne(d => d.accounting_period_)
                    .WithMany(p => p.equipment_usage_transaction)
                    .HasForeignKey(d => d.accounting_period_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_accounting_period_id");

                entity.HasOne(d => d.advance_contract_reference_)
                    .WithMany(p => p.equipment_usage_transaction)
                    .HasForeignKey(d => d.advance_contract_reference_id)
                    .HasConstraintName("fk_advance_contract_reference_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.equipment_usage_transaction)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<equipment_usage_transaction_detail>(entity =>
            {
                entity.HasIndex(e => new { e.cn_unit_id, e.event_category_id, e.date })
                    .HasName("uk_EquipmentUsageTransactionDetail")
                    .IsUnique();

                entity.Property(e => e.cn_unit_id).IsRequired();

                entity.Property(e => e.duration).HasColumnType("numeric");

                entity.Property(e => e.equipment_usage_transaction_id).IsRequired();

                entity.Property(e => e.event_category_id).IsRequired();

                entity.HasOne(d => d.equipment_usage_transaction_)
                    .WithMany(p => p.equipment_usage_transaction_detail)
                    .HasForeignKey(d => d.equipment_usage_transaction_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_equipment_usage_transaction_equipment_usage_transaction_deta");

                entity.HasOne(d => d.event_category_)
                    .WithMany(p => p.equipment_usage_transaction_detail)
                    .HasForeignKey(d => d.event_category_id)
                    .HasConstraintName("fk_equipment_usage_transaction_detail_event_category_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.equipment_usage_transaction_detail)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<event_category>(entity =>
            {
                entity.HasIndex(e => new { e.organization_id, e.event_category_code })
                    .HasName("uk2_event_category")
                    .IsUnique();

                entity.HasIndex(e => new { e.organization_id, e.event_category_name })
                    .HasName("uk1_event_category")
                    .IsUnique();

                entity.Property(e => e.event_category_code).IsRequired();

                entity.Property(e => e.event_category_name).IsRequired();

                entity.HasOne(d => d.event_definition_category_)
                    .WithMany(p => p.event_category)
                    .HasForeignKey(d => d.event_definition_category_id)
                    .HasConstraintName("event_category_event_definition_category_id_fkey");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.event_category)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<event_definition_category>(entity =>
            {
                entity.HasIndex(e => e.organization_id)
                    .HasName("ix_event_definition_category_organization_id");

                entity.HasIndex(e => new { e.organization_id, e.event_definition_category_code })
                    .HasName("uk_event_definition_category")
                    .IsUnique();

                entity.Property(e => e.event_definition_category_name).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.event_definition_category)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<explosive_usage_plan>(entity =>
            {
                entity.Property(e => e.date_time).HasColumnType("date");

                entity.Property(e => e.explosive_usage_plan_number).IsRequired();

                entity.Property(e => e.powder_factor).HasColumnType("numeric");

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.HasOne(d => d.explosive_type_)
                    .WithMany(p => p.explosive_usage_plan)
                    .HasForeignKey(d => d.explosive_type_id)
                    .HasConstraintName("fk_master_list_explosive_type_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.explosive_usage_plan)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.uom_)
                    .WithMany(p => p.explosive_usage_plan)
                    .HasForeignKey(d => d.uom_id)
                    .HasConstraintName("fk_uom_uom_id");
            });

            modelBuilder.Entity<explosive_usage_plan_detail>(entity =>
            {
                entity.Property(e => e.accessories).IsRequired();

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.HasOne(d => d.accessoriesNavigation)
                    .WithMany(p => p.explosive_usage_plan_detail)
                    .HasForeignKey(d => d.accessories)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_master_list_accessories");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.explosive_usage_plan_detail)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.uom_)
                    .WithMany(p => p.explosive_usage_plan_detail)
                    .HasForeignKey(d => d.uom_id)
                    .HasConstraintName("fk_uom_uom_id");
            });

            modelBuilder.Entity<exposed_coal>(entity =>
            {
                entity.Property(e => e.mine_location_id).IsRequired();

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.transaction_date).HasColumnType("date");

                entity.HasOne(d => d.mine_location_)
                    .WithMany(p => p.exposed_coal)
                    .HasForeignKey(d => d.mine_location_id)
                    .HasConstraintName("fk_mine_location_mine_location_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.exposed_coal)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<fuel_consumption>(entity =>
            {
                entity.Property(e => e.fuel_coal_ratio).HasColumnType("numeric");

                entity.Property(e => e.fuel_coal_value).HasColumnType("numeric");

                entity.Property(e => e.fuel_consumption_date).HasColumnType("date");

                entity.Property(e => e.fuel_ob_ratio).HasColumnType("numeric");

                entity.Property(e => e.fuel_ob_value).HasColumnType("numeric");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.fuel_consumption)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<hauling_closing>(entity =>
            {
                entity.Property(e => e.distance).HasColumnType("numeric");

                entity.Property(e => e.volume).HasColumnType("numeric");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.hauling_closing)
                    .HasForeignKey(d => d.organization_id)
                    .HasConstraintName("hauling_closing_organization_id_fkey");
            });

            modelBuilder.Entity<hauling_closing_item>(entity =>
            {
                entity.Property(e => e.hauling_closing_id).IsRequired();

                entity.Property(e => e.quantity_item).HasColumnType("numeric");

                entity.Property(e => e.transaction_item_date).HasColumnType("date");

                entity.HasOne(d => d.hauling_closing_)
                    .WithMany(p => p.hauling_closing_item)
                    .HasForeignKey(d => d.hauling_closing_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_hauling_closing_item");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.hauling_closing_item)
                    .HasForeignKey(d => d.organization_id)
                    .HasConstraintName("hauling_closing_organization_id_fkey");
            });

            modelBuilder.Entity<hauling_plan>(entity =>
            {
                entity.HasIndex(e => new { e.organization_id, e.hauling_plan_number })
                    .HasName("uk_hauling_plan")
                    .IsUnique();

                entity.Property(e => e.hauling_plan_number).IsRequired();

                entity.HasOne(d => d.location_)
                    .WithMany(p => p.hauling_planlocation_)
                    .HasForeignKey(d => d.location_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_business_area_location_id");

                entity.HasOne(d => d.master_list_)
                    .WithMany(p => p.hauling_plan)
                    .HasForeignKey(d => d.master_list_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_master_list_master_list_id");

                entity.HasOne(d => d.mine_location_)
                    .WithMany(p => p.hauling_plan)
                    .HasForeignKey(d => d.mine_location_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_mine_location_mine_location_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.hauling_plan)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.pit_)
                    .WithMany(p => p.hauling_planpit_)
                    .HasForeignKey(d => d.pit_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_business_area_pit_id");
            });

            modelBuilder.Entity<hauling_plan_daily>(entity =>
            {
                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.HasOne(d => d.hauling_plan_monthly_)
                    .WithMany(p => p.hauling_plan_daily)
                    .HasForeignKey(d => d.hauling_plan_monthly_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_hauling_plan_monthly_hauling_plan_monthly_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.hauling_plan_daily)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<hauling_plan_history>(entity =>
            {
                entity.HasIndex(e => new { e.organization_id, e.hauling_plan_number })
                    .HasName("uk_hauling_plan_history")
                    .IsUnique();

                entity.Property(e => e.hauling_plan_id).IsRequired();

                entity.Property(e => e.hauling_plan_number).IsRequired();

                entity.HasOne(d => d.hauling_plan_)
                    .WithMany(p => p.hauling_plan_history)
                    .HasForeignKey(d => d.hauling_plan_id)
                    .HasConstraintName("fk_hauling_plan_hauling_plan_id");

                entity.HasOne(d => d.location_)
                    .WithMany(p => p.hauling_plan_historylocation_)
                    .HasForeignKey(d => d.location_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_business_area_location_id");

                entity.HasOne(d => d.master_list_)
                    .WithMany(p => p.hauling_plan_history)
                    .HasForeignKey(d => d.master_list_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_master_list_master_list_id");

                entity.HasOne(d => d.mine_location_)
                    .WithMany(p => p.hauling_plan_history)
                    .HasForeignKey(d => d.mine_location_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_mine_location_mine_location_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.hauling_plan_history)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.pit_)
                    .WithMany(p => p.hauling_plan_historypit_)
                    .HasForeignKey(d => d.pit_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_business_area_pit_id");
            });

            modelBuilder.Entity<hauling_plan_monthly>(entity =>
            {
                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.HasOne(d => d.hauling_plan_)
                    .WithMany(p => p.hauling_plan_monthly)
                    .HasForeignKey(d => d.hauling_plan_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_hauling_plan_hauling_plan_id");

                entity.HasOne(d => d.month_)
                    .WithMany(p => p.hauling_plan_monthly)
                    .HasForeignKey(d => d.month_id)
                    .HasConstraintName("fk_months_month_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.hauling_plan_monthly)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<hauling_plan_monthly_history>(entity =>
            {
                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.HasOne(d => d.hauling_plan_)
                    .WithMany(p => p.hauling_plan_monthly_history)
                    .HasForeignKey(d => d.hauling_plan_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_hauling_plan_hauling_plan_id");

                entity.HasOne(d => d.hauling_plan_monthly_)
                    .WithMany(p => p.hauling_plan_monthly_history)
                    .HasForeignKey(d => d.hauling_plan_monthly_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_hauling_plan_monthly_hauling_plan_monthly_id");

                entity.HasOne(d => d.month_)
                    .WithMany(p => p.hauling_plan_monthly_history)
                    .HasForeignKey(d => d.month_id)
                    .HasConstraintName("fk_months_month_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.hauling_plan_monthly_history)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<hauling_schema>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.qty).HasColumnType("numeric");
            });

            modelBuilder.Entity<hauling_transaction>(entity =>
            {
                entity.HasIndex(e => e.accounting_period_id)
                    .HasName("ix_hauling_transaction_accounting_period_id");

                entity.HasIndex(e => e.process_flow_id)
                    .HasName("ix_hauling_transaction_process_flow_id");

                entity.HasIndex(e => e.product_id)
                    .HasName("ix_hauling_transaction_product_id");

                entity.HasIndex(e => e.survey_id)
                    .HasName("ix_hauling_transaction_survey_id");

                entity.HasIndex(e => e.uom_id)
                    .HasName("ix_hauling_transaction_uom_id");

                entity.HasIndex(e => new { e.organization_id, e.transaction_number })
                    .HasName("uk_hauling_transaction")
                    .IsUnique();

                entity.Property(e => e.destination_location_id).IsRequired();

                entity.Property(e => e.distance).HasColumnType("numeric");

                entity.Property(e => e.gross).HasColumnType("numeric");

                entity.Property(e => e.hour_usage).HasColumnType("numeric");

                entity.Property(e => e.integration_status).HasDefaultValueSql("'NOT APPROVED'::text");

                entity.Property(e => e.loading_quantity).HasColumnType("numeric");

                entity.Property(e => e.netto_rekon).HasColumnType("numeric");

                entity.Property(e => e.source_location_id).IsRequired();

                entity.Property(e => e.tare).HasColumnType("numeric");

                entity.Property(e => e.transaction_number).IsRequired();

                entity.Property(e => e.unloading_quantity).HasColumnType("numeric");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.hauling_transaction)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<haze>(entity =>
            {
                entity.Property(e => e.business_area_id).IsRequired();

                entity.Property(e => e.haze_value).HasColumnType("numeric");

                entity.HasOne(d => d.business_area_)
                    .WithMany(p => p.haze)
                    .HasForeignKey(d => d.business_area_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("haze_business_area_id_fkey");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.haze)
                    .HasForeignKey(d => d.organization_id)
                    .HasConstraintName("haze_organization_id_fkey");
            });

            modelBuilder.Entity<historical_stock_mutation>(entity =>
            {
                entity.Property(e => e.closing).HasColumnType("numeric");

                entity.Property(e => e.closing_before).HasColumnType("numeric");

                entity.Property(e => e.dates).HasColumnType("date");
            });

            modelBuilder.Entity<incident>(entity =>
            {
                entity.HasIndex(e => e.organization_id)
                    .HasName("ix_incident_organization_id");

                entity.Property(e => e.hour_duration).HasColumnType("numeric");

                entity.Property(e => e.incident_category_id).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.incident)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<incident_category>(entity =>
            {
                entity.HasIndex(e => e.organization_id)
                    .HasName("ix_incident_category_organization_id");

                entity.HasIndex(e => new { e.organization_id, e.incident_category_name })
                    .HasName("uk_incident_category")
                    .IsUnique();

                entity.Property(e => e.incident_category_name).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.incident_category)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<initial_information>(entity =>
            {
                entity.Property(e => e.despatch_order_id).IsRequired();

                entity.Property(e => e.eta_plan).HasColumnType("date");

                entity.HasOne(d => d.despatch_order_)
                    .WithMany(p => p.initial_information)
                    .HasForeignKey(d => d.despatch_order_id)
                    .HasConstraintName("fk_despatch_order_despatch_order_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.initial_information)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<integrasi_ickm33>(entity =>
            {
                entity.HasIndex(e => e.filename)
                    .HasName("idx_integrasi_ickm33");
            });

            modelBuilder.Entity<integrasi_icport>(entity =>
            {
                entity.HasIndex(e => e.filename)
                    .HasName("idx_integrasi_icport");
            });

            modelBuilder.Entity<integrasi_kmiashs>(entity =>
            {
                entity.HasIndex(e => e.filename)
                    .HasName("idx_integrasi_kmiashs");
            });

            modelBuilder.Entity<integrasi_timesheet>(entity =>
            {
                entity.HasIndex(e => e.filename)
                    .HasName("idx_integrasi_timesheet");
            });

            modelBuilder.Entity<integrasi_timesheetplan>(entity =>
            {
                entity.HasIndex(e => e.filename)
                    .HasName("idx_integrasi_timesheetplan");
            });

            modelBuilder.Entity<inventory_transaction>(entity =>
            {
                entity.HasIndex(e => e.accounting_period_id)
                    .HasName("ix_inventory_transaction_accounting_period_id");

                entity.HasIndex(e => e.destination_location_id)
                    .HasName("ix_inventory_transaction_destination_location_id");

                entity.HasIndex(e => e.organization_id)
                    .HasName("ix_inventory_transaction_organization_id");

                entity.HasIndex(e => e.product_id)
                    .HasName("ix_inventory_transaction_product_id");

                entity.HasIndex(e => e.source_location_id)
                    .HasName("ix_inventory_transaction_source_location_id");

                entity.HasIndex(e => e.transaction_datetime)
                    .HasName("ix_inventory_transaction_transaction_datetime");

                entity.HasIndex(e => e.uom_id)
                    .HasName("ix_inventory_transaction_uom_id");

                entity.HasIndex(e => new { e.organization_id, e.transaction_number })
                    .HasName("uk_inventory_transaction")
                    .IsUnique();

                entity.Property(e => e.accounting_period_id).IsRequired();

                entity.Property(e => e.hour_usage).HasColumnType("numeric");

                entity.Property(e => e.overriden_quantity).HasColumnType("numeric");

                entity.Property(e => e.product_id).IsRequired();

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.transaction_number).IsRequired();

                entity.Property(e => e.uom_id).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.inventory_transaction)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<inventory_transaction_detail>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.id).IsRequired();

                entity.Property(e => e.inventory_transaction_id).IsRequired();

                entity.Property(e => e.quantity).HasColumnType("numeric(38,8)");

                entity.Property(e => e.transaction_datetime).IsRequired();

                entity.Property(e => e.uom_id).IsRequired();
            });

            modelBuilder.Entity<item_class_view>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<item_group>(entity =>
            {
                entity.Property(e => e.item_group_name).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.item_group)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<item_list>(entity =>
            {
                entity.Property(e => e.item_group_id).IsRequired();

                entity.Property(e => e.item_name).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.item_list)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<joint_survey>(entity =>
            {
                entity.HasIndex(e => e.organization_id)
                    .HasName("ix_join_survey_organization_id");

                entity.HasIndex(e => e.progress_claim_id)
                    .HasName("ix_join_survey_stock_location_id");

                entity.HasIndex(e => new { e.organization_id, e.join_survey_number })
                    .HasName("uk1_joint_survey")
                    .IsUnique();

                entity.Property(e => e.distance).HasColumnType("numeric");

                entity.Property(e => e.distance_carry_over).HasColumnType("numeric");

                entity.Property(e => e.elevation).HasColumnType("numeric");

                entity.Property(e => e.elevation_carry_over).HasColumnType("numeric");

                entity.Property(e => e.end_period_date).HasColumnType("date");

                entity.Property(e => e.join_survey_number).IsRequired();

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.quantity_carry_over).HasColumnType("numeric");

                entity.Property(e => e.start_period_date).HasColumnType("date");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.joint_survey)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.progress_claim_)
                    .WithMany(p => p.joint_survey)
                    .HasForeignKey(d => d.progress_claim_id)
                    .HasConstraintName("fk_progress_claim_progress_claim_id");
            });

            modelBuilder.Entity<joint_survey_analyte>(entity =>
            {
                entity.HasIndex(e => new { e.joint_survey_id, e.analyte_id })
                    .HasName("uk_joint_survey_analyte")
                    .IsUnique();

                entity.Property(e => e.analyte_id).IsRequired();

                entity.Property(e => e.analyte_value).HasColumnType("numeric");

                entity.Property(e => e.joint_survey_id).IsRequired();

                entity.HasOne(d => d.analyte_)
                    .WithMany(p => p.joint_survey_analyte)
                    .HasForeignKey(d => d.analyte_id)
                    .HasConstraintName("fk_analyte_analyte_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.joint_survey_analyte)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<joint_survey_attachment>(entity =>
            {
                entity.Property(e => e.joint_survey_id).IsRequired();
            });

            modelBuilder.Entity<joint_survey_detail>(entity =>
            {
                entity.HasIndex(e => e.organization_id)
                    .HasName("ix_join_survey_organization_id_1");

                entity.HasIndex(e => e.progress_claim_id)
                    .HasName("ix_join_survey_stock_location_id_1");

                entity.Property(e => e.distance).HasColumnType("numeric");

                entity.Property(e => e.distance_carry_over).HasColumnType("numeric");

                entity.Property(e => e.elevation).HasColumnType("numeric");

                entity.Property(e => e.elevation_carry_over).HasColumnType("numeric");

                entity.Property(e => e.end_period_date).HasColumnType("date");

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.quantity_carry_over).HasColumnType("numeric");

                entity.Property(e => e.start_period_date).HasColumnType("date");
            });

            modelBuilder.Entity<land_clearing>(entity =>
            {
                entity.Property(e => e.actual_luasan_area).HasColumnType("numeric");

                entity.Property(e => e.land_clearing_date).HasColumnType("date");

                entity.Property(e => e.total_luasan_area).HasColumnType("numeric");
            });

            modelBuilder.Entity<land_clearing_detail>(entity =>
            {
                entity.Property(e => e.end_time).HasColumnType("time without time zone");

                entity.Property(e => e.land_clearing_date).HasColumnType("date");

                entity.Property(e => e.luasan_area).HasColumnType("numeric");

                entity.Property(e => e.start_time).HasColumnType("time without time zone");

                entity.HasOne(d => d.land_clearing_)
                    .WithMany(p => p.land_clearing_detail)
                    .HasForeignKey(d => d.land_clearing_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("land_clearing_detail_fk_1");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.land_clearing_detail)
                    .HasForeignKey(d => d.organization_id)
                    .HasConstraintName("land_clearing_detail_fk");
            });

            modelBuilder.Entity<list_key>(entity =>
            {
                entity.HasIndex(e => new { e.organization_id, e.key_name })
                    .HasName("uk_list_key")
                    .IsUnique();

                entity.Property(e => e.key_name).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.list_key)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<list_value>(entity =>
            {
                entity.HasIndex(e => new { e.list_key_id, e.key_value })
                    .HasName("uk_list_value")
                    .IsUnique();

                entity.Property(e => e.key_value).IsRequired();

                entity.Property(e => e.list_key_id).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.list_value)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<lq_proportion>(entity =>
            {
                entity.Property(e => e.adjustment).HasColumnType("numeric");

                entity.Property(e => e.presentage).HasColumnType("numeric");

                entity.Property(e => e.quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<master_list>(entity =>
            {
                entity.HasIndex(e => new { e.organization_id, e.item_name, e.item_group })
                    .HasName("master_list_organization_id_idx");

                entity.Property(e => e.item_group).IsRequired();

                entity.Property(e => e.item_name).IsRequired();

                entity.Property(e => e.sort_number).HasColumnType("numeric");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.master_list)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<master_month>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.orders).HasColumnType("numeric");
            });

            modelBuilder.Entity<master_month_historical_rain>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.orders).HasColumnType("numeric");
            });

            modelBuilder.Entity<material_type>(entity =>
            {
                entity.HasIndex(e => new { e.organization_id, e.material_type_name })
                    .HasName("uk_material_type")
                    .IsUnique();

                entity.Property(e => e.material_type_name).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.material_type)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<material_type_analyte>(entity =>
            {
                entity.HasIndex(e => new { e.material_type_id, e.analyte_id })
                    .HasName("uk_material_type_analyte")
                    .IsUnique();

                entity.Property(e => e.analyte_id).IsRequired();

                entity.Property(e => e.material_type_id).IsRequired();

                entity.HasOne(d => d.analyte_)
                    .WithMany(p => p.material_type_analyte)
                    .HasForeignKey(d => d.analyte_id)
                    .HasConstraintName("fk_analyte_analyte_id");

                entity.HasOne(d => d.material_type_)
                    .WithMany(p => p.material_type_analyte)
                    .HasForeignKey(d => d.material_type_id)
                    .HasConstraintName("fk_material_type_material_type_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.material_type_analyte)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<mine_location>(entity =>
            {
                entity.HasIndex(e => e.organization_id)
                    .HasName("ix_mine_location_organization_id");

                entity.HasIndex(e => new { e.organization_id, e.mine_location_code })
                    .HasName("uk2_mine_location")
                    .IsUnique();

                entity.HasIndex(e => new { e.stock_location_name, e.parent_stock_location_id, e.business_area_id })
                    .HasName("uk1_mine_location")
                    .IsUnique();

                entity.Property(e => e.closing_date).HasColumnType("date");

                entity.Property(e => e.current_stock).HasColumnType("numeric");

                entity.Property(e => e.maximum_capacity).HasColumnType("numeric");

                entity.Property(e => e.mine_location_code).IsRequired();

                entity.Property(e => e.minimum_capacity).HasColumnType("numeric");

                entity.Property(e => e.opening_date).HasColumnType("date");

                entity.Property(e => e.proved_reserve).HasColumnType("numeric");

                entity.Property(e => e.target_capacity).HasColumnType("numeric");

                entity.HasOne(d => d.business_area_)
                    .WithMany(p => p.mine_location)
                    .HasForeignKey(d => d.business_area_id)
                    .HasConstraintName("mine_location_business_area_id_fkey");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.mine_location)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<mine_location_quality>(entity =>
            {
                entity.Property(e => e.ash).HasColumnType("numeric");

                entity.Property(e => e.fc).HasColumnType("numeric");

                entity.Property(e => e.gcv_adb).HasColumnType("numeric");

                entity.Property(e => e.gcv_ar).HasColumnType("numeric");

                entity.Property(e => e.hgi).HasColumnType("numeric");

                entity.Property(e => e.im).HasColumnType("numeric");

                entity.Property(e => e.mine_location_id).IsRequired();

                entity.Property(e => e.rd).HasColumnType("numeric");

                entity.Property(e => e.rdi).HasColumnType("numeric");

                entity.Property(e => e.tm).HasColumnType("numeric");

                entity.Property(e => e.ts).HasColumnType("numeric");

                entity.Property(e => e.vm).HasColumnType("numeric");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.mine_location_quality)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<mine_location_quality_pit>(entity =>
            {
                entity.Property(e => e.ash).HasColumnType("numeric");

                entity.Property(e => e.fc).HasColumnType("numeric");

                entity.Property(e => e.gcv_adb).HasColumnType("numeric");

                entity.Property(e => e.gcv_ar).HasColumnType("numeric");

                entity.Property(e => e.im).HasColumnType("numeric");

                entity.Property(e => e.mine_location_id).IsRequired();

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.tm).HasColumnType("numeric");

                entity.Property(e => e.ts).HasColumnType("numeric");

                entity.Property(e => e.vm).HasColumnType("numeric");

                entity.HasOne(d => d.mine_location_)
                    .WithMany(p => p.mine_location_quality_pit)
                    .HasForeignKey(d => d.mine_location_id)
                    .HasConstraintName("fk_mine_location_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.mine_location_quality_pit)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<mine_plan_geology>(entity =>
            {
                entity.Property(e => e.ash_adb).HasColumnType("numeric");

                entity.Property(e => e.cv_adb).HasColumnType("numeric");

                entity.Property(e => e.cv_arb).HasColumnType("numeric");

                entity.Property(e => e.fc_adb).HasColumnType("numeric");

                entity.Property(e => e.hgi).HasColumnType("numeric");

                entity.Property(e => e.im_adb).HasColumnType("numeric");

                entity.Property(e => e.mass_tonnage).HasColumnType("numeric");

                entity.Property(e => e.mine_code).IsRequired();

                entity.Property(e => e.rd).HasColumnType("numeric");

                entity.Property(e => e.rdi).HasColumnType("numeric");

                entity.Property(e => e.tm_ar).HasColumnType("numeric");

                entity.Property(e => e.truethick).HasColumnType("numeric");

                entity.Property(e => e.ts_adb).HasColumnType("numeric");

                entity.Property(e => e.vm_adb).HasColumnType("numeric");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.mine_plan_geology)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<mine_plan_ltp>(entity =>
            {
                entity.Property(e => e.ash_ar).HasColumnType("numeric");

                entity.Property(e => e.coal_tonnage).HasColumnType("numeric");

                entity.Property(e => e.fc_ar).HasColumnType("numeric");

                entity.Property(e => e.gcv_adb_ar).HasColumnType("numeric");

                entity.Property(e => e.gcv_ar_ar).HasColumnType("numeric");

                entity.Property(e => e.hgi_ar).HasColumnType("numeric");

                entity.Property(e => e.im_ar).HasColumnType("numeric");

                entity.Property(e => e.int_truethick).HasColumnType("numeric");

                entity.Property(e => e.rd_ar).HasColumnType("numeric");

                entity.Property(e => e.rdi_ar).HasColumnType("numeric");

                entity.Property(e => e.tm_ar).HasColumnType("numeric");

                entity.Property(e => e.ts_ar).HasColumnType("numeric");

                entity.Property(e => e.vm_ar).HasColumnType("numeric");

                entity.Property(e => e.waste_bcm).HasColumnType("numeric");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.mine_plan_ltp)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<mine_schema>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.qty).HasColumnType("numeric");
            });

            modelBuilder.Entity<mineplan_geology_history>(entity =>
            {
                entity.Property(e => e.ash_adb).HasColumnType("numeric");

                entity.Property(e => e.cv_adb).HasColumnType("numeric");

                entity.Property(e => e.cv_arb).HasColumnType("numeric");

                entity.Property(e => e.fc_adb).HasColumnType("numeric");

                entity.Property(e => e.hgi).HasColumnType("numeric");

                entity.Property(e => e.im_adb).HasColumnType("numeric");

                entity.Property(e => e.mass_tonnage).HasColumnType("numeric");

                entity.Property(e => e.rd).HasColumnType("numeric");

                entity.Property(e => e.rdi).HasColumnType("numeric");

                entity.Property(e => e.tm_ar).HasColumnType("numeric");

                entity.Property(e => e.truethick).HasColumnType("numeric");

                entity.Property(e => e.ts_adb).HasColumnType("numeric");

                entity.Property(e => e.vm_adb).HasColumnType("numeric");

                entity.HasOne(d => d.header_)
                    .WithMany(p => p.mineplan_geology_history)
                    .HasForeignKey(d => d.header_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_mine_plan_geology_mine_plan_geology_id");
            });

            modelBuilder.Entity<mineplan_ltp_history>(entity =>
            {
                entity.Property(e => e.ash_ar).HasColumnType("numeric");

                entity.Property(e => e.coal_tonnage).HasColumnType("numeric");

                entity.Property(e => e.fc_ar).HasColumnType("numeric");

                entity.Property(e => e.gcv_adb_ar).HasColumnType("numeric");

                entity.Property(e => e.gcv_ar_ar).HasColumnType("numeric");

                entity.Property(e => e.hgi_ar).HasColumnType("numeric");

                entity.Property(e => e.im_ar).HasColumnType("numeric");

                entity.Property(e => e.int_truethick).HasColumnType("numeric");

                entity.Property(e => e.rd_ar).HasColumnType("numeric");

                entity.Property(e => e.rdi_ar).HasColumnType("numeric");

                entity.Property(e => e.tm_ar).HasColumnType("numeric");

                entity.Property(e => e.ts_ar).HasColumnType("numeric");

                entity.Property(e => e.vm_ar).HasColumnType("numeric");

                entity.Property(e => e.waste_bcm).HasColumnType("numeric");

                entity.HasOne(d => d.header_)
                    .WithMany(p => p.mineplan_ltp_history)
                    .HasForeignKey(d => d.header_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_mine_plan_ltp_mine_plan_ltp_id");
            });

            modelBuilder.Entity<model_geology>(entity =>
            {
                entity.Property(e => e.ash).HasColumnType("numeric");

                entity.Property(e => e.fc).HasColumnType("numeric");

                entity.Property(e => e.gcv_adb).HasColumnType("numeric");

                entity.Property(e => e.gcv_ar).HasColumnType("numeric");

                entity.Property(e => e.hgi).HasColumnType("numeric");

                entity.Property(e => e.im).HasColumnType("numeric");

                entity.Property(e => e.mine_location_id).IsRequired();

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.rd).HasColumnType("numeric");

                entity.Property(e => e.rdi).HasColumnType("numeric");

                entity.Property(e => e.tm).HasColumnType("numeric");

                entity.Property(e => e.ts).HasColumnType("numeric");

                entity.Property(e => e.vm).HasColumnType("numeric");

                entity.Property(e => e.waste_bcm).HasColumnType("numeric");

                entity.HasOne(d => d.mine_location_)
                    .WithMany(p => p.model_geology)
                    .HasForeignKey(d => d.mine_location_id)
                    .HasConstraintName("fk_mine_location_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.model_geology)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<months>(entity =>
            {
                entity.HasIndex(e => e.id)
                    .HasName("months_id_idx")
                    .IsUnique();

                entity.Property(e => e.id).ValueGeneratedNever();

                entity.Property(e => e.month_name).HasColumnType("character varying");

                entity.Property(e => e.nama_bulan).HasColumnType("character varying");
            });

            modelBuilder.Entity<mutation_product>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.closing).HasColumnType("numeric");

                entity.Property(e => e.mutation).HasColumnType("numeric");

                entity.Property(e => e.opening).HasColumnType("numeric");

                entity.Property(e => e.survey).HasColumnType("numeric");

                entity.Property(e => e.trans_in).HasColumnType("numeric");

                entity.Property(e => e.trans_out).HasColumnType("numeric");
            });

            modelBuilder.Entity<mv_business_area_index>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<mv_mutation>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.analyte_in).HasColumnType("numeric");

                entity.Property(e => e.closing).HasColumnType("numeric");

                entity.Property(e => e.mutation).HasColumnType("numeric");

                entity.Property(e => e.opening).HasColumnType("numeric");

                entity.Property(e => e.survey).HasColumnType("numeric");
            });

            modelBuilder.Entity<mv_stockpile_mutation>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.closing).HasColumnType("numeric");

                entity.Property(e => e.created_on).HasColumnType("date");

                entity.Property(e => e.modified_on).HasColumnType("date");

                entity.Property(e => e.mutation).HasColumnType("numeric");

                entity.Property(e => e.opening).HasColumnType("numeric");

                entity.Property(e => e.survey).HasColumnType("numeric");

                entity.Property(e => e.trans_in).HasColumnType("numeric");

                entity.Property(e => e.trans_out).HasColumnType("numeric");
            });

            modelBuilder.Entity<mv_stockpile_mutation_arutmin>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.closing).HasColumnType("numeric");

                entity.Property(e => e.mutation).HasColumnType("numeric");

                entity.Property(e => e.opening).HasColumnType("numeric");

                entity.Property(e => e.survey).HasColumnType("numeric");

                entity.Property(e => e.trans_in).HasColumnType("numeric");

                entity.Property(e => e.trans_out).HasColumnType("numeric");
            });

            modelBuilder.Entity<mv_stockpile_mutation_daily>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.closing).HasColumnType("numeric");

                entity.Property(e => e.mutation).HasColumnType("numeric");

                entity.Property(e => e.opening).HasColumnType("numeric");

                entity.Property(e => e.survey).HasColumnType("numeric");

                entity.Property(e => e.trans_in).HasColumnType("numeric");

                entity.Property(e => e.trans_out).HasColumnType("numeric");
            });

            modelBuilder.Entity<mv_stockpile_mutation_kmia>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.closing).HasColumnType("numeric");

                entity.Property(e => e.mutation).HasColumnType("numeric");

                entity.Property(e => e.opening).HasColumnType("numeric");

                entity.Property(e => e.survey).HasColumnType("numeric");

                entity.Property(e => e.trans_in).HasColumnType("numeric");

                entity.Property(e => e.trans_out).HasColumnType("numeric");
            });

            modelBuilder.Entity<mv_stockpile_mutation_mstr>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.closing).HasColumnType("numeric");

                entity.Property(e => e.created_on).HasColumnType("date");

                entity.Property(e => e.modified_on).HasColumnType("date");

                entity.Property(e => e.mutation).HasColumnType("numeric");

                entity.Property(e => e.opening).HasColumnType("numeric");

                entity.Property(e => e.survey).HasColumnType("numeric");

                entity.Property(e => e.trans_in).HasColumnType("numeric");

                entity.Property(e => e.trans_out).HasColumnType("numeric");
            });

            modelBuilder.Entity<mv_stockpile_mutation_quality>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.analyte_closing).HasColumnType("numeric");

                entity.Property(e => e.analyte_in).HasColumnType("numeric");

                entity.Property(e => e.analyte_opening).HasColumnType("numeric");

                entity.Property(e => e.closing).HasColumnType("numeric");

                entity.Property(e => e.mutation).HasColumnType("numeric");

                entity.Property(e => e.opening).HasColumnType("numeric");

                entity.Property(e => e.survey).HasColumnType("numeric");
            });

            modelBuilder.Entity<mv_stockpile_mutation_quality_kmia>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.analyte_closing).HasColumnType("numeric");

                entity.Property(e => e.analyte_in).HasColumnType("numeric");

                entity.Property(e => e.analyte_opening).HasColumnType("numeric");

                entity.Property(e => e.closing).HasColumnType("numeric");

                entity.Property(e => e.mutation).HasColumnType("numeric");

                entity.Property(e => e.opening).HasColumnType("numeric");

                entity.Property(e => e.survey).HasColumnType("numeric");
            });

            modelBuilder.Entity<mv_stockpile_mutation_quality_mstr>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.analyte_closing).HasColumnType("numeric");

                entity.Property(e => e.analyte_in).HasColumnType("numeric");

                entity.Property(e => e.analyte_opening).HasColumnType("numeric");

                entity.Property(e => e.closing).HasColumnType("numeric");

                entity.Property(e => e.mutation).HasColumnType("numeric");

                entity.Property(e => e.opening).HasColumnType("numeric");

                entity.Property(e => e.survey).HasColumnType("numeric");
            });

            modelBuilder.Entity<national_holiday>(entity =>
            {
                entity.Property(e => e.calendar_id).IsRequired();

                entity.Property(e => e.date).HasColumnType("date");

                entity.HasOne(d => d.calendar_)
                    .WithMany(p => p.national_holiday)
                    .HasForeignKey(d => d.calendar_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_calendar_calendar_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.national_holiday)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<nplct_stock_raw_data>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.closing).HasColumnType("numeric");

                entity.Property(e => e.mutation).HasColumnType("numeric");

                entity.Property(e => e.opening).HasColumnType("numeric");

                entity.Property(e => e.survey).HasColumnType("numeric");

                entity.Property(e => e.trans_in).HasColumnType("numeric");

                entity.Property(e => e.trans_out).HasColumnType("numeric");
            });

            modelBuilder.Entity<organization>(entity =>
            {
                entity.HasIndex(e => e.created_by)
                    .HasName("ix_organization_created_by")
                    .IncludeProperties(new[] { "created_on" });

                entity.HasIndex(e => e.entity_id)
                    .HasName("ix_organization_entity_id");

                entity.HasIndex(e => e.is_active)
                    .HasName("ix_organization_is_active")
                    .IncludeProperties(new[] { "owner_id" });

                entity.HasIndex(e => e.modified_by)
                    .HasName("ix_organization_modified_by")
                    .IncludeProperties(new[] { "modified_on" });

                entity.HasIndex(e => e.organization_name)
                    .HasName("uk_organization")
                    .IsUnique();

                entity.HasIndex(e => e.owner_id)
                    .HasName("ix_organization_owner_id")
                    .IncludeProperties(new[] { "is_active" });

                entity.HasIndex(e => e.parent_organization_id)
                    .HasName("ix_organization_parent_organization_id")
                    .IncludeProperties(new[] { "is_active", "owner_id" });

                entity.Property(e => e.organization_id).IsRequired();

                entity.Property(e => e.organization_name).IsRequired();

                entity.Property(e => e.owner_id).IsRequired();
            });

            modelBuilder.Entity<parent_despatch_order>(entity =>
            {
                entity.Property(e => e.down_payment_amount).HasColumnType("numeric");
            });

            modelBuilder.Entity<port_location>(entity =>
            {
                entity.HasIndex(e => e.organization_id)
                    .HasName("ix_port_location_organization_id");

                entity.HasIndex(e => new { e.organization_id, e.port_location_code })
                    .HasName("uk2_port_location")
                    .IsUnique();

                entity.HasIndex(e => new { e.business_area_id, e.parent_stock_location_id, e.stock_location_name })
                    .HasName("uk1_port_location")
                    .IsUnique();

                entity.Property(e => e.closing_date).HasColumnType("date");

                entity.Property(e => e.current_stock).HasColumnType("numeric");

                entity.Property(e => e.maximum_capacity).HasColumnType("numeric");

                entity.Property(e => e.minimum_capacity).HasColumnType("numeric");

                entity.Property(e => e.opening_date).HasColumnType("date");

                entity.Property(e => e.port_location_code).IsRequired();

                entity.Property(e => e.target_capacity).HasColumnType("numeric");

                entity.HasOne(d => d.business_area_)
                    .WithMany(p => p.port_location)
                    .HasForeignKey(d => d.business_area_id)
                    .HasConstraintName("port_location_business_area_id_fkey");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.port_location)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<price_adjustment_product_specification>(entity =>
            {
                entity.Property(e => e.adjust_value).HasColumnType("numeric");

                entity.Property(e => e.fine_value).HasColumnType("numeric");

                entity.Property(e => e.maximum).HasColumnType("numeric");

                entity.Property(e => e.minimum).HasColumnType("numeric");

                entity.Property(e => e.price_adjustment_id).IsRequired();

                entity.Property(e => e.target).HasColumnType("numeric");

                entity.Property(e => e.value).HasColumnType("numeric");

                entity.HasOne(d => d.analyte_)
                    .WithMany(p => p.price_adjustment_product_specification)
                    .HasForeignKey(d => d.analyte_id)
                    .HasConstraintName("fk_analyte_analyte_id");

                entity.HasOne(d => d.analyte_standard_)
                    .WithMany(p => p.price_adjustment_product_specification)
                    .HasForeignKey(d => d.analyte_standard_id)
                    .HasConstraintName("fk_master_list_analyte_standard_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.price_adjustment_product_specification)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.price_adjustment_)
                    .WithMany(p => p.price_adjustment_product_specification)
                    .HasForeignKey(d => d.price_adjustment_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_price_adjustment_id");

                entity.HasOne(d => d.uom_)
                    .WithMany(p => p.price_adjustment_product_specification)
                    .HasForeignKey(d => d.uom_id)
                    .HasConstraintName("fk_uom_id");
            });

            modelBuilder.Entity<price_analyst_arm>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.cv_gar).HasColumnType("numeric");

                entity.Property(e => e.fob_price).HasColumnType("numeric");

                entity.Property(e => e.hpb_price).HasColumnType("numeric");

                entity.Property(e => e.qty).HasColumnType("numeric");
            });

            modelBuilder.Entity<price_analyst_invoice_arm>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.cv_gar).HasColumnType("numeric");

                entity.Property(e => e.fob_price).HasColumnType("numeric");

                entity.Property(e => e.hpb_price).HasColumnType("numeric");

                entity.Property(e => e.qty).HasColumnType("numeric");
            });

            modelBuilder.Entity<price_analyst_uninvoice_arm>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.cv_gar).HasColumnType("numeric");

                entity.Property(e => e.fob_price).HasColumnType("numeric");

                entity.Property(e => e.hpb_price).HasColumnType("numeric");

                entity.Property(e => e.qty).HasColumnType("numeric");
            });

            modelBuilder.Entity<price_index>(entity =>
            {
                entity.HasIndex(e => new { e.organization_id, e.price_index_code })
                    .HasName("uk_price_index")
                    .IsUnique();

                entity.Property(e => e.price_index_name).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.price_index)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<price_index_history>(entity =>
            {
                entity.HasIndex(e => new { e.price_index_id, e.index_date, e.is_forecast })
                    .HasName("uk_price_index_history")
                    .IsUnique();

                entity.Property(e => e.index_date).HasColumnType("date");

                entity.Property(e => e.index_value).HasColumnType("numeric");

                entity.Property(e => e.price_index_id).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.price_index_history)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.price_index_)
                    .WithMany(p => p.price_index_history)
                    .HasForeignKey(d => d.price_index_id)
                    .HasConstraintName("fk_price_index_price_index_id");
            });

            modelBuilder.Entity<price_index_map>(entity =>
            {
                entity.HasIndex(e => new { e.organization_id, e.name })
                    .HasName("uk_price_index_map")
                    .IsUnique();

                entity.Property(e => e.name).IsRequired();

                entity.Property(e => e.price_index_id).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.price_index_map)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.price_index_)
                    .WithMany(p => p.price_index_map)
                    .HasForeignKey(d => d.price_index_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_price_index_price_index_map");
            });

            modelBuilder.Entity<price_index_map_detail>(entity =>
            {
                entity.Property(e => e.end_range).HasColumnType("numeric");

                entity.Property(e => e.price_index_map_id).IsRequired();

                entity.Property(e => e.start_range).HasColumnType("numeric");

                entity.Property(e => e.value).HasColumnType("numeric");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.price_index_map_detail)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.price_index_map_)
                    .WithMany(p => p.price_index_map_detail)
                    .HasForeignKey(d => d.price_index_map_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_price_index_map_price_index_map_detail");
            });

            modelBuilder.Entity<price_index_product_specification>(entity =>
            {
                entity.Property(e => e.adjust_value).HasColumnType("numeric");

                entity.Property(e => e.fine_value).HasColumnType("numeric");

                entity.Property(e => e.maximum).HasColumnType("numeric");

                entity.Property(e => e.minimum).HasColumnType("numeric");

                entity.Property(e => e.price_index_id).IsRequired();

                entity.Property(e => e.target).HasColumnType("numeric");

                entity.Property(e => e.value).HasColumnType("numeric");

                entity.HasOne(d => d.analyte_)
                    .WithMany(p => p.price_index_product_specification)
                    .HasForeignKey(d => d.analyte_id)
                    .HasConstraintName("fk_analyte_analyte_id");

                entity.HasOne(d => d.analyte_standard_)
                    .WithMany(p => p.price_index_product_specification)
                    .HasForeignKey(d => d.analyte_standard_id)
                    .HasConstraintName("fk_master_list_analyte_standard_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.price_index_product_specification)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.price_index_)
                    .WithMany(p => p.price_index_product_specification)
                    .HasForeignKey(d => d.price_index_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_price_index_id");

                entity.HasOne(d => d.uom_)
                    .WithMany(p => p.price_index_product_specification)
                    .HasForeignKey(d => d.uom_id)
                    .HasConstraintName("fk_uom_id");
            });

            modelBuilder.Entity<price_series>(entity =>
            {
                entity.HasIndex(e => new { e.organization_id, e.price_series_name })
                    .HasName("uk_price_series")
                    .IsUnique();

                entity.Property(e => e.end_date).HasColumnType("date");

                entity.Property(e => e.price_series_name).IsRequired();

                entity.Property(e => e.start_date).HasColumnType("date");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.price_series)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<price_series_detail>(entity =>
            {
                entity.Property(e => e.date).HasColumnType("date");

                entity.Property(e => e.price_series_id).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.price_series_detail)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<process_flow>(entity =>
            {
                entity.HasIndex(e => e.organization_id)
                    .HasName("ix_process_flow_organization_id");

                entity.HasIndex(e => new { e.organization_id, e.process_flow_code })
                    .HasName("uk2_process_flow")
                    .IsUnique();

                entity.HasIndex(e => new { e.organization_id, e.process_flow_name })
                    .HasName("uk1_process_flow")
                    .IsUnique();

                entity.Property(e => e.assume_source_quality).HasDefaultValueSql("true");

                entity.Property(e => e.process_flow_code).IsRequired();

                entity.Property(e => e.process_flow_name).IsRequired();

                entity.HasOne(d => d.destination_location_)
                    .WithMany(p => p.process_flowdestination_location_)
                    .HasForeignKey(d => d.destination_location_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_business_area_destination_location_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.process_flow)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.source_location_)
                    .WithMany(p => p.process_flowsource_location_)
                    .HasForeignKey(d => d.source_location_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_business_area_source_location_id");
            });

            modelBuilder.Entity<process_flow_analyte>(entity =>
            {
                entity.HasIndex(e => e.organization_id)
                    .HasName("ix_process_flow_analyte_organization_id");

                entity.HasIndex(e => new { e.process_flow_id, e.analyte_id })
                    .HasName("uk_process_flow_analyte")
                    .IsUnique();

                entity.Property(e => e.analyte_id).IsRequired();

                entity.Property(e => e.process_flow_id).IsRequired();

                entity.Property(e => e.uom_id).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.process_flow_analyte)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<processing_category>(entity =>
            {
                entity.HasIndex(e => new { e.organization_id, e.processing_category_code })
                    .HasName("uk2_processing_category")
                    .IsUnique();

                entity.HasIndex(e => new { e.organization_id, e.processing_category_name })
                    .HasName("uk1_processing_category")
                    .IsUnique();

                entity.Property(e => e.processing_category_code).IsRequired();

                entity.Property(e => e.processing_category_name).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.processing_category)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<processing_closing>(entity =>
            {
                entity.Property(e => e.distance).HasColumnType("numeric");

                entity.Property(e => e.volume).HasColumnType("numeric");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.processing_closing)
                    .HasForeignKey(d => d.organization_id)
                    .HasConstraintName("processing_closing_organization_id_fkey");
            });

            modelBuilder.Entity<processing_closing_item>(entity =>
            {
                entity.Property(e => e.processing_closing_id).IsRequired();

                entity.Property(e => e.quantity_item).HasColumnType("numeric");

                entity.Property(e => e.transaction_item_date).HasColumnType("date");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.processing_closing_item)
                    .HasForeignKey(d => d.organization_id)
                    .HasConstraintName("processing_closing_organization_id_fkey");

                entity.HasOne(d => d.processing_closing_)
                    .WithMany(p => p.processing_closing_item)
                    .HasForeignKey(d => d.processing_closing_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_processing_closing_item");
            });

            modelBuilder.Entity<processing_transaction>(entity =>
            {
                entity.HasIndex(e => e.accounting_period_id)
                    .HasName("ix_processing_transaction_accounting_period_id");

                entity.HasIndex(e => e.destination_product_id)
                    .HasName("ix_processing_transaction_destination_product_id");

                entity.HasIndex(e => e.destination_uom_id)
                    .HasName("ix_processing_transaction_destination_uom_id");

                entity.HasIndex(e => e.process_flow_id)
                    .HasName("ix_processing_transaction_process_flow_id");

                entity.HasIndex(e => e.source_product_id)
                    .HasName("ix_processing_transaction_source_product_id");

                entity.HasIndex(e => e.source_uom_id)
                    .HasName("ix_processing_transaction_source_uom_id");

                entity.HasIndex(e => e.survey_id)
                    .HasName("ix_processing_transaction_survey_id");

                entity.HasIndex(e => new { e.organization_id, e.transaction_number })
                    .HasName("uk_processing_transaction")
                    .IsUnique();

                entity.Property(e => e.destination_location_id).IsRequired();

                entity.Property(e => e.hour_usage).HasColumnType("numeric");

                entity.Property(e => e.loading_quantity).HasColumnType("numeric");

                entity.Property(e => e.process_flow_id).IsRequired();

                entity.Property(e => e.transaction_number).IsRequired();

                entity.Property(e => e.unloading_quantity).HasColumnType("numeric");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.processing_transaction)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<produce_schema>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.qty).HasColumnType("numeric");
            });

            modelBuilder.Entity<product>(entity =>
            {
                entity.HasIndex(e => new { e.organization_id, e.product_code })
                    .HasName("uk2_product")
                    .IsUnique();

                entity.HasIndex(e => new { e.organization_id, e.product_name })
                    .HasName("uk1_product")
                    .IsUnique();

                entity.Property(e => e.density).HasColumnType("numeric");

                entity.Property(e => e.product_category_id).IsRequired();

                entity.Property(e => e.product_code).IsRequired();

                entity.Property(e => e.product_name).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.product)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.product_category_)
                    .WithMany(p => p.product)
                    .HasForeignKey(d => d.product_category_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("product_product_category_id_fkey");
            });

            modelBuilder.Entity<product_category>(entity =>
            {
                entity.HasIndex(e => new { e.organization_id, e.product_category_code })
                    .HasName("uk2_product_category")
                    .IsUnique();

                entity.HasIndex(e => new { e.organization_id, e.product_category_name })
                    .HasName("uk1_product_category")
                    .IsUnique();

                entity.Property(e => e.product_category_code).IsRequired();

                entity.Property(e => e.product_category_name).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.product_category)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<product_specification>(entity =>
            {
                entity.HasIndex(e => new { e.product_id, e.analyte_id, e.applicable_date })
                    .HasName("uk_product_specification")
                    .IsUnique();

                entity.Property(e => e.analyte_id).IsRequired();

                entity.Property(e => e.applicable_date).HasColumnType("date");

                entity.Property(e => e.maximum_value).HasColumnType("numeric(18,6)");

                entity.Property(e => e.minimum_value).HasColumnType("numeric(18,6)");

                entity.Property(e => e.product_id).IsRequired();

                entity.Property(e => e.target_value).HasColumnType("numeric(18,6)");

                entity.Property(e => e.uom_id).IsRequired();

                entity.HasOne(d => d.analyte_)
                    .WithMany(p => p.product_specification)
                    .HasForeignKey(d => d.analyte_id)
                    .HasConstraintName("fk_analyte_analyte_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.product_specification)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.product_)
                    .WithMany(p => p.product_specification)
                    .HasForeignKey(d => d.product_id)
                    .HasConstraintName("fk_product_product_id");

                entity.HasOne(d => d.uom_)
                    .WithMany(p => p.product_specification)
                    .HasForeignKey(d => d.uom_id)
                    .HasConstraintName("fk_uom_uom_id");
            });

            modelBuilder.Entity<product_standard>(entity =>
            {
                entity.Property(e => e.header_id).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.product_standard)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<production_closing>(entity =>
            {
                entity.Property(e => e.distance).HasColumnType("numeric");

                entity.Property(e => e.volume).HasColumnType("numeric");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.production_closing)
                    .HasForeignKey(d => d.organization_id)
                    .HasConstraintName("production_closing_organization_id_fkey");
            });

            modelBuilder.Entity<production_closing_item>(entity =>
            {
                entity.Property(e => e.production_closing_id).IsRequired();

                entity.Property(e => e.quantity_item).HasColumnType("numeric");

                entity.Property(e => e.transaction_item_date).HasColumnType("date");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.production_closing_item)
                    .HasForeignKey(d => d.organization_id)
                    .HasConstraintName("production_closing_organization_id_fkey");

                entity.HasOne(d => d.production_closing_)
                    .WithMany(p => p.production_closing_item)
                    .HasForeignKey(d => d.production_closing_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_production_closing_item");
            });

            modelBuilder.Entity<production_plan>(entity =>
            {
                entity.HasIndex(e => new { e.organization_id, e.production_plan_number })
                    .HasName("uk_production_plan")
                    .IsUnique();

                entity.Property(e => e.production_plan_number).IsRequired();

                entity.HasOne(d => d.location_)
                    .WithMany(p => p.production_planlocation_)
                    .HasForeignKey(d => d.location_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_business_area_location_id");

                entity.HasOne(d => d.master_list_)
                    .WithMany(p => p.production_plan)
                    .HasForeignKey(d => d.master_list_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_master_list_master_list_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.production_plan)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.pit_)
                    .WithMany(p => p.production_planpit_)
                    .HasForeignKey(d => d.pit_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_business_area_pit_id");
            });

            modelBuilder.Entity<production_plan_daily>(entity =>
            {
                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.production_plan_daily)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.production_plan_monthly_)
                    .WithMany(p => p.production_plan_daily)
                    .HasForeignKey(d => d.production_plan_monthly_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_production_plan_monthly_production_plan_monthly_id");
            });

            modelBuilder.Entity<production_plan_history>(entity =>
            {
                entity.Property(e => e.previous_quantity).HasColumnType("numeric");

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.production_plan_history)
                    .HasForeignKey(d => d.organization_id)
                    .HasConstraintName("production_plan_history_organization_id_fkey");
            });

            modelBuilder.Entity<production_plan_monthly>(entity =>
            {
                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.HasOne(d => d.month_)
                    .WithMany(p => p.production_plan_monthly)
                    .HasForeignKey(d => d.month_id)
                    .HasConstraintName("fk_months_month_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.production_plan_monthly)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.production_plan_)
                    .WithMany(p => p.production_plan_monthly)
                    .HasForeignKey(d => d.production_plan_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_production_plan_production_plan_id");
            });

            modelBuilder.Entity<production_plan_monthly_history>(entity =>
            {
                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.HasOne(d => d.month_)
                    .WithMany(p => p.production_plan_monthly_history)
                    .HasForeignKey(d => d.month_id)
                    .HasConstraintName("fk_months_month_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.production_plan_monthly_history)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.production_plan_)
                    .WithMany(p => p.production_plan_monthly_history)
                    .HasForeignKey(d => d.production_plan_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_production_plan_production_plan_id");

                entity.HasOne(d => d.production_plan_monthly_)
                    .WithMany(p => p.production_plan_monthly_history)
                    .HasForeignKey(d => d.production_plan_monthly_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_production_plan_monthly_production_plan_monthly_id");
            });

            modelBuilder.Entity<production_plan_rawdata>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.daily_qty).HasColumnType("numeric");

                entity.Property(e => e.monthly_qty).HasColumnType("numeric");
            });

            modelBuilder.Entity<production_plan_schema>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.dates).HasColumnType("date");

                entity.Property(e => e.quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<production_transaction>(entity =>
            {
                entity.HasIndex(e => e.accounting_period_id)
                    .HasName("ix_production_transaction_accounting_period_id");

                entity.HasIndex(e => e.process_flow_id)
                    .HasName("ix_production_transaction_process_flow_id");

                entity.HasIndex(e => e.product_id)
                    .HasName("ix_production_transaction_product_id");

                entity.HasIndex(e => e.survey_id)
                    .HasName("ix_production_transaction_survey_id");

                entity.HasIndex(e => e.uom_id)
                    .HasName("ix_production_transaction_uom_id");

                entity.Property(e => e.density).HasColumnType("numeric");

                entity.Property(e => e.destination_location_id).IsRequired();

                entity.Property(e => e.distance).HasColumnType("numeric");

                entity.Property(e => e.duration).HasColumnType("time without time zone");

                entity.Property(e => e.elevation).HasColumnType("numeric");

                entity.Property(e => e.gross).HasColumnType("numeric");

                entity.Property(e => e.hour_usage).HasColumnType("numeric");

                entity.Property(e => e.integration_status).HasDefaultValueSql("'NOT APPROVED'::text");

                entity.Property(e => e.loading_quantity).HasColumnType("numeric");

                entity.Property(e => e.netto_rekon).HasColumnType("numeric");

                entity.Property(e => e.process_flow_id).IsRequired();

                entity.Property(e => e.ritase).HasColumnType("numeric");

                entity.Property(e => e.source_location_id).IsRequired();

                entity.Property(e => e.tare).HasColumnType("numeric");

                entity.Property(e => e.transaction_number).IsRequired();

                entity.Property(e => e.unloading_quantity).HasColumnType("numeric");

                entity.Property(e => e.volume).HasColumnType("numeric");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.production_transaction)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<production_transaction_item>(entity =>
            {
                entity.Property(e => e.jam01).HasColumnType("numeric");

                entity.Property(e => e.jam02).HasColumnType("numeric");

                entity.Property(e => e.jam03).HasColumnType("numeric");

                entity.Property(e => e.jam04).HasColumnType("numeric");

                entity.Property(e => e.jam05).HasColumnType("numeric");

                entity.Property(e => e.jam06).HasColumnType("numeric");

                entity.Property(e => e.jam07).HasColumnType("numeric");

                entity.Property(e => e.jam08).HasColumnType("numeric");

                entity.Property(e => e.jam09).HasColumnType("numeric");

                entity.Property(e => e.jam10).HasColumnType("numeric");

                entity.Property(e => e.jam11).HasColumnType("numeric");

                entity.Property(e => e.jam12).HasColumnType("numeric");

                entity.Property(e => e.production_transaction_id).IsRequired();

                entity.Property(e => e.ritase).HasColumnType("numeric");

                entity.Property(e => e.truck_factor).HasColumnType("numeric");

                entity.Property(e => e.truck_id).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.production_transaction_item)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<progress_claim>(entity =>
            {
                entity.Property(e => e.actual_quantity).HasColumnType("numeric");

                entity.Property(e => e.advance_contract_id).IsRequired();

                entity.Property(e => e.base_fuel_price).HasColumnType("numeric");

                entity.Property(e => e.base_overdistance_price).HasColumnType("numeric");

                entity.Property(e => e.base_unit_price).HasColumnType("numeric");

                entity.Property(e => e.end_date).HasColumnType("date");

                entity.Property(e => e.start_date).HasColumnType("date");

                entity.Property(e => e.target_quantity).HasColumnType("numeric");

                entity.HasOne(d => d.advance_contract_)
                    .WithMany(p => p.progress_claim)
                    .HasForeignKey(d => d.advance_contract_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_advance_contract_advance_contract_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.progress_claim)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<progress_claim_charge>(entity =>
            {
                entity.Property(e => e.charge_amount).HasColumnType("numeric");

                entity.Property(e => e.charge_currency_id).IsRequired();

                entity.Property(e => e.charge_name).IsRequired();

                entity.Property(e => e.progress_claim_id).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.progress_claim_charge)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<province>(entity =>
            {
                entity.HasIndex(e => new { e.country_id, e.province_name })
                    .HasName("uk_province")
                    .IsUnique();

                entity.Property(e => e.country_id).IsRequired();

                entity.Property(e => e.province_name).IsRequired();

                entity.HasOne(d => d.country_)
                    .WithMany(p => p.province)
                    .HasForeignKey(d => d.country_id)
                    .HasConstraintName("fk_country_country_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.province)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<quality_sampling>(entity =>
            {
                entity.Property(e => e.sampling_number).IsRequired();

                entity.Property(e => e.sampling_template_id).IsRequired();

                entity.Property(e => e.stock_location_id).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.quality_sampling)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<quality_sampling_analyte>(entity =>
            {
                entity.HasIndex(e => new { e.quality_sampling_id, e.analyte_id })
                    .HasName("uk_quality_sampling_analyte")
                    .IsUnique();

                entity.Property(e => e.analyte_id).IsRequired();

                entity.Property(e => e.analyte_value).HasColumnType("numeric");

                entity.Property(e => e.quality_sampling_id).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.quality_sampling_analyte)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.quality_sampling_)
                    .WithMany(p => p.quality_sampling_analyte)
                    .HasForeignKey(d => d.quality_sampling_id)
                    .HasConstraintName("quality_sampling_analyte_quality_sampling_id_fkey");
            });

            modelBuilder.Entity<quality_sampling_approval>(entity =>
            {
                entity.Property(e => e.quality_sampling_id).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.quality_sampling_approval)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<quality_sampling_document>(entity =>
            {
                entity.Property(e => e.quality_sampling_id).IsRequired();

                entity.HasOne(d => d.quality_sampling_)
                    .WithMany(p => p.quality_sampling_document)
                    .HasForeignKey(d => d.quality_sampling_id)
                    .HasConstraintName("quality_sampling_document_quality_sampling_id_fkey");
            });

            modelBuilder.Entity<railing_transaction>(entity =>
            {
                entity.HasIndex(e => e.accounting_period_id)
                    .HasName("ix_railing_transaction_accounting_period_id");

                entity.HasIndex(e => e.process_flow_id)
                    .HasName("ix_railing_transaction_process_flow_id");

                entity.HasIndex(e => e.product_id)
                    .HasName("ix_railing_transaction_product_id");

                entity.HasIndex(e => e.survey_id)
                    .HasName("ix_railing_transaction_survey_id");

                entity.HasIndex(e => e.uom_id)
                    .HasName("ix_railing_transaction_uom_id");

                entity.HasIndex(e => new { e.organization_id, e.transaction_number })
                    .HasName("uk_railing_transaction")
                    .IsUnique();

                entity.Property(e => e.hour_usage).HasColumnType("numeric");

                entity.Property(e => e.loading_quantity).HasColumnType("numeric");

                entity.Property(e => e.product_id).IsRequired();

                entity.Property(e => e.source_location_id).IsRequired();

                entity.Property(e => e.transaction_number).IsRequired();

                entity.Property(e => e.unloading_quantity).HasColumnType("numeric");

                entity.Property(e => e.uom_id).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.railing_transaction)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<railing_transaction_detail>(entity =>
            {
                entity.HasIndex(e => new { e.railing_transaction_id, e.wagon_id })
                    .HasName("uk_railing_transaction_detail")
                    .IsUnique();

                entity.Property(e => e.loading_quantity).HasColumnType("numeric");

                entity.Property(e => e.railing_transaction_id).IsRequired();

                entity.Property(e => e.unloading_quantity).HasColumnType("numeric");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.railing_transaction_detail)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.railing_transaction_)
                    .WithMany(p => p.railing_transaction_detail)
                    .HasForeignKey(d => d.railing_transaction_id)
                    .HasConstraintName("fk_railing_transaction_railing_transaction_id");
            });

            modelBuilder.Entity<raindelay_graph_aims>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.dates).HasColumnType("date");

                entity.Property(e => e.duration).HasColumnType("numeric");
            });

            modelBuilder.Entity<raindelay_graph_ecocoal_aims>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.avg_budget).HasColumnType("numeric");

                entity.Property(e => e.avg_eco).HasColumnType("numeric");

                entity.Property(e => e.avg_forecast).HasColumnType("numeric");

                entity.Property(e => e.avg_val).HasColumnType("numeric");

                entity.Property(e => e.dates).HasColumnType("date");

                entity.Property(e => e.val).HasColumnType("numeric");
            });

            modelBuilder.Entity<raindelay_graph_ecoob_aims>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.avg_budget).HasColumnType("numeric");

                entity.Property(e => e.avg_eco).HasColumnType("numeric");

                entity.Property(e => e.avg_forecast).HasColumnType("numeric");

                entity.Property(e => e.avg_val).HasColumnType("numeric");

                entity.Property(e => e.dates).HasColumnType("date");

                entity.Property(e => e.val).HasColumnType("numeric");
            });

            modelBuilder.Entity<raindelay_graph_ob_aims>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.dates).HasColumnType("date");

                entity.Property(e => e.val).HasColumnType("numeric");
            });

            modelBuilder.Entity<rainfall>(entity =>
            {
                entity.Property(e => e.business_area_id).IsRequired();

                entity.Property(e => e.duration).HasColumnType("numeric");

                entity.Property(e => e.frequency).HasColumnType("numeric");

                entity.Property(e => e.rainfall_delay).HasColumnType("numeric");

                entity.Property(e => e.rainfall_value).HasColumnType("numeric");

                entity.HasOne(d => d.business_area_)
                    .WithMany(p => p.rainfall)
                    .HasForeignKey(d => d.business_area_id)
                    .HasConstraintName("fk_business_area_business_area_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.rainfall)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<rainfall_graph_aims>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.dates).HasColumnType("date");

                entity.Property(e => e.rainfall_value).HasColumnType("numeric");
            });

            modelBuilder.Entity<rainfall_historical>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.avg_10_year).HasColumnType("numeric");

                entity.Property(e => e.avg_5_year).HasColumnType("numeric");

                entity.Property(e => e.duration).HasColumnType("numeric");

                entity.Property(e => e.rainfall_value).HasColumnType("numeric");
            });

            modelBuilder.Entity<rainfall_landingpage>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<rainfall_schema>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.dates).HasColumnType("date");

                entity.Property(e => e.duration).HasColumnType("numeric");

                entity.Property(e => e.rainfall_value).HasColumnType("numeric");
            });

            modelBuilder.Entity<raw_data_soa_export>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.ash_adb).HasColumnType("numeric");

                entity.Property(e => e.ash_arb).HasColumnType("numeric");

                entity.Property(e => e.bl_date).HasColumnType("date");

                entity.Property(e => e.est_date_received).HasColumnType("date");

                entity.Property(e => e.gcv_arb).HasColumnType("numeric");

                entity.Property(e => e.hpb_price).HasColumnType("numeric");

                entity.Property(e => e.im_adb).HasColumnType("numeric");

                entity.Property(e => e.price_adj_per_ton).HasColumnType("numeric");

                entity.Property(e => e.sales_invoice).HasColumnType("numeric");

                entity.Property(e => e.tm_arb).HasColumnType("numeric");

                entity.Property(e => e.total_tons).HasColumnType("numeric");

                entity.Property(e => e.ts_adb).HasColumnType("numeric");

                entity.Property(e => e.ts_arb).HasColumnType("numeric");

                entity.Property(e => e.year).HasColumnType("numeric");
            });

            modelBuilder.Entity<raw_data_soa_pln>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.arrival_date).HasColumnType("date");

                entity.Property(e => e.basic_fob).HasColumnType("numeric");

                entity.Property(e => e.berthing_date).HasColumnType("date");

                entity.Property(e => e.bl_date).HasColumnType("date");

                entity.Property(e => e.cif_adj_price_per_ton).HasColumnType("numeric");

                entity.Property(e => e.denda_penolakan).HasColumnType("numeric");

                entity.Property(e => e.denda_telat).HasColumnType("numeric");

                entity.Property(e => e.discharge_date).HasColumnType("date");

                entity.Property(e => e.discharge_tonnage).HasColumnType("numeric");

                entity.Property(e => e.est_date_received).HasColumnType("date");

                entity.Property(e => e.fob_adj_price_per_ton).HasColumnType("numeric");

                entity.Property(e => e.fob_value).HasColumnType("numeric");

                entity.Property(e => e.freight).HasColumnType("numeric");

                entity.Property(e => e.loading_tonnage).HasColumnType("numeric");

                entity.Property(e => e.payment_received).HasColumnType("numeric");

                entity.Property(e => e.penyesuaian).HasColumnType("numeric");

                entity.Property(e => e.sales_invoice).HasColumnType("numeric");

                entity.Property(e => e.sales_invoice_tanpa_denda_telat).HasColumnType("numeric");

                entity.Property(e => e.year).HasColumnType("numeric");
            });

            modelBuilder.Entity<raw_report_stockpile_summary_fp>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.cpp_product).HasColumnType("numeric");

                entity.Property(e => e.dates).HasColumnType("date");

                entity.Property(e => e.port_product).HasColumnType("numeric");

                entity.Property(e => e.rom_cpp).HasColumnType("numeric");

                entity.Property(e => e.rom_pit).HasColumnType("numeric");
            });

            modelBuilder.Entity<ready_to_get>(entity =>
            {
                entity.Property(e => e.mine_location_id).IsRequired();

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.HasOne(d => d.mine_location_)
                    .WithMany(p => p.ready_to_getNavigation)
                    .HasForeignKey(d => d.mine_location_id)
                    .HasConstraintName("fk_mine_location_mine_location_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.ready_to_get)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<reference_price_series>(entity =>
            {
                entity.Property(e => e.ash).HasColumnType("numeric");

                entity.Property(e => e.calori).HasColumnType("numeric");

                entity.Property(e => e.end_date).HasColumnType("date");

                entity.Property(e => e.price).HasColumnType("numeric");

                entity.Property(e => e.start_date).HasColumnType("date");

                entity.Property(e => e.total_moisture).HasColumnType("numeric");

                entity.Property(e => e.total_sulphur).HasColumnType("numeric");
            });

            modelBuilder.Entity<rehandling_closing>(entity =>
            {
                entity.Property(e => e.distance).HasColumnType("numeric");

                entity.Property(e => e.volume).HasColumnType("numeric");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.rehandling_closing)
                    .HasForeignKey(d => d.organization_id)
                    .HasConstraintName("rehandling_closing_organization_id_fkey");
            });

            modelBuilder.Entity<rehandling_closing_item>(entity =>
            {
                entity.Property(e => e.quantity_item).HasColumnType("numeric");

                entity.Property(e => e.rehandling_closing_id).IsRequired();

                entity.Property(e => e.transaction_item_date).HasColumnType("date");

                entity.HasOne(d => d.rehandling_closing_)
                    .WithMany(p => p.rehandling_closing_item)
                    .HasForeignKey(d => d.rehandling_closing_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("rehandling_closing_item_fk");
            });

            modelBuilder.Entity<rehandling_transaction>(entity =>
            {
                entity.HasIndex(e => e.accounting_period_id)
                    .HasName("ix_rehandling_transaction_accounting_period_id");

                entity.HasIndex(e => e.process_flow_id)
                    .HasName("ix_rehandling_transaction_process_flow_id");

                entity.HasIndex(e => e.product_id)
                    .HasName("ix_rehandling_transaction_product_id");

                entity.HasIndex(e => e.survey_id)
                    .HasName("ix_rehandling_transaction_survey_id");

                entity.HasIndex(e => e.uom_id)
                    .HasName("ix_rehandling_transaction_uom_id");

                entity.HasIndex(e => new { e.organization_id, e.transaction_number })
                    .HasName("uk_rehandling_transaction")
                    .IsUnique();

                entity.Property(e => e.destination_location_id).IsRequired();

                entity.Property(e => e.distance).HasColumnType("numeric");

                entity.Property(e => e.hour_usage).HasColumnType("numeric");

                entity.Property(e => e.integration_status).HasDefaultValueSql("'NOT APPROVED'::text");

                entity.Property(e => e.loading_quantity).HasColumnType("numeric");

                entity.Property(e => e.process_flow_id).IsRequired();

                entity.Property(e => e.transaction_number).IsRequired();

                entity.Property(e => e.unloading_quantity).HasColumnType("numeric");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.rehandling_transaction)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<report_template>(entity =>
            {
                entity.HasIndex(e => e.organization_id)
                    .HasName("ix_report_template_organization_id");

                entity.HasIndex(e => new { e.organization_id, e.report_name })
                    .HasName("uk_report_template")
                    .IsUnique();

                entity.HasIndex(e => new { e.id, e.report_name, e.menu_id, e.created_on, e.organization_id })
                    .HasName("ix_report_template")
                    .HasSortOrder(new[] { SortOrder.Ascending, SortOrder.Ascending, SortOrder.Ascending, SortOrder.Descending, SortOrder.Ascending });

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.report_template)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<role_access>(entity =>
            {
                entity.HasIndex(e => e.created_by)
                    .HasName("ix_role_access_created_by")
                    .IncludeProperties(new[] { "created_on" });

                entity.HasIndex(e => e.entity_id)
                    .HasName("ix_role_acess_entity_id");

                entity.HasIndex(e => e.is_active)
                    .HasName("ix_role_access_is_active")
                    .IncludeProperties(new[] { "owner_id", "organization_id" });

                entity.HasIndex(e => e.modified_by)
                    .HasName("ix_role_access_modified_by")
                    .IncludeProperties(new[] { "modified_on" });

                entity.HasIndex(e => e.organization_id)
                    .HasName("ix_role_access_organization_id")
                    .IncludeProperties(new[] { "is_active", "owner_id" });

                entity.HasIndex(e => e.owner_id)
                    .HasName("ix_role_access_owner_id")
                    .IncludeProperties(new[] { "is_active", "organization_id" });

                entity.HasIndex(e => new { e.application_role_id, e.application_entity_id })
                    .HasName("uk_role_access")
                    .IsUnique();

                entity.Property(e => e.application_entity_id).IsRequired();

                entity.Property(e => e.application_role_id).IsRequired();

                entity.Property(e => e.organization_id).IsRequired();

                entity.Property(e => e.owner_id).IsRequired();

                entity.HasOne(d => d.application_entity_)
                    .WithMany(p => p.role_access)
                    .HasForeignKey(d => d.application_entity_id)
                    .HasConstraintName("fk_application_entity_application_entity_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.role_access)
                    .HasForeignKey(d => d.organization_id)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<role_business_unit>(entity =>
            {
                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.role_business_unit)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<role_exclusion>(entity =>
            {
                entity.HasKey(e => e.display_name)
                    .HasName("pk_role_exclusion");
            });

            modelBuilder.Entity<royalty>(entity =>
            {
                entity.Property(e => e.volume_loading).HasColumnType("numeric");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.royalty)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<royalty_cost>(entity =>
            {
                entity.Property(e => e.barging_cost).HasColumnType("numeric");

                entity.Property(e => e.dist_barge_to_anchorage).HasColumnType("numeric");

                entity.Property(e => e.freight_cost).HasColumnType("numeric");

                entity.Property(e => e.total_join_cost).HasColumnType("numeric");

                entity.Property(e => e.transhipment_cost).HasColumnType("numeric");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.royalty_cost)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<royalty_payment>(entity =>
            {
                entity.Property(e => e.bmn_outstanding).HasColumnType("numeric");

                entity.Property(e => e.bmn_paid_off).HasColumnType("numeric");

                entity.Property(e => e.dhpb_outstanding).HasColumnType("numeric");

                entity.Property(e => e.dhpb_paid_off).HasColumnType("numeric");

                entity.Property(e => e.pht_outstanding).HasColumnType("numeric");

                entity.Property(e => e.pht_paid_off).HasColumnType("numeric");

                entity.Property(e => e.royalty_outstanding).HasColumnType("numeric");

                entity.Property(e => e.royalty_paid_off).HasColumnType("numeric");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.royalty_payment)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<royalty_pricing>(entity =>
            {
                entity.Property(e => e.fob_price).HasColumnType("numeric");

                entity.Property(e => e.freight_cost).HasColumnType("numeric");

                entity.Property(e => e.hba_0).HasColumnType("numeric");

                entity.Property(e => e.hba_value).HasColumnType("numeric");

                entity.Property(e => e.hpb_barge).HasColumnType("numeric");

                entity.Property(e => e.hpb_vessel).HasColumnType("numeric");

                entity.Property(e => e.total_amount).HasColumnType("numeric");

                entity.Property(e => e.total_selling_price).HasColumnType("numeric");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.royalty_pricing)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<royalty_quantity_quality>(entity =>
            {
                entity.Property(e => e.analyte_value).HasColumnType("numeric");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.royalty_quantity_quality)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<royalty_valuation>(entity =>
            {
                entity.Property(e => e.base_price_royalty).HasColumnType("numeric");

                entity.Property(e => e.bmn_awal).HasColumnType("numeric");

                entity.Property(e => e.bmn_calc).HasColumnType("numeric");

                entity.Property(e => e.bmn_value).HasColumnType("numeric");

                entity.Property(e => e.dhpb_final_awal).HasColumnType("numeric");

                entity.Property(e => e.dhpb_final_calc).HasColumnType("numeric");

                entity.Property(e => e.dhpb_final_value).HasColumnType("numeric");

                entity.Property(e => e.pht_awal).HasColumnType("numeric");

                entity.Property(e => e.pht_calc).HasColumnType("numeric");

                entity.Property(e => e.pht_value).HasColumnType("numeric");

                entity.Property(e => e.royalty_awal).HasColumnType("numeric");

                entity.Property(e => e.royalty_calc).HasColumnType("numeric");

                entity.Property(e => e.royalty_value).HasColumnType("numeric");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.royalty_valuation)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<rpt_join_survey_contract>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.distance).HasColumnType("numeric");

                entity.Property(e => e.quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<rpt_joint_survey>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.distance).HasColumnType("numeric");

                entity.Property(e => e.quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<rpt_production>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.unloading_quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<rpt_timesheet>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.coal).HasColumnType("numeric");

                entity.Property(e => e.ob).HasColumnType("numeric");

                entity.Property(e => e.vol_distance).HasColumnType("numeric");

                entity.Property(e => e.volume).HasColumnType("numeric");
            });

            modelBuilder.Entity<rpt_timesheet_actplan>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.achievement).HasColumnType("numeric");

                entity.Property(e => e.vol_distance).HasColumnType("numeric");

                entity.Property(e => e.vol_distance_achievement).HasColumnType("numeric");

                entity.Property(e => e.vol_distance_plan).HasColumnType("numeric");

                entity.Property(e => e.volume).HasColumnType("numeric");

                entity.Property(e => e.volume_plan).HasColumnType("numeric");
            });

            modelBuilder.Entity<rpt_timesheet_actplan_shift>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.s1_vol).HasColumnType("numeric");

                entity.Property(e => e.s1_vol_distance).HasColumnType("numeric");

                entity.Property(e => e.s2_vol).HasColumnType("numeric");

                entity.Property(e => e.s2_vol_distance).HasColumnType("numeric");

                entity.Property(e => e.vol_distance).HasColumnType("numeric");

                entity.Property(e => e.vol_distance_plan).HasColumnType("numeric");

                entity.Property(e => e.volume).HasColumnType("numeric");

                entity.Property(e => e.volume_plan).HasColumnType("numeric");
            });

            modelBuilder.Entity<rpt_timesheet_coal_day>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.distances1).HasColumnType("numeric");

                entity.Property(e => e.distances2).HasColumnType("numeric");

                entity.Property(e => e.vol_distance).HasColumnType("numeric");

                entity.Property(e => e.vols1).HasColumnType("numeric");

                entity.Property(e => e.vols2).HasColumnType("numeric");

                entity.Property(e => e.volume).HasColumnType("numeric");
            });

            modelBuilder.Entity<rpt_timesheet_coal_day_event>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.minute).HasColumnType("numeric");

                entity.Property(e => e.timesheet_hour).HasColumnType("numeric");

                entity.Property(e => e.timesheet_time).HasColumnType("time without time zone");
            });

            modelBuilder.Entity<rpt_timesheet_coal_day_plan>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.distances1_plan).HasColumnType("numeric");

                entity.Property(e => e.distances2_plan).HasColumnType("numeric");

                entity.Property(e => e.vol_distance_plan).HasColumnType("numeric");

                entity.Property(e => e.vols1_plan).HasColumnType("numeric");

                entity.Property(e => e.vols2_plan).HasColumnType("numeric");

                entity.Property(e => e.volume_plan).HasColumnType("numeric");
            });

            modelBuilder.Entity<rpt_timesheet_detail>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.vol_distance).HasColumnType("numeric");

                entity.Property(e => e.volume).HasColumnType("numeric");
            });

            modelBuilder.Entity<rpt_timesheet_hmkaryawan>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.hm).HasColumnType("numeric");

                entity.Property(e => e.hour_end).HasColumnType("numeric");

                entity.Property(e => e.hour_start).HasColumnType("numeric");
            });

            modelBuilder.Entity<rpt_timesheet_hourly>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.classification).HasColumnType("time without time zone");

                entity.Property(e => e.minute).HasColumnType("numeric");

                entity.Property(e => e.volume).HasColumnType("numeric");
            });

            modelBuilder.Entity<rpt_timesheet_hourly_paua>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.breakdown).HasColumnType("numeric");

                entity.Property(e => e.breakdownhr).HasColumnType("numeric");

                entity.Property(e => e.classification).HasColumnType("time without time zone");

                entity.Property(e => e.delay).HasColumnType("numeric");

                entity.Property(e => e.delayhr).HasColumnType("numeric");

                entity.Property(e => e.idle).HasColumnType("numeric");

                entity.Property(e => e.idlehr).HasColumnType("numeric");

                entity.Property(e => e.losstime).HasColumnType("numeric");

                entity.Property(e => e.losstimehr).HasColumnType("numeric");

                entity.Property(e => e.nonproduction).HasColumnType("numeric");

                entity.Property(e => e.nonproductionhr).HasColumnType("numeric");

                entity.Property(e => e.pa).HasColumnType("numeric");

                entity.Property(e => e.production).HasColumnType("numeric");

                entity.Property(e => e.productionhr).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");

                entity.Property(e => e.totalhr).HasColumnType("numeric");

                entity.Property(e => e.ua).HasColumnType("numeric");
            });

            modelBuilder.Entity<rpt_timesheet_hourly_paua_byloader>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.breakdown).HasColumnType("numeric");

                entity.Property(e => e.breakdownhr).HasColumnType("numeric");

                entity.Property(e => e.delay).HasColumnType("numeric");

                entity.Property(e => e.delayhr).HasColumnType("numeric");

                entity.Property(e => e.idle).HasColumnType("numeric");

                entity.Property(e => e.idlehr).HasColumnType("numeric");

                entity.Property(e => e.losstime).HasColumnType("numeric");

                entity.Property(e => e.losstimehr).HasColumnType("numeric");

                entity.Property(e => e.nonproduction).HasColumnType("numeric");

                entity.Property(e => e.nonproductionhr).HasColumnType("numeric");

                entity.Property(e => e.pa).HasColumnType("numeric");

                entity.Property(e => e.production).HasColumnType("numeric");

                entity.Property(e => e.productionhr).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");

                entity.Property(e => e.totalhr).HasColumnType("numeric");

                entity.Property(e => e.ua).HasColumnType("numeric");
            });

            modelBuilder.Entity<rpt_timesheet_hourly_paua_byloader_performance>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.pa).HasColumnType("numeric");

                entity.Property(e => e.pa_achive).HasColumnType("numeric");

                entity.Property(e => e.pa_dev).HasColumnType("numeric");

                entity.Property(e => e.pa_plan).HasColumnType("numeric");

                entity.Property(e => e.ua).HasColumnType("numeric");

                entity.Property(e => e.ua_achive).HasColumnType("numeric");

                entity.Property(e => e.ua_dev).HasColumnType("numeric");

                entity.Property(e => e.ua_plan).HasColumnType("numeric");
            });

            modelBuilder.Entity<rpt_timesheet_hourly_paua_byloader_plan>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.breakdown).HasColumnType("numeric");

                entity.Property(e => e.breakdownhr).HasColumnType("numeric");

                entity.Property(e => e.delay).HasColumnType("numeric");

                entity.Property(e => e.delayhr).HasColumnType("numeric");

                entity.Property(e => e.idle).HasColumnType("numeric");

                entity.Property(e => e.idlehr).HasColumnType("numeric");

                entity.Property(e => e.losstime).HasColumnType("numeric");

                entity.Property(e => e.losstimehr).HasColumnType("numeric");

                entity.Property(e => e.nonproduction).HasColumnType("numeric");

                entity.Property(e => e.nonproductionhr).HasColumnType("numeric");

                entity.Property(e => e.pa).HasColumnType("numeric");

                entity.Property(e => e.production).HasColumnType("numeric");

                entity.Property(e => e.productionhr).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");

                entity.Property(e => e.totalhr).HasColumnType("numeric");

                entity.Property(e => e.ua).HasColumnType("numeric");
            });

            modelBuilder.Entity<rpt_timesheet_hourly_paua_old>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.breakdown).HasColumnType("numeric");

                entity.Property(e => e.breakdownhr).HasColumnType("numeric");

                entity.Property(e => e.delay).HasColumnType("numeric");

                entity.Property(e => e.delayhr).HasColumnType("numeric");

                entity.Property(e => e.idle).HasColumnType("numeric");

                entity.Property(e => e.idlehr).HasColumnType("numeric");

                entity.Property(e => e.losstime).HasColumnType("numeric");

                entity.Property(e => e.losstimehr).HasColumnType("numeric");

                entity.Property(e => e.nonproduction).HasColumnType("numeric");

                entity.Property(e => e.nonproductionhr).HasColumnType("numeric");

                entity.Property(e => e.pa).HasColumnType("numeric");

                entity.Property(e => e.production).HasColumnType("numeric");

                entity.Property(e => e.productionhr).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");

                entity.Property(e => e.totalhr).HasColumnType("numeric");

                entity.Property(e => e.ua).HasColumnType("numeric");
            });

            modelBuilder.Entity<rpt_timesheet_hourly_paua_plan>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.breakdown).HasColumnType("numeric");

                entity.Property(e => e.breakdownhr).HasColumnType("numeric");

                entity.Property(e => e.classification).HasColumnType("time without time zone");

                entity.Property(e => e.delay).HasColumnType("numeric");

                entity.Property(e => e.delayhr).HasColumnType("numeric");

                entity.Property(e => e.idle).HasColumnType("numeric");

                entity.Property(e => e.idlehr).HasColumnType("numeric");

                entity.Property(e => e.losstime).HasColumnType("numeric");

                entity.Property(e => e.losstimehr).HasColumnType("numeric");

                entity.Property(e => e.nonproduction).HasColumnType("numeric");

                entity.Property(e => e.nonproductionhr).HasColumnType("numeric");

                entity.Property(e => e.pa).HasColumnType("numeric");

                entity.Property(e => e.production).HasColumnType("numeric");

                entity.Property(e => e.productionhr).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");

                entity.Property(e => e.totalhr).HasColumnType("numeric");

                entity.Property(e => e.ua).HasColumnType("numeric");
            });

            modelBuilder.Entity<rpt_timesheet_hourly_plan>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.classification).HasColumnType("time without time zone");

                entity.Property(e => e.minute).HasColumnType("numeric");

                entity.Property(e => e.volume).HasColumnType("numeric");
            });

            modelBuilder.Entity<rpt_timesheet_hr_only>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.achievement).HasColumnType("numeric");

                entity.Property(e => e.coal).HasColumnType("numeric");

                entity.Property(e => e.coalplan).HasColumnType("numeric");

                entity.Property(e => e.hr_end).HasColumnType("time without time zone");

                entity.Property(e => e.hr_start).HasColumnType("time without time zone");

                entity.Property(e => e.ob).HasColumnType("numeric");

                entity.Property(e => e.obplan).HasColumnType("numeric");

                entity.Property(e => e.vol_distance).HasColumnType("numeric");

                entity.Property(e => e.vol_distance_plan).HasColumnType("numeric");

                entity.Property(e => e.volume).HasColumnType("numeric");

                entity.Property(e => e.volume_plan).HasColumnType("numeric");
            });

            modelBuilder.Entity<rpt_timesheet_hr_only_ach>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.achivements).HasColumnType("numeric");

                entity.Property(e => e.hr_start).HasColumnType("time without time zone");

                entity.Property(e => e.volume_plans).HasColumnType("numeric");

                entity.Property(e => e.volumes).HasColumnType("numeric");
            });

            modelBuilder.Entity<rpt_timesheet_hr_only_ach_fuel>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.achivements).HasColumnType("numeric");

                entity.Property(e => e.fuel_coal_ratio).HasColumnType("numeric");

                entity.Property(e => e.fuel_coal_value).HasColumnType("numeric");

                entity.Property(e => e.fuel_ob_ratio).HasColumnType("numeric");

                entity.Property(e => e.fuel_ob_value).HasColumnType("numeric");

                entity.Property(e => e.hr_start).HasColumnType("time without time zone");

                entity.Property(e => e.volume_plans).HasColumnType("numeric");

                entity.Property(e => e.volumes).HasColumnType("numeric");
            });

            modelBuilder.Entity<rpt_timesheet_hr_only_achivement>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.achivement).HasColumnType("numeric");

                entity.Property(e => e.volume).HasColumnType("numeric");

                entity.Property(e => e.volume_plan).HasColumnType("numeric");
            });

            modelBuilder.Entity<rpt_timesheet_hr_only_bycnunit>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.classification).HasColumnType("time without time zone");

                entity.Property(e => e.vol_distance).HasColumnType("numeric");

                entity.Property(e => e.volume).HasColumnType("numeric");
            });

            modelBuilder.Entity<rpt_timesheet_hr_only_bycnunit_obcoal>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.classification).HasColumnType("time without time zone");

                entity.Property(e => e.coal_distance).HasColumnType("numeric");

                entity.Property(e => e.coal_vol_distance).HasColumnType("numeric");

                entity.Property(e => e.coal_volume).HasColumnType("numeric");

                entity.Property(e => e.ob_distance).HasColumnType("numeric");

                entity.Property(e => e.ob_vol_distance).HasColumnType("numeric");

                entity.Property(e => e.ob_volume).HasColumnType("numeric");
            });

            modelBuilder.Entity<rpt_timesheet_ob_day>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.distances1).HasColumnType("numeric");

                entity.Property(e => e.distances2).HasColumnType("numeric");

                entity.Property(e => e.vol_distance).HasColumnType("numeric");

                entity.Property(e => e.vols1).HasColumnType("numeric");

                entity.Property(e => e.vols2).HasColumnType("numeric");

                entity.Property(e => e.volume).HasColumnType("numeric");
            });

            modelBuilder.Entity<rpt_timesheet_ob_day_event>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.minute).HasColumnType("numeric");

                entity.Property(e => e.timesheet_hour).HasColumnType("numeric");

                entity.Property(e => e.timesheet_time).HasColumnType("time without time zone");
            });

            modelBuilder.Entity<rpt_timesheet_ob_day_plan>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.distances1_plan).HasColumnType("numeric");

                entity.Property(e => e.distances2_plan).HasColumnType("numeric");

                entity.Property(e => e.vol_distance_plan).HasColumnType("numeric");

                entity.Property(e => e.vols1_plan).HasColumnType("numeric");

                entity.Property(e => e.vols2_plan).HasColumnType("numeric");

                entity.Property(e => e.volume_plan).HasColumnType("numeric");
            });

            modelBuilder.Entity<rpt_timesheet_paua>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.breakdown).HasColumnType("numeric");

                entity.Property(e => e.breakdownhr).HasColumnType("numeric");

                entity.Property(e => e.classification).HasColumnType("time without time zone");

                entity.Property(e => e.delay).HasColumnType("numeric");

                entity.Property(e => e.delayhr).HasColumnType("numeric");

                entity.Property(e => e.idle).HasColumnType("numeric");

                entity.Property(e => e.idlehr).HasColumnType("numeric");

                entity.Property(e => e.losstime).HasColumnType("numeric");

                entity.Property(e => e.losstimehr).HasColumnType("numeric");

                entity.Property(e => e.nonproduction).HasColumnType("numeric");

                entity.Property(e => e.nonproductionhr).HasColumnType("numeric");

                entity.Property(e => e.pa).HasColumnType("numeric");

                entity.Property(e => e.production).HasColumnType("numeric");

                entity.Property(e => e.productionhr).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");

                entity.Property(e => e.totalhr).HasColumnType("numeric");

                entity.Property(e => e.ua).HasColumnType("numeric");
            });

            modelBuilder.Entity<rpt_timesheet_paua_old>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.breakdown).HasColumnType("numeric");

                entity.Property(e => e.breakdownhr).HasColumnType("numeric");

                entity.Property(e => e.delay).HasColumnType("numeric");

                entity.Property(e => e.delayhr).HasColumnType("numeric");

                entity.Property(e => e.idle).HasColumnType("numeric");

                entity.Property(e => e.idlehr).HasColumnType("numeric");

                entity.Property(e => e.losstime).HasColumnType("numeric");

                entity.Property(e => e.losstimehr).HasColumnType("numeric");

                entity.Property(e => e.nonproduction).HasColumnType("numeric");

                entity.Property(e => e.nonproductionhr).HasColumnType("numeric");

                entity.Property(e => e.pa).HasColumnType("numeric");

                entity.Property(e => e.production).HasColumnType("numeric");

                entity.Property(e => e.productionhr).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");

                entity.Property(e => e.totalhr).HasColumnType("numeric");

                entity.Property(e => e.ua).HasColumnType("numeric");
            });

            modelBuilder.Entity<rpt_timesheet_paua_performance>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.breakdownhr).HasColumnType("numeric");

                entity.Property(e => e.delayhr).HasColumnType("numeric");

                entity.Property(e => e.idlehr).HasColumnType("numeric");

                entity.Property(e => e.losstimehr).HasColumnType("numeric");

                entity.Property(e => e.nonproductionhr).HasColumnType("numeric");

                entity.Property(e => e.pa).HasColumnType("numeric");

                entity.Property(e => e.pa_achive).HasColumnType("numeric");

                entity.Property(e => e.pa_dev).HasColumnType("numeric");

                entity.Property(e => e.pa_plan).HasColumnType("numeric");

                entity.Property(e => e.productionhr).HasColumnType("numeric");

                entity.Property(e => e.totalhr).HasColumnType("numeric");

                entity.Property(e => e.ua).HasColumnType("numeric");

                entity.Property(e => e.ua_achive).HasColumnType("numeric");

                entity.Property(e => e.ua_dev).HasColumnType("numeric");

                entity.Property(e => e.ua_plan).HasColumnType("numeric");
            });

            modelBuilder.Entity<rpt_timesheet_paua_productivity>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.breakdown).HasColumnType("numeric");

                entity.Property(e => e.breakdownhr).HasColumnType("numeric");

                entity.Property(e => e.classification).HasColumnType("time without time zone");

                entity.Property(e => e.coal_distance).HasColumnType("numeric");

                entity.Property(e => e.coal_productivity).HasColumnType("numeric");

                entity.Property(e => e.coal_productivityhr).HasColumnType("numeric");

                entity.Property(e => e.coal_vol_distance).HasColumnType("numeric");

                entity.Property(e => e.coal_volume).HasColumnType("numeric");

                entity.Property(e => e.delay).HasColumnType("numeric");

                entity.Property(e => e.delayhr).HasColumnType("numeric");

                entity.Property(e => e.idle).HasColumnType("numeric");

                entity.Property(e => e.idlehr).HasColumnType("numeric");

                entity.Property(e => e.losstime).HasColumnType("numeric");

                entity.Property(e => e.losstimehr).HasColumnType("numeric");

                entity.Property(e => e.nonproduction).HasColumnType("numeric");

                entity.Property(e => e.nonproductionhr).HasColumnType("numeric");

                entity.Property(e => e.ob_distance).HasColumnType("numeric");

                entity.Property(e => e.ob_productivity).HasColumnType("numeric");

                entity.Property(e => e.ob_productivityhr).HasColumnType("numeric");

                entity.Property(e => e.ob_vol_distance).HasColumnType("numeric");

                entity.Property(e => e.ob_volume).HasColumnType("numeric");

                entity.Property(e => e.pa).HasColumnType("numeric");

                entity.Property(e => e.production).HasColumnType("numeric");

                entity.Property(e => e.productionhr).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");

                entity.Property(e => e.totalhr).HasColumnType("numeric");

                entity.Property(e => e.ua).HasColumnType("numeric");
            });

            modelBuilder.Entity<rpt_timesheet_plan>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.coalplan).HasColumnType("numeric");

                entity.Property(e => e.obplan).HasColumnType("numeric");

                entity.Property(e => e.vol_distance_plan).HasColumnType("numeric");

                entity.Property(e => e.volume_plan).HasColumnType("numeric");
            });

            modelBuilder.Entity<rpt_timesheet_plan_detail>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.volum).HasColumnType("numeric");
            });

            modelBuilder.Entity<rpt_timesheet_plan_detail_sh1>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.volum).HasColumnType("numeric");
            });

            modelBuilder.Entity<rpt_timesheet_plan_detail_sh2>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.volum).HasColumnType("numeric");
            });

            modelBuilder.Entity<rpt_timesheet_plan_hourly>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.distance).HasColumnType("numeric");

                entity.Property(e => e.minute).HasColumnType("numeric");

                entity.Property(e => e.volume_plan).HasColumnType("numeric");
            });

            modelBuilder.Entity<rpt_timesheet_progressive>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.commulative).HasColumnType("numeric");

                entity.Property(e => e.volume).HasColumnType("numeric");
            });

            modelBuilder.Entity<rpt_timesheet_progressive_actplan>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.commulative).HasColumnType("numeric");

                entity.Property(e => e.volume).HasColumnType("numeric");
            });

            modelBuilder.Entity<rpt_timesheet_progressive_plan>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.commulative).HasColumnType("numeric");
            });

            modelBuilder.Entity<rpt_timesheethr>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.hr_end).HasColumnType("time without time zone");

                entity.Property(e => e.hr_start).HasColumnType("time without time zone");

                entity.Property(e => e.minute).HasColumnType("numeric");

                entity.Property(e => e.vol_distance).HasColumnType("numeric");

                entity.Property(e => e.volume).HasColumnType("numeric");
            });

            modelBuilder.Entity<sales_charge>(entity =>
            {
                entity.HasOne(d => d.charge_type_)
                    .WithMany(p => p.sales_charge)
                    .HasForeignKey(d => d.charge_type_id)
                    .HasConstraintName("fk_master_list_charge_type_id");
            });

            modelBuilder.Entity<sales_contract>(entity =>
            {
                entity.HasIndex(e => e.sales_contract_name)
                    .HasName("sales_contract_contract_number_idx")
                    .IsUnique();

                entity.Property(e => e.end_date).HasColumnType("date");

                entity.Property(e => e.start_date).HasColumnType("date");

                entity.HasOne(d => d.commitment_)
                    .WithMany(p => p.sales_contractcommitment_)
                    .HasForeignKey(d => d.commitment_id)
                    .HasConstraintName("fk_master_list_commitment_id");

                entity.HasOne(d => d.contract_basis_)
                    .WithMany(p => p.sales_contractcontract_basis_)
                    .HasForeignKey(d => d.contract_basis_id)
                    .HasConstraintName("fk_master_list_contract_basis_id");

                entity.HasOne(d => d.contract_status_)
                    .WithMany(p => p.sales_contractcontract_status_)
                    .HasForeignKey(d => d.contract_status_id)
                    .HasConstraintName("fk_master_list_contract_status_id");

                entity.HasOne(d => d.customer_)
                    .WithMany(p => p.sales_contractcustomer_)
                    .HasForeignKey(d => d.customer_id)
                    .HasConstraintName("fk_customer_customer_id");

                entity.HasOne(d => d.end_user_)
                    .WithMany(p => p.sales_contractend_user_)
                    .HasForeignKey(d => d.end_user_id)
                    .HasConstraintName("fk_customer_end_user_id");

                entity.HasOne(d => d.invoice_target_)
                    .WithMany(p => p.sales_contractinvoice_target_)
                    .HasForeignKey(d => d.invoice_target_id)
                    .HasConstraintName("fk_customer_invoice_target_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.sales_contractorganization_)
                    .HasForeignKey(d => d.organization_id)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.seller_)
                    .WithMany(p => p.sales_contractseller_)
                    .HasForeignKey(d => d.seller_id)
                    .HasConstraintName("fk_organization_seller_id");
            });

            modelBuilder.Entity<sales_contract_attachment>(entity =>
            {
                entity.HasOne(d => d.sales_contract_)
                    .WithMany(p => p.sales_contract_attachment)
                    .HasForeignKey(d => d.sales_contract_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_sales_contract_sales_contract_id");
            });

            modelBuilder.Entity<sales_contract_charges>(entity =>
            {
                entity.Property(e => e.decimal_places).HasColumnType("numeric");

                entity.Property(e => e.sales_contract_term_id).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.sales_contract_charges)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.rounding_type_)
                    .WithMany(p => p.sales_contract_charges)
                    .HasForeignKey(d => d.rounding_type_id)
                    .HasConstraintName("fk_master_list_rounding_type_id");

                entity.HasOne(d => d.sales_charge_)
                    .WithMany(p => p.sales_contract_charges)
                    .HasForeignKey(d => d.sales_charge_id)
                    .HasConstraintName("fk_sales_charge_sales_charge_id");

                entity.HasOne(d => d.sales_contract_term_)
                    .WithMany(p => p.sales_contract_charges)
                    .HasForeignKey(d => d.sales_contract_term_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_sales_contract_term_sales_contract_term_id");
            });

            modelBuilder.Entity<sales_contract_despatch_demurrage_delay>(entity =>
            {
                entity.Property(e => e.demurrage_applicable_percentage).HasColumnType("numeric");

                entity.Property(e => e.despatch_applicable_percentage).HasColumnType("numeric");

                entity.Property(e => e.sales_contract_despatch_demurrage_id).IsRequired();

                entity.HasOne(d => d.incident_)
                    .WithMany(p => p.sales_contract_despatch_demurrage_delay)
                    .HasForeignKey(d => d.incident_id)
                    .HasConstraintName("fk_event_category_incident_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.sales_contract_despatch_demurrage_delay)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.sales_contract_despatch_demurrage_)
                    .WithMany(p => p.sales_contract_despatch_demurrage_delay)
                    .HasForeignKey(d => d.sales_contract_despatch_demurrage_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_sales_contract_despatch_demurrage_sales_contract_despatch_de");
            });

            modelBuilder.Entity<sales_contract_despatch_demurrage_term>(entity =>
            {
                entity.Property(e => e.despatch_percentage).HasColumnType("numeric");

                entity.Property(e => e.loading_rate).HasColumnType("numeric");

                entity.Property(e => e.loading_rate_geared).HasColumnType("numeric");

                entity.Property(e => e.loading_rate_gearless).HasColumnType("numeric");

                entity.Property(e => e.loading_rate_nplct).HasColumnType("numeric");

                entity.Property(e => e.rate).HasColumnType("numeric");

                entity.Property(e => e.sales_contract_term_id).IsRequired();

                entity.Property(e => e.turn_time).HasColumnType("numeric");

                entity.Property(e => e.turn_time_ending).HasColumnType("numeric");

                entity.HasOne(d => d.currency_)
                    .WithMany(p => p.sales_contract_despatch_demurrage_term)
                    .HasForeignKey(d => d.currency_id)
                    .HasConstraintName("fk_currency_currency_id");

                entity.HasOne(d => d.loading_rate_uom_)
                    .WithMany(p => p.sales_contract_despatch_demurrage_termloading_rate_uom_)
                    .HasForeignKey(d => d.loading_rate_uom_id)
                    .HasConstraintName("fk_uom_loading_rate_uom_id");

                entity.HasOne(d => d.location_)
                    .WithMany(p => p.sales_contract_despatch_demurrage_term)
                    .HasForeignKey(d => d.location_id)
                    .HasConstraintName("fk_port_location_location_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.sales_contract_despatch_demurrage_term)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.sales_contract_term_)
                    .WithMany(p => p.sales_contract_despatch_demurrage_term)
                    .HasForeignKey(d => d.sales_contract_term_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_sales_contract_term_sales_contract_term_id");

                entity.HasOne(d => d.sof_)
                    .WithMany(p => p.sales_contract_despatch_demurrage_term)
                    .HasForeignKey(d => d.sof_id)
                    .HasConstraintName("fk_sof_sof_id");

                entity.HasOne(d => d.turn_time_uom_)
                    .WithMany(p => p.sales_contract_despatch_demurrage_termturn_time_uom_)
                    .HasForeignKey(d => d.turn_time_uom_id)
                    .HasConstraintName("fk_uom_turn_time_uom_id");
            });

            modelBuilder.Entity<sales_contract_despatch_plan>(entity =>
            {
                entity.Property(e => e.despatch_date).HasColumnType("date");

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.sales_contract_term_id).IsRequired();

                entity.HasOne(d => d.delivery_term_)
                    .WithMany(p => p.sales_contract_despatch_plandelivery_term_)
                    .HasForeignKey(d => d.delivery_term_id)
                    .HasConstraintName("fk_master_list_delivery_term_id");

                entity.HasOne(d => d.fulfilment_type_)
                    .WithMany(p => p.sales_contract_despatch_planfulfilment_type_)
                    .HasForeignKey(d => d.fulfilment_type_id)
                    .HasConstraintName("fk_master_list_fulfilment_type_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.sales_contract_despatch_plan)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.sales_contract_term_)
                    .WithMany(p => p.sales_contract_despatch_plan)
                    .HasForeignKey(d => d.sales_contract_term_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_sales_contract_term_sales_contract_term_id");

                entity.HasOne(d => d.uom_)
                    .WithMany(p => p.sales_contract_despatch_plan)
                    .HasForeignKey(d => d.uom_id)
                    .HasConstraintName("fk_uom_uom_id");
            });

            modelBuilder.Entity<sales_contract_detail>(entity =>
            {
                entity.Property(e => e.price).HasColumnType("numeric");

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.tax_rate).HasColumnType("numeric");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.sales_contract_detail)
                    .HasForeignKey(d => d.organization_id)
                    .HasConstraintName("sales_contract_detail_fk");

                entity.HasOne(d => d.product_)
                    .WithMany(p => p.sales_contract_detail)
                    .HasForeignKey(d => d.product_id)
                    .HasConstraintName("sales_contract_detail_fk_2");

                entity.HasOne(d => d.sales_charge_)
                    .WithMany(p => p.sales_contract_detail)
                    .HasForeignKey(d => d.sales_charge_id)
                    .HasConstraintName("sales_contract_detail_fk_4");

                entity.HasOne(d => d.sales_contract_)
                    .WithMany(p => p.sales_contract_detail)
                    .HasForeignKey(d => d.sales_contract_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("sales_contract_detail_fk_1");

                entity.HasOne(d => d.sales_price_)
                    .WithMany(p => p.sales_contract_detail)
                    .HasForeignKey(d => d.sales_price_id)
                    .HasConstraintName("sales_contract_detail_fk_3");

                entity.HasOne(d => d.tax_)
                    .WithMany(p => p.sales_contract_detail)
                    .HasForeignKey(d => d.tax_id)
                    .HasConstraintName("sales_contract_detail_fk_5");

                entity.HasOne(d => d.uom_)
                    .WithMany(p => p.sales_contract_detail)
                    .HasForeignKey(d => d.uom_id)
                    .HasConstraintName("sales_contract_detail_fk_6");
            });

            modelBuilder.Entity<sales_contract_payment_term>(entity =>
            {
                entity.Property(e => e.downpayment_value).HasColumnType("numeric");

                entity.Property(e => e.exchange_rate).HasColumnType("numeric");

                entity.Property(e => e.number_of_days).HasColumnType("numeric");

                entity.Property(e => e.sales_contract_term_id).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.sales_contract_payment_term)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<sales_contract_product>(entity =>
            {
                entity.Property(e => e.mass_required).HasColumnType("numeric");

                entity.Property(e => e.maximum_order).HasColumnType("numeric");

                entity.Property(e => e.minimum_order).HasColumnType("numeric");

                entity.Property(e => e.sales_contract_term_id).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.sales_contract_product)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.product_)
                    .WithMany(p => p.sales_contract_product)
                    .HasForeignKey(d => d.product_id)
                    .HasConstraintName("fk_product_product_id");

                entity.HasOne(d => d.uom_)
                    .WithMany(p => p.sales_contract_product)
                    .HasForeignKey(d => d.uom_id)
                    .HasConstraintName("fk_uom_uom_id");
            });

            modelBuilder.Entity<sales_contract_product_specifications>(entity =>
            {
                entity.Property(e => e.adjust_value).HasColumnType("numeric");

                entity.Property(e => e.analyte_id).IsRequired();

                entity.Property(e => e.fine_value).HasColumnType("numeric");

                entity.Property(e => e.maximum).HasColumnType("numeric");

                entity.Property(e => e.minimum).HasColumnType("numeric");

                entity.Property(e => e.sales_contract_product_id).IsRequired();

                entity.Property(e => e.target).HasColumnType("numeric");

                entity.Property(e => e.value).HasColumnType("numeric");

                entity.HasOne(d => d.analyte_)
                    .WithMany(p => p.sales_contract_product_specifications)
                    .HasForeignKey(d => d.analyte_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_analyte_analyte_id");

                entity.HasOne(d => d.analyte_standard_)
                    .WithMany(p => p.sales_contract_product_specifications)
                    .HasForeignKey(d => d.analyte_standard_id)
                    .HasConstraintName("fk_master_list_analyte_standard_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.sales_contract_product_specifications)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.sales_contract_product_)
                    .WithMany(p => p.sales_contract_product_specifications)
                    .HasForeignKey(d => d.sales_contract_product_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_sales_contract_product_sales_contract_product_id");

                entity.HasOne(d => d.uom_)
                    .WithMany(p => p.sales_contract_product_specifications)
                    .HasForeignKey(d => d.uom_id)
                    .HasConstraintName("fk_uom_upm_id");
            });

            modelBuilder.Entity<sales_contract_quotation_price>(entity =>
            {
                entity.Property(e => e.decimal_places).HasColumnType("numeric");

                entity.Property(e => e.price_value).HasColumnType("numeric");

                entity.Property(e => e.sales_contract_term_id).IsRequired();

                entity.Property(e => e.weightening_value).HasColumnType("numeric");

                entity.HasOne(d => d.currency_)
                    .WithMany(p => p.sales_contract_quotation_price)
                    .HasForeignKey(d => d.currency_id)
                    .HasConstraintName("fk_currency_currency_id");

                entity.HasOne(d => d.frequency_)
                    .WithMany(p => p.sales_contract_quotation_pricefrequency_)
                    .HasForeignKey(d => d.frequency_id)
                    .HasConstraintName("fk_master_list_frequency_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.sales_contract_quotation_price)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.price_index_)
                    .WithMany(p => p.sales_contract_quotation_price)
                    .HasForeignKey(d => d.price_index_id)
                    .HasConstraintName("fk_price_index_price_index_id");

                entity.HasOne(d => d.pricing_method_)
                    .WithMany(p => p.sales_contract_quotation_pricepricing_method_)
                    .HasForeignKey(d => d.pricing_method_id)
                    .HasConstraintName("fk_master_list_pricing_method_id");

                entity.HasOne(d => d.quotation_type_)
                    .WithMany(p => p.sales_contract_quotation_pricequotation_type_)
                    .HasForeignKey(d => d.quotation_type_id)
                    .HasConstraintName("fk_master_list_quotation_type_id");

                entity.HasOne(d => d.quotation_uom_)
                    .WithMany(p => p.sales_contract_quotation_pricequotation_uom_)
                    .HasForeignKey(d => d.quotation_uom_id)
                    .HasConstraintName("fk_uom_quotation_uom_id");

                entity.HasOne(d => d.sales_contract_term_)
                    .WithMany(p => p.sales_contract_quotation_price)
                    .HasForeignKey(d => d.sales_contract_term_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_sales_contract_term_sales_contract_term_id");

                entity.HasOne(d => d.uom_)
                    .WithMany(p => p.sales_contract_quotation_priceuom_)
                    .HasForeignKey(d => d.uom_id)
                    .HasConstraintName("fk_uom_uom_id");
            });

            modelBuilder.Entity<sales_contract_taxes>(entity =>
            {
                entity.Property(e => e.calculation_sign).HasColumnType("numeric");

                entity.Property(e => e.sales_contract_term_id).IsRequired();

                entity.Property(e => e.tax_rate).HasColumnType("numeric");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.sales_contract_taxes)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.sales_contract_term_)
                    .WithMany(p => p.sales_contract_taxes)
                    .HasForeignKey(d => d.sales_contract_term_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_sales_contract_term_sales_contract_term_id");

                entity.HasOne(d => d.tax_)
                    .WithMany(p => p.sales_contract_taxes)
                    .HasForeignKey(d => d.tax_id)
                    .HasConstraintName("fk_tax_tax_id");
            });

            modelBuilder.Entity<sales_contract_term>(entity =>
            {
                entity.Property(e => e.contract_term_name).IsRequired();

                entity.Property(e => e.decimal_places).HasColumnType("numeric");

                entity.Property(e => e.end_date).HasColumnType("date");

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.start_date).HasColumnType("date");

                entity.HasOne(d => d.currency_)
                    .WithMany(p => p.sales_contract_term)
                    .HasForeignKey(d => d.currency_id)
                    .HasConstraintName("sales_contract_term_currency");

                entity.HasOne(d => d.delivery_term_)
                    .WithMany(p => p.sales_contract_term)
                    .HasForeignKey(d => d.delivery_term_id)
                    .HasConstraintName("sales_contract_term_master_list");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.sales_contract_term)
                    .HasForeignKey(d => d.organization_id)
                    .HasConstraintName("sales_contract_term_organization");

                entity.HasOne(d => d.sales_contract_)
                    .WithMany(p => p.sales_contract_term)
                    .HasForeignKey(d => d.sales_contract_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("sales_contract_term_sales_contract");

                entity.HasOne(d => d.uom_)
                    .WithMany(p => p.sales_contract_term)
                    .HasForeignKey(d => d.uom_id)
                    .HasConstraintName("sales_contract_term_uom");
            });

            modelBuilder.Entity<sales_demurrage_rate>(entity =>
            {
                entity.HasIndex(e => new { e.sales_order_id, e.stock_location_id, e.demurrage_name })
                    .HasName("uk_sales_order_demurrage")
                    .IsUnique();

                entity.Property(e => e.demurrage_rate).HasColumnType("numeric");

                entity.Property(e => e.despatch_percent).HasColumnType("numeric");

                entity.Property(e => e.loading_rate).HasColumnType("numeric");

                entity.Property(e => e.sales_order_id).IsRequired();

                entity.Property(e => e.stock_location_id).IsRequired();

                entity.Property(e => e.turn_time).HasColumnType("numeric");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.sales_demurrage_rate)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.sales_order_)
                    .WithMany(p => p.sales_demurrage_rate)
                    .HasForeignKey(d => d.sales_order_id)
                    .HasConstraintName("fk_sales_order_sales_order_id");
            });

            modelBuilder.Entity<sales_invoice>(entity =>
            {
                entity.Property(e => e.alias1).HasColumnType("date");

                entity.Property(e => e.alias19).HasColumnType("date");

                entity.Property(e => e.alias2).HasColumnType("date");

                entity.Property(e => e.alias3).HasColumnType("date");

                entity.Property(e => e.alias4).HasColumnType("date");

                entity.Property(e => e.alias5).HasColumnType("numeric");

                entity.Property(e => e.alias6).HasColumnType("numeric");

                entity.Property(e => e.alias7).HasColumnType("numeric");

                entity.Property(e => e.alias8).HasColumnType("numeric");

                entity.Property(e => e.alias9).HasColumnType("numeric");

                entity.Property(e => e.ash_adb).HasColumnType("numeric");

                entity.Property(e => e.ash_arb).HasColumnType("numeric");

                entity.Property(e => e.decfreightamount).HasColumnType("numeric");

                entity.Property(e => e.decfreightpriceton).HasColumnType("numeric");

                entity.Property(e => e.despatch_order_id).IsRequired();

                entity.Property(e => e.downpayment).HasColumnType("numeric");

                entity.Property(e => e.fob_price).HasColumnType("numeric");

                entity.Property(e => e.freight_cost).HasColumnType("numeric");

                entity.Property(e => e.gcv_adb).HasColumnType("numeric");

                entity.Property(e => e.gcv_arb).HasColumnType("numeric");

                entity.Property(e => e.hba_value).HasColumnType("numeric");

                entity.Property(e => e.hpb).HasColumnType("numeric");

                entity.Property(e => e.hpb_value).HasColumnType("numeric");

                entity.Property(e => e.im_adb).HasColumnType("numeric");

                entity.Property(e => e.insurance_cost).HasColumnType("numeric");

                entity.Property(e => e.invoice_date).HasColumnType("date");

                entity.Property(e => e.invoice_number).IsRequired();

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.subtotal).HasColumnType("numeric");

                entity.Property(e => e.tm_arb).HasColumnType("numeric");

                entity.Property(e => e.total_price).HasColumnType("numeric");

                entity.Property(e => e.total_price_hpb).HasColumnType("numeric");

                entity.Property(e => e.ts_adb).HasColumnType("numeric");

                entity.Property(e => e.ts_arb).HasColumnType("numeric");

                entity.Property(e => e.unit_price).HasColumnType("numeric");

                entity.HasOne(d => d.bank_account_)
                    .WithMany(p => p.sales_invoice)
                    .HasForeignKey(d => d.bank_account_id)
                    .HasConstraintName("fk_bank_account_bank_account_id");

                entity.HasOne(d => d.despatch_order_)
                    .WithMany(p => p.sales_invoice)
                    .HasForeignKey(d => d.despatch_order_id)
                    .HasConstraintName("fk_despatch_order_despatch_order_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.sales_invoice)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<sales_invoice_approval>(entity =>
            {
                entity.Property(e => e.sales_invoice_id).IsRequired();
            });

            modelBuilder.Entity<sales_invoice_attachment>(entity =>
            {
                entity.HasOne(d => d.sales_invoice_)
                    .WithMany(p => p.sales_invoice_attachment)
                    .HasForeignKey(d => d.sales_invoice_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_sales_invoice_attachment");
            });

            modelBuilder.Entity<sales_invoice_charges>(entity =>
            {
                entity.Property(e => e.decimal_places).HasColumnType("numeric");

                entity.Property(e => e.price).HasColumnType("numeric");

                entity.Property(e => e.sales_charge_id).IsRequired();

                entity.Property(e => e.sales_invoice_id).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.sales_invoice_charges)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.rounding_type_)
                    .WithMany(p => p.sales_invoice_charges)
                    .HasForeignKey(d => d.rounding_type_id)
                    .HasConstraintName("fk_master_list_rounding_type_id");

                entity.HasOne(d => d.sales_charge_)
                    .WithMany(p => p.sales_invoice_charges)
                    .HasForeignKey(d => d.sales_charge_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_sales_invoice_sales_charge_id");
            });

            modelBuilder.Entity<sales_invoice_detail>(entity =>
            {
                entity.HasIndex(e => new { e.sales_invoice_id, e.item })
                    .HasName("uk_sales_invoice_detail")
                    .IsUnique();

                entity.Property(e => e.adjustment_value).HasColumnType("numeric");

                entity.Property(e => e.item).IsRequired();

                entity.Property(e => e.item_value).HasColumnType("numeric");

                entity.Property(e => e.sales_invoice_id).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.sales_invoice_detail)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.sales_invoice_)
                    .WithMany(p => p.sales_invoice_detail)
                    .HasForeignKey(d => d.sales_invoice_id)
                    .HasConstraintName("fk_sales_invoice_sales_invoice_id");
            });

            modelBuilder.Entity<sales_invoice_ell>(entity =>
            {
                entity.HasKey(e => e.sync_id)
                    .HasName("sales_invoice_ell_pkey");

                entity.Property(e => e.downpayment).HasColumnType("numeric");

                entity.Property(e => e.freight_cost).HasColumnType("numeric");

                entity.Property(e => e.id).IsRequired();

                entity.Property(e => e.invoice_date).HasColumnType("date");

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.total_price).HasColumnType("numeric");

                entity.Property(e => e.unit_price).HasColumnType("numeric");
            });

            modelBuilder.Entity<sales_invoice_html_content>(entity =>
            {
                entity.Property(e => e.sales_invoice_id).IsRequired();

                entity.HasOne(d => d.sales_invoice_)
                    .WithMany(p => p.sales_invoice_html_content)
                    .HasForeignKey(d => d.sales_invoice_id)
                    .HasConstraintName("sales_invoice_html_content_fk");
            });

            modelBuilder.Entity<sales_invoice_payment>(entity =>
            {
                entity.Property(e => e.payment_value).HasColumnType("numeric");

                entity.Property(e => e.sales_invoice_number).IsRequired();
            });

            modelBuilder.Entity<sales_invoice_product_specifications>(entity =>
            {
                entity.Property(e => e.nilai_denda_penolakan).HasColumnType("numeric");

                entity.Property(e => e.target).HasColumnType("numeric");

                entity.Property(e => e.value).HasColumnType("numeric");
            });

            modelBuilder.Entity<sales_invoice_trace_element>(entity =>
            {
                entity.HasOne(d => d.sales_invoice_)
                    .WithMany(p => p.sales_invoice_trace_element)
                    .HasForeignKey(d => d.sales_invoice_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("sales_invoice_trace_element_sales_invoice_id_fkey");
            });

            modelBuilder.Entity<sales_invoice_transhipment>(entity =>
            {
                entity.HasOne(d => d.despatch_order_)
                    .WithMany(p => p.sales_invoice_transhipment)
                    .HasForeignKey(d => d.despatch_order_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("sales_invoice_transhipment_despatch_order_id_fkey");

                entity.HasOne(d => d.sales_invoice_)
                    .WithMany(p => p.sales_invoice_transhipment)
                    .HasForeignKey(d => d.sales_invoice_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("sales_invoice_transhipment_sales_invoice_id_fkey");
            });

            modelBuilder.Entity<sales_month_summary_aims>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.cv_gar).HasColumnType("numeric");

                entity.Property(e => e.orders).HasColumnType("numeric");

                entity.Property(e => e.price_fob).HasColumnType("numeric");

                entity.Property(e => e.revenue).HasColumnType("numeric");

                entity.Property(e => e.sales).HasColumnType("numeric");

                entity.Property(e => e.years).HasColumnType("numeric");
            });

            modelBuilder.Entity<sales_order>(entity =>
            {
                entity.Property(e => e.accounting_period_id).IsRequired();

                entity.Property(e => e.business_partner_id).IsRequired();

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.sales_date).HasColumnType("date");

                entity.Property(e => e.sales_order_number).IsRequired();

                entity.Property(e => e.shipping_end_date).HasColumnType("date");

                entity.Property(e => e.shipping_start_date).HasColumnType("date");

                entity.Property(e => e.unit_price).HasColumnType("numeric");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.sales_order)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<sales_order_detail>(entity =>
            {
                entity.Property(e => e.maximum_value).HasColumnType("numeric");

                entity.Property(e => e.minimum_value).HasColumnType("numeric");

                entity.Property(e => e.sales_order_id).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.sales_order_detail)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.sales_order_)
                    .WithMany(p => p.sales_order_detail)
                    .HasForeignKey(d => d.sales_order_id)
                    .HasConstraintName("fk_sales_order_sales_order_id");
            });

            modelBuilder.Entity<sales_plan>(entity =>
            {
                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.revision_number).HasColumnType("numeric");

                entity.Property(e => e.rkab_quantity).HasColumnType("numeric");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.sales_plan)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<sales_plan_customer>(entity =>
            {
                entity.Property(e => e.eta_date).HasColumnType("date");

                entity.Property(e => e.laycan_end).HasColumnType("date");

                entity.Property(e => e.laycan_start).HasColumnType("date");

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.HasOne(d => d.customer_)
                    .WithMany(p => p.sales_plan_customer)
                    .HasForeignKey(d => d.customer_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("sales_plan_customer_fk");

                entity.HasOne(d => d.sales_plan_detail_)
                    .WithMany(p => p.sales_plan_customer)
                    .HasForeignKey(d => d.sales_plan_detail_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_sales_plan_customer_master");
            });

            modelBuilder.Entity<sales_plan_detail>(entity =>
            {
                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.sales_plan_id).IsRequired();

                entity.HasOne(d => d.month_)
                    .WithMany(p => p.sales_plan_detail)
                    .HasForeignKey(d => d.month_id)
                    .HasConstraintName("fk_months_month_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.sales_plan_detail)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.sales_plan_)
                    .WithMany(p => p.sales_plan_detail)
                    .HasForeignKey(d => d.sales_plan_id)
                    .HasConstraintName("fk_sales_plan_sales_plan_id");
            });

            modelBuilder.Entity<sales_plan_snapshot>(entity =>
            {
                entity.Property(e => e.data).HasColumnType("json");

                entity.Property(e => e.sales_plan_id).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.sales_plan_snapshot)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.sales_plan_)
                    .WithMany(p => p.sales_plan_snapshot)
                    .HasForeignKey(d => d.sales_plan_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_sales_plan_sales_plan_id");
            });

            modelBuilder.Entity<sales_price>(entity =>
            {
                entity.Property(e => e.price).HasColumnType("numeric");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.sales_price)
                    .HasForeignKey(d => d.organization_id)
                    .HasConstraintName("sales_price_fk");

                entity.HasOne(d => d.price_type_)
                    .WithMany(p => p.sales_price)
                    .HasForeignKey(d => d.price_type_id)
                    .HasConstraintName("sales_price_fk_1");

                entity.HasOne(d => d.product_)
                    .WithMany(p => p.sales_price)
                    .HasForeignKey(d => d.product_id)
                    .HasConstraintName("sales_price_fk_3");

                entity.HasOne(d => d.uom_)
                    .WithMany(p => p.sales_price)
                    .HasForeignKey(d => d.uom_id)
                    .HasConstraintName("sales_price_fk_2");
            });

            modelBuilder.Entity<sales_price_type>(entity =>
            {
                entity.Property(e => e.average).HasColumnType("numeric");

                entity.Property(e => e.fix_value).HasColumnType("numeric");

                entity.Property(e => e.uom_id).HasColumnType("character varying");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.sales_price_type)
                    .HasForeignKey(d => d.organization_id)
                    .HasConstraintName("sales_price_type_fk_1");

                entity.HasOne(d => d.uom_)
                    .WithMany(p => p.sales_price_type)
                    .HasForeignKey(d => d.uom_id)
                    .HasConstraintName("sales_price_type_fk");
            });

            modelBuilder.Entity<sales_product_specification>(entity =>
            {
                entity.HasIndex(e => new { e.sales_order_detail_id, e.analyte_id })
                    .HasName("uk_sales_product_specification")
                    .IsUnique();

                entity.Property(e => e.analyte_id).IsRequired();

                entity.Property(e => e.maximum_value).HasColumnType("numeric");

                entity.Property(e => e.minimum_value).HasColumnType("numeric");

                entity.Property(e => e.sales_order_detail_id).IsRequired();

                entity.HasOne(d => d.analyte_)
                    .WithMany(p => p.sales_product_specification)
                    .HasForeignKey(d => d.analyte_id)
                    .HasConstraintName("fk_analyte_analyte_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.sales_product_specification)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.sales_order_detail_)
                    .WithMany(p => p.sales_product_specification)
                    .HasForeignKey(d => d.sales_order_detail_id)
                    .HasConstraintName("fk_sales_order_detail_sales_order_detail_id");
            });

            modelBuilder.Entity<sales_summary_aims>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.cv_gar).HasColumnType("numeric");

                entity.Property(e => e.price_fob).HasColumnType("numeric");

                entity.Property(e => e.revenue).HasColumnType("numeric");

                entity.Property(e => e.sales).HasColumnType("numeric");
            });

            modelBuilder.Entity<sales_summary_aims_by_brand>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.m1).HasColumnType("numeric");

                entity.Property(e => e.m2).HasColumnType("numeric");

                entity.Property(e => e.m3).HasColumnType("numeric");

                entity.Property(e => e.mtd).HasColumnType("numeric");

                entity.Property(e => e.q1).HasColumnType("numeric");

                entity.Property(e => e.q2).HasColumnType("numeric");

                entity.Property(e => e.q3).HasColumnType("numeric");

                entity.Property(e => e.q4).HasColumnType("numeric");

                entity.Property(e => e.total_year).HasColumnType("numeric");
            });

            modelBuilder.Entity<sales_summary_aims_by_destination>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.m1).HasColumnType("numeric");

                entity.Property(e => e.m2).HasColumnType("numeric");

                entity.Property(e => e.m3).HasColumnType("numeric");

                entity.Property(e => e.mtd).HasColumnType("numeric");

                entity.Property(e => e.q1).HasColumnType("numeric");

                entity.Property(e => e.q2).HasColumnType("numeric");

                entity.Property(e => e.q3).HasColumnType("numeric");

                entity.Property(e => e.q4).HasColumnType("numeric");

                entity.Property(e => e.total_year).HasColumnType("numeric");
            });

            modelBuilder.Entity<sales_summary_aims_by_shipment>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.m1).HasColumnType("numeric");

                entity.Property(e => e.m2).HasColumnType("numeric");

                entity.Property(e => e.m3).HasColumnType("numeric");

                entity.Property(e => e.mtd).HasColumnType("numeric");

                entity.Property(e => e.q1).HasColumnType("numeric");

                entity.Property(e => e.q2).HasColumnType("numeric");

                entity.Property(e => e.q3).HasColumnType("numeric");

                entity.Property(e => e.q4).HasColumnType("numeric");

                entity.Property(e => e.total_year).HasColumnType("numeric");
            });

            modelBuilder.Entity<sales_summary_aims_invoice>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.cv_gar).HasColumnType("numeric");

                entity.Property(e => e.price_fob).HasColumnType("numeric");

                entity.Property(e => e.revenue).HasColumnType("numeric");

                entity.Property(e => e.sales).HasColumnType("numeric");
            });

            modelBuilder.Entity<sales_summary_aims_invoice_by_brand>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.m1).HasColumnType("numeric");

                entity.Property(e => e.m2).HasColumnType("numeric");

                entity.Property(e => e.m3).HasColumnType("numeric");

                entity.Property(e => e.mtd).HasColumnType("numeric");

                entity.Property(e => e.q1).HasColumnType("numeric");

                entity.Property(e => e.q2).HasColumnType("numeric");

                entity.Property(e => e.q3).HasColumnType("numeric");

                entity.Property(e => e.q4).HasColumnType("numeric");

                entity.Property(e => e.total_year).HasColumnType("numeric");
            });

            modelBuilder.Entity<sales_summary_aims_invoice_by_destination>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.m1).HasColumnType("numeric");

                entity.Property(e => e.m2).HasColumnType("numeric");

                entity.Property(e => e.m3).HasColumnType("numeric");

                entity.Property(e => e.mtd).HasColumnType("numeric");

                entity.Property(e => e.q1).HasColumnType("numeric");

                entity.Property(e => e.q2).HasColumnType("numeric");

                entity.Property(e => e.q3).HasColumnType("numeric");

                entity.Property(e => e.q4).HasColumnType("numeric");

                entity.Property(e => e.total_year).HasColumnType("numeric");
            });

            modelBuilder.Entity<sales_summary_aims_invoice_by_shipment>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.m1).HasColumnType("numeric");

                entity.Property(e => e.m2).HasColumnType("numeric");

                entity.Property(e => e.m3).HasColumnType("numeric");

                entity.Property(e => e.mtd).HasColumnType("numeric");

                entity.Property(e => e.q1).HasColumnType("numeric");

                entity.Property(e => e.q2).HasColumnType("numeric");

                entity.Property(e => e.q3).HasColumnType("numeric");

                entity.Property(e => e.q4).HasColumnType("numeric");

                entity.Property(e => e.total_year).HasColumnType("numeric");
            });

            modelBuilder.Entity<sales_summary_aims_non_invoice>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.cv_gar).HasColumnType("numeric");

                entity.Property(e => e.etc).HasColumnType("date");

                entity.Property(e => e.price_fob).HasColumnType("numeric");

                entity.Property(e => e.revenue).HasColumnType("numeric");

                entity.Property(e => e.sales).HasColumnType("numeric");
            });

            modelBuilder.Entity<sales_summary_aims_uninvoice_by_brand>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.m1).HasColumnType("numeric");

                entity.Property(e => e.m2).HasColumnType("numeric");

                entity.Property(e => e.m3).HasColumnType("numeric");

                entity.Property(e => e.mtd).HasColumnType("numeric");

                entity.Property(e => e.q1).HasColumnType("numeric");

                entity.Property(e => e.q2).HasColumnType("numeric");

                entity.Property(e => e.q3).HasColumnType("numeric");

                entity.Property(e => e.q4).HasColumnType("numeric");

                entity.Property(e => e.total_year).HasColumnType("numeric");
            });

            modelBuilder.Entity<sales_summary_aims_uninvoice_by_destination>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.m1).HasColumnType("numeric");

                entity.Property(e => e.m2).HasColumnType("numeric");

                entity.Property(e => e.m3).HasColumnType("numeric");

                entity.Property(e => e.mtd).HasColumnType("numeric");

                entity.Property(e => e.q1).HasColumnType("numeric");

                entity.Property(e => e.q2).HasColumnType("numeric");

                entity.Property(e => e.q3).HasColumnType("numeric");

                entity.Property(e => e.q4).HasColumnType("numeric");

                entity.Property(e => e.total_year).HasColumnType("numeric");
            });

            modelBuilder.Entity<sales_summary_aims_uninvoice_by_shipment>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.m1).HasColumnType("numeric");

                entity.Property(e => e.m2).HasColumnType("numeric");

                entity.Property(e => e.m3).HasColumnType("numeric");

                entity.Property(e => e.mtd).HasColumnType("numeric");

                entity.Property(e => e.q1).HasColumnType("numeric");

                entity.Property(e => e.q2).HasColumnType("numeric");

                entity.Property(e => e.q3).HasColumnType("numeric");

                entity.Property(e => e.q4).HasColumnType("numeric");

                entity.Property(e => e.total_year).HasColumnType("numeric");
            });

            modelBuilder.Entity<sampling_template>(entity =>
            {
                entity.HasIndex(e => new { e.organization_id, e.sampling_template_name })
                    .HasName("uk_sampling_template")
                    .IsUnique();

                entity.Property(e => e.sampling_template_name).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.sampling_template)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<sampling_template_detail>(entity =>
            {
                entity.HasIndex(e => new { e.sampling_template_id, e.analyte_id })
                    .HasName("uk_sampling_template_detail")
                    .IsUnique();

                entity.Property(e => e.analyte_id).IsRequired();

                entity.Property(e => e.sampling_template_id).IsRequired();

                entity.Property(e => e.uom_id).IsRequired();

                entity.HasOne(d => d.analyte_)
                    .WithMany(p => p.sampling_template_detail)
                    .HasForeignKey(d => d.analyte_id)
                    .HasConstraintName("fk_analyte_analyte_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.sampling_template_detail)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.sampling_template_)
                    .WithMany(p => p.sampling_template_detail)
                    .HasForeignKey(d => d.sampling_template_id)
                    .HasConstraintName("fk_sampling_template_sampling_template_id");

                entity.HasOne(d => d.uom_)
                    .WithMany(p => p.sampling_template_detail)
                    .HasForeignKey(d => d.uom_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_uom_uom_id");
            });

            modelBuilder.Entity<sampling_type>(entity =>
            {
                entity.Property(e => e.sampling_type_code).IsRequired();

                entity.Property(e => e.sampling_type_name).IsRequired();
            });

            modelBuilder.Entity<shared_record>(entity =>
            {
                entity.HasIndex(e => e.created_by)
                    .HasName("ix_shared_record_created_by")
                    .IncludeProperties(new[] { "created_on" });

                entity.HasIndex(e => e.entity_id)
                    .HasName("ix_shared_record_entity_id");

                entity.HasIndex(e => e.is_active)
                    .HasName("ix_shared_record_is_active")
                    .IncludeProperties(new[] { "owner_id", "organization_id" });

                entity.HasIndex(e => e.modified_by)
                    .HasName("ix_shared_record_modified_by")
                    .IncludeProperties(new[] { "modified_on" });

                entity.HasIndex(e => e.organization_id)
                    .HasName("ix_shared_record_organization_id");

                entity.HasIndex(e => e.owner_id)
                    .HasName("ix_shared_record_owner_id")
                    .IncludeProperties(new[] { "is_active", "organization_id" });

                entity.HasIndex(e => new { e.record_id, e.shared_to_id })
                    .HasName("uk_shared_record")
                    .IsUnique();

                entity.Property(e => e.organization_id).IsRequired();

                entity.Property(e => e.owner_id).IsRequired();

                entity.Property(e => e.record_id).IsRequired();

                entity.Property(e => e.shared_to_id).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.shared_record)
                    .HasForeignKey(d => d.organization_id)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.shared_to_)
                    .WithMany(p => p.shared_record)
                    .HasForeignKey(d => d.shared_to_id)
                    .HasConstraintName("fk_application_user_shared_to_id");
            });

            modelBuilder.Entity<shift>(entity =>
            {
                entity.HasIndex(e => e.organization_id)
                    .HasName("ix_shift_organization_id");

                entity.HasIndex(e => e.shift_category_id)
                    .HasName("ix_shift_shift_category_id");

                entity.HasIndex(e => new { e.shift_category_id, e.shift_name })
                    .HasName("uk_shift")
                    .IsUnique();

                entity.Property(e => e.duration).HasColumnType("time without time zone");

                entity.Property(e => e.end_time).HasColumnType("time without time zone");

                entity.Property(e => e.shift_category_id).IsRequired();

                entity.Property(e => e.shift_name).IsRequired();

                entity.Property(e => e.start_time).HasColumnType("time without time zone");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.shift)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.shift_category_)
                    .WithMany(p => p.shift)
                    .HasForeignKey(d => d.shift_category_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("shift_shift_category_id_fkey");
            });

            modelBuilder.Entity<shift_category>(entity =>
            {
                entity.HasIndex(e => e.organization_id)
                    .HasName("ix_shift_category_organization_id");

                entity.HasIndex(e => new { e.organization_id, e.shift_category_code })
                    .HasName("Shift Category Code")
                    .IsUnique();

                entity.Property(e => e.shift_category_name).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.shift_category)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<shipment_forecast>(entity =>
            {
                entity.Property(e => e.amount_received).HasColumnType("numeric");

                entity.Property(e => e.bl_date).HasColumnType("date");

                entity.Property(e => e.comm_date).HasColumnType("date");

                entity.Property(e => e.eta).HasColumnType("date");

                entity.Property(e => e.exchange_rate).HasColumnType("numeric");

                entity.Property(e => e.invoice_amount).HasColumnType("numeric");

                entity.Property(e => e.invoice_date).HasColumnType("date");

                entity.Property(e => e.invoice_due_date).HasColumnType("date");

                entity.Property(e => e.invoice_price).HasColumnType("numeric");

                entity.Property(e => e.laycan_end).HasColumnType("date");

                entity.Property(e => e.laycan_start).HasColumnType("date");

                entity.Property(e => e.month).HasColumnType("numeric");

                entity.Property(e => e.order_reference_date).HasColumnType("date");

                entity.Property(e => e.payment_receiving_date).HasColumnType("date");

                entity.Property(e => e.quantity_actual).HasColumnType("numeric");

                entity.Property(e => e.quantity_plan).HasColumnType("numeric");

                entity.Property(e => e.total_shipment).HasColumnType("numeric");

                entity.Property(e => e.year).HasColumnType("numeric");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.shipment_forecast)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<shipment_plan>(entity =>
            {
                entity.HasIndex(e => e.lineup_number)
                    .HasName("u_lineup_number")
                    .IsUnique();

                entity.Property(e => e.despatch_demurrage_rate).HasColumnType("numeric");

                entity.Property(e => e.hpb_forecast).HasColumnType("numeric");

                entity.Property(e => e.laycan_end).HasColumnType("date");

                entity.Property(e => e.laycan_start).HasColumnType("date");

                entity.Property(e => e.loading_rate).HasColumnType("numeric");

                entity.Property(e => e.loading_standart).HasColumnType("numeric");

                entity.Property(e => e.qty_sp).HasColumnType("numeric");

                entity.Property(e => e.stow_plan).HasColumnType("numeric");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.shipment_plan)
                    .HasForeignKey(d => d.organization_id)
                    .HasConstraintName("shipment_plan_fk");
            });

            modelBuilder.Entity<shipping_closing>(entity =>
            {
                entity.Property(e => e.distance).HasColumnType("numeric");

                entity.Property(e => e.volume).HasColumnType("numeric");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.shipping_closing)
                    .HasForeignKey(d => d.organization_id)
                    .HasConstraintName("shipping_closing_organization_id_fkey");
            });

            modelBuilder.Entity<shipping_closing_item>(entity =>
            {
                entity.Property(e => e.quantity_item).HasColumnType("numeric");

                entity.Property(e => e.shipping_closing_id).IsRequired();

                entity.Property(e => e.transaction_item_date).HasColumnType("date");

                entity.HasOne(d => d.shipping_closing_)
                    .WithMany(p => p.shipping_closing_item)
                    .HasForeignKey(d => d.shipping_closing_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("shipping_closing_item_fk");
            });

            modelBuilder.Entity<shipping_cost>(entity =>
            {
                entity.Property(e => e.advance_payment).HasColumnType("numeric");

                entity.Property(e => e.freight_rate).HasColumnType("numeric");

                entity.Property(e => e.insurance_cost).HasColumnType("numeric");

                entity.Property(e => e.late_penalty).HasColumnType("numeric");

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.shipping_cost_number).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<shipping_cost_document>(entity =>
            {
                entity.Property(e => e.shipping_cost_id).IsRequired();
            });

            modelBuilder.Entity<shipping_delay>(entity =>
            {
                entity.Property(e => e.despatch_order_delay_id).IsRequired();

                entity.Property(e => e.shipping_transaction_id).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.shipping_delay)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.shipping_transaction_)
                    .WithMany(p => p.shipping_delay)
                    .HasForeignKey(d => d.shipping_transaction_id)
                    .HasConstraintName("fk_shipping_transaction_shipping_transaction_id");
            });

            modelBuilder.Entity<shipping_document>(entity =>
            {
                entity.Property(e => e.document_name).IsRequired();

                entity.Property(e => e.shipping_transaction_id).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.shipping_document)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.shipping_transaction_)
                    .WithMany(p => p.shipping_document)
                    .HasForeignKey(d => d.shipping_transaction_id)
                    .HasConstraintName("fk_shipping_transaction_shipping_transaction_id");
            });

            modelBuilder.Entity<shipping_instruction>(entity =>
            {
                entity.Property(e => e.advance_royalty_amount).HasColumnType("numeric");

                entity.Property(e => e.despatch_order_id).IsRequired();

                entity.Property(e => e.issued_date).HasColumnType("date");

                entity.Property(e => e.lc_amount).HasColumnType("numeric");

                entity.Property(e => e.lc_date).HasColumnType("date");

                entity.Property(e => e.lc_price).HasColumnType("numeric");

                entity.Property(e => e.released_date).HasColumnType("date");

                entity.Property(e => e.required_quantity).HasColumnType("numeric");

                entity.Property(e => e.royalty_amount).HasColumnType("numeric");

                entity.Property(e => e.shipping_instruction_date).HasColumnType("date");

                entity.Property(e => e.shipping_instruction_number).IsRequired();

                entity.HasOne(d => d.despatch_order_)
                    .WithMany(p => p.shipping_instruction)
                    .HasForeignKey(d => d.despatch_order_id)
                    .HasConstraintName("fk_despatch_order_despatch_order_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.shipping_instruction)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<shipping_instruction_asuransi>(entity =>
            {
                entity.Property(e => e.barge_id).IsRequired();

                entity.Property(e => e.shipping_instruction_id).IsRequired();

                entity.HasOne(d => d.barge_)
                    .WithMany(p => p.shipping_instruction_asuransi)
                    .HasForeignKey(d => d.barge_id)
                    .HasConstraintName("fk_barge_barge_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.shipping_instruction_asuransi)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.shipping_instruction_)
                    .WithMany(p => p.shipping_instruction_asuransi)
                    .HasForeignKey(d => d.shipping_instruction_id)
                    .HasConstraintName("fk_shipping_instruction_shipping_instruction_id");
            });

            modelBuilder.Entity<shipping_instruction_attachment>(entity =>
            {
                entity.Property(e => e.shipping_instruction_id).IsRequired();

                entity.HasOne(d => d.shipping_instruction_)
                    .WithMany(p => p.shipping_instruction_attachment)
                    .HasForeignKey(d => d.shipping_instruction_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("shipping_instruction_attachment_fk");
            });

            modelBuilder.Entity<shipping_instruction_detail_survey>(entity =>
            {
                entity.Property(e => e.master_list_id).IsRequired();

                entity.Property(e => e.shipping_instruction_id).IsRequired();

                entity.HasOne(d => d.master_list_)
                    .WithMany(p => p.shipping_instruction_detail_survey)
                    .HasForeignKey(d => d.master_list_id)
                    .HasConstraintName("fk_master_list_master_list_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.shipping_instruction_detail_survey)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.shipping_instruction_)
                    .WithMany(p => p.shipping_instruction_detail_survey)
                    .HasForeignKey(d => d.shipping_instruction_id)
                    .HasConstraintName("fk_shipping_instruction_shipping_instruction_id");
            });

            modelBuilder.Entity<shipping_instruction_detail_survey_document>(entity =>
            {
                entity.Property(e => e.master_list_id).IsRequired();

                entity.Property(e => e.number_of_copies).HasColumnType("numeric");

                entity.Property(e => e.number_of_original).HasColumnType("numeric");

                entity.Property(e => e.shipping_instruction_id).IsRequired();

                entity.HasOne(d => d.master_list_)
                    .WithMany(p => p.shipping_instruction_detail_survey_document)
                    .HasForeignKey(d => d.master_list_id)
                    .HasConstraintName("fk_master_list_master_list_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.shipping_instruction_detail_survey_document)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.shipping_instruction_)
                    .WithMany(p => p.shipping_instruction_detail_survey_document)
                    .HasForeignKey(d => d.shipping_instruction_id)
                    .HasConstraintName("fk_shipping_instruction_shipping_instruction_id");
            });

            modelBuilder.Entity<shipping_instruction_document_agent>(entity =>
            {
                entity.Property(e => e.master_list_id).IsRequired();

                entity.Property(e => e.shipping_instruction_id).IsRequired();

                entity.HasOne(d => d.master_list_)
                    .WithMany(p => p.shipping_instruction_document_agent)
                    .HasForeignKey(d => d.master_list_id)
                    .HasConstraintName("fk_master_list_master_list_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.shipping_instruction_document_agent)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.shipping_instruction_)
                    .WithMany(p => p.shipping_instruction_document_agent)
                    .HasForeignKey(d => d.shipping_instruction_id)
                    .HasConstraintName("fk_shipping_instruction_shipping_instruction_id");
            });

            modelBuilder.Entity<shipping_instruction_pekerjaan_agent>(entity =>
            {
                entity.Property(e => e.master_list_id).IsRequired();

                entity.Property(e => e.shipping_instruction_id).IsRequired();

                entity.HasOne(d => d.master_list_)
                    .WithMany(p => p.shipping_instruction_pekerjaan_agent)
                    .HasForeignKey(d => d.master_list_id)
                    .HasConstraintName("fk_master_list_master_list_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.shipping_instruction_pekerjaan_agent)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.shipping_instruction_)
                    .WithMany(p => p.shipping_instruction_pekerjaan_agent)
                    .HasForeignKey(d => d.shipping_instruction_id)
                    .HasConstraintName("fk_shipping_instruction_shipping_instruction_id");
            });

            modelBuilder.Entity<shipping_instruction_stevedoring>(entity =>
            {
                entity.Property(e => e.barge_id).IsRequired();

                entity.Property(e => e.cargo).HasColumnType("numeric");

                entity.Property(e => e.port_location_id).IsRequired();

                entity.Property(e => e.shipping_instruction_id).IsRequired();

                entity.HasOne(d => d.barge_)
                    .WithMany(p => p.shipping_instruction_stevedoring)
                    .HasForeignKey(d => d.barge_id)
                    .HasConstraintName("fk_barge_barge_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.shipping_instruction_stevedoring)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.port_location_)
                    .WithMany(p => p.shipping_instruction_stevedoring)
                    .HasForeignKey(d => d.port_location_id)
                    .HasConstraintName("fk_port_location_port_location_id");

                entity.HasOne(d => d.shipping_instruction_)
                    .WithMany(p => p.shipping_instruction_stevedoring)
                    .HasForeignKey(d => d.shipping_instruction_id)
                    .HasConstraintName("fk_shipping_instruction_shipping_instruction_id");
            });

            modelBuilder.Entity<shipping_instruction_to_company>(entity =>
            {
                entity.Property(e => e.contractor_id).IsRequired();

                entity.Property(e => e.master_list_id).IsRequired();

                entity.Property(e => e.shipping_instruction_id).IsRequired();

                entity.HasOne(d => d.contractor_)
                    .WithMany(p => p.shipping_instruction_to_company)
                    .HasForeignKey(d => d.contractor_id)
                    .HasConstraintName("fk_contractor_contractor_id");

                entity.HasOne(d => d.master_list_)
                    .WithMany(p => p.shipping_instruction_to_company)
                    .HasForeignKey(d => d.master_list_id)
                    .HasConstraintName("fk_master_list_master_list_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.shipping_instruction_to_company)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.shipping_instruction_)
                    .WithMany(p => p.shipping_instruction_to_company)
                    .HasForeignKey(d => d.shipping_instruction_id)
                    .HasConstraintName("fk_shipping_instruction_shipping_instruction_id");
            });

            modelBuilder.Entity<shipping_instruction_tug_boat>(entity =>
            {
                entity.Property(e => e.barge_id).IsRequired();

                entity.Property(e => e.shipping_instruction_id).IsRequired();

                entity.HasOne(d => d.barge_)
                    .WithMany(p => p.shipping_instruction_tug_boat)
                    .HasForeignKey(d => d.barge_id)
                    .HasConstraintName("fk_barge_barge_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.shipping_instruction_tug_boat)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.shipping_instruction_)
                    .WithMany(p => p.shipping_instruction_tug_boat)
                    .HasForeignKey(d => d.shipping_instruction_id)
                    .HasConstraintName("fk_shipping_instruction_shipping_instruction_id");
            });

            modelBuilder.Entity<shipping_load_unload_document>(entity =>
            {
                entity.Property(e => e.shipping_transaction_id).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.shipping_load_unload_document)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.shipping_transaction_)
                    .WithMany(p => p.shipping_load_unload_document)
                    .HasForeignKey(d => d.shipping_transaction_id)
                    .HasConstraintName("fk_shipping_transaction_shipping_transaction_id");
            });

            modelBuilder.Entity<shipping_loading_detail>(entity =>
            {
                entity.Property(e => e.presentage).HasColumnType("numeric");

                entity.Property(e => e.quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<shipping_program>(entity =>
            {
                entity.Property(e => e.product_1).HasColumnType("numeric");

                entity.Property(e => e.product_10).HasColumnType("numeric");

                entity.Property(e => e.product_11).HasColumnType("numeric");

                entity.Property(e => e.product_12).HasColumnType("numeric");

                entity.Property(e => e.product_13).HasColumnType("numeric");

                entity.Property(e => e.product_14).HasColumnType("numeric");

                entity.Property(e => e.product_15).HasColumnType("numeric");

                entity.Property(e => e.product_16).HasColumnType("numeric");

                entity.Property(e => e.product_17).HasColumnType("numeric");

                entity.Property(e => e.product_18).HasColumnType("numeric");

                entity.Property(e => e.product_19).HasColumnType("numeric");

                entity.Property(e => e.product_2).HasColumnType("numeric");

                entity.Property(e => e.product_20).HasColumnType("numeric");

                entity.Property(e => e.product_21).HasColumnType("numeric");

                entity.Property(e => e.product_22).HasColumnType("numeric");

                entity.Property(e => e.product_23).HasColumnType("numeric");

                entity.Property(e => e.product_24).HasColumnType("numeric");

                entity.Property(e => e.product_25).HasColumnType("numeric");

                entity.Property(e => e.product_26).HasColumnType("numeric");

                entity.Property(e => e.product_27).HasColumnType("numeric");

                entity.Property(e => e.product_28).HasColumnType("numeric");

                entity.Property(e => e.product_29).HasColumnType("numeric");

                entity.Property(e => e.product_3).HasColumnType("numeric");

                entity.Property(e => e.product_30).HasColumnType("numeric");

                entity.Property(e => e.product_31).HasColumnType("numeric");

                entity.Property(e => e.product_32).HasColumnType("numeric");

                entity.Property(e => e.product_33).HasColumnType("numeric");

                entity.Property(e => e.product_34).HasColumnType("numeric");

                entity.Property(e => e.product_35).HasColumnType("numeric");

                entity.Property(e => e.product_4).HasColumnType("numeric");

                entity.Property(e => e.product_5).HasColumnType("numeric");

                entity.Property(e => e.product_6).HasColumnType("numeric");

                entity.Property(e => e.product_7).HasColumnType("numeric");

                entity.Property(e => e.product_8).HasColumnType("numeric");

                entity.Property(e => e.product_9).HasColumnType("numeric");

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.shipping_program)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<shipping_program_detail>(entity =>
            {
                entity.Property(e => e.presentage).HasColumnType("numeric");

                entity.Property(e => e.product_1).HasColumnType("numeric");

                entity.Property(e => e.product_10).HasColumnType("numeric");

                entity.Property(e => e.product_11).HasColumnType("numeric");

                entity.Property(e => e.product_12).HasColumnType("numeric");

                entity.Property(e => e.product_13).HasColumnType("numeric");

                entity.Property(e => e.product_14).HasColumnType("numeric");

                entity.Property(e => e.product_15).HasColumnType("numeric");

                entity.Property(e => e.product_16).HasColumnType("numeric");

                entity.Property(e => e.product_17).HasColumnType("numeric");

                entity.Property(e => e.product_18).HasColumnType("numeric");

                entity.Property(e => e.product_19).HasColumnType("numeric");

                entity.Property(e => e.product_2).HasColumnType("numeric");

                entity.Property(e => e.product_20).HasColumnType("numeric");

                entity.Property(e => e.product_21).HasColumnType("numeric");

                entity.Property(e => e.product_22).HasColumnType("numeric");

                entity.Property(e => e.product_23).HasColumnType("numeric");

                entity.Property(e => e.product_24).HasColumnType("numeric");

                entity.Property(e => e.product_25).HasColumnType("numeric");

                entity.Property(e => e.product_3).HasColumnType("numeric");

                entity.Property(e => e.product_4).HasColumnType("numeric");

                entity.Property(e => e.product_5).HasColumnType("numeric");

                entity.Property(e => e.product_6).HasColumnType("numeric");

                entity.Property(e => e.product_7).HasColumnType("numeric");

                entity.Property(e => e.product_8).HasColumnType("numeric");

                entity.Property(e => e.product_9).HasColumnType("numeric");

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.shipping_program_detail)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.shipping_program_)
                    .WithMany(p => p.shipping_program_detail)
                    .HasForeignKey(d => d.shipping_program_id)
                    .HasConstraintName("shipping_program_detail_fk");
            });

            modelBuilder.Entity<shipping_transaction>(entity =>
            {
                entity.HasIndex(e => e.accounting_period_id)
                    .HasName("ix_shipping_transaction_accounting_period_id");

                entity.HasIndex(e => e.process_flow_id)
                    .HasName("ix_shipping_transaction_process_flow_id");

                entity.HasIndex(e => e.product_id)
                    .HasName("ix_shipping_transaction_product_id");

                entity.HasIndex(e => e.survey_id)
                    .HasName("ix_shipping_transaction_survey_id");

                entity.HasIndex(e => e.uom_id)
                    .HasName("ix_shipping_transaction_uom_id");

                entity.HasIndex(e => new { e.organization_id, e.transaction_number })
                    .HasName("uk_shipping_transaction")
                    .IsUnique();

                entity.Property(e => e.bill_lading_date).HasColumnType("time with time zone");

                entity.Property(e => e.distance).HasColumnType("numeric");

                entity.Property(e => e.hour_usage).HasColumnType("numeric");

                entity.Property(e => e.integration_status).HasDefaultValueSql("'NOT APPROVED'::text");

                entity.Property(e => e.original_quantity).HasColumnType("numeric");

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.ship_location_id).IsRequired();

                entity.Property(e => e.stowage_plan_actual).HasColumnType("numeric");

                entity.Property(e => e.transaction_number).IsRequired();

                entity.Property(e => e.uom_id).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.shipping_transaction)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<shipping_transaction_detail>(entity =>
            {
                entity.HasIndex(e => e.transaction_number)
                    .HasName("uk_shipping_transaction_detail")
                    .IsUnique();

                entity.Property(e => e.final_quantity).HasColumnType("numeric");

                entity.Property(e => e.hour_usage).HasColumnType("numeric");

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.shipping_transaction_id).IsRequired();

                entity.Property(e => e.transaction_number).IsRequired();

                entity.Property(e => e.uom_id).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.shipping_transaction_detail)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.shipping_transaction_)
                    .WithMany(p => p.shipping_transaction_detail)
                    .HasForeignKey(d => d.shipping_transaction_id)
                    .HasConstraintName("fk_shipping_transaction_shipping_transaction_id");
            });

            modelBuilder.Entity<shipping_transaction_lq>(entity =>
            {
                entity.HasIndex(e => e.transaction_number)
                    .HasName("uk_shipping_transaction_lq")
                    .IsUnique();

                entity.Property(e => e.final_quantity).HasColumnType("numeric");

                entity.Property(e => e.hour_usage).HasColumnType("numeric");

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.shipping_transaction_id).IsRequired();

                entity.Property(e => e.transaction_number).IsRequired();
            });

            modelBuilder.Entity<sils>(entity =>
            {
                entity.HasIndex(e => e.barge_id)
                    .HasName("ix_blending_plan_survey_id_1");

                entity.HasIndex(e => e.barge_rotation_id)
                    .HasName("ix_blending_plan_accounting_period_id_1");

                entity.HasIndex(e => e.destination_location)
                    .HasName("ix_blending_plan_uom_id_1");

                entity.HasIndex(e => e.master_list_id)
                    .HasName("ix_blending_plan_process_flow_id_1");

                entity.HasIndex(e => e.tug_id)
                    .HasName("ix_blending_plan_product_id_1");

                entity.Property(e => e.analyte_1).HasColumnType("numeric");

                entity.Property(e => e.analyte_2).HasColumnType("numeric");

                entity.Property(e => e.barge_loading_number).IsRequired();

                entity.Property(e => e.belt_scale).HasColumnType("numeric");

                entity.Property(e => e.chemical_consumption).HasColumnType("numeric");

                entity.Property(e => e.draft_scale).HasColumnType("numeric");

                entity.Property(e => e.tug_id).IsRequired();

                entity.Property(e => e.water_consumption).HasColumnType("numeric");
            });

            modelBuilder.Entity<sils_detail>(entity =>
            {
                entity.Property(e => e.crew_id).IsRequired();

                entity.Property(e => e.flow_meter).HasColumnType("numeric");

                entity.Property(e => e.progress).HasColumnType("numeric");

                entity.Property(e => e.progressive_total).HasColumnType("numeric");

                entity.Property(e => e.sils_id).IsRequired();

                entity.Property(e => e.total).HasColumnType("numeric");

                entity.Property(e => e.total_down_time).HasColumnType("numeric");
            });

            modelBuilder.Entity<sils_nplct>(entity =>
            {
                entity.Property(e => e.bw_end).HasColumnType("numeric");

                entity.Property(e => e.bw_start).HasColumnType("numeric");

                entity.Property(e => e.gross_loading_rate).HasColumnType("numeric");

                entity.Property(e => e.gross_loading_time).HasColumnType("numeric");

                entity.Property(e => e.nett_loading_rate).HasColumnType("numeric");

                entity.Property(e => e.nett_loading_time).HasColumnType("numeric");

                entity.Property(e => e.supervisor).HasColumnType("numeric");

                entity.Property(e => e.tonnage_draft).HasColumnType("numeric");

                entity.Property(e => e.tonnage_scale).HasColumnType("numeric");

                entity.Property(e => e.total_on_board).HasColumnType("numeric");
            });

            modelBuilder.Entity<sils_nplct_detail>(entity =>
            {
                entity.Property(e => e.gross_loading_rate).HasColumnType("numeric");

                entity.Property(e => e.gross_loading_time).HasColumnType("numeric");

                entity.Property(e => e.nett_loading_rate).HasColumnType("numeric");

                entity.Property(e => e.nett_loading_time).HasColumnType("numeric");

                entity.Property(e => e.total_down_time).HasColumnType("numeric");

                entity.Property(e => e.total_flow_time).HasColumnType("numeric");

                entity.Property(e => e.total_on_board).HasColumnType("numeric");

                entity.Property(e => e.total_shift).HasColumnType("numeric");
            });

            modelBuilder.Entity<sils_nplct_subdetail>(entity =>
            {
                entity.Property(e => e.hatch_this_run).HasColumnType("numeric");

                entity.Property(e => e.hatch_total).HasColumnType("numeric");

                entity.Property(e => e.progressive_total).HasColumnType("numeric");

                entity.Property(e => e.total_time).HasColumnType("numeric");
            });

            modelBuilder.Entity<sils_shipment_nplct_details>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<sils_unloading>(entity =>
            {
                entity.Property(e => e.analyte_1).HasColumnType("numeric");

                entity.Property(e => e.analyte_2).HasColumnType("numeric");

                entity.Property(e => e.analyte_3).HasColumnType("numeric");

                entity.Property(e => e.belt_scale).HasColumnType("numeric");

                entity.Property(e => e.draft_scale).HasColumnType("numeric");

                entity.Property(e => e.site_scale).HasColumnType("numeric");

                entity.Property(e => e.tonnage).HasColumnType("numeric");
            });

            modelBuilder.Entity<sils_unloading_detail>(entity =>
            {
                entity.Property(e => e.bypass).HasColumnType("numeric");

                entity.Property(e => e.gross_loading_rate).HasColumnType("numeric");

                entity.Property(e => e.gross_loading_time).HasColumnType("numeric");

                entity.Property(e => e.nett_loading_rate).HasColumnType("numeric");

                entity.Property(e => e.nett_loading_time).HasColumnType("numeric");

                entity.Property(e => e.total_down_time).HasColumnType("numeric");

                entity.Property(e => e.total_flow_time).HasColumnType("numeric");
            });

            modelBuilder.Entity<sils_unloading_nplct_details>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<sils_unloading_subdetail>(entity =>
            {
                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<slippery>(entity =>
            {
                entity.Property(e => e.business_area_id).IsRequired();

                entity.Property(e => e.rainfall_delay).HasColumnType("numeric");

                entity.Property(e => e.slippery_value).HasColumnType("numeric");

                entity.HasOne(d => d.business_area_)
                    .WithMany(p => p.slippery)
                    .HasForeignKey(d => d.business_area_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("slippery_business_area_id_fkey");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.slippery)
                    .HasForeignKey(d => d.organization_id)
                    .HasConstraintName("slippery_organization_id_fkey");
            });

            modelBuilder.Entity<slippery_schema>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.dates).HasColumnType("date");

                entity.Property(e => e.slippery_value).HasColumnType("numeric");
            });

            modelBuilder.Entity<sof>(entity =>
            {
                entity.Property(e => e.sof_number).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.sof)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.vessel_)
                    .WithMany(p => p.sof)
                    .HasForeignKey(d => d.vessel_id)
                    .HasConstraintName("fk_vessel_vessel_id");
            });

            modelBuilder.Entity<sof_detail>(entity =>
            {
                entity.Property(e => e.percentage).HasColumnType("numeric");

                entity.Property(e => e.sof_id).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.sof_detail)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.sof_)
                    .WithMany(p => p.sof_detail)
                    .HasForeignKey(d => d.sof_id)
                    .HasConstraintName("fk_sof_sof_id");
            });

            modelBuilder.Entity<sof_document>(entity =>
            {
                entity.Property(e => e.sof_id).IsRequired();
            });

            modelBuilder.Entity<sr_actual_all>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_mine).HasColumnType("numeric");

                entity.Property(e => e.bit_waste).HasColumnType("numeric");

                entity.Property(e => e.eco_mine).HasColumnType("numeric");

                entity.Property(e => e.eco_waste).HasColumnType("numeric");

                entity.Property(e => e.sarongga_mine).HasColumnType("numeric");

                entity.Property(e => e.sarongga_waste).HasColumnType("numeric");

                entity.Property(e => e.total_mine).HasColumnType("numeric");

                entity.Property(e => e.total_waste).HasColumnType("numeric");
            });

            modelBuilder.Entity<sr_budget_all>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_mine).HasColumnType("numeric");

                entity.Property(e => e.bit_waste).HasColumnType("numeric");

                entity.Property(e => e.eco_mine).HasColumnType("numeric");

                entity.Property(e => e.eco_waste).HasColumnType("numeric");

                entity.Property(e => e.sarongga_mine).HasColumnType("numeric");

                entity.Property(e => e.sarongga_waste).HasColumnType("numeric");

                entity.Property(e => e.total_mine).HasColumnType("numeric");

                entity.Property(e => e.total_waste).HasColumnType("numeric");
            });

            modelBuilder.Entity<sr_daily_actual>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_mine).HasColumnType("numeric");

                entity.Property(e => e.bit_waste).HasColumnType("numeric");

                entity.Property(e => e.eco_mine).HasColumnType("numeric");

                entity.Property(e => e.eco_waste).HasColumnType("numeric");

                entity.Property(e => e.sarongga_mine).HasColumnType("numeric");

                entity.Property(e => e.sarongga_waste).HasColumnType("numeric");

                entity.Property(e => e.total_mine).HasColumnType("numeric");

                entity.Property(e => e.total_waste).HasColumnType("numeric");
            });

            modelBuilder.Entity<sr_daily_budget>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_mine).HasColumnType("numeric");

                entity.Property(e => e.bit_waste).HasColumnType("numeric");

                entity.Property(e => e.eco_mine).HasColumnType("numeric");

                entity.Property(e => e.eco_waste).HasColumnType("numeric");

                entity.Property(e => e.sarongga_mine).HasColumnType("numeric");

                entity.Property(e => e.sarongga_waste).HasColumnType("numeric");

                entity.Property(e => e.total_mine).HasColumnType("numeric");

                entity.Property(e => e.total_waste).HasColumnType("numeric");
            });

            modelBuilder.Entity<sr_mtd_actual>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_mine).HasColumnType("numeric");

                entity.Property(e => e.bit_waste).HasColumnType("numeric");

                entity.Property(e => e.eco_mine).HasColumnType("numeric");

                entity.Property(e => e.eco_waste).HasColumnType("numeric");

                entity.Property(e => e.sarongga_mine).HasColumnType("numeric");

                entity.Property(e => e.sarongga_waste).HasColumnType("numeric");

                entity.Property(e => e.total_mine).HasColumnType("numeric");

                entity.Property(e => e.total_waste).HasColumnType("numeric");
            });

            modelBuilder.Entity<sr_mtd_budget>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_mine).HasColumnType("numeric");

                entity.Property(e => e.bit_waste).HasColumnType("numeric");

                entity.Property(e => e.eco_mine).HasColumnType("numeric");

                entity.Property(e => e.eco_waste).HasColumnType("numeric");

                entity.Property(e => e.sarongga_mine).HasColumnType("numeric");

                entity.Property(e => e.sarongga_waste).HasColumnType("numeric");

                entity.Property(e => e.total_mine).HasColumnType("numeric");

                entity.Property(e => e.total_waste).HasColumnType("numeric");
            });

            modelBuilder.Entity<sr_ytd_actual>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_mine).HasColumnType("numeric");

                entity.Property(e => e.bit_waste).HasColumnType("numeric");

                entity.Property(e => e.eco_mine).HasColumnType("numeric");

                entity.Property(e => e.eco_waste).HasColumnType("numeric");

                entity.Property(e => e.sarongga_mine).HasColumnType("numeric");

                entity.Property(e => e.sarongga_waste).HasColumnType("numeric");

                entity.Property(e => e.total_mine).HasColumnType("numeric");

                entity.Property(e => e.total_waste).HasColumnType("numeric");
            });

            modelBuilder.Entity<sr_ytd_budget>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_mine).HasColumnType("numeric");

                entity.Property(e => e.bit_waste).HasColumnType("numeric");

                entity.Property(e => e.eco_mine).HasColumnType("numeric");

                entity.Property(e => e.eco_waste).HasColumnType("numeric");

                entity.Property(e => e.sarongga_mine).HasColumnType("numeric");

                entity.Property(e => e.sarongga_waste).HasColumnType("numeric");

                entity.Property(e => e.total_mine).HasColumnType("numeric");

                entity.Property(e => e.total_waste).HasColumnType("numeric");
            });

            modelBuilder.Entity<standard_cost_mining>(entity =>
            {
                entity.Property(e => e.end_date).HasColumnType("date");

                entity.Property(e => e.standard_cost_name).IsRequired();

                entity.Property(e => e.start_date).HasColumnType("date");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.standard_cost_mining)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<standard_cost_mining_detail>(entity =>
            {
                entity.Property(e => e.coefficient).HasColumnType("numeric");

                entity.Property(e => e.distance).HasColumnType("numeric");

                entity.Property(e => e.elevation).HasColumnType("numeric");

                entity.Property(e => e.fixed_cost).HasColumnType("numeric");

                entity.Property(e => e.standard_cost_mining_id).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.standard_cost_mining_detail)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.standard_cost_mining_)
                    .WithMany(p => p.standard_cost_mining_detail)
                    .HasForeignKey(d => d.standard_cost_mining_id)
                    .HasConstraintName("fk_standard_cost_mining_standard_cost_mining_id");
            });

            modelBuilder.Entity<stock_location>(entity =>
            {
                entity.HasIndex(e => e.organization_id)
                    .HasName("ix_stock_location_organization_id");

                entity.HasIndex(e => new { e.organization_id, e.stock_location_name })
                    .HasName("uk_stock_location")
                    .IsUnique();

                entity.Property(e => e.closing_date).HasColumnType("date");

                entity.Property(e => e.current_stock).HasColumnType("numeric");

                entity.Property(e => e.maximum_capacity).HasColumnType("numeric");

                entity.Property(e => e.minimum_capacity).HasColumnType("numeric");

                entity.Property(e => e.opening_date).HasColumnType("date");

                entity.Property(e => e.target_capacity).HasColumnType("numeric");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.stock_location)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.product_)
                    .WithMany(p => p.stock_location)
                    .HasForeignKey(d => d.product_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_product_product_id");

                entity.HasOne(d => d.uom_)
                    .WithMany(p => p.stock_location)
                    .HasForeignKey(d => d.uom_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_uom_uom_id");
            });

            modelBuilder.Entity<stock_nplct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.qty).HasColumnType("numeric");
            });

            modelBuilder.Entity<stock_nplct_all_transaction>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.nplct_inventory).HasColumnType("numeric");
            });

            modelBuilder.Entity<stock_nplct_discharge>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.daily_stock).HasColumnType("numeric");

                entity.Property(e => e.last_in_date).HasColumnType("date");

                entity.Property(e => e.last_out_date).HasColumnType("date");

                entity.Property(e => e.stock_in).HasColumnType("numeric");

                entity.Property(e => e.stock_out).HasColumnType("numeric");
            });

            modelBuilder.Entity<stock_perunit_production_aims>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.avg_stock).HasColumnType("numeric");

                entity.Property(e => e.dates).HasColumnType("date");

                entity.Property(e => e.stock).HasColumnType("numeric");
            });

            modelBuilder.Entity<stock_production_aims>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.avg_stock).HasColumnType("numeric");

                entity.Property(e => e.dates).HasColumnType("date");

                entity.Property(e => e.stock).HasColumnType("numeric");
            });

            modelBuilder.Entity<stock_rom_hopper>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.qty).HasColumnType("numeric");
            });

            modelBuilder.Entity<stock_rom_pit>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.qty).HasColumnType("numeric");
            });

            modelBuilder.Entity<stock_rom_port>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.qty).HasColumnType("numeric");
            });

            modelBuilder.Entity<stock_state>(entity =>
            {
                entity.HasIndex(e => e.stock_location_id)
                    .HasName("ix_stock_state_stock_location_id");

                entity.HasIndex(e => e.transaction_datetime)
                    .HasName("ix_stock_state_transaction_datetime")
                    .HasNullSortOrder(new[] { NullSortOrder.NullsLast })
                    .HasSortOrder(new[] { SortOrder.Descending });

                entity.HasIndex(e => e.transaction_id)
                    .HasName("ix_stock_state_transaction_id");

                entity.HasIndex(e => new { e.stock_location_id, e.transaction_id })
                    .HasName("uk_stock_state")
                    .IsUnique();

                entity.Property(e => e.qty_adjustment).HasColumnType("numeric");

                entity.Property(e => e.qty_adjustment_provisional).HasColumnType("numeric");

                entity.Property(e => e.qty_closing).HasColumnType("numeric");

                entity.Property(e => e.qty_closing_provisional).HasColumnType("numeric");

                entity.Property(e => e.qty_in).HasColumnType("numeric");

                entity.Property(e => e.qty_in_provisional).HasColumnType("numeric");

                entity.Property(e => e.qty_opening).HasColumnType("numeric");

                entity.Property(e => e.qty_opening_provisional).HasColumnType("numeric");

                entity.Property(e => e.qty_out).HasColumnType("numeric");

                entity.Property(e => e.qty_out_provisional).HasColumnType("numeric");

                entity.Property(e => e.qty_survey).HasColumnType("numeric");

                entity.Property(e => e.qty_survey_provisional).HasColumnType("numeric");

                entity.Property(e => e.stock_location_id).IsRequired();

                entity.Property(e => e.transaction_id).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.stock_state)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<stock_state_analyte>(entity =>
            {
                entity.HasIndex(e => new { e.stock_state_id, e.analyte_id })
                    .HasName("uk_stock_state_analyte")
                    .IsUnique();

                entity.Property(e => e.analyte_id).IsRequired();

                entity.Property(e => e.analyte_value).HasColumnType("numeric");

                entity.Property(e => e.moving_avg_value).HasColumnType("numeric");

                entity.Property(e => e.stock_state_id).IsRequired();

                entity.Property(e => e.weighted_value).HasColumnType("numeric");

                entity.HasOne(d => d.analyte_)
                    .WithMany(p => p.stock_state_analyte)
                    .HasForeignKey(d => d.analyte_id)
                    .HasConstraintName("fk_analyte_analyte_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.stock_state_analyte)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<stockpile_location>(entity =>
            {
                entity.HasIndex(e => e.organization_id)
                    .HasName("ix_stockpile_location_organization_id");

                entity.HasIndex(e => new { e.organization_id, e.stockpile_location_code })
                    .HasName("uk2_stockpile_location")
                    .IsUnique();

                entity.HasIndex(e => new { e.business_area_id, e.parent_stock_location_id, e.stock_location_name })
                    .HasName("uk1_stockpile_location")
                    .IsUnique();

                entity.Property(e => e.closing_date).HasColumnType("date");

                entity.Property(e => e.current_stock).HasColumnType("numeric");

                entity.Property(e => e.maximum_capacity).HasColumnType("numeric");

                entity.Property(e => e.minimum_capacity).HasColumnType("numeric");

                entity.Property(e => e.opening_date).HasColumnType("date");

                entity.Property(e => e.stockpile_location_code).IsRequired();

                entity.Property(e => e.target_capacity).HasColumnType("numeric");

                entity.HasOne(d => d.business_area_)
                    .WithMany(p => p.stockpile_location)
                    .HasForeignKey(d => d.business_area_id)
                    .HasConstraintName("stockpile_location_business_area_id_fkey");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.stockpile_location)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<stockpile_location_detail>(entity =>
            {
                entity.Property(e => e.presentage).HasColumnType("numeric");

                entity.Property(e => e.quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<stockpile_state>(entity =>
            {
                entity.HasIndex(e => e.stockpile_location_id)
                    .HasName("ix_stockpile_state_stockpile_location_id");

                entity.HasIndex(e => e.transaction_datetime)
                    .HasName("ix_stockpile_state_transaction_datetime")
                    .HasNullSortOrder(new[] { NullSortOrder.NullsLast })
                    .HasSortOrder(new[] { SortOrder.Descending });

                entity.HasIndex(e => e.transaction_id)
                    .HasName("ix_stockpile_state_transaction_id");

                entity.HasIndex(e => new { e.stockpile_location_id, e.transaction_id })
                    .HasName("uk_stockpile_state")
                    .IsUnique();

                entity.Property(e => e.qty_adjustment).HasColumnType("numeric");

                entity.Property(e => e.qty_adjustment_provisional).HasColumnType("numeric");

                entity.Property(e => e.qty_closing).HasColumnType("numeric");

                entity.Property(e => e.qty_closing_provisional).HasColumnType("numeric");

                entity.Property(e => e.qty_in).HasColumnType("numeric");

                entity.Property(e => e.qty_in_provisional).HasColumnType("numeric");

                entity.Property(e => e.qty_opening).HasColumnType("numeric");

                entity.Property(e => e.qty_out).HasColumnType("numeric");

                entity.Property(e => e.qty_out_provisional).HasColumnType("numeric");

                entity.Property(e => e.stockpile_location_id).IsRequired();

                entity.Property(e => e.transaction_id).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.stockpile_state)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<survey>(entity =>
            {
                entity.HasIndex(e => e.organization_id)
                    .HasName("ix_survey_organization_id");

                entity.HasIndex(e => e.stock_location_id)
                    .HasName("ix_survey_stock_location_id");

                entity.HasIndex(e => new { e.organization_id, e.survey_number })
                    .HasName("uk_survey")
                    .IsUnique();

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.stock_location_id).IsRequired();

                entity.Property(e => e.survey_number).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.survey)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<survey_analyte>(entity =>
            {
                entity.HasIndex(e => new { e.survey_id, e.analyte_id })
                    .HasName("uk_survey_analyte")
                    .IsUnique();

                entity.Property(e => e.analyte_id).IsRequired();

                entity.Property(e => e.analyte_value).HasColumnType("numeric");

                entity.Property(e => e.survey_id).IsRequired();

                entity.HasOne(d => d.analyte_)
                    .WithMany(p => p.survey_analyte)
                    .HasForeignKey(d => d.analyte_id)
                    .HasConstraintName("fk_analyte_analyte_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.survey_analyte)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<survey_detail>(entity =>
            {
                entity.Property(e => e.distance).HasColumnType("numeric");

                entity.Property(e => e.elevation).HasColumnType("numeric");

                entity.Property(e => e.percentage).HasColumnType("numeric");

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.survey_id).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.survey_detail)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.survey_)
                    .WithMany(p => p.survey_detail)
                    .HasForeignKey(d => d.survey_id)
                    .HasConstraintName("fk_survey_survey_id");
            });

            modelBuilder.Entity<sync_log>(entity =>
            {
                entity.Property(e => e.id).HasDefaultValueSql("ufn_new_id()");

                entity.Property(e => e.date_time).HasDefaultValueSql("now()");
            });

            modelBuilder.Entity<tax>(entity =>
            {
                entity.Property(e => e.calculation_sign)
                    .HasColumnType("numeric")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.rate).HasColumnType("numeric");
            });

            modelBuilder.Entity<team>(entity =>
            {
                entity.HasIndex(e => e.created_by)
                    .HasName("ix_team_created_by")
                    .IncludeProperties(new[] { "created_on" });

                entity.HasIndex(e => e.entity_id)
                    .HasName("ix_team_entity_id");

                entity.HasIndex(e => e.is_active)
                    .HasName("ix_team_is_active")
                    .IncludeProperties(new[] { "owner_id", "organization_id" });

                entity.HasIndex(e => e.modified_by)
                    .HasName("ix_team_modified_by")
                    .IncludeProperties(new[] { "modified_on" });

                entity.HasIndex(e => e.organization_id)
                    .HasName("ix_team_organization_id")
                    .IncludeProperties(new[] { "is_active", "owner_id" });

                entity.HasIndex(e => e.owner_id)
                    .HasName("ix_team_owner_id")
                    .IncludeProperties(new[] { "is_active", "organization_id" });

                entity.HasIndex(e => new { e.organization_id, e.team_name })
                    .HasName("uk_team")
                    .IsUnique();

                entity.Property(e => e.organization_id).IsRequired();

                entity.Property(e => e.owner_id).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.team)
                    .HasForeignKey(d => d.organization_id)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<team_member>(entity =>
            {
                entity.HasIndex(e => new { e.team_id, e.application_user_id })
                    .HasName("uk_team_member")
                    .IsUnique();

                entity.Property(e => e.application_user_id).IsRequired();

                entity.Property(e => e.organization_id).IsRequired();

                entity.Property(e => e.owner_id).IsRequired();

                entity.Property(e => e.team_id).IsRequired();

                entity.HasOne(d => d.application_user_)
                    .WithMany(p => p.team_member)
                    .HasForeignKey(d => d.application_user_id)
                    .HasConstraintName("fk_application_user_application_user_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.team_member)
                    .HasForeignKey(d => d.organization_id)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.team_)
                    .WithMany(p => p.team_member)
                    .HasForeignKey(d => d.team_id)
                    .HasConstraintName("fk_team_team_id");
            });

            modelBuilder.Entity<team_role>(entity =>
            {
                entity.HasIndex(e => e.created_by)
                    .HasName("ix_team_role_created_by")
                    .IncludeProperties(new[] { "created_on" });

                entity.HasIndex(e => e.entity_id)
                    .HasName("ix_team_role_entity_id");

                entity.HasIndex(e => e.is_active)
                    .HasName("ix_team_role_is_active")
                    .IncludeProperties(new[] { "owner_id", "organization_id" });

                entity.HasIndex(e => e.modified_by)
                    .HasName("ix_team_role_modified_by")
                    .IncludeProperties(new[] { "modified_on" });

                entity.HasIndex(e => e.organization_id)
                    .HasName("ix_team_role_organization")
                    .IncludeProperties(new[] { "is_active", "owner_id" });

                entity.HasIndex(e => e.owner_id)
                    .HasName("ix_team_role_owner_id")
                    .IncludeProperties(new[] { "is_active", "organization_id" });

                entity.HasIndex(e => new { e.team_id, e.application_role_id })
                    .HasName("uk_team_role")
                    .IsUnique();

                entity.Property(e => e.application_role_id)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.organization_id).IsRequired();

                entity.Property(e => e.owner_id).IsRequired();

                entity.Property(e => e.team_id)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.application_role_)
                    .WithMany(p => p.team_role)
                    .HasForeignKey(d => d.application_role_id)
                    .HasConstraintName("fk_application_role_application_role_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.team_role)
                    .HasForeignKey(d => d.organization_id)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.team_)
                    .WithMany(p => p.team_role)
                    .HasForeignKey(d => d.team_id)
                    .HasConstraintName("fk_team_team_id");
            });

            modelBuilder.Entity<temp_quality_mutation>(entity =>
            {
                entity.HasNoKey();

                entity.HasIndex(e => new { e.trans_date, e.stock_location_id, e.analyte_id })
                    .HasName("idx_temp_quality_mutation")
                    .HasSortOrder(new[] { SortOrder.Descending, SortOrder.Ascending, SortOrder.Ascending });
            });

            modelBuilder.Entity<temp_quality_mutation1>(entity =>
            {
                entity.HasNoKey();

                entity.HasIndex(e => new { e.trans_date, e.stock_location_id, e.analyte_id })
                    .HasName("idx_temp_quality_mutation1")
                    .HasSortOrder(new[] { SortOrder.Descending, SortOrder.Ascending, SortOrder.Ascending });
            });

            modelBuilder.Entity<temp_quality_mutation10_kmia>(entity =>
            {
                entity.HasNoKey();

                entity.HasIndex(e => new { e.trans_date, e.stock_location_id, e.analyte_id })
                    .HasName("idx_temp_quality_mutation10_kmia")
                    .HasSortOrder(new[] { SortOrder.Descending, SortOrder.Ascending, SortOrder.Ascending });
            });

            modelBuilder.Entity<temp_quality_mutation11_kmia>(entity =>
            {
                entity.HasNoKey();

                entity.HasIndex(e => new { e.trans_date, e.stock_location_id, e.analyte_id })
                    .HasName("idx_temp_quality_mutation11_kmia")
                    .HasSortOrder(new[] { SortOrder.Descending, SortOrder.Ascending, SortOrder.Ascending });
            });

            modelBuilder.Entity<temp_quality_mutation1_kmia>(entity =>
            {
                entity.HasNoKey();

                entity.HasIndex(e => new { e.trans_date, e.stock_location_id, e.analyte_id })
                    .HasName("idx_temp_quality_mutation1_kmia")
                    .HasSortOrder(new[] { SortOrder.Descending, SortOrder.Ascending, SortOrder.Ascending });
            });

            modelBuilder.Entity<temp_quality_mutation2>(entity =>
            {
                entity.HasNoKey();

                entity.HasIndex(e => new { e.trans_date, e.stock_location_id, e.analyte_id })
                    .HasName("idx_temp_quality_mutation2")
                    .HasSortOrder(new[] { SortOrder.Descending, SortOrder.Ascending, SortOrder.Ascending });
            });

            modelBuilder.Entity<temp_quality_mutation2_kmia>(entity =>
            {
                entity.HasNoKey();

                entity.HasIndex(e => new { e.trans_date, e.stock_location_id, e.analyte_id })
                    .HasName("idx_temp_quality_mutation2_kmia")
                    .HasSortOrder(new[] { SortOrder.Descending, SortOrder.Ascending, SortOrder.Ascending });
            });

            modelBuilder.Entity<temp_quality_mutation3>(entity =>
            {
                entity.HasNoKey();

                entity.HasIndex(e => new { e.trans_date, e.stock_location_id, e.analyte_id })
                    .HasName("idx_temp_quality_mutation3")
                    .HasSortOrder(new[] { SortOrder.Descending, SortOrder.Ascending, SortOrder.Ascending });
            });

            modelBuilder.Entity<temp_quality_mutation3_kmia>(entity =>
            {
                entity.HasNoKey();

                entity.HasIndex(e => new { e.trans_date, e.stock_location_id, e.analyte_id })
                    .HasName("idx_temp_quality_mutation3_kmia")
                    .HasSortOrder(new[] { SortOrder.Descending, SortOrder.Ascending, SortOrder.Ascending });
            });

            modelBuilder.Entity<temp_quality_mutation4>(entity =>
            {
                entity.HasNoKey();

                entity.HasIndex(e => new { e.trans_date, e.stock_location_id, e.analyte_id })
                    .HasName("idx_temp_quality_mutation4")
                    .HasSortOrder(new[] { SortOrder.Descending, SortOrder.Ascending, SortOrder.Ascending });
            });

            modelBuilder.Entity<temp_quality_mutation4_kmia>(entity =>
            {
                entity.HasNoKey();

                entity.HasIndex(e => new { e.trans_date, e.stock_location_id, e.analyte_id })
                    .HasName("idx_temp_quality_mutation4_kmia")
                    .HasSortOrder(new[] { SortOrder.Descending, SortOrder.Ascending, SortOrder.Ascending });
            });

            modelBuilder.Entity<temp_quality_mutation5>(entity =>
            {
                entity.HasNoKey();

                entity.HasIndex(e => new { e.trans_date, e.stock_location_id, e.analyte_id })
                    .HasName("idx_temp_quality_mutation5")
                    .HasSortOrder(new[] { SortOrder.Descending, SortOrder.Ascending, SortOrder.Ascending });
            });

            modelBuilder.Entity<temp_quality_mutation5_kmia>(entity =>
            {
                entity.HasNoKey();

                entity.HasIndex(e => new { e.trans_date, e.stock_location_id, e.analyte_id })
                    .HasName("idx_temp_quality_mutation5_kmia")
                    .HasSortOrder(new[] { SortOrder.Descending, SortOrder.Ascending, SortOrder.Ascending });
            });

            modelBuilder.Entity<temp_quality_mutation6>(entity =>
            {
                entity.HasNoKey();

                entity.HasIndex(e => new { e.trans_date, e.stock_location_id, e.analyte_id })
                    .HasName("idx_temp_quality_mutation6")
                    .HasSortOrder(new[] { SortOrder.Descending, SortOrder.Ascending, SortOrder.Ascending });
            });

            modelBuilder.Entity<temp_quality_mutation6_kmia>(entity =>
            {
                entity.HasNoKey();

                entity.HasIndex(e => new { e.trans_date, e.stock_location_id, e.analyte_id })
                    .HasName("idx_temp_quality_mutation6_kmia")
                    .HasSortOrder(new[] { SortOrder.Descending, SortOrder.Ascending, SortOrder.Ascending });
            });

            modelBuilder.Entity<temp_quality_mutation7>(entity =>
            {
                entity.HasNoKey();

                entity.HasIndex(e => new { e.trans_date, e.stock_location_id, e.analyte_id })
                    .HasName("idx_temp_quality_mutation7")
                    .HasSortOrder(new[] { SortOrder.Descending, SortOrder.Ascending, SortOrder.Ascending });
            });

            modelBuilder.Entity<temp_quality_mutation7_kmia>(entity =>
            {
                entity.HasNoKey();

                entity.HasIndex(e => new { e.trans_date, e.stock_location_id, e.analyte_id })
                    .HasName("idx_temp_quality_mutation7_kmia")
                    .HasSortOrder(new[] { SortOrder.Descending, SortOrder.Ascending, SortOrder.Ascending });
            });

            modelBuilder.Entity<temp_quality_mutation8>(entity =>
            {
                entity.HasNoKey();

                entity.HasIndex(e => new { e.trans_date, e.stock_location_id, e.analyte_id })
                    .HasName("idx_temp_quality_mutation8")
                    .HasSortOrder(new[] { SortOrder.Descending, SortOrder.Ascending, SortOrder.Ascending });
            });

            modelBuilder.Entity<temp_quality_mutation8_kmia>(entity =>
            {
                entity.HasNoKey();

                entity.HasIndex(e => new { e.trans_date, e.stock_location_id, e.analyte_id })
                    .HasName("idx_temp_quality_mutation8_kmia")
                    .HasSortOrder(new[] { SortOrder.Descending, SortOrder.Ascending, SortOrder.Ascending });
            });

            modelBuilder.Entity<temp_quality_mutation9_kmia>(entity =>
            {
                entity.HasNoKey();

                entity.HasIndex(e => new { e.trans_date, e.stock_location_id, e.analyte_id })
                    .HasName("idx_temp_quality_mutation9_kmia")
                    .HasSortOrder(new[] { SortOrder.Descending, SortOrder.Ascending, SortOrder.Ascending });
            });

            modelBuilder.Entity<tidalwave>(entity =>
            {
                entity.Property(e => e.business_area_id).IsRequired();

                entity.Property(e => e.tidalwave_value).HasColumnType("numeric");

                entity.Property(e => e.wind_speed).HasColumnType("numeric");

                entity.HasOne(d => d.business_area_)
                    .WithMany(p => p.tidalwave)
                    .HasForeignKey(d => d.business_area_id)
                    .HasConstraintName("fk_business_area_business_area_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.tidalwave)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<timesheet>(entity =>
            {
                entity.HasIndex(e => e.activity_id)
                    .HasName("ix_timesheet_activity_id");

                entity.HasIndex(e => e.cn_unit_id)
                    .HasName("ix_timesheet_cn_unit_id");

                entity.HasIndex(e => e.hour_end)
                    .HasName("ix_timesheet_hour_end");

                entity.HasIndex(e => e.hour_start)
                    .HasName("ix_timesheet_hour_start");

                entity.HasIndex(e => e.material_id)
                    .HasName("ix_timesheet_material_id");

                entity.HasIndex(e => e.shift_id)
                    .HasName("ix_timesheet_shift_id");

                entity.HasIndex(e => e.supervisor_id)
                    .HasName("ix_timesheet_supervisor_id");

                entity.HasIndex(e => new { e.id, e.timesheet_date, e.organization_id, e.cn_unit_id, e.operator_id, e.supervisor_id, e.shift_id })
                    .HasName("ix_timesheet")
                    .HasNullSortOrder(new[] { NullSortOrder.NullsLast, NullSortOrder.NullsLast, NullSortOrder.NullsLast, NullSortOrder.NullsLast, NullSortOrder.NullsLast, NullSortOrder.NullsLast, NullSortOrder.NullsLast })
                    .HasSortOrder(new[] { SortOrder.Ascending, SortOrder.Descending, SortOrder.Ascending, SortOrder.Ascending, SortOrder.Ascending, SortOrder.Ascending, SortOrder.Ascending });

                entity.Property(e => e.cn_unit_id).IsRequired();

                entity.Property(e => e.hour_end).HasColumnType("numeric");

                entity.Property(e => e.hour_start).HasColumnType("numeric");

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.HasOne(d => d.activity_)
                    .WithMany(p => p.timesheet)
                    .HasForeignKey(d => d.activity_id)
                    .HasConstraintName("fk_master_list_activity_id");

                entity.HasOne(d => d.mine_location_)
                    .WithMany(p => p.timesheet)
                    .HasForeignKey(d => d.mine_location_id)
                    .HasConstraintName("fk_mine_location_mine_location_id");

                entity.HasOne(d => d.operator_)
                    .WithMany(p => p.timesheetoperator_)
                    .HasForeignKey(d => d.operator_id)
                    .HasConstraintName("fk_employee_operator_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.timesheet)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.shift_)
                    .WithMany(p => p.timesheet)
                    .HasForeignKey(d => d.shift_id)
                    .HasConstraintName("fk_shift_shift_id");

                entity.HasOne(d => d.supervisor_)
                    .WithMany(p => p.timesheetsupervisor_)
                    .HasForeignKey(d => d.supervisor_id)
                    .HasConstraintName("fk_employee_supervisor_id");

                entity.HasOne(d => d.uom_)
                    .WithMany(p => p.timesheet)
                    .HasForeignKey(d => d.uom_id)
                    .HasConstraintName("fk_uom_uom_id");
            });

            modelBuilder.Entity<timesheet_detail>(entity =>
            {
                entity.Property(e => e.classification).HasColumnType("time without time zone");

                entity.Property(e => e.distance).HasColumnType("numeric");

                entity.Property(e => e.duration).HasColumnType("numeric");

                entity.Property(e => e.end_time).HasColumnType("time without time zone");

                entity.Property(e => e.productivity).HasColumnType("numeric");

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.refuelling_quantity).HasColumnType("numeric");

                entity.Property(e => e.rit_rehandling).HasColumnType("numeric");

                entity.Property(e => e.ritase).HasColumnType("numeric");

                entity.Property(e => e.start_time).HasColumnType("time without time zone");

                entity.Property(e => e.timesheet_id).IsRequired();

                entity.Property(e => e.timesheet_time).HasColumnType("time without time zone");

                entity.Property(e => e.vol_density).HasColumnType("numeric");

                entity.Property(e => e.vol_distance).HasColumnType("numeric");

                entity.Property(e => e.vol_rehandling).HasColumnType("numeric");

                entity.Property(e => e.volume).HasColumnType("numeric");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.timesheet_detail)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.timesheet_)
                    .WithMany(p => p.timesheet_detail)
                    .HasForeignKey(d => d.timesheet_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_timesheet_timesheet_id");
            });

            modelBuilder.Entity<timesheet_detail_event>(entity =>
            {
                entity.Property(e => e.event_category_id).IsRequired();

                entity.Property(e => e.minute).HasColumnType("numeric");

                entity.HasOne(d => d.event_category_)
                    .WithMany(p => p.timesheet_detail_event)
                    .HasForeignKey(d => d.event_category_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_event_category_event_category_id");

                entity.HasOne(d => d.event_definition_category_)
                    .WithMany(p => p.timesheet_detail_event)
                    .HasForeignKey(d => d.event_definition_category_id)
                    .HasConstraintName("fk_event_definition_category_event_definition_category_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.timesheet_detail_event)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.timesheet_detail_)
                    .WithMany(p => p.timesheet_detail_event)
                    .HasForeignKey(d => d.timesheet_detail_id)
                    .HasConstraintName("fk_timesheet_detail_timesheet_detail_id");
            });

            modelBuilder.Entity<timesheet_detail_event_del>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.minute).HasColumnType("numeric");
            });

            modelBuilder.Entity<timesheet_detail_event_plan>(entity =>
            {
                entity.Property(e => e.event_category_id).IsRequired();

                entity.Property(e => e.minute).HasColumnType("numeric");

                entity.HasOne(d => d.event_category_)
                    .WithMany(p => p.timesheet_detail_event_plan)
                    .HasForeignKey(d => d.event_category_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_event_category_event_category_id");

                entity.HasOne(d => d.event_definition_category_)
                    .WithMany(p => p.timesheet_detail_event_plan)
                    .HasForeignKey(d => d.event_definition_category_id)
                    .HasConstraintName("fk_event_definition_category_event_definition_category_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.timesheet_detail_event_plan)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.timesheet_detail_)
                    .WithMany(p => p.timesheet_detail_event_plan)
                    .HasForeignKey(d => d.timesheet_detail_id)
                    .HasConstraintName("fk_timesheet_detail_timesheet_detail_id");
            });

            modelBuilder.Entity<timesheet_detail_plan>(entity =>
            {
                entity.Property(e => e.classification).HasColumnType("time without time zone");

                entity.Property(e => e.distance).HasColumnType("numeric");

                entity.Property(e => e.duration).HasColumnType("numeric");

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.refuelling_quantity).HasColumnType("numeric");

                entity.Property(e => e.ritase).HasColumnType("numeric");

                entity.Property(e => e.timesheet_id).IsRequired();

                entity.Property(e => e.timesheet_time).HasColumnType("time without time zone");

                entity.Property(e => e.volume).HasColumnType("numeric");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.timesheet_detail_plan)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.timesheet_)
                    .WithMany(p => p.timesheet_detail_plan)
                    .HasForeignKey(d => d.timesheet_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_timesheet_timesheet_id");
            });

            modelBuilder.Entity<timesheet_detail_productivity_problem>(entity =>
            {
                entity.Property(e => e.event_category_id).IsRequired();

                entity.Property(e => e.frequency).HasColumnType("numeric");

                entity.HasOne(d => d.event_category_)
                    .WithMany(p => p.timesheet_detail_productivity_problem)
                    .HasForeignKey(d => d.event_category_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_event_category_event_category_id");

                entity.HasOne(d => d.event_definition_category_)
                    .WithMany(p => p.timesheet_detail_productivity_problem)
                    .HasForeignKey(d => d.event_definition_category_id)
                    .HasConstraintName("fk_event_definition_category_event_definition_category_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.timesheet_detail_productivity_problem)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.timesheet_detail_)
                    .WithMany(p => p.timesheet_detail_productivity_problem)
                    .HasForeignKey(d => d.timesheet_detail_id)
                    .HasConstraintName("fk_timesheet_detail_timesheet_detail_id");
            });

            modelBuilder.Entity<timesheet_detail_productivity_problem_plan>(entity =>
            {
                entity.Property(e => e.event_category_id).IsRequired();

                entity.Property(e => e.frequency).HasColumnType("numeric");

                entity.HasOne(d => d.event_category_)
                    .WithMany(p => p.timesheet_detail_productivity_problem_plan)
                    .HasForeignKey(d => d.event_category_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_event_category_event_category_id");

                entity.HasOne(d => d.event_definition_category_)
                    .WithMany(p => p.timesheet_detail_productivity_problem_plan)
                    .HasForeignKey(d => d.event_definition_category_id)
                    .HasConstraintName("fk_event_definition_category_event_definition_category_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.timesheet_detail_productivity_problem_plan)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.timesheet_detail_)
                    .WithMany(p => p.timesheet_detail_productivity_problem_plan)
                    .HasForeignKey(d => d.timesheet_detail_id)
                    .HasConstraintName("fk_timesheet_detail_timesheet_detail_id");
            });

            modelBuilder.Entity<timesheet_plan>(entity =>
            {
                entity.Property(e => e.hour_end).HasColumnType("numeric");

                entity.Property(e => e.hour_start).HasColumnType("numeric");

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.HasOne(d => d.activity_)
                    .WithMany(p => p.timesheet_plan)
                    .HasForeignKey(d => d.activity_id)
                    .HasConstraintName("fk_master_list_activity_id");

                entity.HasOne(d => d.mine_location_)
                    .WithMany(p => p.timesheet_plan)
                    .HasForeignKey(d => d.mine_location_id)
                    .HasConstraintName("fk_mine_location_mine_location_id");

                entity.HasOne(d => d.operator_)
                    .WithMany(p => p.timesheet_planoperator_)
                    .HasForeignKey(d => d.operator_id)
                    .HasConstraintName("fk_employee_operator_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.timesheet_plan)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.shift_)
                    .WithMany(p => p.timesheet_plan)
                    .HasForeignKey(d => d.shift_id)
                    .HasConstraintName("fk_shift_shift_id");

                entity.HasOne(d => d.supervisor_)
                    .WithMany(p => p.timesheet_plansupervisor_)
                    .HasForeignKey(d => d.supervisor_id)
                    .HasConstraintName("fk_employee_supervisor_id");

                entity.HasOne(d => d.uom_)
                    .WithMany(p => p.timesheet_plan)
                    .HasForeignKey(d => d.uom_id)
                    .HasConstraintName("fk_uom_uom_id");
            });

            modelBuilder.Entity<train>(entity =>
            {
                entity.HasIndex(e => new { e.organization_id, e.vehicle_name })
                    .HasName("uk_train")
                    .IsUnique();

                entity.Property(e => e.vehicle_name).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.train)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<transfer_schema>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.qty).HasColumnType("numeric");
            });

            modelBuilder.Entity<transhipment_nplct_details>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<transport>(entity =>
            {
                entity.HasIndex(e => new { e.organization_id, e.vehicle_name })
                    .HasName("uk_transport")
                    .IsUnique();

                entity.Property(e => e.vehicle_name).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.transport)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<truck>(entity =>
            {
                entity.HasIndex(e => e.organization_id)
                    .HasName("ix_truck_organization_id");

                entity.HasIndex(e => e.vehicle_id)
                    .HasName("ix_truck_vehicle_id");

                entity.HasIndex(e => e.vendor_id)
                    .HasName("ix_truck_vendor_id");

                entity.HasIndex(e => new { e.organization_id, e.vehicle_id })
                    .HasName("Truck")
                    .IsUnique();

                entity.HasIndex(e => new { e.id, e.organization_id, e.vehicle_id })
                    .HasName("ix_truck");

                entity.Property(e => e.average_scale).HasColumnType("numeric");

                entity.Property(e => e.density).HasColumnType("numeric");

                entity.Property(e => e.tare).HasColumnType("numeric");

                entity.Property(e => e.typical_tonnage).HasColumnType("numeric");

                entity.Property(e => e.typical_volume).HasColumnType("numeric");

                entity.Property(e => e.vehicle_id).IsRequired();

                entity.Property(e => e.vehicle_name).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.truck)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<truck_cost_rate>(entity =>
            {
                entity.HasIndex(e => e.organization_id)
                    .HasName("ix_truck_cost_rate_organization_id");

                entity.HasIndex(e => e.truck_id)
                    .HasName("ix_truck_cost_rate_truck_id");

                entity.HasIndex(e => new { e.truck_id, e.accounting_period_id })
                    .HasName("uk_truck_cost_rate")
                    .IsUnique();

                entity.Property(e => e.hourly_rate).HasColumnType("numeric");

                entity.Property(e => e.trip_rate).HasColumnType("numeric");

                entity.Property(e => e.truck_id).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.truck_cost_rate)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.truck_)
                    .WithMany(p => p.truck_cost_rate)
                    .HasForeignKey(d => d.truck_id)
                    .HasConstraintName("fk_truck_truck_id");
            });

            modelBuilder.Entity<trucking_nplct_details>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<tug>(entity =>
            {
                entity.HasIndex(e => new { e.organization_id, e.vehicle_name })
                    .HasName("uk_tug")
                    .IsUnique();

                entity.Property(e => e.vehicle_name).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.tug)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<uom>(entity =>
            {
                entity.HasIndex(e => new { e.organization_id, e.uom_name })
                    .HasName("uk_uom")
                    .IsUnique();

                entity.Property(e => e.uom_category_id).IsRequired();

                entity.Property(e => e.uom_name).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.uom)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.uom_category_)
                    .WithMany(p => p.uom)
                    .HasForeignKey(d => d.uom_category_id)
                    .HasConstraintName("fk_uom_category_uom_category_id");
            });

            modelBuilder.Entity<uom_category>(entity =>
            {
                entity.HasIndex(e => new { e.organization_id, e.uom_category_code })
                    .HasName("UOM Category")
                    .IsUnique();

                entity.Property(e => e.uom_category_name).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.uom_category)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<uom_conversion>(entity =>
            {
                entity.HasIndex(e => new { e.source_uom_id, e.target_uom_id })
                    .HasName("uk_uom_conversion")
                    .IsUnique();

                entity.Property(e => e.conversion_value).HasColumnType("numeric(38,8)");

                entity.Property(e => e.source_uom_id).IsRequired();

                entity.Property(e => e.target_uom_id).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.uom_conversion)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.source_uom_)
                    .WithMany(p => p.uom_conversionsource_uom_)
                    .HasForeignKey(d => d.source_uom_id)
                    .HasConstraintName("fk_uom_source_uom_id");

                entity.HasOne(d => d.target_uom_)
                    .WithMany(p => p.uom_conversiontarget_uom_)
                    .HasForeignKey(d => d.target_uom_id)
                    .HasConstraintName("fk_uom_target_uom_id");
            });

            modelBuilder.Entity<user_role>(entity =>
            {
                entity.HasIndex(e => e.created_by)
                    .HasName("ix_user_role_created_by")
                    .IncludeProperties(new[] { "created_on" });

                entity.HasIndex(e => e.entity_id)
                    .HasName("ix_user_role_entity_id");

                entity.HasIndex(e => e.is_active)
                    .HasName("ix_user_role_is_active")
                    .IncludeProperties(new[] { "owner_id", "organization_id" });

                entity.HasIndex(e => e.modified_by)
                    .HasName("ix_user_role_modified_by")
                    .IncludeProperties(new[] { "modified_on" });

                entity.HasIndex(e => e.organization_id)
                    .HasName("ix_user_role_organization_id")
                    .IncludeProperties(new[] { "is_active", "owner_id" });

                entity.HasIndex(e => e.owner_id)
                    .HasName("ix_user_role_owner_id")
                    .IncludeProperties(new[] { "is_active", "organization_id" });

                entity.HasIndex(e => new { e.application_user_id, e.application_role_id })
                    .HasName("uk_user_role")
                    .IsUnique();

                entity.Property(e => e.application_role_id).IsRequired();

                entity.Property(e => e.application_user_id).IsRequired();

                entity.Property(e => e.organization_id).IsRequired();

                entity.Property(e => e.owner_id).IsRequired();

                entity.HasOne(d => d.application_role_)
                    .WithMany(p => p.user_role)
                    .HasForeignKey(d => d.application_role_id)
                    .HasConstraintName("fk_application_role_application_role_id");

                entity.HasOne(d => d.application_user_)
                    .WithMany(p => p.user_role)
                    .HasForeignKey(d => d.application_user_id)
                    .HasConstraintName("fk_application_user_application_user_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.user_role)
                    .HasForeignKey(d => d.organization_id)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<vessel>(entity =>
            {
                entity.HasIndex(e => new { e.organization_id, e.vehicle_name })
                    .HasName("uk_vessel")
                    .IsUnique();

                entity.Property(e => e.closing_date).HasColumnType("date");

                entity.Property(e => e.current_stock).HasColumnType("numeric");

                entity.Property(e => e.maximum_capacity).HasColumnType("numeric");

                entity.Property(e => e.minimum_capacity).HasColumnType("numeric");

                entity.Property(e => e.opening_date).HasColumnType("date");

                entity.Property(e => e.target_capacity).HasColumnType("numeric");

                entity.Property(e => e.vehicle_name).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.vessel)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<vw_accounting_period>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.end_date).HasColumnType("date");

                entity.Property(e => e.start_date).HasColumnType("date");
            });

            modelBuilder.Entity<vw_advance_contract>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.contract_value).HasColumnType("numeric");

                entity.Property(e => e.end_date).HasColumnType("date");

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.start_date).HasColumnType("date");
            });

            modelBuilder.Entity<vw_advance_contract_charge>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.advance_contract_detail_amount).HasColumnType("numeric");

                entity.Property(e => e.decimal_places).HasColumnType("numeric");

                entity.Property(e => e.end_date).HasColumnType("date");

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.start_date).HasColumnType("date");
            });

            modelBuilder.Entity<vw_advance_contract_charge_detail>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.advance_contract_detail_amount).HasColumnType("numeric");

                entity.Property(e => e.joint_survey_quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_advance_contract_detail>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.amount).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_advance_contract_item>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.end_date).HasColumnType("date");

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.start_date).HasColumnType("date");
            });

            modelBuilder.Entity<vw_advance_contract_item_detail>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.amount).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_advance_contract_reference>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.actual_quantity).HasColumnType("numeric");

                entity.Property(e => e.end_date).HasColumnType("date");

                entity.Property(e => e.start_date).HasColumnType("date");

                entity.Property(e => e.target_quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_advance_contract_reference_detail>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.decimal_places).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_advance_contract_valuation>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.actual_quantity).HasColumnType("numeric");

                entity.Property(e => e.end_date).HasColumnType("date");

                entity.Property(e => e.start_date).HasColumnType("date");

                entity.Property(e => e.target_quantity).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");

                entity.Property(e => e.total_valuation).HasColumnType("numeric");

                entity.Property(e => e.total_value).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_advance_contract_valuation_detail>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.convertion_amount).HasColumnType("numeric");

                entity.Property(e => e.value).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_advance_contract_valuation_download>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.final_value).HasColumnType("numeric");

                entity.Property(e => e.item_value).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_analyte>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_application_business_unit>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_application_entity>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_application_role>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_application_role_report>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_application_user>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_arutmin_waste_removal>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.density).HasColumnType("numeric");

                entity.Property(e => e.ritase).HasColumnType("numeric");

                entity.Property(e => e.tonnage).HasColumnType("numeric");

                entity.Property(e => e.volume_bcm).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_arutmin_waste_removal_item>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.ritase).HasColumnType("numeric");

                entity.Property(e => e.tonnage).HasColumnType("numeric");

                entity.Property(e => e.truck_factor).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_audit_trail>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_bank>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_bank_account>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_barge>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.trip_plan).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_barge_rotation>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.ash_adb).HasColumnType("numeric(18,6)");

                entity.Property(e => e.gcv_arb).HasColumnType("numeric(18,6)");

                entity.Property(e => e.net_quantity).HasColumnType("numeric");

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.ts_adb).HasColumnType("numeric(18,6)");
            });

            modelBuilder.Entity<vw_barging_load_unload_document>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_barging_plan>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.total_quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_barging_plan_daily>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.loading_rate).HasColumnType("numeric");

                entity.Property(e => e.operational_hours).HasColumnType("numeric");

                entity.Property(e => e.quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_barging_plan_monthly>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.month_name).HasColumnType("character varying");

                entity.Property(e => e.nama_bulan).HasColumnType("character varying");

                entity.Property(e => e.quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_barging_plan_monthly_history>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.month_name).HasColumnType("character varying");

                entity.Property(e => e.nama_bulan).HasColumnType("character varying");

                entity.Property(e => e.quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_barging_plan_nplct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_barging_transaction>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.beltscale_quantity).HasColumnType("numeric");

                entity.Property(e => e.distance).HasColumnType("numeric");

                entity.Property(e => e.draft_survey_date).HasColumnType("date");

                entity.Property(e => e.hour_usage).HasColumnType("numeric");

                entity.Property(e => e.intermediate_quantity).HasColumnType("numeric");

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.return_cargo).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_belt_scale>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.loading_rate).HasColumnType("numeric");

                entity.Property(e => e.quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_benchmark_price_series>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.end_date).HasColumnType("date");

                entity.Property(e => e.reference_ash).HasColumnType("numeric");

                entity.Property(e => e.reference_calori).HasColumnType("numeric");

                entity.Property(e => e.reference_end_date).HasColumnType("date");

                entity.Property(e => e.reference_price).HasColumnType("numeric");

                entity.Property(e => e.reference_start_date).HasColumnType("date");

                entity.Property(e => e.reference_total_moisture).HasColumnType("numeric");

                entity.Property(e => e.reference_total_sulphur).HasColumnType("numeric");

                entity.Property(e => e.start_date).HasColumnType("date");
            });

            modelBuilder.Entity<vw_benchmark_price_series_detail>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.ash).HasColumnType("numeric");

                entity.Property(e => e.calori).HasColumnType("numeric");

                entity.Property(e => e.price).HasColumnType("numeric");

                entity.Property(e => e.total_moisture).HasColumnType("numeric");

                entity.Property(e => e.total_sulphur).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_blending_plan>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.hour_usage).HasColumnType("numeric");

                entity.Property(e => e.sumprod_analyte_1).HasColumnType("numeric");

                entity.Property(e => e.sumprod_analyte_10).HasColumnType("numeric");

                entity.Property(e => e.sumprod_analyte_11).HasColumnType("numeric");

                entity.Property(e => e.sumprod_analyte_2).HasColumnType("numeric");

                entity.Property(e => e.sumprod_analyte_3).HasColumnType("numeric");

                entity.Property(e => e.sumprod_analyte_4).HasColumnType("numeric");

                entity.Property(e => e.sumprod_analyte_5).HasColumnType("numeric");

                entity.Property(e => e.sumprod_analyte_6).HasColumnType("numeric");

                entity.Property(e => e.sumprod_analyte_7).HasColumnType("numeric");

                entity.Property(e => e.sumprod_analyte_8).HasColumnType("numeric");

                entity.Property(e => e.sumprod_analyte_9).HasColumnType("numeric");

                entity.Property(e => e.sumprod_analyte_product_1).HasColumnType("numeric");

                entity.Property(e => e.sumprod_analyte_product_10).HasColumnType("numeric");

                entity.Property(e => e.sumprod_analyte_product_11).HasColumnType("numeric");

                entity.Property(e => e.sumprod_analyte_product_2).HasColumnType("numeric");

                entity.Property(e => e.sumprod_analyte_product_3).HasColumnType("numeric");

                entity.Property(e => e.sumprod_analyte_product_4).HasColumnType("numeric");

                entity.Property(e => e.sumprod_analyte_product_5).HasColumnType("numeric");

                entity.Property(e => e.sumprod_analyte_product_6).HasColumnType("numeric");

                entity.Property(e => e.sumprod_analyte_product_7).HasColumnType("numeric");

                entity.Property(e => e.sumprod_analyte_product_8).HasColumnType("numeric");

                entity.Property(e => e.sumprod_analyte_product_9).HasColumnType("numeric");

                entity.Property(e => e.sumprod_volume).HasColumnType("numeric");

                entity.Property(e => e.sumprod_volume_product).HasColumnType("numeric");

                entity.Property(e => e.unloading_quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_blending_plan_product>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.analyte_1).HasColumnType("numeric");

                entity.Property(e => e.analyte_10).HasColumnType("numeric");

                entity.Property(e => e.analyte_11).HasColumnType("numeric");

                entity.Property(e => e.analyte_2).HasColumnType("numeric");

                entity.Property(e => e.analyte_3).HasColumnType("numeric");

                entity.Property(e => e.analyte_4).HasColumnType("numeric");

                entity.Property(e => e.analyte_5).HasColumnType("numeric");

                entity.Property(e => e.analyte_6).HasColumnType("numeric");

                entity.Property(e => e.analyte_7).HasColumnType("numeric");

                entity.Property(e => e.analyte_8).HasColumnType("numeric");

                entity.Property(e => e.analyte_9).HasColumnType("numeric");

                entity.Property(e => e.hour_usage).HasColumnType("numeric");

                entity.Property(e => e.loading_quantity).HasColumnType("numeric");

                entity.Property(e => e.spec_ts).HasColumnType("numeric");

                entity.Property(e => e.volume).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_blending_plan_quality>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_blending_plan_source>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.analyte_1).HasColumnType("numeric");

                entity.Property(e => e.analyte_10).HasColumnType("numeric");

                entity.Property(e => e.analyte_11).HasColumnType("numeric");

                entity.Property(e => e.analyte_2).HasColumnType("numeric");

                entity.Property(e => e.analyte_3).HasColumnType("numeric");

                entity.Property(e => e.analyte_4).HasColumnType("numeric");

                entity.Property(e => e.analyte_5).HasColumnType("numeric");

                entity.Property(e => e.analyte_6).HasColumnType("numeric");

                entity.Property(e => e.analyte_7).HasColumnType("numeric");

                entity.Property(e => e.analyte_8).HasColumnType("numeric");

                entity.Property(e => e.analyte_9).HasColumnType("numeric");

                entity.Property(e => e.hour_usage).HasColumnType("numeric");

                entity.Property(e => e.loading_quantity).HasColumnType("numeric");

                entity.Property(e => e.spec_ts).HasColumnType("numeric");

                entity.Property(e => e.volume).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_blending_plan_value>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.analyte_value).HasColumnType("numeric");

                entity.Property(e => e.quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_business_area>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_business_area_breakdown_structure>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_business_area_structure>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_business_partner>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_business_unit>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_business_unit_structure>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_chls>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.date).HasColumnType("date");

                entity.Property(e => e.sum_cpp).HasColumnType("numeric");

                entity.Property(e => e.sum_hauling).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_chls_additional_info>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.sort_number).HasColumnType("numeric");

                entity.Property(e => e.start_count).HasColumnType("numeric");

                entity.Property(e => e.stop_count).HasColumnType("numeric");

                entity.Property(e => e.total_count).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_city>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_coa>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_coa_subaccount>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_coal_barged_sp>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.date).HasColumnType("date");

                entity.Property(e => e.quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_coal_mined_sp>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.date).HasColumnType("date");

                entity.Property(e => e.quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_coal_movement_sp>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.date).HasColumnType("date");

                entity.Property(e => e.quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_coal_transfer>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.distance).HasColumnType("numeric");

                entity.Property(e => e.gross).HasColumnType("numeric");

                entity.Property(e => e.hour_usage).HasColumnType("numeric");

                entity.Property(e => e.loading_quantity).HasColumnType("numeric");

                entity.Property(e => e.tare).HasColumnType("numeric");

                entity.Property(e => e.unloading_quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_contact>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_contractor>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.barge_trip_plan).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_contractor_document>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_contractor_type>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_country>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_cpp_location>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.closing_date).HasColumnType("date");

                entity.Property(e => e.current_stock).HasColumnType("numeric");

                entity.Property(e => e.maximum_capacity).HasColumnType("numeric");

                entity.Property(e => e.minimum_capacity).HasColumnType("numeric");

                entity.Property(e => e.opening_date).HasColumnType("date");

                entity.Property(e => e.target_capacity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_credit_limit_history>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.credit_limit_value).HasColumnType("numeric");

                entity.Property(e => e.current_credit_limit).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_currency>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_currency_exchange>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.buying_rate).HasColumnType("numeric");

                entity.Property(e => e.end_date).HasColumnType("date");

                entity.Property(e => e.exchange_rate).HasColumnType("numeric");

                entity.Property(e => e.selling_rate).HasColumnType("numeric");

                entity.Property(e => e.start_date).HasColumnType("date");
            });

            modelBuilder.Entity<vw_customer>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.credit_limit).HasColumnType("numeric");

                entity.Property(e => e.remained_credit_limit).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_customer_ell>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.credit_limit).HasColumnType("numeric");

                entity.Property(e => e.remained_credit_limit).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_customer_transaction_history>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.credit_limit).HasColumnType("numeric");

                entity.Property(e => e.invoice_date).HasColumnType("date");

                entity.Property(e => e.outstanding).HasColumnType("numeric");

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.receipt).HasColumnType("numeric");

                entity.Property(e => e.unit_price).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_customer_type>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_daily_stock_port>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_daywork>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.hm_duration).HasColumnType("numeric");

                entity.Property(e => e.hm_end).HasColumnType("numeric");

                entity.Property(e => e.hm_rate).HasColumnType("numeric");

                entity.Property(e => e.hm_start).HasColumnType("numeric");

                entity.Property(e => e.hm_value).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_daywork_closing>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.total_hm).HasColumnType("numeric");

                entity.Property(e => e.total_value).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_delay>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.delay_date).HasColumnType("date");

                entity.Property(e => e.duration).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_delay_details>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.duration).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_despatch_demurrage>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.end_date).HasColumnType("date");

                entity.Property(e => e.start_date).HasColumnType("date");
            });

            modelBuilder.Entity<vw_despatch_demurrage_debit_credit_note>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.cow_bill_lading_date).HasColumnType("date");

                entity.Property(e => e.cow_quantity).HasColumnType("numeric");

                entity.Property(e => e.currency_exchange_end_date).HasColumnType("date");

                entity.Property(e => e.currency_exchange_rate).HasColumnType("numeric");

                entity.Property(e => e.currency_exchange_start_date).HasColumnType("date");

                entity.Property(e => e.debit_credit_date).HasColumnType("date");

                entity.Property(e => e.rate).HasColumnType("numeric");

                entity.Property(e => e.total_price).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_despatch_demurrage_delay>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_despatch_demurrage_detail>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.despatch_percent).HasColumnType("numeric");

                entity.Property(e => e.loading_rate).HasColumnType("numeric");

                entity.Property(e => e.rate).HasColumnType("numeric");

                entity.Property(e => e.turn_time).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_despatch_demurrage_invoice>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.invoice_date).HasColumnType("date");

                entity.Property(e => e.laytime_allowed_duration).HasColumnType("numeric");

                entity.Property(e => e.laytime_used_duration).HasColumnType("numeric");

                entity.Property(e => e.rate).HasColumnType("numeric");

                entity.Property(e => e.total_price).HasColumnType("numeric");

                entity.Property(e => e.total_price_final).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_despatch_order>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bill_of_lading_date).HasColumnType("date");

                entity.Property(e => e.despatch_demurrage_rate).HasColumnType("numeric");

                entity.Property(e => e.despatch_order_date).HasColumnType("date");

                entity.Property(e => e.despatch_percentage).HasColumnType("numeric");

                entity.Property(e => e.dops_target).HasColumnType("numeric");

                entity.Property(e => e.dops_value).HasColumnType("numeric");

                entity.Property(e => e.draft_survey_bill_lading_date).HasColumnType("date");

                entity.Property(e => e.draft_survey_date).HasColumnType("date");

                entity.Property(e => e.draft_survey_quantity).HasColumnType("numeric");

                entity.Property(e => e.final_quantity).HasColumnType("numeric");

                entity.Property(e => e.laycan_end).HasColumnType("date");

                entity.Property(e => e.laycan_start).HasColumnType("date");

                entity.Property(e => e.laytime_duration).HasColumnType("numeric");

                entity.Property(e => e.loading_rate).HasColumnType("numeric");

                entity.Property(e => e.order_reference_date).HasColumnType("date");

                entity.Property(e => e.planned_despatch_date).HasColumnType("date");

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.required_quantity).HasColumnType("numeric");

                entity.Property(e => e.scdp_despatch_date).HasColumnType("date");

                entity.Property(e => e.turn_time).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_despatch_order_delay>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_despatch_order_delivery_term>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_despatch_order_document>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_despatch_order_ell>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_despatch_order_product_specifications>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_details_customer_invoice_history>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bill_lading_date).HasColumnType("date");

                entity.Property(e => e.billing).HasColumnType("numeric");

                entity.Property(e => e.credit_limit).HasColumnType("numeric");

                entity.Property(e => e.tdate).HasColumnType("date");
            });

            modelBuilder.Entity<vw_details_customer_payment_history>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.credit_limit).HasColumnType("numeric");

                entity.Property(e => e.receipt).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_do_inv_currency_exchange>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bill_of_lading_date).HasColumnType("date");

                entity.Property(e => e.despatch_demurrage_rate).HasColumnType("numeric");

                entity.Property(e => e.despatch_order_date).HasColumnType("date");

                entity.Property(e => e.exchange_date).HasColumnType("date");

                entity.Property(e => e.final_quantity).HasColumnType("numeric");

                entity.Property(e => e.laycan_end).HasColumnType("date");

                entity.Property(e => e.laycan_start).HasColumnType("date");

                entity.Property(e => e.laytime_duration).HasColumnType("numeric");

                entity.Property(e => e.loading_rate).HasColumnType("numeric");

                entity.Property(e => e.order_reference_date).HasColumnType("date");

                entity.Property(e => e.planned_despatch_date).HasColumnType("date");

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.required_quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_document_type>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_draft_survey>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bill_lading_date).HasColumnType("date");

                entity.Property(e => e.draught_survey_issued).HasColumnType("date");

                entity.Property(e => e.ds_issued_date).HasColumnType("date");

                entity.Property(e => e.lc_amount).HasColumnType("numeric");

                entity.Property(e => e.lc_date).HasColumnType("date");

                entity.Property(e => e.lc_price).HasColumnType("numeric");

                entity.Property(e => e.peb_date).HasColumnType("date");

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.sof_issued_date).HasColumnType("date");

                entity.Property(e => e.survey_date).HasColumnType("date");

                entity.Property(e => e.wlcr).HasColumnType("date");
            });

            modelBuilder.Entity<vw_draft_survey_detail>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bill_lading_date).HasColumnType("date");

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.survey_date).HasColumnType("date");
            });

            modelBuilder.Entity<vw_drill_blast_plan>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.blast_volume).HasColumnType("numeric");

                entity.Property(e => e.end_date).HasColumnType("date");

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.request_level).HasColumnType("numeric");

                entity.Property(e => e.start_date).HasColumnType("date");
            });

            modelBuilder.Entity<vw_eight_week_forecast>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.total).HasColumnType("numeric");

                entity.Property(e => e.total_quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_eight_week_forecast_item>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.sum).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");

                entity.Property(e => e.total_quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_eight_week_forecast_simulation>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.end_week).HasColumnType("date");

                entity.Property(e => e.start_week).HasColumnType("date");

                entity.Property(e => e.sum).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_email_notification>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_employee>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_equipment>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.capacity).HasColumnType("numeric(38,6)");

                entity.Property(e => e.plan).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_equipment_category>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_equipment_cost_rate>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.fuel_per_hour).HasColumnType("numeric");

                entity.Property(e => e.hourly_rate).HasColumnType("numeric");

                entity.Property(e => e.monthly_rate).HasColumnType("numeric");

                entity.Property(e => e.trip_rate).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_equipment_cost_rate_latest>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.fuel_per_hour).HasColumnType("numeric");

                entity.Property(e => e.hourly_rate).HasColumnType("numeric");

                entity.Property(e => e.monthly_rate).HasColumnType("numeric");

                entity.Property(e => e.trip_rate).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_equipment_group>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_equipment_group_item>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.density).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_equipment_incident>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.hour_duration).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_equipment_type>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_equipment_usage_transaction>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.actual_quantity).HasColumnType("numeric");

                entity.Property(e => e.target_quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_equipment_usage_transaction_detail>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.duration).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_equipment_usage_transaction_detail_lookup>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.duration).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_event_category>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_event_definition_category>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_explosive_usage_plan>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.date_time).HasColumnType("date");

                entity.Property(e => e.powder_factor).HasColumnType("numeric");

                entity.Property(e => e.quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_explosive_usage_plan_detail>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_exposed_coal>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.transaction_date).HasColumnType("date");
            });

            modelBuilder.Entity<vw_fuel_inventory>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_hauling_plan>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.total_quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_hauling_plan_daily>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_hauling_plan_history>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_hauling_plan_monthly>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.month_name).HasColumnType("character varying");

                entity.Property(e => e.nama_bulan).HasColumnType("character varying");

                entity.Property(e => e.quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_hauling_plan_monthly_history>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.month_name).HasColumnType("character varying");

                entity.Property(e => e.nama_bulan).HasColumnType("character varying");

                entity.Property(e => e.quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_hauling_sp>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.date).HasColumnType("date");

                entity.Property(e => e.quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_hauling_transaction>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.distance).HasColumnType("numeric");

                entity.Property(e => e.gross).HasColumnType("numeric");

                entity.Property(e => e.hour_usage).HasColumnType("numeric");

                entity.Property(e => e.loading_quantity).HasColumnType("numeric");

                entity.Property(e => e.netto_rekon).HasColumnType("numeric");

                entity.Property(e => e.tare).HasColumnType("numeric");

                entity.Property(e => e.unloading_quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_haze>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.haze_value).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_incident>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.hour_duration).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_incident_category>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_initial_information>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.despatch_demurrage_rate).HasColumnType("numeric");

                entity.Property(e => e.despatch_order_date).HasColumnType("date");

                entity.Property(e => e.laycan_end).HasColumnType("date");

                entity.Property(e => e.laycan_start).HasColumnType("date");

                entity.Property(e => e.loading_rate).HasColumnType("numeric");

                entity.Property(e => e.required_quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_inventory_transaction>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.hour_usage).HasColumnType("numeric");

                entity.Property(e => e.overriden_quantity).HasColumnType("numeric");

                entity.Property(e => e.quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_invoice_overdue>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.credit_limit).HasColumnType("numeric");

                entity.Property(e => e.current_date).HasColumnType("date");

                entity.Property(e => e.invoice_date).HasColumnType("date");

                entity.Property(e => e.number_of_days).HasColumnType("numeric");

                entity.Property(e => e.payment_value).HasColumnType("numeric");

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.total_price).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_item_list>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_joint_survey>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.distance).HasColumnType("numeric");

                entity.Property(e => e.distance_carry_over).HasColumnType("numeric");

                entity.Property(e => e.elevation).HasColumnType("numeric");

                entity.Property(e => e.elevation_carry_over).HasColumnType("numeric");

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.quantity_carry_over).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_joint_survey_analyte>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.analyte_value).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_location>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_lookup_despatch_order_for_invoice>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_lookup_despatch_order_for_quotation>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_master_date>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.date_trunc).HasColumnType("date");
            });

            modelBuilder.Entity<vw_material_type>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_material_type_analyte>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_mine_location>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.closing_date).HasColumnType("date");

                entity.Property(e => e.current_stock).HasColumnType("numeric");

                entity.Property(e => e.maximum_capacity).HasColumnType("numeric");

                entity.Property(e => e.minimum_capacity).HasColumnType("numeric");

                entity.Property(e => e.opening_date).HasColumnType("date");

                entity.Property(e => e.proved_reserve).HasColumnType("numeric");

                entity.Property(e => e.target_capacity).HasColumnType("numeric");

                entity.Property(e => e.total_extracted).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_mine_location_breakdown_structure>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.closing_date).HasColumnType("date");

                entity.Property(e => e.current_stock).HasColumnType("numeric");

                entity.Property(e => e.maximum_capacity).HasColumnType("numeric");

                entity.Property(e => e.minimum_capacity).HasColumnType("numeric");

                entity.Property(e => e.opening_date).HasColumnType("date");

                entity.Property(e => e.proved_reserve).HasColumnType("numeric");

                entity.Property(e => e.target_capacity).HasColumnType("numeric");

                entity.Property(e => e.total_extracted).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_mine_location_quality>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.ash).HasColumnType("numeric");

                entity.Property(e => e.fc).HasColumnType("numeric");

                entity.Property(e => e.gcv_adb).HasColumnType("numeric");

                entity.Property(e => e.gcv_ar).HasColumnType("numeric");

                entity.Property(e => e.im).HasColumnType("numeric");

                entity.Property(e => e.tm).HasColumnType("numeric");

                entity.Property(e => e.ts).HasColumnType("numeric");

                entity.Property(e => e.vm).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_mine_location_quality_pit>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.ash).HasColumnType("numeric");

                entity.Property(e => e.fc).HasColumnType("numeric");

                entity.Property(e => e.gcv_adb).HasColumnType("numeric");

                entity.Property(e => e.gcv_ar).HasColumnType("numeric");

                entity.Property(e => e.im).HasColumnType("numeric");

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.tm).HasColumnType("numeric");

                entity.Property(e => e.ts).HasColumnType("numeric");

                entity.Property(e => e.vm).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_mine_plan_ltp>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.ash_ar).HasColumnType("numeric");

                entity.Property(e => e.coal_tonnage).HasColumnType("numeric");

                entity.Property(e => e.fc_ar).HasColumnType("numeric");

                entity.Property(e => e.gcv_adb_ar).HasColumnType("numeric");

                entity.Property(e => e.gcv_ar_ar).HasColumnType("numeric");

                entity.Property(e => e.hgi_ar).HasColumnType("numeric");

                entity.Property(e => e.im_ar).HasColumnType("numeric");

                entity.Property(e => e.int_truethick).HasColumnType("numeric");

                entity.Property(e => e.rd_ar).HasColumnType("numeric");

                entity.Property(e => e.rdi_ar).HasColumnType("numeric");

                entity.Property(e => e.tm_ar).HasColumnType("numeric");

                entity.Property(e => e.ts_ar).HasColumnType("numeric");

                entity.Property(e => e.vm_ar).HasColumnType("numeric");

                entity.Property(e => e.waste_bcm).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_mineplan_geology_history>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.ash_adb).HasColumnType("numeric");

                entity.Property(e => e.cv_adb).HasColumnType("numeric");

                entity.Property(e => e.cv_arb).HasColumnType("numeric");

                entity.Property(e => e.fc_adb).HasColumnType("numeric");

                entity.Property(e => e.hgi).HasColumnType("numeric");

                entity.Property(e => e.im_adb).HasColumnType("numeric");

                entity.Property(e => e.mass_tonnage).HasColumnType("numeric");

                entity.Property(e => e.rd).HasColumnType("numeric");

                entity.Property(e => e.rdi).HasColumnType("numeric");

                entity.Property(e => e.tm_ar).HasColumnType("numeric");

                entity.Property(e => e.truethick).HasColumnType("numeric");

                entity.Property(e => e.ts_adb).HasColumnType("numeric");

                entity.Property(e => e.vm_adb).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_mineplan_ltp_history>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.ash_ar).HasColumnType("numeric");

                entity.Property(e => e.coal_tonnage).HasColumnType("numeric");

                entity.Property(e => e.fc_ar).HasColumnType("numeric");

                entity.Property(e => e.gcv_adb_ar).HasColumnType("numeric");

                entity.Property(e => e.gcv_ar_ar).HasColumnType("numeric");

                entity.Property(e => e.hgi_ar).HasColumnType("numeric");

                entity.Property(e => e.im_ar).HasColumnType("numeric");

                entity.Property(e => e.int_truethick).HasColumnType("numeric");

                entity.Property(e => e.rd_ar).HasColumnType("numeric");

                entity.Property(e => e.rdi_ar).HasColumnType("numeric");

                entity.Property(e => e.tm_ar).HasColumnType("numeric");

                entity.Property(e => e.ts_ar).HasColumnType("numeric");

                entity.Property(e => e.vm_ar).HasColumnType("numeric");

                entity.Property(e => e.waste_bcm).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_model_geology>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.ash).HasColumnType("numeric");

                entity.Property(e => e.fc).HasColumnType("numeric");

                entity.Property(e => e.gcv_adb).HasColumnType("numeric");

                entity.Property(e => e.gcv_ar).HasColumnType("numeric");

                entity.Property(e => e.hgi).HasColumnType("numeric");

                entity.Property(e => e.im).HasColumnType("numeric");

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.rd).HasColumnType("numeric");

                entity.Property(e => e.rdi).HasColumnType("numeric");

                entity.Property(e => e.tm).HasColumnType("numeric");

                entity.Property(e => e.ts).HasColumnType("numeric");

                entity.Property(e => e.vm).HasColumnType("numeric");

                entity.Property(e => e.waste_bcm).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_operator>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_organization>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_parent_despatch_order>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.down_payment_amount).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_port_location>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.closing_date).HasColumnType("date");

                entity.Property(e => e.current_stock).HasColumnType("numeric");

                entity.Property(e => e.maximum_capacity).HasColumnType("numeric");

                entity.Property(e => e.minimum_capacity).HasColumnType("numeric");

                entity.Property(e => e.opening_date).HasColumnType("date");

                entity.Property(e => e.target_capacity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_price_index>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_price_index_history>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.index_date).HasColumnType("date");

                entity.Property(e => e.index_value).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_price_index_map>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_price_index_map_detail>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.end_range).HasColumnType("numeric");

                entity.Property(e => e.start_range).HasColumnType("numeric");

                entity.Property(e => e.value).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_process_flow>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_process_flow_analyte>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_processing_category>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_processing_closing_item>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.quantity_item).HasColumnType("numeric");

                entity.Property(e => e.transaction_item_date).HasColumnType("date");
            });

            modelBuilder.Entity<vw_processing_transaction>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.hour_usage).HasColumnType("numeric");

                entity.Property(e => e.loading_quantity).HasColumnType("numeric");

                entity.Property(e => e.unloading_quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_product>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_product_category>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_product_specification>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.applicable_date).HasColumnType("date");

                entity.Property(e => e.maximum_value).HasColumnType("numeric(18,6)");

                entity.Property(e => e.minimum_value).HasColumnType("numeric(18,6)");

                entity.Property(e => e.target_value).HasColumnType("numeric(18,6)");
            });

            modelBuilder.Entity<vw_production_actual_plan>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_production_closing>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.distance).HasColumnType("numeric");

                entity.Property(e => e.volume).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_production_plan>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.total_quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_production_plan_daily>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.week_count).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_production_plan_monthly>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.month_name).HasColumnType("character varying");

                entity.Property(e => e.nama_bulan).HasColumnType("character varying");

                entity.Property(e => e.quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_production_plan_monthly_history>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.month_name).HasColumnType("character varying");

                entity.Property(e => e.nama_bulan).HasColumnType("character varying");

                entity.Property(e => e.quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_production_transaction>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.density).HasColumnType("numeric");

                entity.Property(e => e.distance).HasColumnType("numeric");

                entity.Property(e => e.duration).HasColumnType("time without time zone");

                entity.Property(e => e.elevation).HasColumnType("numeric");

                entity.Property(e => e.gross).HasColumnType("numeric");

                entity.Property(e => e.hour_usage).HasColumnType("numeric");

                entity.Property(e => e.loading_quantity).HasColumnType("numeric");

                entity.Property(e => e.ritase).HasColumnType("numeric");

                entity.Property(e => e.tare).HasColumnType("numeric");

                entity.Property(e => e.unloading_quantity).HasColumnType("numeric");

                entity.Property(e => e.volume).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_progress_claim>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.actual_quantity).HasColumnType("numeric");

                entity.Property(e => e.base_fuel_price).HasColumnType("numeric");

                entity.Property(e => e.base_overdistance_price).HasColumnType("numeric");

                entity.Property(e => e.base_unit_price).HasColumnType("numeric");

                entity.Property(e => e.end_date).HasColumnType("date");

                entity.Property(e => e.start_date).HasColumnType("date");

                entity.Property(e => e.target_quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_progress_claim_charge>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.charge_amount).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_province>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_quality_sampling>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_quality_sampling_analyte>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.analyte_value).HasColumnType("numeric");

                entity.Property(e => e.maximum).HasColumnType("numeric");

                entity.Property(e => e.minimum).HasColumnType("numeric");

                entity.Property(e => e.target).HasColumnType("numeric");

                entity.Property(e => e.value).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_railing_transaction>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.hour_usage).HasColumnType("numeric");

                entity.Property(e => e.loading_quantity).HasColumnType("numeric");

                entity.Property(e => e.unloading_quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_rainfall>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.duration).HasColumnType("numeric");

                entity.Property(e => e.frequency).HasColumnType("numeric");

                entity.Property(e => e.rainfall_value).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_raw_data_lineup_all>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.ash).HasColumnType("numeric");

                entity.Property(e => e.barge_or_vessel_provider).HasMaxLength(225);

                entity.Property(e => e.brand).HasMaxLength(225);

                entity.Property(e => e.buyer).HasMaxLength(225);

                entity.Property(e => e.cob).HasColumnType("numeric");

                entity.Property(e => e.commitment).HasMaxLength(225);

                entity.Property(e => e.confirmation).HasMaxLength(225);

                entity.Property(e => e.contract_term_name).HasMaxLength(225);

                entity.Property(e => e.contracted_price).HasColumnType("numeric");

                entity.Property(e => e.contracted_tonnage).HasColumnType("numeric");

                entity.Property(e => e.dem_days).HasColumnType("numeric");

                entity.Property(e => e.dem_hours).HasColumnType("numeric");

                entity.Property(e => e.dem_usd).HasColumnType("numeric");

                entity.Property(e => e.dem_usd_per_day).HasColumnType("numeric");

                entity.Property(e => e.destination_country).HasMaxLength(225);

                entity.Property(e => e.disc_tonnage).HasColumnType("numeric");

                entity.Property(e => e.dmo).HasMaxLength(225);

                entity.Property(e => e.eco_asamasam).HasColumnType("numeric");

                entity.Property(e => e.eco_jumbang).HasColumnType("numeric");

                entity.Property(e => e.eco_kintap).HasColumnType("numeric");

                entity.Property(e => e.eco_mulia).HasColumnType("numeric");

                entity.Property(e => e.eco_sarongga).HasColumnType("numeric");

                entity.Property(e => e.end_buyer).HasMaxLength(225);

                entity.Property(e => e.eta_status).HasMaxLength(225);

                entity.Property(e => e.fc_provider).HasMaxLength(225);

                entity.Property(e => e.fob_gross_revenue).HasColumnType("numeric");

                entity.Property(e => e.fob_price).HasColumnType("numeric");

                entity.Property(e => e.freight).HasColumnType("numeric");

                entity.Property(e => e.gar).HasColumnType("numeric");

                entity.Property(e => e.gcnewc).HasColumnType("numeric");

                entity.Property(e => e.hba).HasColumnType("numeric");

                entity.Property(e => e.hpb).HasColumnType("numeric");

                entity.Property(e => e.ici1).HasColumnType("numeric");

                entity.Property(e => e.ici4).HasColumnType("numeric");

                entity.Property(e => e.ici5).HasColumnType("numeric");

                entity.Property(e => e.invoice_number).HasMaxLength(225);

                entity.Property(e => e.loading_port).HasMaxLength(225);

                entity.Property(e => e.loading_rate).HasColumnType("numeric");

                entity.Property(e => e.loading_standart_tpd).HasColumnType("numeric");

                entity.Property(e => e.loading_status).HasMaxLength(225);

                entity.Property(e => e.loadport_agent).HasMaxLength(225);

                entity.Property(e => e.loadrate_actual_tpd).HasColumnType("numeric");

                entity.Property(e => e.month_name).HasMaxLength(225);

                entity.Property(e => e.month_sales).HasColumnType("numeric");

                entity.Property(e => e.parcel).HasMaxLength(225);

                entity.Property(e => e.plastts_5900).HasColumnType("numeric");

                entity.Property(e => e.remarks).HasMaxLength(225);

                entity.Property(e => e.results).HasColumnType("numeric");

                entity.Property(e => e.royalty_number).HasMaxLength(225);

                entity.Property(e => e.sales_agent).HasMaxLength(225);

                entity.Property(e => e.sat10).HasColumnType("numeric");

                entity.Property(e => e.sat13).HasColumnType("numeric");

                entity.Property(e => e.sat15).HasColumnType("numeric");

                entity.Property(e => e.sen08).HasColumnType("numeric");

                entity.Property(e => e.sen10).HasColumnType("numeric");

                entity.Property(e => e.sen15).HasColumnType("numeric");

                entity.Property(e => e.sen20).HasColumnType("numeric");

                entity.Property(e => e.shipment_number).HasMaxLength(225);

                entity.Property(e => e.shipping_term).HasMaxLength(225);

                entity.Property(e => e.stow_plan).HasColumnType("numeric");

                entity.Property(e => e.superintending_company).HasMaxLength(225);

                entity.Property(e => e.tm).HasColumnType("numeric");

                entity.Property(e => e.total_freight).HasColumnType("numeric");

                entity.Property(e => e.total_gross_revenue).HasColumnType("numeric");

                entity.Property(e => e.total_loading_tons).HasColumnType("numeric");

                entity.Property(e => e.transport_type).HasMaxLength(225);

                entity.Property(e => e.ts).HasColumnType("numeric");

                entity.Property(e => e.vessel).HasMaxLength(225);

                entity.Property(e => e.year).HasMaxLength(225);
            });

            modelBuilder.Entity<vw_raw_data_lineup_db>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.ash).HasColumnType("numeric");

                entity.Property(e => e.barge_or_vessel_provider).HasMaxLength(225);

                entity.Property(e => e.brand).HasMaxLength(225);

                entity.Property(e => e.buyer).HasMaxLength(225);

                entity.Property(e => e.cob).HasColumnType("numeric");

                entity.Property(e => e.commitment).HasMaxLength(225);

                entity.Property(e => e.confirmation).HasMaxLength(225);

                entity.Property(e => e.contract_term_name).HasMaxLength(225);

                entity.Property(e => e.contracted_price).HasColumnType("numeric");

                entity.Property(e => e.contracted_tonnage).HasColumnType("numeric");

                entity.Property(e => e.dem_days).HasColumnType("numeric");

                entity.Property(e => e.dem_hours).HasColumnType("numeric");

                entity.Property(e => e.dem_usd).HasColumnType("numeric");

                entity.Property(e => e.dem_usd_per_day).HasColumnType("numeric");

                entity.Property(e => e.destination_country).HasMaxLength(225);

                entity.Property(e => e.disc_tonnage).HasColumnType("numeric");

                entity.Property(e => e.dmo).HasMaxLength(225);

                entity.Property(e => e.eco_asamasam).HasColumnType("numeric");

                entity.Property(e => e.eco_jumbang).HasColumnType("numeric");

                entity.Property(e => e.eco_kintap).HasColumnType("numeric");

                entity.Property(e => e.eco_mulia).HasColumnType("numeric");

                entity.Property(e => e.eco_sarongga).HasColumnType("numeric");

                entity.Property(e => e.end_buyer).HasMaxLength(225);

                entity.Property(e => e.eta_status).HasMaxLength(225);

                entity.Property(e => e.fc_provider).HasMaxLength(225);

                entity.Property(e => e.fob_gross_revenue).HasColumnType("numeric");

                entity.Property(e => e.fob_price).HasColumnType("numeric");

                entity.Property(e => e.freight).HasColumnType("numeric");

                entity.Property(e => e.gar).HasColumnType("numeric");

                entity.Property(e => e.gcnewc).HasColumnType("numeric");

                entity.Property(e => e.hba).HasColumnType("numeric");

                entity.Property(e => e.hpb).HasColumnType("numeric");

                entity.Property(e => e.ici1).HasColumnType("numeric");

                entity.Property(e => e.ici4).HasColumnType("numeric");

                entity.Property(e => e.ici5).HasColumnType("numeric");

                entity.Property(e => e.invoice_number).HasMaxLength(225);

                entity.Property(e => e.loading_port).HasMaxLength(225);

                entity.Property(e => e.loading_rate).HasColumnType("numeric");

                entity.Property(e => e.loading_standart_tpd).HasColumnType("numeric");

                entity.Property(e => e.loading_status).HasMaxLength(225);

                entity.Property(e => e.loadport_agent).HasMaxLength(225);

                entity.Property(e => e.loadrate_actual_tpd).HasColumnType("numeric");

                entity.Property(e => e.month_name).HasMaxLength(225);

                entity.Property(e => e.month_sales).HasColumnType("numeric");

                entity.Property(e => e.parcel).HasMaxLength(225);

                entity.Property(e => e.plastts_5900).HasColumnType("numeric");

                entity.Property(e => e.remarks).HasMaxLength(225);

                entity.Property(e => e.results).HasColumnType("numeric");

                entity.Property(e => e.royalty_number).HasMaxLength(225);

                entity.Property(e => e.sales_agent).HasMaxLength(225);

                entity.Property(e => e.sat10).HasColumnType("numeric");

                entity.Property(e => e.sat13).HasColumnType("numeric");

                entity.Property(e => e.sat15).HasColumnType("numeric");

                entity.Property(e => e.sen08).HasColumnType("numeric");

                entity.Property(e => e.sen10).HasColumnType("numeric");

                entity.Property(e => e.sen15).HasColumnType("numeric");

                entity.Property(e => e.sen20).HasColumnType("numeric");

                entity.Property(e => e.shipment_number).HasMaxLength(225);

                entity.Property(e => e.shipping_term).HasMaxLength(225);

                entity.Property(e => e.stow_plan).HasColumnType("numeric");

                entity.Property(e => e.superintending_company).HasMaxLength(225);

                entity.Property(e => e.tm).HasColumnType("numeric");

                entity.Property(e => e.total_freight).HasColumnType("numeric");

                entity.Property(e => e.total_gross_revenue).HasColumnType("numeric");

                entity.Property(e => e.total_loading_tons).HasColumnType("numeric");

                entity.Property(e => e.transport_type).HasMaxLength(225);

                entity.Property(e => e.ts).HasColumnType("numeric");

                entity.Property(e => e.vessel).HasMaxLength(225);

                entity.Property(e => e.year).HasMaxLength(225);
            });

            modelBuilder.Entity<vw_raw_data_lineup_nplct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.ash).HasColumnType("numeric");

                entity.Property(e => e.barge_or_vessel_provider).HasMaxLength(225);

                entity.Property(e => e.brand).HasMaxLength(225);

                entity.Property(e => e.buyer).HasMaxLength(225);

                entity.Property(e => e.cob).HasColumnType("numeric");

                entity.Property(e => e.commitment).HasMaxLength(225);

                entity.Property(e => e.confirmation).HasMaxLength(225);

                entity.Property(e => e.contract_term_name).HasMaxLength(225);

                entity.Property(e => e.contracted_price).HasColumnType("numeric");

                entity.Property(e => e.contracted_tonnage).HasColumnType("numeric");

                entity.Property(e => e.dem_days).HasColumnType("numeric");

                entity.Property(e => e.dem_hours).HasColumnType("numeric");

                entity.Property(e => e.dem_usd).HasColumnType("numeric");

                entity.Property(e => e.dem_usd_per_day).HasColumnType("numeric");

                entity.Property(e => e.destination_country).HasMaxLength(225);

                entity.Property(e => e.disc_tonnage).HasColumnType("numeric");

                entity.Property(e => e.dmo).HasMaxLength(225);

                entity.Property(e => e.eco_asamasam).HasColumnType("numeric");

                entity.Property(e => e.eco_jumbang).HasColumnType("numeric");

                entity.Property(e => e.eco_kintap).HasColumnType("numeric");

                entity.Property(e => e.eco_mulia).HasColumnType("numeric");

                entity.Property(e => e.eco_sarongga).HasColumnType("numeric");

                entity.Property(e => e.end_buyer).HasMaxLength(225);

                entity.Property(e => e.eta_status).HasMaxLength(225);

                entity.Property(e => e.fc_provider).HasMaxLength(225);

                entity.Property(e => e.fob_gross_revenue).HasColumnType("numeric");

                entity.Property(e => e.fob_price).HasColumnType("numeric");

                entity.Property(e => e.freight).HasColumnType("numeric");

                entity.Property(e => e.gar).HasColumnType("numeric");

                entity.Property(e => e.gcnewc).HasColumnType("numeric");

                entity.Property(e => e.hba).HasColumnType("numeric");

                entity.Property(e => e.hpb).HasColumnType("numeric");

                entity.Property(e => e.ici1).HasColumnType("numeric");

                entity.Property(e => e.ici4).HasColumnType("numeric");

                entity.Property(e => e.ici5).HasColumnType("numeric");

                entity.Property(e => e.invoice_number).HasMaxLength(225);

                entity.Property(e => e.loading_port).HasMaxLength(225);

                entity.Property(e => e.loading_rate).HasColumnType("numeric");

                entity.Property(e => e.loading_standart_tpd).HasColumnType("numeric");

                entity.Property(e => e.loading_status).HasMaxLength(225);

                entity.Property(e => e.loadport_agent).HasMaxLength(225);

                entity.Property(e => e.loadrate_actual_tpd).HasColumnType("numeric");

                entity.Property(e => e.month_name).HasMaxLength(225);

                entity.Property(e => e.month_sales).HasColumnType("numeric");

                entity.Property(e => e.parcel).HasMaxLength(225);

                entity.Property(e => e.plastts_5900).HasColumnType("numeric");

                entity.Property(e => e.remarks).HasMaxLength(225);

                entity.Property(e => e.results).HasColumnType("numeric");

                entity.Property(e => e.royalty_number).HasMaxLength(225);

                entity.Property(e => e.sales_agent).HasMaxLength(225);

                entity.Property(e => e.sat10).HasColumnType("numeric");

                entity.Property(e => e.sat13).HasColumnType("numeric");

                entity.Property(e => e.sat15).HasColumnType("numeric");

                entity.Property(e => e.sen08).HasColumnType("numeric");

                entity.Property(e => e.sen10).HasColumnType("numeric");

                entity.Property(e => e.sen15).HasColumnType("numeric");

                entity.Property(e => e.sen20).HasColumnType("numeric");

                entity.Property(e => e.shipment_number).HasMaxLength(225);

                entity.Property(e => e.shipping_term).HasMaxLength(225);

                entity.Property(e => e.stow_plan).HasColumnType("numeric");

                entity.Property(e => e.superintending_company).HasMaxLength(225);

                entity.Property(e => e.tm).HasColumnType("numeric");

                entity.Property(e => e.total_freight).HasColumnType("numeric");

                entity.Property(e => e.total_gross_revenue).HasColumnType("numeric");

                entity.Property(e => e.total_loading_tons).HasColumnType("numeric");

                entity.Property(e => e.transport_type).HasMaxLength(225);

                entity.Property(e => e.ts).HasColumnType("numeric");

                entity.Property(e => e.vessel).HasMaxLength(225);

                entity.Property(e => e.year).HasMaxLength(225);
            });

            modelBuilder.Entity<vw_raw_data_lineup_transhipment>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.ash).HasColumnType("numeric");

                entity.Property(e => e.barge_or_vessel_provider).HasMaxLength(225);

                entity.Property(e => e.brand).HasMaxLength(225);

                entity.Property(e => e.buyer).HasMaxLength(225);

                entity.Property(e => e.cob).HasColumnType("numeric");

                entity.Property(e => e.commitment).HasMaxLength(225);

                entity.Property(e => e.confirmation).HasMaxLength(225);

                entity.Property(e => e.contract_term_name).HasMaxLength(225);

                entity.Property(e => e.contracted_price).HasColumnType("numeric");

                entity.Property(e => e.contracted_tonnage).HasColumnType("numeric");

                entity.Property(e => e.dem_days).HasColumnType("numeric");

                entity.Property(e => e.dem_hours).HasColumnType("numeric");

                entity.Property(e => e.dem_usd).HasColumnType("numeric");

                entity.Property(e => e.dem_usd_per_day).HasColumnType("numeric");

                entity.Property(e => e.destination_country).HasMaxLength(225);

                entity.Property(e => e.disc_tonnage).HasColumnType("numeric");

                entity.Property(e => e.dmo).HasMaxLength(225);

                entity.Property(e => e.eco_asamasam).HasColumnType("numeric");

                entity.Property(e => e.eco_jumbang).HasColumnType("numeric");

                entity.Property(e => e.eco_kintap).HasColumnType("numeric");

                entity.Property(e => e.eco_mulia).HasColumnType("numeric");

                entity.Property(e => e.eco_sarongga).HasColumnType("numeric");

                entity.Property(e => e.end_buyer).HasMaxLength(225);

                entity.Property(e => e.eta_status).HasMaxLength(225);

                entity.Property(e => e.fc_provider).HasMaxLength(225);

                entity.Property(e => e.fob_gross_revenue).HasColumnType("numeric");

                entity.Property(e => e.fob_price).HasColumnType("numeric");

                entity.Property(e => e.freight).HasColumnType("numeric");

                entity.Property(e => e.gar).HasColumnType("numeric");

                entity.Property(e => e.gcnewc).HasColumnType("numeric");

                entity.Property(e => e.hba).HasColumnType("numeric");

                entity.Property(e => e.hpb).HasColumnType("numeric");

                entity.Property(e => e.ici1).HasColumnType("numeric");

                entity.Property(e => e.ici4).HasColumnType("numeric");

                entity.Property(e => e.ici5).HasColumnType("numeric");

                entity.Property(e => e.invoice_number).HasMaxLength(225);

                entity.Property(e => e.loading_port).HasMaxLength(225);

                entity.Property(e => e.loading_rate).HasColumnType("numeric");

                entity.Property(e => e.loading_standart_tpd).HasColumnType("numeric");

                entity.Property(e => e.loading_status).HasMaxLength(225);

                entity.Property(e => e.loadport_agent).HasMaxLength(225);

                entity.Property(e => e.loadrate_actual_tpd).HasColumnType("numeric");

                entity.Property(e => e.month_name).HasMaxLength(225);

                entity.Property(e => e.month_sales).HasColumnType("numeric");

                entity.Property(e => e.parcel).HasMaxLength(225);

                entity.Property(e => e.plastts_5900).HasColumnType("numeric");

                entity.Property(e => e.remarks).HasMaxLength(225);

                entity.Property(e => e.results).HasColumnType("numeric");

                entity.Property(e => e.royalty_number).HasMaxLength(225);

                entity.Property(e => e.sales_agent).HasMaxLength(225);

                entity.Property(e => e.sat10).HasColumnType("numeric");

                entity.Property(e => e.sat13).HasColumnType("numeric");

                entity.Property(e => e.sat15).HasColumnType("numeric");

                entity.Property(e => e.sen08).HasColumnType("numeric");

                entity.Property(e => e.sen10).HasColumnType("numeric");

                entity.Property(e => e.sen15).HasColumnType("numeric");

                entity.Property(e => e.sen20).HasColumnType("numeric");

                entity.Property(e => e.shipment_number).HasMaxLength(225);

                entity.Property(e => e.shipping_term).HasMaxLength(225);

                entity.Property(e => e.stow_plan).HasColumnType("numeric");

                entity.Property(e => e.superintending_company).HasMaxLength(225);

                entity.Property(e => e.tm).HasColumnType("numeric");

                entity.Property(e => e.total_freight).HasColumnType("numeric");

                entity.Property(e => e.total_gross_revenue).HasColumnType("numeric");

                entity.Property(e => e.total_loading_tons).HasColumnType("numeric");

                entity.Property(e => e.transport_type).HasMaxLength(225);

                entity.Property(e => e.ts).HasColumnType("numeric");

                entity.Property(e => e.vessel).HasMaxLength(225);

                entity.Property(e => e.year).HasMaxLength(225);
            });

            modelBuilder.Entity<vw_ready_to_get>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_rehandling_transaction>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.distance).HasColumnType("numeric");

                entity.Property(e => e.hour_usage).HasColumnType("numeric");

                entity.Property(e => e.loading_quantity).HasColumnType("numeric");

                entity.Property(e => e.unloading_quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_report_template>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_report_template2>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_report_viewer>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_role_access>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_role_business_unit>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_royalty>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.volume_loading).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_royalty_cost>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.barging_cost).HasColumnType("numeric");

                entity.Property(e => e.dist_barge_to_anchorage).HasColumnType("numeric");

                entity.Property(e => e.freight_cost).HasColumnType("numeric");

                entity.Property(e => e.total_join_cost).HasColumnType("numeric");

                entity.Property(e => e.transhipment_cost).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_royalty_payment>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bmn_outstanding).HasColumnType("numeric");

                entity.Property(e => e.bmn_paid_off).HasColumnType("numeric");

                entity.Property(e => e.bmn_value).HasColumnType("numeric");

                entity.Property(e => e.dhpb_final_value).HasColumnType("numeric");

                entity.Property(e => e.dhpb_outstanding).HasColumnType("numeric");

                entity.Property(e => e.dhpb_paid_off).HasColumnType("numeric");

                entity.Property(e => e.pht_outstanding).HasColumnType("numeric");

                entity.Property(e => e.pht_paid_off).HasColumnType("numeric");

                entity.Property(e => e.pht_value).HasColumnType("numeric");

                entity.Property(e => e.royalty_outstanding).HasColumnType("numeric");

                entity.Property(e => e.royalty_paid_off).HasColumnType("numeric");

                entity.Property(e => e.royalty_value).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_royalty_pricing>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.fob_price).HasColumnType("numeric");

                entity.Property(e => e.freight_cost).HasColumnType("numeric");

                entity.Property(e => e.hba_0).HasColumnType("numeric");

                entity.Property(e => e.hba_value).HasColumnType("numeric");

                entity.Property(e => e.hpb_barge).HasColumnType("numeric");

                entity.Property(e => e.hpb_vessel).HasColumnType("numeric");

                entity.Property(e => e.total_amount).HasColumnType("numeric");

                entity.Property(e => e.total_selling_price).HasColumnType("numeric");

                entity.Property(e => e.volume_loading).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_royalty_quantity_quality>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.analyte_value).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_royalty_valuation>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.base_price_royalty).HasColumnType("numeric");

                entity.Property(e => e.bmn_awal).HasColumnType("numeric");

                entity.Property(e => e.bmn_calc).HasColumnType("numeric");

                entity.Property(e => e.bmn_value).HasColumnType("numeric");

                entity.Property(e => e.dhpb_final_awal).HasColumnType("numeric");

                entity.Property(e => e.dhpb_final_calc).HasColumnType("numeric");

                entity.Property(e => e.dhpb_final_value).HasColumnType("numeric");

                entity.Property(e => e.pht_awal).HasColumnType("numeric");

                entity.Property(e => e.pht_calc).HasColumnType("numeric");

                entity.Property(e => e.pht_value).HasColumnType("numeric");

                entity.Property(e => e.royalty_awal).HasColumnType("numeric");

                entity.Property(e => e.royalty_calc).HasColumnType("numeric");

                entity.Property(e => e.royalty_value).HasColumnType("numeric");

                entity.Property(e => e.volume_loading).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_royalty_valuation_recalc>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.base_price_royalty).HasColumnType("numeric");

                entity.Property(e => e.bmn).HasColumnType("numeric");

                entity.Property(e => e.bmn_awal).HasColumnType("numeric");

                entity.Property(e => e.bmn_calc).HasColumnType("numeric");

                entity.Property(e => e.bmn_value).HasColumnType("numeric");

                entity.Property(e => e.dhpb).HasColumnType("numeric");

                entity.Property(e => e.dhpb_final_awal).HasColumnType("numeric");

                entity.Property(e => e.dhpb_final_calc).HasColumnType("numeric");

                entity.Property(e => e.dhpb_final_value).HasColumnType("numeric");

                entity.Property(e => e.pht).HasColumnType("numeric");

                entity.Property(e => e.pht_awal).HasColumnType("numeric");

                entity.Property(e => e.pht_calc).HasColumnType("numeric");

                entity.Property(e => e.pht_value).HasColumnType("numeric");

                entity.Property(e => e.royalty).HasColumnType("numeric");

                entity.Property(e => e.royalty_awal).HasColumnType("numeric");

                entity.Property(e => e.royalty_calc).HasColumnType("numeric");

                entity.Property(e => e.royalty_value).HasColumnType("numeric");

                entity.Property(e => e.volume_loading).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_sales_charge>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_sales_contract>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.credit_limit).HasColumnType("numeric");

                entity.Property(e => e.end_date).HasColumnType("date");

                entity.Property(e => e.start_date).HasColumnType("date");
            });

            modelBuilder.Entity<vw_sales_contract_charges>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.decimal_places).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_sales_contract_despatch_demurrage_delay>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.demurrage_applicable_percentage).HasColumnType("numeric");

                entity.Property(e => e.despatch_applicable_percentage).HasColumnType("numeric");

                entity.Property(e => e.despatch_percentage).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_sales_contract_despatch_demurrage_term>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.despatch_percentage).HasColumnType("numeric");

                entity.Property(e => e.loading_rate).HasColumnType("numeric");

                entity.Property(e => e.loading_rate_geared).HasColumnType("numeric");

                entity.Property(e => e.loading_rate_gearless).HasColumnType("numeric");

                entity.Property(e => e.loading_rate_nplct).HasColumnType("numeric");

                entity.Property(e => e.rate).HasColumnType("numeric");

                entity.Property(e => e.turn_time).HasColumnType("numeric");

                entity.Property(e => e.turn_time_ending).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_sales_contract_despatch_plan>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.despatch_date).HasColumnType("date");

                entity.Property(e => e.quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_sales_contract_detail>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.price).HasColumnType("numeric");

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.rate).HasColumnType("numeric");

                entity.Property(e => e.sp_price).HasColumnType("numeric");

                entity.Property(e => e.tax_rate).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_sales_contract_end_user>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_sales_contract_invoice_target>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_sales_contract_payment_term>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.downpayment_value).HasColumnType("numeric");

                entity.Property(e => e.exchange_rate).HasColumnType("numeric");

                entity.Property(e => e.number_of_days).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_sales_contract_product>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.mass_required).HasColumnType("numeric");

                entity.Property(e => e.maximum_order).HasColumnType("numeric");

                entity.Property(e => e.minimum_order).HasColumnType("numeric");

                entity.Property(e => e.quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_sales_contract_product_specifications>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.maximum).HasColumnType("numeric");

                entity.Property(e => e.minimum).HasColumnType("numeric");

                entity.Property(e => e.target).HasColumnType("numeric");

                entity.Property(e => e.value).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_sales_contract_qty>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_sales_contract_quotation_price>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.decimal_places).HasColumnType("numeric");

                entity.Property(e => e.price_value).HasColumnType("numeric");

                entity.Property(e => e.weightening_value).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_sales_contract_taxes>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.calculation_sign).HasColumnType("numeric");

                entity.Property(e => e.tax_rate).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_sales_contract_term>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.decimal_places).HasColumnType("numeric");

                entity.Property(e => e.despatch_demurrage_term_rate).HasColumnType("numeric");

                entity.Property(e => e.despatch_percentage).HasColumnType("numeric");

                entity.Property(e => e.end_date).HasColumnType("date");

                entity.Property(e => e.loading_rate_geared).HasColumnType("numeric");

                entity.Property(e => e.loading_rate_gearless).HasColumnType("numeric");

                entity.Property(e => e.maximum_order).HasColumnType("numeric");

                entity.Property(e => e.minimum_order).HasColumnType("numeric");

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.start_date).HasColumnType("date");

                entity.Property(e => e.turn_time).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_sales_invoice>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.alias19).HasColumnType("date");

                entity.Property(e => e.alias3).HasColumnType("date");

                entity.Property(e => e.alias5).HasColumnType("numeric");

                entity.Property(e => e.alias6).HasColumnType("numeric");

                entity.Property(e => e.alias7).HasColumnType("numeric");

                entity.Property(e => e.alias8).HasColumnType("numeric");

                entity.Property(e => e.alias9).HasColumnType("numeric");

                entity.Property(e => e.amount).HasColumnType("numeric");

                entity.Property(e => e.ash_adb).HasColumnType("numeric");

                entity.Property(e => e.ash_arb).HasColumnType("numeric");

                entity.Property(e => e.bl_date).HasColumnType("date");

                entity.Property(e => e.coa_date).HasColumnType("date");

                entity.Property(e => e.credit_limit).HasColumnType("numeric");

                entity.Property(e => e.decfreightamount).HasColumnType("numeric");

                entity.Property(e => e.decfreightpriceton).HasColumnType("numeric");

                entity.Property(e => e.downpayment).HasColumnType("numeric");

                entity.Property(e => e.exchange_rate).HasColumnType("numeric");

                entity.Property(e => e.final_amount).HasColumnType("numeric");

                entity.Property(e => e.final_price).HasColumnType("numeric");

                entity.Property(e => e.fob_price).HasColumnType("numeric");

                entity.Property(e => e.freight_cost).HasColumnType("numeric");

                entity.Property(e => e.gcv_adb).HasColumnType("numeric");

                entity.Property(e => e.gcv_arb).HasColumnType("numeric");

                entity.Property(e => e.hba_value).HasColumnType("numeric");

                entity.Property(e => e.hpb).HasColumnType("numeric");

                entity.Property(e => e.hpb_value).HasColumnType("numeric");

                entity.Property(e => e.im_adb).HasColumnType("numeric");

                entity.Property(e => e.insurance_cost).HasColumnType("numeric");

                entity.Property(e => e.invoice_date).HasColumnType("date");

                entity.Property(e => e.payment_date).HasColumnType("date");

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.sips_target).HasColumnType("numeric");

                entity.Property(e => e.sips_value).HasColumnType("numeric");

                entity.Property(e => e.submit_date).HasColumnType("date");

                entity.Property(e => e.subtotal).HasColumnType("numeric");

                entity.Property(e => e.tm_arb).HasColumnType("numeric");

                entity.Property(e => e.total_price).HasColumnType("numeric");

                entity.Property(e => e.total_price_hpb).HasColumnType("numeric");

                entity.Property(e => e.ts_adb).HasColumnType("numeric");

                entity.Property(e => e.ts_arb).HasColumnType("numeric");

                entity.Property(e => e.unit_price).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_sales_invoice_attachment>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_sales_invoice_datagrid>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.alias19).HasColumnType("date");

                entity.Property(e => e.alias3).HasColumnType("date");

                entity.Property(e => e.alias5).HasColumnType("numeric");

                entity.Property(e => e.alias6).HasColumnType("numeric");

                entity.Property(e => e.alias7).HasColumnType("numeric");

                entity.Property(e => e.alias8).HasColumnType("numeric");

                entity.Property(e => e.alias9).HasColumnType("numeric");

                entity.Property(e => e.amount).HasColumnType("numeric");

                entity.Property(e => e.ash_adb).HasColumnType("numeric");

                entity.Property(e => e.ash_arb).HasColumnType("numeric");

                entity.Property(e => e.bl_date).HasColumnType("date");

                entity.Property(e => e.coa_date).HasColumnType("date");

                entity.Property(e => e.credit_limit).HasColumnType("numeric");

                entity.Property(e => e.decfreightamount).HasColumnType("numeric");

                entity.Property(e => e.decfreightpriceton).HasColumnType("numeric");

                entity.Property(e => e.downpayment).HasColumnType("numeric");

                entity.Property(e => e.exchange_rate).HasColumnType("numeric");

                entity.Property(e => e.final_amount).HasColumnType("numeric");

                entity.Property(e => e.final_price).HasColumnType("numeric");

                entity.Property(e => e.fob_price).HasColumnType("numeric");

                entity.Property(e => e.freight_cost).HasColumnType("numeric");

                entity.Property(e => e.gcv_adb).HasColumnType("numeric");

                entity.Property(e => e.gcv_arb).HasColumnType("numeric");

                entity.Property(e => e.hba_value).HasColumnType("numeric");

                entity.Property(e => e.hpb).HasColumnType("numeric");

                entity.Property(e => e.hpb_value).HasColumnType("numeric");

                entity.Property(e => e.im_adb).HasColumnType("numeric");

                entity.Property(e => e.insurance_cost).HasColumnType("numeric");

                entity.Property(e => e.invoice_date).HasColumnType("date");

                entity.Property(e => e.payment_date).HasColumnType("date");

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.submit_date).HasColumnType("date");

                entity.Property(e => e.subtotal).HasColumnType("numeric");

                entity.Property(e => e.tm_arb).HasColumnType("numeric");

                entity.Property(e => e.total_price).HasColumnType("numeric");

                entity.Property(e => e.total_price_hpb).HasColumnType("numeric");

                entity.Property(e => e.ts_adb).HasColumnType("numeric");

                entity.Property(e => e.ts_arb).HasColumnType("numeric");

                entity.Property(e => e.unit_price).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_sales_invoice_ell>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.dp).HasColumnType("numeric");

                entity.Property(e => e.invoice_amount).HasColumnType("numeric");

                entity.Property(e => e.invoice_quantity).HasColumnType("numeric");

                entity.Property(e => e.item_value).HasColumnType("numeric");

                entity.Property(e => e.tax).HasColumnType("numeric");

                entity.Property(e => e.unit_price).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_sales_invoice_html_content>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_sales_invoice_payment>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.credit_limit).HasColumnType("numeric");

                entity.Property(e => e.payment_value).HasColumnType("numeric");

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.total_price).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_sales_invoice_product_specification>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.analyte_value).HasColumnType("numeric");

                entity.Property(e => e.nilai_denda_penolakan).HasColumnType("numeric");

                entity.Property(e => e.orderanalyte).HasColumnType("numeric");

                entity.Property(e => e.target).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_sales_invoice_trace_element>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_sales_invoice_transhipment>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_sales_plan>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.revision_number).HasColumnType("numeric");

                entity.Property(e => e.rkab_quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_sales_plan_customer>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.eta_date).HasColumnType("date");

                entity.Property(e => e.laycan_end).HasColumnType("date");

                entity.Property(e => e.laycan_start).HasColumnType("date");

                entity.Property(e => e.quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_sales_plan_customer_list>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.month_name).HasColumnType("character varying");

                entity.Property(e => e.quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_sales_plan_detail>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.month_name).HasColumnType("character varying");

                entity.Property(e => e.nama_bulan).HasColumnType("character varying");

                entity.Property(e => e.quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_sales_plan_snapshot>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.data).HasColumnType("json");
            });

            modelBuilder.Entity<vw_sampling_template>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_sampling_template_detail>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_sampling_type>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_shared_record>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_shift>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.duration).HasColumnType("time without time zone");

                entity.Property(e => e.end_time).HasColumnType("time without time zone");

                entity.Property(e => e.start_time).HasColumnType("time without time zone");
            });

            modelBuilder.Entity<vw_shift_category>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_shipment_plan>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.despatch_demurrage_rate).HasColumnType("numeric");

                entity.Property(e => e.hpb_forecast).HasColumnType("numeric");

                entity.Property(e => e.laycan_end).HasColumnType("date");

                entity.Property(e => e.laycan_start).HasColumnType("date");

                entity.Property(e => e.loading_rate).HasColumnType("numeric");

                entity.Property(e => e.loading_standart).HasColumnType("numeric");

                entity.Property(e => e.month_name).HasColumnType("character varying");

                entity.Property(e => e.qty_sp).HasColumnType("numeric");

                entity.Property(e => e.stow_plan).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_shipping_cost>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.advance_payment).HasColumnType("numeric");

                entity.Property(e => e.freight_rate).HasColumnType("numeric");

                entity.Property(e => e.insurance_cost).HasColumnType("numeric");

                entity.Property(e => e.late_penalty).HasColumnType("numeric");

                entity.Property(e => e.quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_shipping_delay>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_shipping_instruction>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.advance_royalty_amount).HasColumnType("numeric");

                entity.Property(e => e.issued_date).HasColumnType("date");

                entity.Property(e => e.laycan_end).HasColumnType("date");

                entity.Property(e => e.laycan_start).HasColumnType("date");

                entity.Property(e => e.lc_amount).HasColumnType("numeric");

                entity.Property(e => e.lc_date).HasColumnType("date");

                entity.Property(e => e.lc_price).HasColumnType("numeric");

                entity.Property(e => e.released_date).HasColumnType("date");

                entity.Property(e => e.req_quantity).HasColumnType("numeric");

                entity.Property(e => e.required_quantity).HasColumnType("numeric");

                entity.Property(e => e.royalty_amount).HasColumnType("numeric");

                entity.Property(e => e.scdp_despatch_date).HasColumnType("date");

                entity.Property(e => e.shipping_instruction_date).HasColumnType("date");
            });

            modelBuilder.Entity<vw_shipping_instruction_asuransi>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_shipping_instruction_detail_survey>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_shipping_instruction_detail_survey_document>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_shipping_instruction_document_agent>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_shipping_instruction_pekerjaan_agent>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_shipping_instruction_stevedoring>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.cargo).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_shipping_instruction_to_company>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_shipping_instruction_tug_boat>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_shipping_load_unload_document>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_shipping_program>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.product_1).HasColumnType("numeric");

                entity.Property(e => e.product_10).HasColumnType("numeric");

                entity.Property(e => e.product_11).HasColumnType("numeric");

                entity.Property(e => e.product_12).HasColumnType("numeric");

                entity.Property(e => e.product_13).HasColumnType("numeric");

                entity.Property(e => e.product_14).HasColumnType("numeric");

                entity.Property(e => e.product_15).HasColumnType("numeric");

                entity.Property(e => e.product_16).HasColumnType("numeric");

                entity.Property(e => e.product_17).HasColumnType("numeric");

                entity.Property(e => e.product_18).HasColumnType("numeric");

                entity.Property(e => e.product_19).HasColumnType("numeric");

                entity.Property(e => e.product_2).HasColumnType("numeric");

                entity.Property(e => e.product_20).HasColumnType("numeric");

                entity.Property(e => e.product_21).HasColumnType("numeric");

                entity.Property(e => e.product_22).HasColumnType("numeric");

                entity.Property(e => e.product_23).HasColumnType("numeric");

                entity.Property(e => e.product_24).HasColumnType("numeric");

                entity.Property(e => e.product_25).HasColumnType("numeric");

                entity.Property(e => e.product_26).HasColumnType("numeric");

                entity.Property(e => e.product_27).HasColumnType("numeric");

                entity.Property(e => e.product_28).HasColumnType("numeric");

                entity.Property(e => e.product_29).HasColumnType("numeric");

                entity.Property(e => e.product_3).HasColumnType("numeric");

                entity.Property(e => e.product_30).HasColumnType("numeric");

                entity.Property(e => e.product_31).HasColumnType("numeric");

                entity.Property(e => e.product_32).HasColumnType("numeric");

                entity.Property(e => e.product_33).HasColumnType("numeric");

                entity.Property(e => e.product_34).HasColumnType("numeric");

                entity.Property(e => e.product_35).HasColumnType("numeric");

                entity.Property(e => e.product_4).HasColumnType("numeric");

                entity.Property(e => e.product_5).HasColumnType("numeric");

                entity.Property(e => e.product_6).HasColumnType("numeric");

                entity.Property(e => e.product_7).HasColumnType("numeric");

                entity.Property(e => e.product_8).HasColumnType("numeric");

                entity.Property(e => e.product_9).HasColumnType("numeric");

                entity.Property(e => e.quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_shipping_program_detail>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.presentage).HasColumnType("numeric");

                entity.Property(e => e.product_1).HasColumnType("numeric");

                entity.Property(e => e.product_10).HasColumnType("numeric");

                entity.Property(e => e.product_11).HasColumnType("numeric");

                entity.Property(e => e.product_12).HasColumnType("numeric");

                entity.Property(e => e.product_13).HasColumnType("numeric");

                entity.Property(e => e.product_14).HasColumnType("numeric");

                entity.Property(e => e.product_15).HasColumnType("numeric");

                entity.Property(e => e.product_16).HasColumnType("numeric");

                entity.Property(e => e.product_17).HasColumnType("numeric");

                entity.Property(e => e.product_18).HasColumnType("numeric");

                entity.Property(e => e.product_19).HasColumnType("numeric");

                entity.Property(e => e.product_2).HasColumnType("numeric");

                entity.Property(e => e.product_20).HasColumnType("numeric");

                entity.Property(e => e.product_21).HasColumnType("numeric");

                entity.Property(e => e.product_22).HasColumnType("numeric");

                entity.Property(e => e.product_23).HasColumnType("numeric");

                entity.Property(e => e.product_24).HasColumnType("numeric");

                entity.Property(e => e.product_25).HasColumnType("numeric");

                entity.Property(e => e.product_3).HasColumnType("numeric");

                entity.Property(e => e.product_4).HasColumnType("numeric");

                entity.Property(e => e.product_5).HasColumnType("numeric");

                entity.Property(e => e.product_6).HasColumnType("numeric");

                entity.Property(e => e.product_7).HasColumnType("numeric");

                entity.Property(e => e.product_8).HasColumnType("numeric");

                entity.Property(e => e.product_9).HasColumnType("numeric");

                entity.Property(e => e.quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_shipping_progres>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_shipping_transaction>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bill_lading_date).HasColumnType("time with time zone");

                entity.Property(e => e.distance).HasColumnType("numeric");

                entity.Property(e => e.hour_usage).HasColumnType("numeric");

                entity.Property(e => e.original_quantity).HasColumnType("numeric");

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.stowage_plan_actual).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_shipping_transaction_detail>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.final_quantity).HasColumnType("numeric");

                entity.Property(e => e.hour_usage).HasColumnType("numeric");

                entity.Property(e => e.quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_sils>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.analyte_1).HasColumnType("numeric");

                entity.Property(e => e.analyte_2).HasColumnType("numeric");

                entity.Property(e => e.belt_scale).HasColumnType("numeric");

                entity.Property(e => e.chemical_consumption).HasColumnType("numeric");

                entity.Property(e => e.draft_scale).HasColumnType("numeric");

                entity.Property(e => e.sum_progressive).HasColumnType("numeric");

                entity.Property(e => e.water_consumption).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_sils_nplct_subdetail>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.hatch_this_run).HasColumnType("numeric");

                entity.Property(e => e.hatch_total).HasColumnType("numeric");

                entity.Property(e => e.hatch_total1).HasColumnType("numeric");

                entity.Property(e => e.progressive_total).HasColumnType("numeric");

                entity.Property(e => e.total_hatch_this_run).HasColumnType("numeric");

                entity.Property(e => e.total_time).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_sils_unloading>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.analyte_1).HasColumnType("numeric");

                entity.Property(e => e.analyte_2).HasColumnType("numeric");

                entity.Property(e => e.analyte_3).HasColumnType("numeric");

                entity.Property(e => e.belt_scale).HasColumnType("numeric");

                entity.Property(e => e.draft_scale).HasColumnType("numeric");

                entity.Property(e => e.site_scale).HasColumnType("numeric");

                entity.Property(e => e.tonnage).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_sils_unloading_detail>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.by_pass).HasColumnType("numeric");

                entity.Property(e => e.gross_loading_rate).HasColumnType("numeric");

                entity.Property(e => e.gross_loading_time).HasColumnType("numeric");

                entity.Property(e => e.nett_loading_rate).HasColumnType("numeric");

                entity.Property(e => e.nett_loading_time).HasColumnType("numeric");

                entity.Property(e => e.stockpile_1).HasColumnType("numeric");

                entity.Property(e => e.stockpile_2).HasColumnType("numeric");

                entity.Property(e => e.stockpile_3).HasColumnType("numeric");

                entity.Property(e => e.total_down_time).HasColumnType("numeric");

                entity.Property(e => e.total_flow_time).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_slippery>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.slippery_value).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_sof>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.despatch_order_date).HasColumnType("date");

                entity.Property(e => e.maximum_capacity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_sof_detail>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.percentage).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_stock_location>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.closing_date).HasColumnType("date");

                entity.Property(e => e.current_stock).HasColumnType("numeric");

                entity.Property(e => e.maximum_capacity).HasColumnType("numeric");

                entity.Property(e => e.minimum_capacity).HasColumnType("numeric");

                entity.Property(e => e.opening_date).HasColumnType("date");

                entity.Property(e => e.target_capacity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_stock_state>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.draft_survey_date).HasColumnType("date");

                entity.Property(e => e.qty_adjustment).HasColumnType("numeric");

                entity.Property(e => e.qty_adjustment_provisional).HasColumnType("numeric");

                entity.Property(e => e.qty_closing).HasColumnType("numeric");

                entity.Property(e => e.qty_closing_provisional).HasColumnType("numeric");

                entity.Property(e => e.qty_in).HasColumnType("numeric");

                entity.Property(e => e.qty_in_provisional).HasColumnType("numeric");

                entity.Property(e => e.qty_opening).HasColumnType("numeric");

                entity.Property(e => e.qty_out).HasColumnType("numeric");

                entity.Property(e => e.qty_out_provisional).HasColumnType("numeric");

                entity.Property(e => e.qty_survey).HasColumnType("numeric");

                entity.Property(e => e.qty_survey_provisional).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_stock_state_analyte>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.analyte_value).HasColumnType("numeric");

                entity.Property(e => e.moving_avg_value).HasColumnType("numeric");

                entity.Property(e => e.weighted_value).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_stockpile_location>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.closing_date).HasColumnType("date");

                entity.Property(e => e.current_stock).HasColumnType("numeric");

                entity.Property(e => e.maximum_capacity).HasColumnType("numeric");

                entity.Property(e => e.minimum_capacity).HasColumnType("numeric");

                entity.Property(e => e.opening_date).HasColumnType("date");

                entity.Property(e => e.target_capacity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_stockpile_mutation_analyte>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.analyte_value).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_stockpile_state>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.qty_adjustment).HasColumnType("numeric");

                entity.Property(e => e.qty_adjustment_provisional).HasColumnType("numeric");

                entity.Property(e => e.qty_closing).HasColumnType("numeric");

                entity.Property(e => e.qty_closing_provisional).HasColumnType("numeric");

                entity.Property(e => e.qty_in).HasColumnType("numeric");

                entity.Property(e => e.qty_in_provisional).HasColumnType("numeric");

                entity.Property(e => e.qty_opening).HasColumnType("numeric");

                entity.Property(e => e.qty_out).HasColumnType("numeric");

                entity.Property(e => e.qty_out_provisional).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_stockpile_summary>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.a_im).HasColumnType("numeric");

                entity.Property(e => e.qty_closing).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_stockpile_summary_daily>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.closing).HasColumnType("numeric");

                entity.Property(e => e.closing_before).HasColumnType("numeric");

                entity.Property(e => e.dates).HasColumnType("date");
            });

            modelBuilder.Entity<vw_stockpile_summary_monthly>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.closing).HasColumnType("numeric");

                entity.Property(e => e.closing_before).HasColumnType("numeric");

                entity.Property(e => e.dates).HasColumnType("date");
            });

            modelBuilder.Entity<vw_survey>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_survey_analyte>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.analyte_value).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_survey_detail>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.percentage).HasColumnType("numeric");

                entity.Property(e => e.quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_sync_log>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_team>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_team_member>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_team_role>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.application_role_id).HasMaxLength(50);

                entity.Property(e => e.team_id).HasMaxLength(50);
            });

            modelBuilder.Entity<vw_tidalwave>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.tidalwave_value).HasColumnType("numeric");

                entity.Property(e => e.wind_speed).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_timesheet>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.hour_end).HasColumnType("numeric");

                entity.Property(e => e.hour_start).HasColumnType("numeric");

                entity.Property(e => e.quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_timesheet_detail>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.classification).HasColumnType("time without time zone");

                entity.Property(e => e.distance).HasColumnType("numeric");

                entity.Property(e => e.duration).HasColumnType("numeric");

                entity.Property(e => e.end_time).HasColumnType("time without time zone");

                entity.Property(e => e.productivity).HasColumnType("numeric");

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.refuelling_quantity).HasColumnType("numeric");

                entity.Property(e => e.rit_rehandling).HasColumnType("numeric");

                entity.Property(e => e.ritase).HasColumnType("numeric");

                entity.Property(e => e.start_time).HasColumnType("time without time zone");

                entity.Property(e => e.timesheet_time).HasColumnType("time without time zone");

                entity.Property(e => e.vol_density).HasColumnType("numeric");

                entity.Property(e => e.vol_distance).HasColumnType("numeric");

                entity.Property(e => e.vol_rehandling).HasColumnType("numeric");

                entity.Property(e => e.volume).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_timesheet_detail_event>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.minute).HasColumnType("numeric");

                entity.Property(e => e.timesheet_detail_name).HasColumnType("time without time zone");
            });

            modelBuilder.Entity<vw_timesheet_detail_event_plan>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.minute).HasColumnType("numeric");

                entity.Property(e => e.timesheet_detail_name).HasColumnType("time without time zone");
            });

            modelBuilder.Entity<vw_timesheet_detail_plan>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.classification).HasColumnType("time without time zone");

                entity.Property(e => e.distance).HasColumnType("numeric");

                entity.Property(e => e.duration).HasColumnType("numeric");

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.Property(e => e.refuelling_quantity).HasColumnType("numeric");

                entity.Property(e => e.ritase).HasColumnType("numeric");

                entity.Property(e => e.timesheet_time).HasColumnType("time without time zone");

                entity.Property(e => e.volume).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_timesheet_detail_productivity_problem>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.frequency).HasColumnType("numeric");

                entity.Property(e => e.timesheet_detail_name).HasColumnType("time without time zone");
            });

            modelBuilder.Entity<vw_timesheet_detail_productivity_problem_plan>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.frequency).HasColumnType("numeric");

                entity.Property(e => e.timesheet_detail_name).HasColumnType("time without time zone");
            });

            modelBuilder.Entity<vw_timesheet_ell>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.hour_end).HasColumnType("numeric");

                entity.Property(e => e.hour_start).HasColumnType("numeric");

                entity.Property(e => e.minute).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_timesheet_plan>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.hour_end).HasColumnType("numeric");

                entity.Property(e => e.hour_start).HasColumnType("numeric");

                entity.Property(e => e.quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_train>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_transport>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_truck>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.average_scale).HasColumnType("numeric");

                entity.Property(e => e.density).HasColumnType("numeric");

                entity.Property(e => e.tare).HasColumnType("numeric");

                entity.Property(e => e.typical_tonnage).HasColumnType("numeric");

                entity.Property(e => e.typical_volume).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_truck_cost_rate>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.hourly_rate).HasColumnType("numeric");

                entity.Property(e => e.trip_rate).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_tug>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_uom>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_uom_category>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_uom_conversion>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.conversion_value).HasColumnType("numeric(38,8)");
            });

            modelBuilder.Entity<vw_user_role>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_vessel>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_wagon>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_waste>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_waste_category>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<vw_waste_location>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.closing_date).HasColumnType("date");

                entity.Property(e => e.current_stock).HasColumnType("numeric");

                entity.Property(e => e.maximum_capacity).HasColumnType("numeric");

                entity.Property(e => e.minimum_capacity).HasColumnType("numeric");

                entity.Property(e => e.opening_date).HasColumnType("date");

                entity.Property(e => e.target_capacity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_waste_removal>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.capacity).HasColumnType("numeric");

                entity.Property(e => e.distance).HasColumnType("numeric");

                entity.Property(e => e.elevation).HasColumnType("numeric");

                entity.Property(e => e.hour_usage).HasColumnType("numeric");

                entity.Property(e => e.loading_quantity).HasColumnType("numeric");

                entity.Property(e => e.tare).HasColumnType("numeric");

                entity.Property(e => e.unloading_quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_waste_removal_closing_detail>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.loading_quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_waste_specification>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.applicable_date).HasColumnType("date");

                entity.Property(e => e.maximum_value).HasColumnType("numeric(18,4)");

                entity.Property(e => e.minimum_value).HasColumnType("numeric(18,4)");

                entity.Property(e => e.target_value).HasColumnType("numeric(18,4)");
            });

            modelBuilder.Entity<vw_weigh_bridge>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.loading_rate).HasColumnType("numeric");

                entity.Property(e => e.quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<vw_wip_location>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.closing_date).HasColumnType("date");

                entity.Property(e => e.current_stock).HasColumnType("numeric");

                entity.Property(e => e.maximum_capacity).HasColumnType("numeric");

                entity.Property(e => e.minimum_capacity).HasColumnType("numeric");

                entity.Property(e => e.opening_date).HasColumnType("date");

                entity.Property(e => e.target_capacity).HasColumnType("numeric");
            });

            modelBuilder.Entity<wagon>(entity =>
            {
                entity.HasIndex(e => new { e.organization_id, e.vehicle_name })
                    .HasName("uk_wagon")
                    .IsUnique();

                entity.Property(e => e.vehicle_name).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.wagon)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<waste>(entity =>
            {
                entity.HasIndex(e => new { e.organization_id, e.waste_code })
                    .HasName("uk2_waste")
                    .IsUnique();

                entity.Property(e => e.density).HasColumnType("numeric");

                entity.Property(e => e.waste_category_id).IsRequired();

                entity.Property(e => e.waste_code).IsRequired();

                entity.Property(e => e.waste_name).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.waste)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.waste_category_)
                    .WithMany(p => p.waste)
                    .HasForeignKey(d => d.waste_category_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("waste_waste_category_id_fkey");
            });

            modelBuilder.Entity<waste_allsite_daily_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<waste_allsite_monthly_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<waste_allsite_mtd_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<waste_allsite_rawdata>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.capacity).HasColumnType("numeric");

                entity.Property(e => e.distance).HasColumnType("numeric");

                entity.Property(e => e.elevation).HasColumnType("numeric");

                entity.Property(e => e.hour_usage).HasColumnType("numeric");

                entity.Property(e => e.loading_quantity).HasColumnType("numeric");

                entity.Property(e => e.tare).HasColumnType("numeric");

                entity.Property(e => e.unloading_quantity).HasColumnType("numeric");
            });

            modelBuilder.Entity<waste_allsite_yearly_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<waste_allsite_ytd_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<waste_category>(entity =>
            {
                entity.HasIndex(e => new { e.organization_id, e.waste_category_code })
                    .HasName("uk2_waste_category")
                    .IsUnique();

                entity.HasIndex(e => new { e.organization_id, e.waste_category_name })
                    .HasName("uk1_waste_category")
                    .IsUnique();

                entity.Property(e => e.waste_category_code).IsRequired();

                entity.Property(e => e.waste_category_name).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.waste_category)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<waste_daily_details>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<waste_daily_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<waste_details_all>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<waste_location>(entity =>
            {
                entity.HasIndex(e => new { e.organization_id, e.waste_location_code })
                    .HasName("uk2_waste_location")
                    .IsUnique();

                entity.HasIndex(e => new { e.business_area_id, e.parent_stock_location_id, e.stock_location_name })
                    .HasName("uk1_waste_location")
                    .IsUnique();

                entity.Property(e => e.closing_date).HasColumnType("date");

                entity.Property(e => e.current_stock).HasColumnType("numeric");

                entity.Property(e => e.maximum_capacity).HasColumnType("numeric");

                entity.Property(e => e.minimum_capacity).HasColumnType("numeric");

                entity.Property(e => e.opening_date).HasColumnType("date");

                entity.Property(e => e.target_capacity).HasColumnType("numeric");

                entity.Property(e => e.waste_location_code).IsRequired();

                entity.HasOne(d => d.business_area_)
                    .WithMany(p => p.waste_location)
                    .HasForeignKey(d => d.business_area_id)
                    .HasConstraintName("waste_location_business_area_id_fkey");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.waste_location)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<waste_monthly_details_trans>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<waste_monthly_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<waste_mtd_details>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<waste_mtd_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<waste_mtfull_details>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<waste_pct_all>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<waste_removal>(entity =>
            {
                entity.HasIndex(e => e.accounting_period_id)
                    .HasName("ix_waste_removal_accounting_period_id");

                entity.HasIndex(e => e.organization_id)
                    .HasName("ix_waste_removal_organization_id");

                entity.HasIndex(e => e.process_flow_id)
                    .HasName("ix_waste_removal_process_flow_id");

                entity.HasIndex(e => e.uom_id)
                    .HasName("ix_waste_removal_uom_id");

                entity.HasIndex(e => e.waste_id)
                    .HasName("ix_waste_removal_waste_id");

                entity.HasIndex(e => new { e.organization_id, e.transaction_number })
                    .HasName("uk_waste_removal")
                    .IsUnique();

                entity.Property(e => e.capacity).HasColumnType("numeric");

                entity.Property(e => e.density).HasColumnType("numeric");

                entity.Property(e => e.destination_location_id).IsRequired();

                entity.Property(e => e.distance).HasColumnType("numeric");

                entity.Property(e => e.duration).HasColumnType("time without time zone");

                entity.Property(e => e.elevation).HasColumnType("numeric");

                entity.Property(e => e.hour_usage).HasColumnType("numeric");

                entity.Property(e => e.loading_quantity).HasColumnType("numeric");

                entity.Property(e => e.process_flow_id).IsRequired();

                entity.Property(e => e.ritase).HasColumnType("numeric");

                entity.Property(e => e.source_location_id).IsRequired();

                entity.Property(e => e.tare).HasColumnType("numeric");

                entity.Property(e => e.transaction_number).IsRequired();

                entity.Property(e => e.unloading_quantity).HasColumnType("numeric");

                entity.Property(e => e.waste_id).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.waste_removal)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<waste_removal_closing>(entity =>
            {
                entity.Property(e => e.distance).HasColumnType("numeric");

                entity.Property(e => e.volume).HasColumnType("numeric");
            });

            modelBuilder.Entity<waste_removal_closing_item>(entity =>
            {
                entity.Property(e => e.loading_quantity).HasColumnType("numeric");

                entity.Property(e => e.transaction_item_date).HasColumnType("date");

                entity.Property(e => e.waste_removal_closing_id).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.waste_removal_closing_item)
                    .HasForeignKey(d => d.organization_id)
                    .HasConstraintName("waste_removal_closing_item_organization_id_fkey");

                entity.HasOne(d => d.waste_removal_closing_)
                    .WithMany(p => p.waste_removal_closing_item)
                    .HasForeignKey(d => d.waste_removal_closing_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_waste_removal_closing_item");
            });

            modelBuilder.Entity<waste_removal_item>(entity =>
            {
                entity.Property(e => e.jam01).HasColumnType("numeric");

                entity.Property(e => e.jam02).HasColumnType("numeric");

                entity.Property(e => e.jam03).HasColumnType("numeric");

                entity.Property(e => e.jam04).HasColumnType("numeric");

                entity.Property(e => e.jam05).HasColumnType("numeric");

                entity.Property(e => e.jam06).HasColumnType("numeric");

                entity.Property(e => e.jam07).HasColumnType("numeric");

                entity.Property(e => e.jam08).HasColumnType("numeric");

                entity.Property(e => e.jam09).HasColumnType("numeric");

                entity.Property(e => e.jam10).HasColumnType("numeric");

                entity.Property(e => e.jam11).HasColumnType("numeric");

                entity.Property(e => e.jam12).HasColumnType("numeric");

                entity.Property(e => e.ritase).HasColumnType("numeric");

                entity.Property(e => e.truck_factor).HasColumnType("numeric");

                entity.Property(e => e.truck_id).IsRequired();

                entity.Property(e => e.waste_removal_id).IsRequired();

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.waste_removal_item)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.Entity<waste_schema>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.qty).HasColumnType("numeric");
            });

            modelBuilder.Entity<waste_specification>(entity =>
            {
                entity.HasIndex(e => new { e.waste_id, e.analyte_id, e.applicable_date })
                    .HasName("uk_waste_specification")
                    .IsUnique();

                entity.Property(e => e.analyte_id).IsRequired();

                entity.Property(e => e.applicable_date).HasColumnType("date");

                entity.Property(e => e.maximum_value).HasColumnType("numeric(18,4)");

                entity.Property(e => e.minimum_value).HasColumnType("numeric(18,4)");

                entity.Property(e => e.target_value).HasColumnType("numeric(18,4)");

                entity.Property(e => e.uom_id).IsRequired();

                entity.Property(e => e.waste_id).IsRequired();

                entity.HasOne(d => d.analyte_)
                    .WithMany(p => p.waste_specification)
                    .HasForeignKey(d => d.analyte_id)
                    .HasConstraintName("fk_analyte_analyte_id");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.waste_specification)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");

                entity.HasOne(d => d.uom_)
                    .WithMany(p => p.waste_specification)
                    .HasForeignKey(d => d.uom_id)
                    .HasConstraintName("fk_uom_uom_id");

                entity.HasOne(d => d.waste_)
                    .WithMany(p => p.waste_specification)
                    .HasForeignKey(d => d.waste_id)
                    .HasConstraintName("fk_waste_waste_id");
            });

            modelBuilder.Entity<waste_yearly_details_trans>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<waste_yearly_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<waste_ytd_details>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<waste_ytd_pct>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<waste_ytfull_details>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.bit_coal).HasColumnType("numeric");

                entity.Property(e => e.eco_coal).HasColumnType("numeric");

                entity.Property(e => e.sarongga).HasColumnType("numeric");

                entity.Property(e => e.total).HasColumnType("numeric");
            });

            modelBuilder.Entity<weigh_bridge>(entity =>
            {
                entity.Property(e => e.loading_rate).HasColumnType("numeric");

                entity.Property(e => e.quantity).HasColumnType("numeric");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.weigh_bridge)
                    .HasForeignKey(d => d.organization_id)
                    .HasConstraintName("fk_organization_id");
            });

            modelBuilder.Entity<wip_location>(entity =>
            {
                entity.HasIndex(e => e.organization_id)
                    .HasName("ix_wip_location_organization_id");

                entity.HasIndex(e => new { e.business_area_id, e.parent_stock_location_id, e.stock_location_name })
                    .HasName("uk_wip_location")
                    .IsUnique();

                entity.Property(e => e.closing_date).HasColumnType("date");

                entity.Property(e => e.current_stock).HasColumnType("numeric");

                entity.Property(e => e.maximum_capacity).HasColumnType("numeric");

                entity.Property(e => e.minimum_capacity).HasColumnType("numeric");

                entity.Property(e => e.opening_date).HasColumnType("date");

                entity.Property(e => e.target_capacity).HasColumnType("numeric");

                entity.HasOne(d => d.organization_)
                    .WithMany(p => p.wip_location)
                    .HasForeignKey(d => d.organization_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_organization_organization_id");
            });

            modelBuilder.HasSequence("seq_customer_number").HasMax(999999);

            modelBuilder.HasSequence("seq_customer_number_ic").HasMax(999999);

            modelBuilder.HasSequence("seq_customer_number_kmia").HasMax(999999);

            modelBuilder.HasSequence("seq_despatch_order_number")
                .HasMin(0)
                .HasMax(9999);

            modelBuilder.HasSequence("seq_despatch_order_number_ic")
                .StartsAt(92)
                .HasMax(9999)
                .IsCyclic();

            modelBuilder.HasSequence("seq_despatch_order_number_kmia")
                .StartsAt(81)
                .HasMax(9999)
                .IsCyclic();

            modelBuilder.HasSequence("seq_despatch_order_number_udu").HasMax(9999);

            modelBuilder.HasSequence("seq_processing_number").HasMax(999999);

            modelBuilder.HasSequence("seq_shipping_program_number")
                .HasMin(0)
                .HasMax(999999999);

            modelBuilder.HasSequence("seq_transaction_number").HasMin(0);

            modelBuilder.HasSequence("seq_waste_removal_number")
                .HasMax(99999)
                .IsCyclic();

            modelBuilder.HasSequence("seq_waste_removal_number_ic")
                .HasMax(99999)
                .IsCyclic();

            modelBuilder.HasSequence("seq_waste_removal_number_kmia")
                .HasMax(99999)
                .IsCyclic();

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
