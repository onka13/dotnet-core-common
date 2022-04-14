using System;
using CoreCommon.Infrastructure.Domain.Business;
using Microsoft.Extensions.Logging;

namespace CoreCommon.Infrastructure.Exceptions
{
    public class AppException : Exception
    {
        public AppException(string message, params object[] errorData)
           : this(ServiceResultCode.Error, message, errorData: errorData)
        {
        }

        public AppException(int code, string message, params object[] errorData)
           : this(code, message, logLevel: LogLevel.None, errorData: errorData)
        {
        }

        public AppException(
            int code = ServiceResultCode.Error,
            string message = "",
            LogLevel logLevel = LogLevel.Error,
            Exception innerException = null,
            AppExceptionType exceptionType = AppExceptionType.Error,
            params object[] errorData)
            : base(message, innerException)
        {
            Code = code;
            ErrorData = errorData;
            LogLevel = logLevel;
            Type = exceptionType;
        }

        public int Code { get; set; }

        public object[] ErrorData { get; }

        public LogLevel LogLevel { get; set; }

        public AppExceptionType Type { get; set; }

        public ServiceResult<object> ToServiceResult()
        {
            return ToServiceResult<object>();
        }

        public ServiceResult<T> ToServiceResult<T>()
        {
            var result = ServiceResult<T>.Instance;
            result.Success = false;
            result.ErrorData = ErrorData;
            result.Code = Code;
            result.Message = Message;
            return result;
        }
    }
}
