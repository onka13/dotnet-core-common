using CoreCommon.Data.Domain.Business;
using CoreCommon.Data.Domain.Models;
using CoreCommon.ModuleBase.Filters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

        protected IActionResult SuccessResponse(object msg = null, int resultCode = 0)
        {
            var response = ServiceResult<object>.Instance.SuccessResult(msg, resultCode);
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

        protected IActionResult SuccessListResponse(List<object> resultValue, long total)
        {
            var response = ServiceListResult<object>.Instance.SuccessResult(resultValue, total);
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
                    response.Value.Add(item.Key);
            }
            return Json(response);
        }

        protected FormFile ToFormFile(IFormFile file)
        {
            if (file == null) return null;
            var model = new FormFile
            {
                Extension = Path.GetExtension(file.FileName).Replace(".", ""),
                Name = file.FileName,
                ContentType = file.ContentType,
                Stream = file.OpenReadStream()
            };
            if (file.ContentType.Contains("image"))
                model.FileType = FileType.Image;
            else if (file.ContentType.Contains("json"))
                model.FileType = FileType.Json;
            else if (file.ContentType.Contains("audio"))
                model.FileType = FileType.Audio;
            else if (file.ContentType.Contains("video"))
                model.FileType = FileType.Video;
            else
                model.FileType = FileType.Other;

            return model;
        }
    }
}
