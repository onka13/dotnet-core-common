using System.Linq;
using CoreCommon.Data.Domain.Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CoreCommon.Application.WebAPIBase.Components
{
    public class ModelStateFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext actionContext)
        {
            if (actionContext.ActionArguments.Count > 0 && actionContext.ActionArguments.Any(kv => kv.Value == null))
            {
                actionContext.Result = new BadRequestObjectResult(ServiceResult<string>.Instance.ErrorResult(ServiceResultCode.EmptyModel, "Empty Model"));
                return;
            }

            if (!actionContext.ModelState.IsValid)
            {
                string msg = "Invalid Model";
                foreach (var item in actionContext.ModelState.Where(x => x.Value.ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid))
                {
                    msg += "\n" + item.Key;
                    foreach (var err in item.Value.Errors)
                    {
                        msg += "\n" + err.ErrorMessage;
                        if (err.Exception != null)
                        {
                            msg += "exception: " + err.Exception.Message;
                        }
                    }
                }

                actionContext.Result = new BadRequestObjectResult(ServiceResult<string>.Instance.ErrorResult(ServiceResultCode.InvalidModel, msg));
                return;
            }
        }
    }
}
