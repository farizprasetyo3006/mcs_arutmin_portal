using DataAccess.Interfaces;
using DataAccess.Repository;
using Newtonsoft.Json;
using Omu.ValueInjecter;
using PetaPoco;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace DataAccess
{
    public partial class DataContext
    {
        public T MapToEntity<T>(T record, ref bool IsNew) where T : IEntity
        {
            var id = "";
            var result = Activator.CreateInstance<T>();
            if (!string.IsNullOrEmpty(record.id)) id = record.id;

            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    IsNew = true;
                    result = Activator.CreateInstance<T>();
                    logger.Debug($"New {typeof(T).Name} record");
                }
                else
                {
                    result = Database.FirstOrDefault<T>(" WHERE id = @0 ", id);
                    logger.Trace(Database.LastCommand);
                    logger.Trace("Record Map Entity");
                    logger.Trace(JsonConvert.SerializeObject(result));

                    IsNew = (result == null);
                    if (IsNew)
                    {
                        result = Activator.CreateInstance<T>();
                        logger.Debug($"New {typeof(T).Name} record");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }
            finally
            {
                logger.Debug(Database.LastCommand);
            }

            PropertyInfo[] destinationProperties = result.GetType().GetProperties();


            if (IsNew)
            {
                foreach (PropertyInfo destinationPi in destinationProperties)
                {
                    try
                    {
                        PropertyInfo sourcePi = record.GetType().GetProperty(destinationPi.Name);

                        if (destinationPi.Name == "ModifiedColumns")
                            continue;

                        if (destinationPi.Name == "organization_id")
                        {
                            if (destinationPi.CanWrite)
                            {
                                destinationPi.SetValue(result, this.OrganizationId, null);
                            }

                            continue;
                        }
                        if (sourcePi.GetValue(record, null) == null) continue;

                        if (destinationPi.CanWrite)
                            destinationPi.SetValue(result, sourcePi.GetValue(record, null), null);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.ToString());
                    }
                }
            }
            else
            {
                foreach (PropertyInfo destinationPi in destinationProperties)
                {
                    try
                    {
                        if (destinationPi.Name == "ModifiedColumns")
                            continue;

                        PropertyInfo sourcePi = record.GetType().GetProperty(destinationPi.Name);
                        if (sourcePi.GetValue(record, null) == null) continue;

                        if (destinationPi.CanWrite)
                            destinationPi.SetValue(result, sourcePi.GetValue(record, null), null);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.ToString());                        
                    }
                }
            }

            return result;
        }

        public T MapValues<T>(string values, ref bool IsNew) where T : IEntity
        {
            var id = "";
            var result = Activator.CreateInstance<T>();
            dynamic d = JsonConvert.DeserializeObject<dynamic>(values);
            logger.Trace($"Is an object = { d != null }");

            try
            {
                if (d != null)
                {
                    id = (string)d.id;
                }

                if (string.IsNullOrEmpty(id))
                {
                    IsNew = true;
                    result = Activator.CreateInstance<T>();
                    logger.Debug($"New {typeof(T).Name} record");
                }
                else
                {
                    result = Database.FirstOrDefault<T>(" WHERE id = @0 ", id);
                    logger.Debug("Record Map Entity");
                    logger.Debug(JsonConvert.SerializeObject(result));
                    IsNew = (result == null);
                    if (IsNew)
                    {
                        result = Activator.CreateInstance<T>();
                        logger.Debug($"New {typeof(T).Name} record");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }
            finally
            {
                logger.Debug(Database.LastCommand);
            }

            if (IsNew)
            {
                JsonConvert.PopulateObject(values, result);
                result.created_by = AppUserId;
                result.created_on = DateTime.Now;
                result.owner_id = AppUserId;
                result.is_active = true;
                result.organization_id = OrganizationId;
            }
            else if (d != null && !string.IsNullOrEmpty(id))
            {
                var e = new entity();
                e.InjectFrom(result);
                JsonConvert.PopulateObject(values, result);
                result.InjectFrom(e);
            }

            return result;
        }
    }
}