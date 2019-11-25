using System;
using System.Collections.Generic;

namespace CoreCommon.Data.Domain.Business
{
    /// <summary>
    /// Service result interface
    /// </summary>
    public interface IServiceResult
    {
        bool Success { get; }
        int Code { get; set; }
    }

    /// <summary>
    /// Service result model which includes paging prev and next functionality.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ServiceListMoreResult<T> : ServiceResult<List<T>>
    {
        /// <summary>
        /// If resultset has next page.
        /// </summary>
        public bool HasMore { get; set; }

        /// <summary>
        /// Creates an instance
        /// </summary>
        public new static ServiceListMoreResult<T> Instance
        {
            get { return new ServiceListMoreResult<T>(); }
        }

        /// <summary>
        /// Returns success result
        /// </summary>
        /// <param name="value"></param>
        /// <param name="take"></param>
        /// <returns></returns>
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
    /// <typeparam name="T"></typeparam>
    public class ServiceListResult<T> : ServiceResult<List<T>>
    {
        /// <summary>
        /// Total count of results.
        /// </summary>
        public long Total { get; set; }

        /// <summary>
        /// Creates an instance
        /// </summary>
        public new static ServiceListResult<T> Instance
        {
            get { return new ServiceListResult<T>(); }
        }

        /// <summary>
        /// Returns success result
        /// </summary>
        /// <param name="value"></param>
        /// <param name="take"></param>
        /// <returns></returns>
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
        /// True if service succeeded
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Service result code
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Custom message for service results
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Service result object data 
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// Creates an instance
        /// </summary>
        public static ServiceResult<T> Instance
        {
            get { return new ServiceResult<T>(); }
        }

        /// <summary>
        /// Returns error result
        /// </summary>
        /// <param name="resultCode"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public virtual ServiceResult<T> ErrorResult(int resultCode, Exception exception)
        {
            var msg = exception.Message;
            if (exception.InnerException != null) msg = exception.InnerException.Message;
            return ErrorResult(resultCode, msg);
        }

        /// <summary>
        /// Returns error result
        /// </summary>
        /// <param name="resultCode"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public virtual ServiceResult<T> ErrorResult(int resultCode = 0, string message = "")
        {
            Value = default(T);
            return SetResult(false, resultCode, message);
        }

        /// <summary>
        /// Returns success result
        /// </summary>
        /// <param name="value"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public virtual ServiceResult<T> SuccessResult(T resultValue, int resultCode = 0)
        {
            Value = resultValue;
            return SetResult(true, resultCode, null);
        }

        /// <summary>
        /// Set properties simply
        /// </summary>
        protected ServiceResult<T> SetResult(bool success, int resultCode, string message)
        {
            Success = success;
            Code = resultCode;
            Message = message;
            return this;
        }
    }

    /// <summary>
    /// non generic usage of service result 
    /// </summary>
    public class ServiceResult : ServiceResult<string>
    {
    }
}
