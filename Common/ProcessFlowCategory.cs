using System;
using System.Collections.Generic;

namespace Common
{
    public class ProcessFlowCategory
    {
        public const string WASTE_REMOVAL = "Waste Removal";
        public const string COAL_MINED = "Coal Mined";
        public const string COAL_PRODUCE = "Coal Produce";
        public const string TRUCKING = "Trucking";
        public const string BLENDING = "Blending";
        public const string HAULING = "Hauling";
        public const string COAL_TRANSFER = "Coal Transfer";
        public const string BARGING = "Barging";
        public const string SHIPPING = "Shipping";
        public const string REHANDLING = "Rehandling";
        public const string COALMOVEMENT = "Coal Movement";

        public static List<string> ProcessFlowCategories = new List<string>
        {
            WASTE_REMOVAL,
            COAL_MINED,
            HAULING,
            COAL_PRODUCE,
            COAL_TRANSFER,
            BLENDING,
            REHANDLING,
            TRUCKING,
            BARGING,
            SHIPPING,
            COALMOVEMENT
        };
    }
}
