using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace CoreCommon.Infra.Logging
{
    /// <summary>
    /// Common Log Service
    ///
    /// LogWarning("The person {PersonId} could not be found.", personId);
    /// Properties are stored as seperate custom properties of the log entry
    ///
    /// ! Do not use dots in property names.
    /// </summary>
    public class LogService : ILogService
    {
        public LogService(ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory.CreateLogger("CoreTemplate");
        }

        public ILogger Logger { get; set; }

        public void Log(LogLevel logLevel, string msg, params object[] args)
        {
            Log(logLevel, msg, null, args);
        }

        public void Log(LogLevel logLevel, string msg, Exception exception = null, params object[] args)
        {
            Logger.Log(logLevel, exception, msg, args);
        }

        public void Debug(string msg, params object[] args)
        {
            Logger.LogDebug(msg, args);
        }

        public void Information(string msg, params object[] args)
        {
            Logger.LogInformation(msg, args);
        }

        public void Warning(string msg, params object[] args)
        {
            Logger.LogWarning(msg, args);
        }

        public void Warning(Exception exception, string msg, params object[] args)
        {
            // !! always pass the exception object as first argument
            Logger.LogWarning(exception, msg, args);
        }

        public void Error(string msg, params object[] args)
        {
            Logger.LogError(msg, args);
        }

        public void Error(Exception exception)
        {
            Logger.LogError(exception, exception.Message);
        }

        public void Error(Exception exception, string msg, params object[] args)
        {
            // !! always pass the exception object as first argument
            Logger.LogError(exception, msg, args);
        }

        public void Critical(string msg, params object[] args)
        {
            Logger.LogCritical(msg, args);
        }

        public void Critical(Exception exception)
        {
            Logger.LogCritical(exception, exception.Message);
        }

        public void Critical(Exception exception, string msg, params object[] args)
        {
            // !! always pass the exception object as first argument
            Logger.LogCritical(exception, msg, args);
        }

        public virtual IDisposable BeginScope(string name, object value)
        {
            return BeginScope(new Dictionary<string, object> { { name, value } });
        }

        public virtual IDisposable BeginScope(Dictionary<string, object> state)
        {
            return Logger.BeginScope(state);
        }
    }
}
