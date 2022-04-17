using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CoreCommon.Infrastructure.Domain.Business;
using CoreCommon.Infrastructure.Exceptions;
using CoreCommon.Infrastructure.Helpers;
using CoreCommon.Infrastructure.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CoreCommon.Application.WebBase.Components
{
    public class AppExceptionMiddleware
    {
        private const string ServerErrorMessageFormat = "An error occured! (RefId: {0})";
        private readonly RequestDelegate next;
        private readonly ILogService logService;
        private readonly IHostEnvironment environment;

        public AppExceptionMiddleware(RequestDelegate requestDelegate, ILogService log, IHostEnvironment env)
        {
            next = requestDelegate;
            logService = log;
            environment = env;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (AppException error)
            {
                if (error.LogLevel != LogLevel.None)
                {
                    logService.Log(error.LogLevel, "Code: {Code}, Message: {Message}, Data: {Data}", error, error.Code, error.Message, error.ErrorData);
                }

                var result = error.ToServiceResult();

                HttpStatusCode statusCode;

                switch (error.Type)
                {
                    case AppExceptionType.Warning:
                    case AppExceptionType.Error:
                        statusCode = HttpStatusCode.BadRequest;
                        break;
                    default:
                        statusCode = HttpStatusCode.OK;
                        break;
                }

                await WriteResponse(context, result, statusCode, error);
            }
            catch (Exception error)
            {
                // UnHandled Exceptions.
                var refId = Guid.NewGuid().ToString("N");
                logService.Error(error, "Message: {Message}, RefId: {RefId}", error.Message, refId);

                var result = ServiceResult<string>.Instance.ErrorResult(ServiceResultCode.ServerError, string.Format(ServerErrorMessageFormat, refId));
                if (!environment.IsProduction())
                {
                    result.Message = error.Message;
                }

                await WriteResponse(context, result, HttpStatusCode.InternalServerError, error);
            }
        }

        private async Task WriteResponse<T>(HttpContext context, ServiceResult<T> result, HttpStatusCode statusCode, Exception exception)
        {
            if (!environment.IsProduction())
            {
                var innerMessages = new List<string>();
                innerMessages.Add(exception.Message);
                var innerException = exception.InnerException;
                while (innerException != null)
                {
                    innerMessages.Add(innerException.Message);
                    innerException = innerException.InnerException;
                }

                result.Debug = new
                {
                    innerMessages,
                    stackTraceLines = exception.StackTrace?.Split("\r\n").Select(x => x?.Trim()).ToList(),
                };
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;
            await context.Response.WriteAsync(ConversionHelper.Serialize(result, isCamelCase: true, isIndented: !environment.IsProduction()));
        }
    }
}
