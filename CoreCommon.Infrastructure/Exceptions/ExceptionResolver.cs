using System;
using System.Threading.Tasks;
using CoreCommon.Data.Domain.Business;

namespace CoreCommon.Infrastructure.Exceptions
{
    public static class ExceptionResolver
    {
        /// <summary>
        /// Catch all possible exceptions.
        /// </summary>
        public static async Task<ServiceResult<string>> CatchAllExceptionsAsync(this Task task)
        {
            return await task.CatchAsync<Exception>();
        }

        /// <summary>
        /// Catch only CoreCommon Exceptions.
        /// </summary>
        public static async Task<ServiceResult<string>> CatchAsync(this Task task)
        {
            return await task.CatchAsync<CoreCommonException>();
        }

        /// <summary>
        /// Catch only CoreCommon Exceptions.
        /// </summary>
        public static async Task<ServiceResult<T>> CatchAsync<T>(this Task<T> task)
        {
            return await task.CatchAsync<CoreCommonException, T>();
        }

        public static async Task<ServiceResult<T>> CatchAsync<TException, T>(this Task<T> task)
            where TException : Exception
        {
            var response = ServiceResult<T>.Instance;
            try
            {
                var data = await task;
                if (data != null && data is ServiceResult<T>)
                {
                    return data as ServiceResult<T>;
                }

                return response.SuccessResult(data);
            }
            catch (TException ex)
            {
                if (ex is CoreCommonException)
                {
                    return (ex as CoreCommonException).ToServiceResult<T>();
                }

                return response.ErrorResult(0, ex);
            }
        }

        public static async Task<ServiceResult<string>> CatchAsync<TException>(this Task task)
            where TException : Exception
        {
            var response = ServiceResult.Instance;
            try
            {
                await task;
                return response.SuccessResult();
            }
            catch (TException ex)
            {
                if (ex is CoreCommonException)
                {
                    return (ex as CoreCommonException).ToServiceResult<string>();
                }

                return response.ErrorResult(0, ex);
            }
        }

        public static async Task<T> ReThrowOnAPIExceptionAsServerError<T>(this Task<T> task)
        {
            try
            {
                return await task;
            }
            catch (CoreCommonException innerException)
            {
                var ex = new CoreCommonException(code: 500, message: innerException.Message, innerException: innerException);
                throw ex;
            }
        }
    }
}
