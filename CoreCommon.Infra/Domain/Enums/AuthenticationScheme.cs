using System.ComponentModel;

namespace CoreCommon.Infra.Domain.Enums
{
    public enum AuthenticationScheme
    {
        [Description("Basic")]
        Basic = 0,

        [Description("Bearer")]
        Bearer = 1,
    }
}
