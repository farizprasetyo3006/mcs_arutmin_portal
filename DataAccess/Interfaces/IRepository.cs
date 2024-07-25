using DataAccess.DataTables;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess.Interfaces
{
    public interface IRepository<TEntity, TEntityView>
    {
        #region TEntity
        Task<TEntity> SaveWithMapEntity(TEntity record, Action<bool, bool> continueWith);
        Task<TEntity> GetByIdAsync(string id);

        Task<TEntity> GetFirstOrDefaultAsync(string sqlFilter, params object[] args);

        Task<TEntity> GetFirstOrDefaultAsync(Sql sqlFilter);

        Task<List<TEntity>> GetAllAsync();

        Task<List<TEntity>> GetAllAsync(Sql sqlFilter);

        Task<List<TEntity>> GetAllAsync(string sqlFilter, params object[] args);

        Task<Page<TEntity>> GetAllAsync(long page, long itemsPerPage);

        Task<Page<TEntity>> GetAllAsync(long page, long itemsPerPage, Sql sqlFilter);

        Task<Page<TEntity>> GetAllAsync(long page, long itemsPerPage, string sqlFilter, params object[] args);

        Task<long?> GetCountAsync();

        Task<long?> GetCountAsync(Sql sqlFilter);

        Task<long?> GetCountAsync(string sqlFilter, params object[] args);

        Task<bool?> RemoveAsync(string id);

        Task<bool?> RemoveAllAsync(List<string> ids);

        #endregion TEntity

        #region TEntityView

        Task<TEntityView> GetViewByIdAsync(string id);

        Task<TEntityView> GetViewFirstOrDefaultAsync(Sql sqlFilter);

        Task<TEntityView> GetViewFirstOrDefaultAsync(string sqlFilter, params object[] args);

        Task<List<TEntityView>> GetViewAllAsync();

        Task<List<TEntityView>> GetViewAllAsync(Sql sqlFilter);

        Task<List<TEntityView>> GetViewAllAsync(string sqlFilter, params object[] args);

        Task<Page<TEntityView>> GetViewAllAsync(long page, long itemsPerPage);

        Task<Page<TEntityView>> GetViewAllAsync(long page, long itemsPerPage, Sql sqlFilter);

        Task<Page<TEntityView>> GetViewAllAsync(long page, long itemsPerPage, string sqlFilter, params object[] args);

        Task<long?> GetViewCountAsync();

        Task<long?> GetViewCountAsync(Sql sqlFilter);

        Task<long?> GetViewCountAsync(string sqlFilter, params object[] args);

        Task<DTResponse> GetDataTablesResponseAsync(DTRequest request, Sql additionalFilters = null);
        #endregion TEntityView
    }
}