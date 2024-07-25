using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Utilities;
using DataAccess;
using DataAccess.DTO;
using DataAccess.Repository;
using NLog;
using Omu.ValueInjecter;
using PetaPoco;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace BusinessLogic.Entity
{
    public partial class Customer : ServiceRepository<customer, vw_customer>
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly UserContext userContext;
        protected readonly IConfiguration configuration;
        public Customer(UserContext userContext)
            : base(userContext.GetDataContext())
        {
            this.userContext = userContext;            
        }

        public async Task<StandardResult> SaveCustomer(CustomerDTO customerDTO)
        {
            var result = new StandardResult();

            try
            {
                var db = context.Database;

                using (var tx = db.GetTransaction())
                {
                    try
                    {
                        var success = false;

                        var record = await GetByIdAsync(customerDTO.id);
                        logger.Trace(db.LastCommand);

                        if (record == null)
                        {
                            //fill the customer object
                            var custRecord = new customer();
                            custRecord.InjectFrom(customerDTO);
                            custRecord.id = Guid.NewGuid().ToString("N");
                            custRecord.created_by = userContext.AppUserId;
                            custRecord.created_on = DateTime.Now;
                            custRecord.modified_by = null;
                            custRecord.modified_on = null;
                            custRecord.is_active = true;
                            custRecord.is_default = null;
                            custRecord.is_locked = null;
                            custRecord.entity_id = null;
                            custRecord.owner_id = userContext.AppUserId;
                            custRecord.organization_id = userContext.OrganizationId;
                            custRecord.is_customer = true;
                            custRecord.is_government = null;
                            custRecord.is_vendor = null;

                            #region Create or update customer
                            success = await context.SaveEntity(custRecord);
                            logger.Trace(db.LastCommand);
                            #endregion

                            #region Create or update customer attachment
                            if (success)
                            {
                                if(customerDTO.customer_Attachments.Count > 0)
                                {
                                    //save to server
                                    var uploadPath = configuration["Path:UploadPathCustomer"].ToString();
                                    var filePath = uploadPath + custRecord.id;
                                    if (!System.IO.Directory.Exists(filePath))
                                    {
                                        System.IO.Directory.CreateDirectory(filePath);
                                    }


                                    foreach (var formFile in customerDTO.customer_Attachments)
                                    {
                                        if (formFile.Length > 0)
                                        {
                                            //save to folder
                                            var fullPath = filePath + formFile.FileName;
                                            if (System.IO.File.Exists(fullPath))
                                            {
                                                System.IO.File.Delete(fullPath);
                                            }
                                            using (var stream = System.IO.File.Create(fullPath))
                                            {
                                                await formFile.CopyToAsync(stream);
                                            }
                                            string savedFileName = custRecord.id + "\\" + formFile.FileName;
                                            
                                            //insert data to customer attachment
                                            var customerAttachment = new customer_attachment()
                                            {
                                                id = Guid.NewGuid().ToString("N"),
                                                created_by = userContext.AppUserId,
                                                created_on = DateTime.Now,
                                                modified_by = null,
                                                modified_on = null,
                                                is_active = true,
                                                is_locked = null,
                                                is_default = null,
                                                owner_id = userContext.AppUserId,
                                                organization_id = userContext.OrganizationId,
                                                entity_id = null,
                                                filename = savedFileName,
                                                customer_id = custRecord.id
                                            };

                                            success = await context.SaveEntity(customerAttachment);
                                            logger.Trace(db.LastCommand);
                                        }
                                    }                                                                        
                                }
                                
                            }
                            #endregion

                            if (success)
                            {
                                
                                tx.Complete();
                            }
                        }
                        else
                        {
                            var r = await SaveWithMapEntity(record, (isNew, success) =>
                            {
                                result.Success = success;
                            });

                            if (result.Success)
                            {
                                result.Data = r;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.ToString());
                        result.Message = ex.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                result.Message = ex.Message;
            }

            return result;
        }
    }
}
