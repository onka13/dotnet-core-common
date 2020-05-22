namespace CoreCommon.Data.ElasticSearch.Base
{
    /// <summary>
    /// Default elastic search entity interface
    /// </summary>
    /// <typeparam name="TPrimaryKey">Primary Key Type</typeparam>
    public interface IElasticSearchEntityBase<TPrimaryKey>
    {
        TPrimaryKey Id { get; set; }
    }
}
