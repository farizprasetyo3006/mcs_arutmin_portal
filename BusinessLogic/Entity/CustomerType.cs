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
    public partial class CustomerType : ServiceRepository<currency, vw_currency>
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly UserContext userContext;

        public CustomerType(UserContext userContext)
            : base(userContext.GetDataContext())
        {
            this.userContext = userContext;
        }
    }
}
