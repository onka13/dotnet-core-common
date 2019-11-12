using System.ComponentModel;

namespace CoreCommon.Data.Domain.Enums
{
    public enum Status : byte
    {
        [Description("")]
        Active = 0,
        [Description("")]
        Passive = 1,        
    }
}
