using CoreCommon.Data.EntityFrameworkBase.Base;

namespace CoreCommon.Data.EntityFrameworkBase.Components
{
    /// <summary>
    /// A simple Db Context.
    /// </summary>
    public class EmptyDbContext : DbContextBase
    {
        private string name;

        public override string Name { get => name; }

        public static EmptyDbContext Init(string provider, string connectionString)
        {
            var context = new EmptyDbContext();
            context.Provider = provider;
            context.ConnectionString = connectionString;
            return context;
        }

        public void SetName(string name)
        {
            this.name = name;
        }
    }
}
