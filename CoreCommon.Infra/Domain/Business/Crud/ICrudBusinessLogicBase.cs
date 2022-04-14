using System.Collections.Generic;

namespace CoreCommon.Infrastructure.Domain.Business.Crud
{
    /// <summary>
    /// Business logic base interface
    /// </summary>
    /// <typeparam name="TEntity">Entity Type</typeparam>
    public interface ICrudBusinessLogicBase<TEntity>
    {
        ServiceResult<IEnumerable<TEntity>> GetAll();
        ServiceResult<TEntity> Add(TEntity entity);
        ServiceResult<TEntity> Delete(TEntity entity);
        ServiceResult<int> Edit(TEntity entity);
        ServiceResult<int> BulkInsert(List<TEntity> entities);
        ServiceResult<int> BulkDelete(List<TEntity> entities);
        ServiceResult<int> BulkEdit(List<TEntity> entities);
    }
}
