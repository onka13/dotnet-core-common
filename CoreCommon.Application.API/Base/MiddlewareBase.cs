using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CoreCommon.Application.API.Base
{
    /// <summary>
    /// Middleware base class
    /// </summary>
    public class MiddlewareBase
    {
        private readonly RequestDelegate _next;

        public MiddlewareBase(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            PreInvoke(context);

            if (_next != null)
            {
                await _next(context);
            }

            AfterInvoke(context);
        }

        public virtual void PreInvoke(HttpContext context)
        {
        }

        public virtual void AfterInvoke(HttpContext context)
        {
        }
    }
}
