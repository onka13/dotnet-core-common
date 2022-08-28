using System;
using System.Collections.Generic;
using CoreCommon.Data.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CoreCommon.Infrastructure.Logging
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
        private readonly IConfiguration configuration;

        public LogService(ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            Logger = loggerFactory.CreateLogger("onka");
            this.configuration = configuration;
        }

        public ILogger Logger { get; set; }

        public void Log(LogLevel logLevel, string msg, params object[] args)
        {
            Log(logLevel, msg, null, args);
        }

        public void Log(LogLevel logLevel, string msg, Exception exception = null, params object[] args)
        {
            var properties = new Dictionary<string, object>
            {
                { "MachineName", Environment.MachineName },
                { "MachineUserName", Environment.UserName },
                { "LogLevel", logLevel.ToString() },
                { "ProjectName", configuration["ProjectName"] },
                { "Version", configuration["Version"] },
                { "SettingName", configuration["SettingName"] },
            };

            using (BeginScope(properties))
            {
                if (exception is AppException && (args == null || args.Length == 0))
                {
                    var AppException = exception as AppException;
                    args = AppException.ErrorData;
                }

                Logger.Log(logLevel, exception, msg, args);
            }
        }

        public void Trace(string msg, params object[] args)
        {
            Log(LogLevel.Trace, msg, args);
        }

        public void Debug(string msg, params object[] args)
        {
            Log(LogLevel.Debug, msg, args);
        }

        public void Information(string msg, params object[] args)
        {
            Log(LogLevel.Information, msg, args);
        }

        public void Warning(string msg, params object[] args)
        {
            Log(LogLevel.Warning, msg, args);
        }

        public void Warning(Exception exception, string msg, params object[] args)
        {
            Log(LogLevel.Warning, msg, exception, args);
        }

        public void Error(string msg, params object[] args)
        {
            Log(LogLevel.Error, msg, args);
        }

        public void Error(Exception exception)
        {
            Log(LogLevel.Error, exception.Message, exception);
        }

        public void AppError(Exception exception)
        {
            if (exception is AppException AppException)
            {
                Log(AppException.LogLevel, AppException.Message, AppException, AppException.ErrorData);
            }
            else
            {
                Error(exception);
            }
        }

        public void Error(Exception exception, string msg, params object[] args)
        {
            Log(LogLevel.Error, msg, exception, args);
        }

        public void Critical(string msg, params object[] args)
        {
            Log(LogLevel.Critical, msg, args);
        }

        public void Critical(Exception exception)
        {
            Log(LogLevel.Critical, exception.Message, exception);
        }

        public void Critical(Exception exception, string msg, params object[] args)
        {
            Log(LogLevel.Critical, msg, args);
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
