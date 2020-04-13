using CoreCommon.Data.Domain.Business;
using CoreCommon.ModuleBase.Filters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreCommon.ModuleBase.Components
{
    [TypeFilter(typeof(ModelStateFilter))]
    public abstract class CommonBaseController : Controller
    {
        public IHostingEnvironment HostingEnvironment { get; set; }
        public IHttpContextAccessor HttpContextAccessor { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
        }

        protected string GetIpAddress()
        {
            return HttpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
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

        protected IActionResult SuccessResponse(object msg = null)
        {
            var response = ServiceResult<object>.Instance.SuccessResult(msg);
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

        protected JsonResult Json<T>(T content)
        {
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver()
            {
                //NamingStrategy = new CamelCaseNamingStrategy
                //{
                //    ProcessDictionaryKeys = true,
                //    ProcessExtensionDataNames = true,
                //    OverrideSpecifiedNames = true
                //}
            };
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            serializerSettings.NullValueHandling = NullValueHandling.Ignore;
#if DEBUG
            // Pretty json for developers.
            serializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
#else
            serializerSettings.Formatting = Formatting.None;
#endif

            return base.Json(content, serializerSettings);
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
                    response.Value.Add(item.Key);
            }
            return Json(response);
        }
    }
}
