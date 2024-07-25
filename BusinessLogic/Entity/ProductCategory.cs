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
    public partial class ProductCategory: ServiceRepository<product_category, vw_product_category>
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly UserContext userContext;

        public ProductCategory(UserContext userContext)
            : base(userContext.GetDataContext())
        {
            this.userContext = userContext;
        }
    }
}
