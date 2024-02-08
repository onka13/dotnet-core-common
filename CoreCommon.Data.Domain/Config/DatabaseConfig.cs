namespace CoreCommon.Data.Domain.Config
{
    public class DatabaseConfig
    {
        public string ConnectionString { get; set; }

        public string DatabaseName { get; set; }

        /// <summary>
        /// Gets or sets Provider (mssql, mysql, postgresql).
        /// </summary>
        public string Provider { get; set; }
    }
}
