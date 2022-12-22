using System;
using CoreCommon.Data.Domain.Models;
using Microsoft.Extensions.Logging;

namespace CoreCommon.Infrastructure.Exceptions
{
    public static class ExceptionExtenstionHelpers
    {
        public static LogLevel GetLogLevel(this Exception ex)
        {
            if (ex is not AppException)
            {
                return LogLevel.Error;
            }

            return (ex as AppException).LogLevel;
        }
    }
}
