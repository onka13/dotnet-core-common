namespace CoreCommon.Infrastructure.Domain.Config
{
    public class DatabaseConfig
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        /// <summary>
        /// mssql, mysql, postgresql
        /// </summary>
        public string Provider { get; set; }
    }
}
