using System;
using Microsoft.Extensions.Logging;

namespace CoreCommon.Infrastructure.Exceptions
{
    public static class ExceptionExtenstionHelpers
    {
        public static LogLevel GetLogLevel(this Exception ex)
        {
            if (ex is not CoreCommonException)
            {
                return LogLevel.Error;
            }

            return (ex as CoreCommonException).LogLevel;
        }
    }
}
