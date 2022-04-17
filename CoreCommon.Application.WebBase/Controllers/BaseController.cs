using System.Collections.Generic;
using System.Linq;
using CoreCommon.Application.WebBase.Components;
using CoreCommon.Infrastructure.Domain.Business;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace CoreCommon.Application.WebBase.Controllers
{
    /// <summary>
    /// Base controller for all controllers.
    /// </summary>
    [TypeFilter(typeof(ModelStateFilter))]
    [ApiController]
    public abstract class BaseController : Controller
    {
        public IWebHostEnvironment HostingEnvironment { get; set; }

        public IHttpContextAccessor HttpContextAccessor { get; set; }

        public override async void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
        }

        protected string GetIpAddress()
        {
            return (HttpContextAccessor?.HttpContext ?? HttpContext).Connection.RemoteIpAddress.MapToIPv4().ToString();
        }

        protected string GetFromHeader(string headerName)
        {
            StringValues values;
            if (Request.Headers.TryGetValue(headerName, out values))
            {
                return values.ToArray().ToList().FirstOrDefault();
            }

            return null;
        }

        protected IActionResult SuccessResponse()
        {
            var response = ServiceResult<string>.Instance.SuccessResult();
            return Json(response);
        }

        protected ServiceResult<T> ServiceResponse<T>(T resultValue = default, int resultCode = 0)
        {
            return ServiceResult<T>.Instance.SuccessResult(resultValue, resultCode);
        }

        protected IActionResult SuccessResponse<T>(T resultValue = default, int resultCode = 0)
        {
            var response = ServiceResult<T>.Instance.SuccessResult(resultValue, resultCode);
            return Json(response);
        }

        protected IActionResult SuccessListResponse(List<object> resultValue, long total)
        {
            var response = ServiceListResult<object>.Instance.SuccessResult(resultValue, total);
            return Json(response);
        }

        protected IActionResult ErrorResponse(int responseCode = 2, string message = "")
        {
            var response = ServiceResult<object>.Instance.ErrorResult(responseCode, message);
            return Json(response);
        }

        protected IActionResult ResponseNextPage<T>(ServiceResult<List<T>> result, int take)
        {
            var response = ServiceListMoreResult<T>.Instance.SuccessResult(result.Value, take);
            return Json(response);
        }

        protected IActionResult ResponseNextPage<T>(List<T> result, int take)
        {
            var response = ServiceListMoreResult<T>.Instance.SuccessResult(result, take);
            return Json(response);
        }

        /// <summary>
        /// Returns invalid models as ServiceResult error.
        /// </summary>
        /// <returns></returns>
        protected ActionResult InvalidModelResult()
        {
            var response = ServiceResult<List<string>>.Instance.ErrorResult(ServiceResultCode.InvalidModel);
            response.Value = new List<string>();
            foreach (var item in ModelState)
            {
                if (item.Value.Errors.Count > 0)
                {
                    response.Value.Add(item.Key);
                }
            }

            return Json(response);
        }
    }
}
