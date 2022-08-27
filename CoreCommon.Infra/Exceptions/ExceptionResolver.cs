using System;
using System.Threading.Tasks;
using CoreCommon.Infra.Domain.Business;

namespace CoreCommon.Infra.Exceptions
{
    public static class ExceptionResolver
    {
        public static async Task<ServiceResult<string>> CatchAsync(this Task task)
        {
            return await task.CatchAsync<AppException>();
        }

        public static async Task<ServiceResult<T>> CatchAsync<T>(this Task<T> task)
        {
            return await task.CatchAsync<AppException, T>();
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
                if (ex is AppException)
                {
                    return (ex as AppException).ToServiceResult<T>();
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
                if (ex is AppException)
                {
                    return (ex as AppException).ToServiceResult<string>();
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
            catch (AppException innerException)
            {
                var ex = new AppException(code: 500, message: innerException.Message, innerException: innerException);
                throw ex;
            }
        }
    }
}
