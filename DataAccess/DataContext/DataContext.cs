using DataAccess.DTO;
using DataAccess.Repository;
using Newtonsoft.Json;
using NLog;
using PetaPoco;
using System;

namespace DataAccess
{
    public partial class DataContext
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public string AppUserId { get; private set; }
        public string AppUsername { get; private set; }
        public string AppFullname { get; private set; }
        public string OrganizationId { get; private set; }
        public string OrganizationName { get; private set; }
        public bool IsDefaultUser { get; private set; }
        public bool IsSysAdmin { get; private set; }

        public IDatabase Database { get; }

        public DataContext(IDatabase Database, string Username, SysAdmin sysAdmin = null)
        {
            this.Database = Database;

            try
            {
                if (!string.IsNullOrEmpty(Username))
                {
                    var applicationUser = this.Database.FirstOrDefault<vw_application_user>(
                        " WHERE is_active = TRUE AND application_username = @0 ", Username);
                    logger.Trace(this.Database.LastCommand);

                    if (applicationUser != null)
                    {
                        logger.Trace("applicationUser != null");

                        AppUserId = applicationUser.id;
                        AppUsername = applicationUser.application_username;
                        AppFullname = applicationUser.fullname;
                        OrganizationId = applicationUser.organization_id;
                        OrganizationName = applicationUser.organization_name;
                        IsDefaultUser = applicationUser.is_default ?? false;
                        IsSysAdmin = applicationUser.is_sysadmin ?? false;
                    }
                    else if (sysAdmin != null && sysAdmin.IsEnabled)
                    {
                        logger.Trace("sysAdmin != null && sysAdmin.IsEnabled");

                        AppUserId = Guid.Empty.ToString("N");
                        AppUsername = sysAdmin.Username;
                        OrganizationId = Guid.Empty.ToString("N");
                        IsDefaultUser = true;
                        IsSysAdmin = true;
                    }
                    else
                    {
                        logger.Trace($"sysAdmin = {JsonConvert.SerializeObject(sysAdmin)}");
                    }
                }
                else
                {
                    logger.Trace("string.IsNullOrEmpty(Username)");
                }
            }
            catch (Exception ex)
            {
                logger.Debug(this.Database.LastCommand);
                logger.Error(ex.ToString());
            }
        }
    }
}