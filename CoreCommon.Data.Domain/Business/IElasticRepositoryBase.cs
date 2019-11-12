using System.Collections.Generic;

namespace CoreCommon.Data.Domain.Business
{
    public interface IElasticRepositoryBase<T, T2>
    {
        IEnumerable<T> GetAll();
        T GetBy(T2 id);
        bool Delete(T2 id);
        bool Edit(T entity);
        T Add(T entity);
    }
}
