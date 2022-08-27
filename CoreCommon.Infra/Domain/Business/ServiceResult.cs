using System;
using System.Collections.Generic;

namespace CoreCommon.Infra.Domain.Business
{
    /// <summary>
    /// Service result interface.
    /// </summary>
    public interface IServiceResult
    {
        bool Success { get; }

        int Code { get; set; }
    }

    /// <summary>
    /// Service result model which includes paging prev and next functionality.
    /// </summary>
    /// <typeparam name="T">Model.</typeparam>
    public class ServiceListMoreResult<T> : ServiceResult<List<T>>
    {
        /// <summary>
        /// Gets an instance for ServiceListMoreResult.
        /// </summary>
        public static new ServiceListMoreResult<T> Instance
        {
            get { return new ServiceListMoreResult<T>(); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether HasMore.
        /// </summary>
        public bool HasMore { get; set; }

        /// <summary>
        /// Returns success result.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="take">Limit.</param>
        /// <returns><see cref="ServiceListMoreResult{T}"/>.</returns>
        public new ServiceListMoreResult<T> SuccessResult(List<T> value, int take)
        {
            HasMore = false;
            if (value != null && value.Count > take)
            {
                value.RemoveAt(take);
                HasMore = true;
            }

            base.SuccessResult(value, 0);
            return this;
        }
    }

    /// <summary>
    /// Service result model which includes count value of the resultset for paging.
    /// </summary>
    /// <typeparam name="T">Model.</typeparam>
    public class ServiceListResult<T> : ServiceResult<List<T>>
    {
        /// <summary>
        /// Gets an instance for ServiceListResult.
        /// </summary>
        public static new ServiceListResult<T> Instance
        {
            get { return new ServiceListResult<T>(); }
        }

        /// <summary>
        /// Gets or sets total.
        /// </summary>
        public long Total { get; set; }

        /// <summary>
        /// Returns success result.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="total">Limit.</param>
        /// <returns><see cref="ServiceListResult{T}"/>.</returns>
        public ServiceListResult<T> SuccessResult(List<T> value, long total)
        {
            Total = total;
            base.SuccessResult(value, 0);
            return this;
        }
    }

    public class ServiceResult<T> : IServiceResult
    {
        /// <summary>
        /// Gets an instance.
        /// </summary>
        public static ServiceResult<T> Instance
        {
            get { return new ServiceResult<T>(); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Success.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets code.
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Gets or sets Message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets data .
        /// </summary>
        public T Value { get; set; }

        public object[] ErrorData { get; set; }

        /// <summary>
        /// Gets or sets Debug.
        /// </summary>
        public object Debug { get; set; }

        /// <summary>
        /// Returns error result.
        /// </summary>
        /// <param name="resultCode">ResultCode.</param>
        /// <param name="exception">Exception.</param>
        /// <returns><see cref="ServiceResult{T}"/>.</returns>
        public virtual ServiceResult<T> ErrorResult(int resultCode, Exception exception)
        {
            var msg = exception.Message;
            if (exception.InnerException != null)
            {
                msg = exception.InnerException.Message;
            }

            return ErrorResult(resultCode, msg);
        }

        /// <summary>
        /// Returns error result.
        /// </summary>
        /// <param name="resultCode">ResultCode.</param>
        /// <param name="message">Message.</param>
        /// <returns><see cref="ServiceResult{T}"/>.</returns>
        public virtual ServiceResult<T> ErrorResult(int resultCode = 0, string message = "")
        {
            Value = default;
            return SetResult(false, resultCode, message);
        }

        /// <summary>
        /// Returns success result.
        /// </summary>
        /// <param name="resultValue">ResultValue.</param>
        /// <param name="resultCode">ResultCode.</param>
        /// <returns><see cref="ServiceResult{T}"/>.</returns>
        public virtual ServiceResult<T> SuccessResult(T resultValue = default, int resultCode = 0)
        {
            Value = resultValue;
            return SetResult(true, resultCode, null);
        }

        public virtual ServiceResult<T> SuccessResult()
        {
            Success = true;
            return this;
        }

        /// <summary>
        /// SetResult.
        /// </summary>
        /// <param name="success">Success.</param>
        /// <param name="resultCode">ResultCode.</param>
        /// <param name="message">Message.</param>
        /// <returns><see cref="ServiceResult{T}"/>.</returns>
        protected ServiceResult<T> SetResult(bool success, int resultCode, string message)
        {
            Success = success;
            Code = resultCode;
            Message = message;
            return this;
        }
    }

    /// <summary>
    /// non generic usage of service result .
    /// </summary>
    public class ServiceResult : ServiceResult<string>
    {
    }
}
