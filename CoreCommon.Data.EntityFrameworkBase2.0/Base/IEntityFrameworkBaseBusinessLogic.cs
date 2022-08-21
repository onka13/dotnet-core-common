using CoreCommon.Data.Domain.Business;

namespace CoreCommon.Data.EntityFrameworkBase.Base
{
    public interface IEntityFrameworkBaseBusinessLogic<TEntity> : ITransactionBusinessLogicBase<TEntity>
    {
    }
}
