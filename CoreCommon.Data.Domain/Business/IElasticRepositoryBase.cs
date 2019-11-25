using System.Collections.Generic;

namespace CoreCommon.Data.Domain.Business
{
    /// <summary>
    /// Elastic search base repository interface
    /// </summary>
    /// <typeparam name="TEntity">Entity Type.</typeparam>
    /// <typeparam name="TPrimaryKey">Primary key type.</typeparam>
    public interface IElasticRepositoryBase<TEntity, TPrimaryKey>
    {
        IEnumerable<TEntity> GetAll();
        TEntity GetBy(TPrimaryKey id);
        bool Delete(TPrimaryKey id);
        bool Edit(TEntity entity);
        TEntity Add(TEntity entity);
    }
}
