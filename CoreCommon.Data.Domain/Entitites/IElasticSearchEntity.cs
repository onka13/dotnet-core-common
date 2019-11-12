namespace CoreCommon.Data.Domain.Entitites
{
    public interface IElasticSearchEntity<T>
    {
        T Id { get; set; }
    }
}
