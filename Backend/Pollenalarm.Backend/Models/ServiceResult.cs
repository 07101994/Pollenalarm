using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pollenalarm.Backend.Models
{
    public class ServiceResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public ServiceResult(bool success)
        {
            Success = success;
            Message = null;
        }

        public ServiceResult(string message)
        {
            Success = false;
            Message = message;
        }

        public ServiceResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }

    public class ServiceResult<T> : ServiceResult
    {
        public T Content { get; set; }

        public ServiceResult(bool success)
            : base(success)
        {
            Content = default(T);
        }

        public ServiceResult(T result)
            : base(true)
        {
            Content = result;
        }

        public ServiceResult(string message)
            : base(message)
        {
            Content = default(T);
        }

        public ServiceResult(bool success, string message, T result)
            : base(success, message)
        {
            Content = result;
        }
    }
}
