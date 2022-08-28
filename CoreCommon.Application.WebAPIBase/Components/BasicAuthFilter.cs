using System;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using CoreCommon.Data.Domain.Business;
using CoreCommon.Data.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace CoreCommon.Application.WebAPIBase.Components
{
    public class BasicAuthFilter<TBasicAuthModel> : IAsyncActionFilter
        where TBasicAuthModel : BasicAuthModel
    {
        private readonly IOptions<TBasicAuthModel> basicAuthModel;

        public BasicAuthFilter(IOptions<TBasicAuthModel> basicAuthModel)
        {
            this.basicAuthModel = basicAuthModel;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.HttpContext.Request.Headers.ContainsKey("Authorization") && basicAuthModel.Value != null)
            {
                try
                {
                    var authHeader = AuthenticationHeaderValue.Parse(context.HttpContext.Request.Headers["Authorization"]);
                    var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                    var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);
                    var username = credentials[0];
                    var password = credentials[1];

                    if (username == basicAuthModel.Value.Username && password == basicAuthModel.Value.Password)
                    {
                        await next();
                        return;
                    }
                }
                catch
                {
                    // ignored
                }
            }

            context.Result = new UnauthorizedObjectResult(ServiceResult.Instance.ErrorResult(ServiceResultCode.NoPermission, "Unauthorized Request"));
        }
    }
}
