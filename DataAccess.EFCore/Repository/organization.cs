using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class organization
    {
        public organization()
        {
            accounting_period = new HashSet<accounting_period>();
            advance_contract = new HashSet<advance_contract>();
            advance_contract_charge = new HashSet<advance_contract_charge>();
            advance_contract_charge_detail = new HashSet<advance_contract_charge_detail>();
            advance_contract_detail = new HashSet<advance_contract_detail>();
            advance_contract_item = new HashSet<advance_contract_item>();
            advance_contract_item_detail = new HashSet<advance_contract_item_detail>();
            analyte = new HashSet<analyte>();
            application_business_unit = new HashSet<application_business_unit>();
            application_role = new HashSet<application_role>();
            application_user = new HashSet<application_user>();
            arutmin_waste_removal = new HashSet<arutmin_waste_removal>();
            arutmin_waste_removal_item = new HashSet<arutmin_waste_removal_item>();
            bank = new HashSet<bank>();
            bank_account = new HashSet<bank_account>();
            barge = new HashSet<barge>();
            barge_rotation = new HashSet<barge_rotation>();
            barging_closing = new HashSet<barging_closing>();
            barging_load_unload_document = new HashSet<barging_load_unload_document>();
            barging_plan = new HashSet<barging_plan>();
            barging_plan_daily = new HashSet<barging_plan_daily>();
            barging_plan_monthly = new HashSet<barging_plan_monthly>();
            barging_plan_monthly_history = new HashSet<barging_plan_monthly_history>();
            barging_transaction = new HashSet<barging_transaction>();
            bill_of_lading = new HashSet<bill_of_lading>();
            blending_plan = new HashSet<blending_plan>();
            blending_plan_quality = new HashSet<blending_plan_quality>();
            blending_plan_source = new HashSet<blending_plan_source>();
            blending_plan_value = new HashSet<blending_plan_value>();
            business_area = new HashSet<business_area>();
            business_days = new HashSet<business_days>();
            business_partner = new HashSet<business_partner>();
            business_unit = new HashSet<business_unit>();
            calendar = new HashSet<calendar>();
            chls = new HashSet<chls>();
            chls_additional_info = new HashSet<chls_additional_info>();
            chls_cpp = new HashSet<chls_cpp>();
            chls_hauling = new HashSet<chls_hauling>();
            city = new HashSet<city>();
            coa = new HashSet<coa>();
            coa_subaccount = new HashSet<coa_subaccount>();
            coal_transfer = new HashSet<coal_transfer>();
            contact = new HashSet<contact>();
            contractor = new HashSet<contractor>();
            contractor_document = new HashSet<contractor_document>();
            cost_rate = new HashSet<cost_rate>();
            country = new HashSet<country>();
            cpp_location = new HashSet<cpp_location>();
            currency = new HashSet<currency>();
            currency_exchange = new HashSet<currency_exchange>();
            customer = new HashSet<customer>();
            daily_record = new HashSet<daily_record>();
            data_extension = new HashSet<data_extension>();
            daywork = new HashSet<daywork>();
            daywork_closing = new HashSet<daywork_closing>();
            delay = new HashSet<delay>();
            delay_category = new HashSet<delay_category>();
            delay_details = new HashSet<delay_details>();
            despatch_demurrage = new HashSet<despatch_demurrage>();
            despatch_demurrage_debit_credit_note = new HashSet<despatch_demurrage_debit_credit_note>();
            despatch_demurrage_delay = new HashSet<despatch_demurrage_delay>();
            despatch_demurrage_detail = new HashSet<despatch_demurrage_detail>();
            despatch_demurrage_invoice = new HashSet<despatch_demurrage_invoice>();
            despatch_order_delay = new HashSet<despatch_order_delay>();
            despatch_orderorganization_ = new HashSet<despatch_order>();
            despatch_orderseller_ = new HashSet<despatch_order>();
            document_type = new HashSet<document_type>();
            draft_survey = new HashSet<draft_survey>();
            draft_survey_detail = new HashSet<draft_survey_detail>();
            drill_blast_plan = new HashSet<drill_blast_plan>();
            eight_week_forecast = new HashSet<eight_week_forecast>();
            eight_week_forecast_item = new HashSet<eight_week_forecast_item>();
            eight_week_forecast_item_detail = new HashSet<eight_week_forecast_item_detail>();
            email_notification = new HashSet<email_notification>();
            email_recipient = new HashSet<email_recipient>();
            employee = new HashSet<employee>();
            equipment = new HashSet<equipment>();
            equipment_category = new HashSet<equipment_category>();
            equipment_cost_rate = new HashSet<equipment_cost_rate>();
            equipment_group = new HashSet<equipment_group>();
            equipment_group_item = new HashSet<equipment_group_item>();
            equipment_incident = new HashSet<equipment_incident>();
            equipment_type = new HashSet<equipment_type>();
            equipment_usage_transaction = new HashSet<equipment_usage_transaction>();
            equipment_usage_transaction_detail = new HashSet<equipment_usage_transaction_detail>();
            event_category = new HashSet<event_category>();
            event_definition_category = new HashSet<event_definition_category>();
            explosive_usage_plan = new HashSet<explosive_usage_plan>();
            explosive_usage_plan_detail = new HashSet<explosive_usage_plan_detail>();
            exposed_coal = new HashSet<exposed_coal>();
            fuel_consumption = new HashSet<fuel_consumption>();
            hauling_closing = new HashSet<hauling_closing>();
            hauling_closing_item = new HashSet<hauling_closing_item>();
            hauling_plan = new HashSet<hauling_plan>();
            hauling_plan_daily = new HashSet<hauling_plan_daily>();
            hauling_plan_history = new HashSet<hauling_plan_history>();
            hauling_plan_monthly = new HashSet<hauling_plan_monthly>();
            hauling_plan_monthly_history = new HashSet<hauling_plan_monthly_history>();
            hauling_transaction = new HashSet<hauling_transaction>();
            haze = new HashSet<haze>();
            incident = new HashSet<incident>();
            incident_category = new HashSet<incident_category>();
            initial_information = new HashSet<initial_information>();
            inventory_transaction = new HashSet<inventory_transaction>();
            item_group = new HashSet<item_group>();
            item_list = new HashSet<item_list>();
            joint_survey = new HashSet<joint_survey>();
            joint_survey_analyte = new HashSet<joint_survey_analyte>();
            land_clearing_detail = new HashSet<land_clearing_detail>();
            list_key = new HashSet<list_key>();
            list_value = new HashSet<list_value>();
            master_list = new HashSet<master_list>();
            material_type = new HashSet<material_type>();
            material_type_analyte = new HashSet<material_type_analyte>();
            mine_location = new HashSet<mine_location>();
            mine_location_quality = new HashSet<mine_location_quality>();
            mine_location_quality_pit = new HashSet<mine_location_quality_pit>();
            mine_plan_geology = new HashSet<mine_plan_geology>();
            mine_plan_ltp = new HashSet<mine_plan_ltp>();
            model_geology = new HashSet<model_geology>();
            national_holiday = new HashSet<national_holiday>();
            port_location = new HashSet<port_location>();
            price_adjustment_product_specification = new HashSet<price_adjustment_product_specification>();
            price_index = new HashSet<price_index>();
            price_index_history = new HashSet<price_index_history>();
            price_index_map = new HashSet<price_index_map>();
            price_index_map_detail = new HashSet<price_index_map_detail>();
            price_index_product_specification = new HashSet<price_index_product_specification>();
            price_series = new HashSet<price_series>();
            price_series_detail = new HashSet<price_series_detail>();
            process_flow = new HashSet<process_flow>();
            process_flow_analyte = new HashSet<process_flow_analyte>();
            processing_category = new HashSet<processing_category>();
            processing_closing = new HashSet<processing_closing>();
            processing_closing_item = new HashSet<processing_closing_item>();
            processing_transaction = new HashSet<processing_transaction>();
            product = new HashSet<product>();
            product_category = new HashSet<product_category>();
            product_specification = new HashSet<product_specification>();
            product_standard = new HashSet<product_standard>();
            production_closing = new HashSet<production_closing>();
            production_closing_item = new HashSet<production_closing_item>();
            production_plan = new HashSet<production_plan>();
            production_plan_daily = new HashSet<production_plan_daily>();
            production_plan_history = new HashSet<production_plan_history>();
            production_plan_monthly = new HashSet<production_plan_monthly>();
            production_plan_monthly_history = new HashSet<production_plan_monthly_history>();
            production_transaction = new HashSet<production_transaction>();
            production_transaction_item = new HashSet<production_transaction_item>();
            progress_claim = new HashSet<progress_claim>();
            progress_claim_charge = new HashSet<progress_claim_charge>();
            province = new HashSet<province>();
            quality_sampling = new HashSet<quality_sampling>();
            quality_sampling_analyte = new HashSet<quality_sampling_analyte>();
            quality_sampling_approval = new HashSet<quality_sampling_approval>();
            railing_transaction = new HashSet<railing_transaction>();
            railing_transaction_detail = new HashSet<railing_transaction_detail>();
            rainfall = new HashSet<rainfall>();
            ready_to_get = new HashSet<ready_to_get>();
            rehandling_closing = new HashSet<rehandling_closing>();
            rehandling_transaction = new HashSet<rehandling_transaction>();
            report_template = new HashSet<report_template>();
            role_access = new HashSet<role_access>();
            role_business_unit = new HashSet<role_business_unit>();
            royalty = new HashSet<royalty>();
            royalty_cost = new HashSet<royalty_cost>();
            royalty_payment = new HashSet<royalty_payment>();
            royalty_pricing = new HashSet<royalty_pricing>();
            royalty_quantity_quality = new HashSet<royalty_quantity_quality>();
            royalty_valuation = new HashSet<royalty_valuation>();
            sales_contract_charges = new HashSet<sales_contract_charges>();
            sales_contract_despatch_demurrage_delay = new HashSet<sales_contract_despatch_demurrage_delay>();
            sales_contract_despatch_demurrage_term = new HashSet<sales_contract_despatch_demurrage_term>();
            sales_contract_despatch_plan = new HashSet<sales_contract_despatch_plan>();
            sales_contract_detail = new HashSet<sales_contract_detail>();
            sales_contract_payment_term = new HashSet<sales_contract_payment_term>();
            sales_contract_product = new HashSet<sales_contract_product>();
            sales_contract_product_specifications = new HashSet<sales_contract_product_specifications>();
            sales_contract_quotation_price = new HashSet<sales_contract_quotation_price>();
            sales_contract_taxes = new HashSet<sales_contract_taxes>();
            sales_contract_term = new HashSet<sales_contract_term>();
            sales_contractorganization_ = new HashSet<sales_contract>();
            sales_contractseller_ = new HashSet<sales_contract>();
            sales_demurrage_rate = new HashSet<sales_demurrage_rate>();
            sales_invoice = new HashSet<sales_invoice>();
            sales_invoice_charges = new HashSet<sales_invoice_charges>();
            sales_invoice_detail = new HashSet<sales_invoice_detail>();
            sales_order = new HashSet<sales_order>();
            sales_order_detail = new HashSet<sales_order_detail>();
            sales_plan = new HashSet<sales_plan>();
            sales_plan_detail = new HashSet<sales_plan_detail>();
            sales_plan_snapshot = new HashSet<sales_plan_snapshot>();
            sales_price = new HashSet<sales_price>();
            sales_price_type = new HashSet<sales_price_type>();
            sales_product_specification = new HashSet<sales_product_specification>();
            sampling_template = new HashSet<sampling_template>();
            sampling_template_detail = new HashSet<sampling_template_detail>();
            shared_record = new HashSet<shared_record>();
            shift = new HashSet<shift>();
            shift_category = new HashSet<shift_category>();
            shipment_forecast = new HashSet<shipment_forecast>();
            shipment_plan = new HashSet<shipment_plan>();
            shipping_closing = new HashSet<shipping_closing>();
            shipping_delay = new HashSet<shipping_delay>();
            shipping_document = new HashSet<shipping_document>();
            shipping_instruction = new HashSet<shipping_instruction>();
            shipping_instruction_asuransi = new HashSet<shipping_instruction_asuransi>();
            shipping_instruction_detail_survey = new HashSet<shipping_instruction_detail_survey>();
            shipping_instruction_detail_survey_document = new HashSet<shipping_instruction_detail_survey_document>();
            shipping_instruction_document_agent = new HashSet<shipping_instruction_document_agent>();
            shipping_instruction_pekerjaan_agent = new HashSet<shipping_instruction_pekerjaan_agent>();
            shipping_instruction_stevedoring = new HashSet<shipping_instruction_stevedoring>();
            shipping_instruction_to_company = new HashSet<shipping_instruction_to_company>();
            shipping_instruction_tug_boat = new HashSet<shipping_instruction_tug_boat>();
            shipping_load_unload_document = new HashSet<shipping_load_unload_document>();
            shipping_program = new HashSet<shipping_program>();
            shipping_program_detail = new HashSet<shipping_program_detail>();
            shipping_transaction = new HashSet<shipping_transaction>();
            shipping_transaction_detail = new HashSet<shipping_transaction_detail>();
            slippery = new HashSet<slippery>();
            sof = new HashSet<sof>();
            sof_detail = new HashSet<sof_detail>();
            standard_cost_mining = new HashSet<standard_cost_mining>();
            standard_cost_mining_detail = new HashSet<standard_cost_mining_detail>();
            stock_location = new HashSet<stock_location>();
            stock_state = new HashSet<stock_state>();
            stock_state_analyte = new HashSet<stock_state_analyte>();
            stockpile_location = new HashSet<stockpile_location>();
            stockpile_state = new HashSet<stockpile_state>();
            survey = new HashSet<survey>();
            survey_analyte = new HashSet<survey_analyte>();
            survey_detail = new HashSet<survey_detail>();
            team = new HashSet<team>();
            team_member = new HashSet<team_member>();
            team_role = new HashSet<team_role>();
            tidalwave = new HashSet<tidalwave>();
            timesheet = new HashSet<timesheet>();
            timesheet_detail = new HashSet<timesheet_detail>();
            timesheet_detail_event = new HashSet<timesheet_detail_event>();
            timesheet_detail_event_plan = new HashSet<timesheet_detail_event_plan>();
            timesheet_detail_plan = new HashSet<timesheet_detail_plan>();
            timesheet_detail_productivity_problem = new HashSet<timesheet_detail_productivity_problem>();
            timesheet_detail_productivity_problem_plan = new HashSet<timesheet_detail_productivity_problem_plan>();
            timesheet_plan = new HashSet<timesheet_plan>();
            train = new HashSet<train>();
            transport = new HashSet<transport>();
            truck = new HashSet<truck>();
            truck_cost_rate = new HashSet<truck_cost_rate>();
            tug = new HashSet<tug>();
            uom = new HashSet<uom>();
            uom_category = new HashSet<uom_category>();
            uom_conversion = new HashSet<uom_conversion>();
            user_role = new HashSet<user_role>();
            vessel = new HashSet<vessel>();
            wagon = new HashSet<wagon>();
            waste = new HashSet<waste>();
            waste_category = new HashSet<waste_category>();
            waste_location = new HashSet<waste_location>();
            waste_removal = new HashSet<waste_removal>();
            waste_removal_closing_item = new HashSet<waste_removal_closing_item>();
            waste_removal_item = new HashSet<waste_removal_item>();
            waste_specification = new HashSet<waste_specification>();
            weigh_bridge = new HashSet<weigh_bridge>();
            wip_location = new HashSet<wip_location>();
        }

        public string id { get; set; }
        public string created_by { get; set; }
        public DateTime? created_on { get; set; }
        public string modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public bool? is_active { get; set; }
        public bool? is_locked { get; set; }
        public bool? is_default { get; set; }
        public string owner_id { get; set; }
        public string organization_id { get; set; }
        public string entity_id { get; set; }
        public string business_unit_id { get; set; }
        public string organization_name { get; set; }
        public string parent_organization_id { get; set; }
        public string connection_string { get; set; }
        public string organization_code { get; set; }

        public virtual ICollection<accounting_period> accounting_period { get; set; }
        public virtual ICollection<advance_contract> advance_contract { get; set; }
        public virtual ICollection<advance_contract_charge> advance_contract_charge { get; set; }
        public virtual ICollection<advance_contract_charge_detail> advance_contract_charge_detail { get; set; }
        public virtual ICollection<advance_contract_detail> advance_contract_detail { get; set; }
        public virtual ICollection<advance_contract_item> advance_contract_item { get; set; }
        public virtual ICollection<advance_contract_item_detail> advance_contract_item_detail { get; set; }
        public virtual ICollection<analyte> analyte { get; set; }
        public virtual ICollection<application_business_unit> application_business_unit { get; set; }
        public virtual ICollection<application_role> application_role { get; set; }
        public virtual ICollection<application_user> application_user { get; set; }
        public virtual ICollection<arutmin_waste_removal> arutmin_waste_removal { get; set; }
        public virtual ICollection<arutmin_waste_removal_item> arutmin_waste_removal_item { get; set; }
        public virtual ICollection<bank> bank { get; set; }
        public virtual ICollection<bank_account> bank_account { get; set; }
        public virtual ICollection<barge> barge { get; set; }
        public virtual ICollection<barge_rotation> barge_rotation { get; set; }
        public virtual ICollection<barging_closing> barging_closing { get; set; }
        public virtual ICollection<barging_load_unload_document> barging_load_unload_document { get; set; }
        public virtual ICollection<barging_plan> barging_plan { get; set; }
        public virtual ICollection<barging_plan_daily> barging_plan_daily { get; set; }
        public virtual ICollection<barging_plan_monthly> barging_plan_monthly { get; set; }
        public virtual ICollection<barging_plan_monthly_history> barging_plan_monthly_history { get; set; }
        public virtual ICollection<barging_transaction> barging_transaction { get; set; }
        public virtual ICollection<bill_of_lading> bill_of_lading { get; set; }
        public virtual ICollection<blending_plan> blending_plan { get; set; }
        public virtual ICollection<blending_plan_quality> blending_plan_quality { get; set; }
        public virtual ICollection<blending_plan_source> blending_plan_source { get; set; }
        public virtual ICollection<blending_plan_value> blending_plan_value { get; set; }
        public virtual ICollection<business_area> business_area { get; set; }
        public virtual ICollection<business_days> business_days { get; set; }
        public virtual ICollection<business_partner> business_partner { get; set; }
        public virtual ICollection<business_unit> business_unit { get; set; }
        public virtual ICollection<calendar> calendar { get; set; }
        public virtual ICollection<chls> chls { get; set; }
        public virtual ICollection<chls_additional_info> chls_additional_info { get; set; }
        public virtual ICollection<chls_cpp> chls_cpp { get; set; }
        public virtual ICollection<chls_hauling> chls_hauling { get; set; }
        public virtual ICollection<city> city { get; set; }
        public virtual ICollection<coa> coa { get; set; }
        public virtual ICollection<coa_subaccount> coa_subaccount { get; set; }
        public virtual ICollection<coal_transfer> coal_transfer { get; set; }
        public virtual ICollection<contact> contact { get; set; }
        public virtual ICollection<contractor> contractor { get; set; }
        public virtual ICollection<contractor_document> contractor_document { get; set; }
        public virtual ICollection<cost_rate> cost_rate { get; set; }
        public virtual ICollection<country> country { get; set; }
        public virtual ICollection<cpp_location> cpp_location { get; set; }
        public virtual ICollection<currency> currency { get; set; }
        public virtual ICollection<currency_exchange> currency_exchange { get; set; }
        public virtual ICollection<customer> customer { get; set; }
        public virtual ICollection<daily_record> daily_record { get; set; }
        public virtual ICollection<data_extension> data_extension { get; set; }
        public virtual ICollection<daywork> daywork { get; set; }
        public virtual ICollection<daywork_closing> daywork_closing { get; set; }
        public virtual ICollection<delay> delay { get; set; }
        public virtual ICollection<delay_category> delay_category { get; set; }
        public virtual ICollection<delay_details> delay_details { get; set; }
        public virtual ICollection<despatch_demurrage> despatch_demurrage { get; set; }
        public virtual ICollection<despatch_demurrage_debit_credit_note> despatch_demurrage_debit_credit_note { get; set; }
        public virtual ICollection<despatch_demurrage_delay> despatch_demurrage_delay { get; set; }
        public virtual ICollection<despatch_demurrage_detail> despatch_demurrage_detail { get; set; }
        public virtual ICollection<despatch_demurrage_invoice> despatch_demurrage_invoice { get; set; }
        public virtual ICollection<despatch_order_delay> despatch_order_delay { get; set; }
        public virtual ICollection<despatch_order> despatch_orderorganization_ { get; set; }
        public virtual ICollection<despatch_order> despatch_orderseller_ { get; set; }
        public virtual ICollection<document_type> document_type { get; set; }
        public virtual ICollection<draft_survey> draft_survey { get; set; }
        public virtual ICollection<draft_survey_detail> draft_survey_detail { get; set; }
        public virtual ICollection<drill_blast_plan> drill_blast_plan { get; set; }
        public virtual ICollection<eight_week_forecast> eight_week_forecast { get; set; }
        public virtual ICollection<eight_week_forecast_item> eight_week_forecast_item { get; set; }
        public virtual ICollection<eight_week_forecast_item_detail> eight_week_forecast_item_detail { get; set; }
        public virtual ICollection<email_notification> email_notification { get; set; }
        public virtual ICollection<email_recipient> email_recipient { get; set; }
        public virtual ICollection<employee> employee { get; set; }
        public virtual ICollection<equipment> equipment { get; set; }
        public virtual ICollection<equipment_category> equipment_category { get; set; }
        public virtual ICollection<equipment_cost_rate> equipment_cost_rate { get; set; }
        public virtual ICollection<equipment_group> equipment_group { get; set; }
        public virtual ICollection<equipment_group_item> equipment_group_item { get; set; }
        public virtual ICollection<equipment_incident> equipment_incident { get; set; }
        public virtual ICollection<equipment_type> equipment_type { get; set; }
        public virtual ICollection<equipment_usage_transaction> equipment_usage_transaction { get; set; }
        public virtual ICollection<equipment_usage_transaction_detail> equipment_usage_transaction_detail { get; set; }
        public virtual ICollection<event_category> event_category { get; set; }
        public virtual ICollection<event_definition_category> event_definition_category { get; set; }
        public virtual ICollection<explosive_usage_plan> explosive_usage_plan { get; set; }
        public virtual ICollection<explosive_usage_plan_detail> explosive_usage_plan_detail { get; set; }
        public virtual ICollection<exposed_coal> exposed_coal { get; set; }
        public virtual ICollection<fuel_consumption> fuel_consumption { get; set; }
        public virtual ICollection<hauling_closing> hauling_closing { get; set; }
        public virtual ICollection<hauling_closing_item> hauling_closing_item { get; set; }
        public virtual ICollection<hauling_plan> hauling_plan { get; set; }
        public virtual ICollection<hauling_plan_daily> hauling_plan_daily { get; set; }
        public virtual ICollection<hauling_plan_history> hauling_plan_history { get; set; }
        public virtual ICollection<hauling_plan_monthly> hauling_plan_monthly { get; set; }
        public virtual ICollection<hauling_plan_monthly_history> hauling_plan_monthly_history { get; set; }
        public virtual ICollection<hauling_transaction> hauling_transaction { get; set; }
        public virtual ICollection<haze> haze { get; set; }
        public virtual ICollection<incident> incident { get; set; }
        public virtual ICollection<incident_category> incident_category { get; set; }
        public virtual ICollection<initial_information> initial_information { get; set; }
        public virtual ICollection<inventory_transaction> inventory_transaction { get; set; }
        public virtual ICollection<item_group> item_group { get; set; }
        public virtual ICollection<item_list> item_list { get; set; }
        public virtual ICollection<joint_survey> joint_survey { get; set; }
        public virtual ICollection<joint_survey_analyte> joint_survey_analyte { get; set; }
        public virtual ICollection<land_clearing_detail> land_clearing_detail { get; set; }
        public virtual ICollection<list_key> list_key { get; set; }
        public virtual ICollection<list_value> list_value { get; set; }
        public virtual ICollection<master_list> master_list { get; set; }
        public virtual ICollection<material_type> material_type { get; set; }
        public virtual ICollection<material_type_analyte> material_type_analyte { get; set; }
        public virtual ICollection<mine_location> mine_location { get; set; }
        public virtual ICollection<mine_location_quality> mine_location_quality { get; set; }
        public virtual ICollection<mine_location_quality_pit> mine_location_quality_pit { get; set; }
        public virtual ICollection<mine_plan_geology> mine_plan_geology { get; set; }
        public virtual ICollection<mine_plan_ltp> mine_plan_ltp { get; set; }
        public virtual ICollection<model_geology> model_geology { get; set; }
        public virtual ICollection<national_holiday> national_holiday { get; set; }
        public virtual ICollection<port_location> port_location { get; set; }
        public virtual ICollection<price_adjustment_product_specification> price_adjustment_product_specification { get; set; }
        public virtual ICollection<price_index> price_index { get; set; }
        public virtual ICollection<price_index_history> price_index_history { get; set; }
        public virtual ICollection<price_index_map> price_index_map { get; set; }
        public virtual ICollection<price_index_map_detail> price_index_map_detail { get; set; }
        public virtual ICollection<price_index_product_specification> price_index_product_specification { get; set; }
        public virtual ICollection<price_series> price_series { get; set; }
        public virtual ICollection<price_series_detail> price_series_detail { get; set; }
        public virtual ICollection<process_flow> process_flow { get; set; }
        public virtual ICollection<process_flow_analyte> process_flow_analyte { get; set; }
        public virtual ICollection<processing_category> processing_category { get; set; }
        public virtual ICollection<processing_closing> processing_closing { get; set; }
        public virtual ICollection<processing_closing_item> processing_closing_item { get; set; }
        public virtual ICollection<processing_transaction> processing_transaction { get; set; }
        public virtual ICollection<product> product { get; set; }
        public virtual ICollection<product_category> product_category { get; set; }
        public virtual ICollection<product_specification> product_specification { get; set; }
        public virtual ICollection<product_standard> product_standard { get; set; }
        public virtual ICollection<production_closing> production_closing { get; set; }
        public virtual ICollection<production_closing_item> production_closing_item { get; set; }
        public virtual ICollection<production_plan> production_plan { get; set; }
        public virtual ICollection<production_plan_daily> production_plan_daily { get; set; }
        public virtual ICollection<production_plan_history> production_plan_history { get; set; }
        public virtual ICollection<production_plan_monthly> production_plan_monthly { get; set; }
        public virtual ICollection<production_plan_monthly_history> production_plan_monthly_history { get; set; }
        public virtual ICollection<production_transaction> production_transaction { get; set; }
        public virtual ICollection<production_transaction_item> production_transaction_item { get; set; }
        public virtual ICollection<progress_claim> progress_claim { get; set; }
        public virtual ICollection<progress_claim_charge> progress_claim_charge { get; set; }
        public virtual ICollection<province> province { get; set; }
        public virtual ICollection<quality_sampling> quality_sampling { get; set; }
        public virtual ICollection<quality_sampling_analyte> quality_sampling_analyte { get; set; }
        public virtual ICollection<quality_sampling_approval> quality_sampling_approval { get; set; }
        public virtual ICollection<railing_transaction> railing_transaction { get; set; }
        public virtual ICollection<railing_transaction_detail> railing_transaction_detail { get; set; }
        public virtual ICollection<rainfall> rainfall { get; set; }
        public virtual ICollection<ready_to_get> ready_to_get { get; set; }
        public virtual ICollection<rehandling_closing> rehandling_closing { get; set; }
        public virtual ICollection<rehandling_transaction> rehandling_transaction { get; set; }
        public virtual ICollection<report_template> report_template { get; set; }
        public virtual ICollection<role_access> role_access { get; set; }
        public virtual ICollection<role_business_unit> role_business_unit { get; set; }
        public virtual ICollection<royalty> royalty { get; set; }
        public virtual ICollection<royalty_cost> royalty_cost { get; set; }
        public virtual ICollection<royalty_payment> royalty_payment { get; set; }
        public virtual ICollection<royalty_pricing> royalty_pricing { get; set; }
        public virtual ICollection<royalty_quantity_quality> royalty_quantity_quality { get; set; }
        public virtual ICollection<royalty_valuation> royalty_valuation { get; set; }
        public virtual ICollection<sales_contract_charges> sales_contract_charges { get; set; }
        public virtual ICollection<sales_contract_despatch_demurrage_delay> sales_contract_despatch_demurrage_delay { get; set; }
        public virtual ICollection<sales_contract_despatch_demurrage_term> sales_contract_despatch_demurrage_term { get; set; }
        public virtual ICollection<sales_contract_despatch_plan> sales_contract_despatch_plan { get; set; }
        public virtual ICollection<sales_contract_detail> sales_contract_detail { get; set; }
        public virtual ICollection<sales_contract_payment_term> sales_contract_payment_term { get; set; }
        public virtual ICollection<sales_contract_product> sales_contract_product { get; set; }
        public virtual ICollection<sales_contract_product_specifications> sales_contract_product_specifications { get; set; }
        public virtual ICollection<sales_contract_quotation_price> sales_contract_quotation_price { get; set; }
        public virtual ICollection<sales_contract_taxes> sales_contract_taxes { get; set; }
        public virtual ICollection<sales_contract_term> sales_contract_term { get; set; }
        public virtual ICollection<sales_contract> sales_contractorganization_ { get; set; }
        public virtual ICollection<sales_contract> sales_contractseller_ { get; set; }
        public virtual ICollection<sales_demurrage_rate> sales_demurrage_rate { get; set; }
        public virtual ICollection<sales_invoice> sales_invoice { get; set; }
        public virtual ICollection<sales_invoice_charges> sales_invoice_charges { get; set; }
        public virtual ICollection<sales_invoice_detail> sales_invoice_detail { get; set; }
        public virtual ICollection<sales_order> sales_order { get; set; }
        public virtual ICollection<sales_order_detail> sales_order_detail { get; set; }
        public virtual ICollection<sales_plan> sales_plan { get; set; }
        public virtual ICollection<sales_plan_detail> sales_plan_detail { get; set; }
        public virtual ICollection<sales_plan_snapshot> sales_plan_snapshot { get; set; }
        public virtual ICollection<sales_price> sales_price { get; set; }
        public virtual ICollection<sales_price_type> sales_price_type { get; set; }
        public virtual ICollection<sales_product_specification> sales_product_specification { get; set; }
        public virtual ICollection<sampling_template> sampling_template { get; set; }
        public virtual ICollection<sampling_template_detail> sampling_template_detail { get; set; }
        public virtual ICollection<shared_record> shared_record { get; set; }
        public virtual ICollection<shift> shift { get; set; }
        public virtual ICollection<shift_category> shift_category { get; set; }
        public virtual ICollection<shipment_forecast> shipment_forecast { get; set; }
        public virtual ICollection<shipment_plan> shipment_plan { get; set; }
        public virtual ICollection<shipping_closing> shipping_closing { get; set; }
        public virtual ICollection<shipping_delay> shipping_delay { get; set; }
        public virtual ICollection<shipping_document> shipping_document { get; set; }
        public virtual ICollection<shipping_instruction> shipping_instruction { get; set; }
        public virtual ICollection<shipping_instruction_asuransi> shipping_instruction_asuransi { get; set; }
        public virtual ICollection<shipping_instruction_detail_survey> shipping_instruction_detail_survey { get; set; }
        public virtual ICollection<shipping_instruction_detail_survey_document> shipping_instruction_detail_survey_document { get; set; }
        public virtual ICollection<shipping_instruction_document_agent> shipping_instruction_document_agent { get; set; }
        public virtual ICollection<shipping_instruction_pekerjaan_agent> shipping_instruction_pekerjaan_agent { get; set; }
        public virtual ICollection<shipping_instruction_stevedoring> shipping_instruction_stevedoring { get; set; }
        public virtual ICollection<shipping_instruction_to_company> shipping_instruction_to_company { get; set; }
        public virtual ICollection<shipping_instruction_tug_boat> shipping_instruction_tug_boat { get; set; }
        public virtual ICollection<shipping_load_unload_document> shipping_load_unload_document { get; set; }
        public virtual ICollection<shipping_program> shipping_program { get; set; }
        public virtual ICollection<shipping_program_detail> shipping_program_detail { get; set; }
        public virtual ICollection<shipping_transaction> shipping_transaction { get; set; }
        public virtual ICollection<shipping_transaction_detail> shipping_transaction_detail { get; set; }
        public virtual ICollection<slippery> slippery { get; set; }
        public virtual ICollection<sof> sof { get; set; }
        public virtual ICollection<sof_detail> sof_detail { get; set; }
        public virtual ICollection<standard_cost_mining> standard_cost_mining { get; set; }
        public virtual ICollection<standard_cost_mining_detail> standard_cost_mining_detail { get; set; }
        public virtual ICollection<stock_location> stock_location { get; set; }
        public virtual ICollection<stock_state> stock_state { get; set; }
        public virtual ICollection<stock_state_analyte> stock_state_analyte { get; set; }
        public virtual ICollection<stockpile_location> stockpile_location { get; set; }
        public virtual ICollection<stockpile_state> stockpile_state { get; set; }
        public virtual ICollection<survey> survey { get; set; }
        public virtual ICollection<survey_analyte> survey_analyte { get; set; }
        public virtual ICollection<survey_detail> survey_detail { get; set; }
        public virtual ICollection<team> team { get; set; }
        public virtual ICollection<team_member> team_member { get; set; }
        public virtual ICollection<team_role> team_role { get; set; }
        public virtual ICollection<tidalwave> tidalwave { get; set; }
        public virtual ICollection<timesheet> timesheet { get; set; }
        public virtual ICollection<timesheet_detail> timesheet_detail { get; set; }
        public virtual ICollection<timesheet_detail_event> timesheet_detail_event { get; set; }
        public virtual ICollection<timesheet_detail_event_plan> timesheet_detail_event_plan { get; set; }
        public virtual ICollection<timesheet_detail_plan> timesheet_detail_plan { get; set; }
        public virtual ICollection<timesheet_detail_productivity_problem> timesheet_detail_productivity_problem { get; set; }
        public virtual ICollection<timesheet_detail_productivity_problem_plan> timesheet_detail_productivity_problem_plan { get; set; }
        public virtual ICollection<timesheet_plan> timesheet_plan { get; set; }
        public virtual ICollection<train> train { get; set; }
        public virtual ICollection<transport> transport { get; set; }
        public virtual ICollection<truck> truck { get; set; }
        public virtual ICollection<truck_cost_rate> truck_cost_rate { get; set; }
        public virtual ICollection<tug> tug { get; set; }
        public virtual ICollection<uom> uom { get; set; }
        public virtual ICollection<uom_category> uom_category { get; set; }
        public virtual ICollection<uom_conversion> uom_conversion { get; set; }
        public virtual ICollection<user_role> user_role { get; set; }
        public virtual ICollection<vessel> vessel { get; set; }
        public virtual ICollection<wagon> wagon { get; set; }
        public virtual ICollection<waste> waste { get; set; }
        public virtual ICollection<waste_category> waste_category { get; set; }
        public virtual ICollection<waste_location> waste_location { get; set; }
        public virtual ICollection<waste_removal> waste_removal { get; set; }
        public virtual ICollection<waste_removal_closing_item> waste_removal_closing_item { get; set; }
        public virtual ICollection<waste_removal_item> waste_removal_item { get; set; }
        public virtual ICollection<waste_specification> waste_specification { get; set; }
        public virtual ICollection<weigh_bridge> weigh_bridge { get; set; }
        public virtual ICollection<wip_location> wip_location { get; set; }
    }
}
