using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Utilities;
using DataAccess;
using DataAccess.Repository;
using NLog;
using Omu.ValueInjecter;
using PetaPoco;

namespace BusinessLogic.Entity
{
    public partial class BlendingPlan: ServiceRepository<blending_plan, vw_blending_plan>
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly UserContext userContext;

        public BlendingPlan(UserContext userContext)
            : base(userContext.GetDataContext())
        {
            this.userContext = userContext;
        }

        public async Task<StandardResult> Calculate(string BlendingPlanId)
        {
            var result = new StandardResult();

            var db = userContext.GetDataContext().Database;
            using(var tx = db.GetTransaction())
            {
                try
                {
                    var blendingPlan = await db.FirstOrDefaultAsync<vw_blending_plan>("WHERE id = @0", BlendingPlanId);
                    if(blendingPlan != null)
                    {
                        var sql = Sql.Builder.Append(" SELECT * FROM vw_analyte WHERE id IN ( "
                            + " SELECT DISTINCT(analyte_id) FROM survey_analyte sa "
                            + " INNER JOIN blending_plan_source bps ON bps.survey_id = sa.survey_id "
                            + " INNER JOIN blending_plan bp ON bp.id = bps.blending_plan_id "
                            + " WHERE bp.id = @0 ) ", blendingPlan.id);
                        var analytes = await db.FetchAsync<vw_analyte>(sql);

                        var blendingPlanSources = await db.FetchAsync<vw_blending_plan_source>(
                                "WHERE blending_plan_id = @0", blendingPlan.id);
                        if(blendingPlanSources?.Count > 0 && analytes?.Count > 0)
                        {
                            foreach(var bps in blendingPlanSources)
                            {
                                if (string.IsNullOrEmpty(bps.survey_id))
                                    continue;

                                foreach(var a in analytes)
                                {
                                    sql = Sql.Builder.Append(" SELECT * FROM survey_analyte "
                                        + " WHERE survey_id = @0 AND analyte_id = @1 ", 
                                        bps.survey_id, a.id);
                                    var sa = await db.FirstOrDefaultAsync<survey_analyte>(sql);

                                    sql = Sql.Builder.Append(" SELECT * FROM blending_plan_value "
                                        + " WHERE blending_plan_source_id = @0 AND analyte_id = @1 ",
                                        bps.id, a.id);
                                    var bpv = await db.FirstOrDefaultAsync<blending_plan_value>(sql);

                                    if(bpv == null)
                                    {
                                        #region Insert blending_plan_value

                                        var e = new entity();
                                        e.InjectFrom(blendingPlan);

                                        bpv = new blending_plan_value();
                                        bpv.InjectFrom(e);

                                        bpv.created_by = userContext.AppUserId;
                                        bpv.created_on = DateTime.Now;

                                        bpv.blending_plan_source_id = bps.id;
                                        bpv.quantity = bps.loading_quantity;
                                        bpv.uom_quantity_id = bps.uom_id;
                                        bpv.analyte_id = a.id;
                                        bpv.analyte_value = sa?.analyte_value;
                                        bpv.uom_analyte_id = a.uom_id;

                                        await db.InsertAsync(bpv);

                                        #endregion
                                    }
                                    else
                                    {
                                        #region Update blending_plan_value

                                        bpv.modified_by = userContext.AppUserId;
                                        bpv.modified_on = DateTime.Now;

                                        bpv.blending_plan_source_id = bps.id;
                                        bpv.quantity = bps.loading_quantity;
                                        bpv.uom_quantity_id = bps.uom_id;
                                        bpv.analyte_id = a.id;
                                        bpv.analyte_value = sa?.analyte_value;
                                        bpv.uom_analyte_id = a.uom_id;

                                        await db.UpdateAsync(bpv);

                                        #endregion
                                    }
                                }
                            }

                            tx.Complete();

                            sql = Sql.Builder.Append(" SELECT bpv.* FROM vw_blending_plan_value bpv "
                                + " INNER JOIN blending_plan_source bps ON bps.id = bpv.blending_plan_source_id "
                                + " INNER JOIN blending_plan bp ON bp.id = bps.blending_plan_id "
                                + " WHERE bp.id = @0 ", blendingPlan.id);
                            result.Data = await db.FetchAsync<vw_blending_plan_value>(sql);
                            result.Success = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    result.Message = ex.Message;
                }
            }

            return result;
        }
    }
}
