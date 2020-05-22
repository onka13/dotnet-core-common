using CoreCommon.Data.Domain.Business;

namespace CoreCommon.Data.EntityFrameworkBase.Base
{
    public abstract class EntityFrameworkBaseBusinessLogic<TEntity, TRepository>
        : TransactionBusinessLogicBase<TEntity, TRepository> 
        where TRepository : IEntityFrameworkBaseRepository<TEntity>
    {

    }
}
