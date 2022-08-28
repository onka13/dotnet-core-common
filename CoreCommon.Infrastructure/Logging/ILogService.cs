using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace CoreCommon.Infrastructure.Logging
{
    public interface ILogService
    {
        void Log(LogLevel logLevel, string msg, params object[] args);

        void Log(LogLevel logLevel, string msg, Exception exception = null, params object[] args);

        void Debug(string msg, params object[] args);

        void Information(string msg, params object[] args);

        void Warning(string msg, params object[] args);

        void Warning(Exception exception, string msg, params object[] args);

        void Error(string msg, params object[] args);

        void Error(Exception exception);

        void Error(Exception exception, string msg, params object[] args);

        void Critical(string msg, params object[] args);

        void Critical(Exception exception);

        void Critical(Exception exception, string msg, params object[] args);

        IDisposable BeginScope(string name, object value);

        IDisposable BeginScope(Dictionary<string, object> state);

        void AppError(Exception exception);
    }
}