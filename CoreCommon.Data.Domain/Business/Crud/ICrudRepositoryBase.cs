using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreCommon.Data.Domain.Business.Crud
{
    /// <summary>
    /// Repository base interface.
    /// </summary>
    /// <typeparam name="TEntity">Entity Type.</typeparam>
    public interface ICrudRepositoryBase<TEntity>
    {
        Task<TEntity> Add(TEntity entity);

        Task<int> Delete(TEntity entity);

        Task<int> Update(TEntity entity);

        Task<int> BulkInsert(List<TEntity> entities);

        Task<int> BulkUpdate(List<TEntity> entities);

        Task<int> BulkDelete(List<TEntity> entities);
    }
}
