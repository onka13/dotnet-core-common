using System.ComponentModel;

namespace CoreCommon.Infrastructure.Domain.Enums
{
    /// <summary>
    /// Status enum
    /// </summary>
    public enum Status : byte
    {
        [Description("Active")]
        Active = 0,

        [Description("Passive")]
        Passive = 1,
    }
}
