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
    public partial class AdvanceContract: ServiceRepository<advance_contract, vw_advance_contract>
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly UserContext userContext;

        public AdvanceContract(UserContext userContext)
            : base(userContext.GetDataContext())
        {
            this.userContext = userContext;
        }
    }
}
