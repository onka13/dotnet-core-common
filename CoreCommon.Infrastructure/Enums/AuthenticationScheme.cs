using System.ComponentModel;

namespace CoreCommon.Infrastructure.Enums
{
    public enum AuthenticationScheme
    {
        [Description("Basic")]
        Basic = 0,

        [Description("Bearer")]
        Bearer = 1,
    }
}
