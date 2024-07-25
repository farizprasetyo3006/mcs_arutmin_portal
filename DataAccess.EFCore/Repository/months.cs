using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class months
    {
        public months()
        {
            barging_plan_monthly = new HashSet<barging_plan_monthly>();
            barging_plan_monthly_history = new HashSet<barging_plan_monthly_history>();
            hauling_plan_monthly = new HashSet<hauling_plan_monthly>();
            hauling_plan_monthly_history = new HashSet<hauling_plan_monthly_history>();
            production_plan_monthly = new HashSet<production_plan_monthly>();
            production_plan_monthly_history = new HashSet<production_plan_monthly_history>();
            sales_plan_detail = new HashSet<sales_plan_detail>();
        }

        public string nama_bulan { get; set; }
        public string month_name { get; set; }
        public int id { get; set; }
        public string month_code { get; set; }

        public virtual ICollection<barging_plan_monthly> barging_plan_monthly { get; set; }
        public virtual ICollection<barging_plan_monthly_history> barging_plan_monthly_history { get; set; }
        public virtual ICollection<hauling_plan_monthly> hauling_plan_monthly { get; set; }
        public virtual ICollection<hauling_plan_monthly_history> hauling_plan_monthly_history { get; set; }
        public virtual ICollection<production_plan_monthly> production_plan_monthly { get; set; }
        public virtual ICollection<production_plan_monthly_history> production_plan_monthly_history { get; set; }
        public virtual ICollection<sales_plan_detail> sales_plan_detail { get; set; }
    }
}
