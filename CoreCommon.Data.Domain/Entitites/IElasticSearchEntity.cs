namespace CoreCommon.Data.Domain.Entitites
{
    /// <summary>
    /// Default elastic search entity interface
    /// </summary>
    /// <typeparam name="TPrimaryKey">Primary Key Type</typeparam>
    public interface IElasticSearchEntity<TPrimaryKey>
    {
        TPrimaryKey Id { get; set; }
    }
}
