using System;
using System.Collections.Generic;

namespace CoreCommon.Data.Domain.Business
{
    public interface IServiceResult
    {
        bool Success { get; }
        int Code { get; set; }
    }

    public class ServiceListMoreResult<T> : ServiceResult<List<T>>
    {
        public bool HasMore { get; set; }

        public new static ServiceListMoreResult<T> Instance
        {
            get { return new ServiceListMoreResult<T>(); }
        }

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

    public class ServiceListResult<T> : ServiceResult<List<T>>
    {
        public long Total { get; set; }

        public new static ServiceListResult<T> Instance
        {
            get { return new ServiceListResult<T>(); }
        }

        public ServiceListResult<T> SuccessResult(List<T> value, long total)
        {
            Total = total;
            base.SuccessResult(value, 0);
            return this;
        }
    }

    public class ServiceResult<T> : IServiceResult
    {
        public bool Success { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }
        public T Value { get; set; }

        public static ServiceResult<T> Instance
        {
            get { return new ServiceResult<T>(); }
        }

        public virtual ServiceResult<T> ErrorResult(int resultCode, Exception exception)
        {
            var msg = exception.Message;
            if (exception.InnerException != null) msg = exception.InnerException.Message;
            return ErrorResult(resultCode, msg);
        }

        public virtual ServiceResult<T> ErrorResult(int resultCode = 0, string message = "")
        {
            Value = default(T);
            return SetResult(false, resultCode, message);
        }

        public virtual ServiceResult<T> SuccessResult(T resultValue, int resultCode = 0)
        {
            Value = resultValue;
            return SetResult(true, resultCode, null);
        }

        protected ServiceResult<T> SetResult(bool success, int resultCode, string message)
        {
            Success = success;
            Code = resultCode;
            Message = message;
            return this;
        }
    }

    public class ServiceResult : ServiceResult<string>
    {
    }
}
