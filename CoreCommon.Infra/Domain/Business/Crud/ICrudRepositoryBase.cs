using System.Collections.Generic;

namespace CoreCommon.Infra.Domain.Business.Crud
{
    /// <summary>
    /// Repository base interface
    /// </summary>
    /// <typeparam name="TEntity">Entity Type</typeparam>
    public interface ICrudRepositoryBase<TEntity>
    {
        IEnumerable<TEntity> GetAll();
        TEntity Add(TEntity entity);
        int Delete(TEntity entity);
        int Edit(TEntity entity);
        int BulkInsert(List<TEntity> entities);
        int BulkUpdate(List<TEntity> entities);
        int BulkDelete(List<TEntity> entities);
    }
}
