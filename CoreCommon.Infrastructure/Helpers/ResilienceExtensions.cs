using System;
using System.Threading.Tasks;
using CoreCommon.Infrastructure.Data.Infra.Exceptions;

namespace CoreCommon.Infrastructure.Helpers
{
    public static class ResilienceExtensions
    {
        public static async Task Retry<TException>(this Func<Task> action, int maxTetry, Func<TException, int, Task> beforeRetry = null)
            where TException : Exception
        {
            await RetryInner(action, maxTetry, beforeRetry);
        }

        private static async Task RetryInner<TException>(Func<Task> action, int maxRetry, Func<TException, int, Task> beforeRetry = null, int retry = 1)
            where TException : Exception
        {
            if (maxRetry < retry)
            {
                throw new CoreCommonException("Max retry exhausted. Max retry {maxRetry}", maxRetry);
            }

            try
            {
                await action();
            }
            catch (TException ex)
            {
                if (beforeRetry != null)
                {
                    await beforeRetry(ex, retry);
                }

                retry++;
                await RetryInner(action, maxRetry, beforeRetry, retry);
            }
        }
    }
}
