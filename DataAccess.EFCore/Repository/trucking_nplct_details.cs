﻿using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class trucking_nplct_details
    {
        public string details { get; set; }
        public decimal? bit_coal { get; set; }
        public decimal? eco_coal { get; set; }
        public decimal? sarongga { get; set; }
        public decimal? total { get; set; }
    }
}
