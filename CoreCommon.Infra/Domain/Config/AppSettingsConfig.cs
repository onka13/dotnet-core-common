using System.Collections.Generic;

namespace CoreCommon.Infrastructure.Domain.Config
{
    /// <summary>
    /// App configuration model which defined in appsettings.
    /// </summary>
    public class AppSettingsConfig
    {
        public List<string> TestEmailReceivers { get; set; }
    }
}
