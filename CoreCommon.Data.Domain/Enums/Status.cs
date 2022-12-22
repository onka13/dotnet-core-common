using System.ComponentModel;

namespace CoreCommon.Data.Domain.Enums
{
    /// <summary>
    /// Status enum.
    /// </summary>
    public enum Status : byte
    {
        [Description("Active")]
        Active = 0,

        [Description("Passive")]
        Passive = 1,
    }
}
