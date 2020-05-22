using CoreCommon.Data.Domain.Business;

namespace CoreCommon.Data.EntityFrameworkBase.Base
{
    public interface IEntityFrameworkBaseRepository<TEntity> : ITransactionRepositoryBase<TEntity>
    {
    }
}
