using BusinessLogic.Utilities;
using DataAccess;
using DataAccess.DTO;
using Newtonsoft.Json;
using NLog;
using PetaPoco;
using PetaPoco.Providers;
using System;

namespace BusinessLogic
{
    public partial class UserContext
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private string _connectionString;

        public string OrganizationId { get; set; }
        public string AppUserId { get; set; }
        public string AppUsername { get; set; }
        public string AppFullname { get; set; }        
        public bool IsSysAdmin { get; set; }
        public string AccessToken { get; set; }
        public DateTime TokenExpiry { get; set; }
        public SysAdmin SystemAdministrator { get; set; }
        public string RoleId { get; set; }
        public string BusinessUnitId { get; set; }
        public string ConnectionString
        {
            get
            {
                return _connectionString;
            }

            set
            {
                if (_connectionString != value)
                {
                    _connectionString = value;
                }
            }
        }

        public DataContext GetDataContext(bool DecryptConnectionString = true)
        {
            DataContext result = null;

            try
            {
                var repository = DatabaseConfiguration
                    .Build()
                    .UsingConnectionString(DecryptConnectionString ?
                        Encryptor.DecryptString(ConnectionString)
                        : ConnectionString)
                    .UsingProvider<PostgreSQLDatabaseProvider>()
                    .Create();

                result = new DataContext(repository, AppUsername, SystemAdministrator);
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());                
            }

            return result;
        }
    }
}