using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CoreCommon.Application.WebAPIBase.Base
{
    /// <summary>
    /// Middleware base class.
    /// </summary>
    public class MiddlewareBase
    {
        private readonly RequestDelegate next;

        /// <summary>
        /// Initializes a new instance of the <see cref="MiddlewareBase"/> class.
        /// </summary>
        /// <param name="next">RequestDelegate.</param>
        public MiddlewareBase(RequestDelegate next)
        {
            this.next = next;
        }

        /// <summary>
        /// Invoke.
        /// </summary>
        /// <param name="context">HttpContext.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task Invoke(HttpContext context)
        {
            PreInvoke(context);

            if (next != null)
            {
                await next(context);
            }

            AfterInvoke(context);
        }

        /// <summary>
        /// Calls before Invoke method.
        /// </summary>
        /// <param name="context">HttpContext.</param>
        public virtual void PreInvoke(HttpContext context)
        {
        }

        /// <summary>
        /// Calls after Invoke method.
        /// </summary>
        /// <param name="context">HttpContext.</param>
        public virtual void AfterInvoke(HttpContext context)
        {
        }
    }
}
