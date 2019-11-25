using CoreCommon.Data.EntityFrameworkBase.Base;

namespace CoreCommon.Data.EntityFrameworkBase.Components
{
    /// <summary>
    /// A simple Db Context
    /// </summary>
    public class EmptyDbContext : DbContextBase
    {
        public override string Name { get => _name; }
        private string _name;

        public void SetName(string name)
        {
            _name = name;
        }
    }
}
