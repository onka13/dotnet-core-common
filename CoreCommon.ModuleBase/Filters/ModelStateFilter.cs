using CoreCommon.Data.Domain.Attributes;
using CoreCommon.Data.Domain.Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

namespace CoreCommon.ModuleBase.Filters
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
                var methodIgnoreAllAttribute = (actionContext.ActionDescriptor as ControllerActionDescriptor).MethodInfo.GetCustomAttributes(false).OfType<ModelStateIgnoreAllAttribute>().FirstOrDefault();
                if (methodIgnoreAllAttribute != null)
                {
                    actionContext.ModelState.Clear();
                    return;
                }
                var methodIgnoreAttribute = (actionContext.ActionDescriptor as ControllerActionDescriptor).MethodInfo.GetCustomAttributes(false).OfType<ModelStateIgnoreAttribute>().FirstOrDefault();                
                var methodFieldsAttribute = (actionContext.ActionDescriptor as ControllerActionDescriptor).MethodInfo.GetCustomAttributes(false).OfType<ModelStateFieldsAttribute>().FirstOrDefault();

                string msg = "Invalid Model";
                foreach (var item in actionContext.ModelState.Where(x => x.Value.ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid))
                {
                    if (methodIgnoreAttribute != null && methodIgnoreAttribute.Names.Contains(item.Key))
                    {
                        actionContext.ModelState.Remove(item.Key);
                        continue;
                    }
                    if (methodFieldsAttribute != null && !methodFieldsAttribute.Names.Contains(item.Key))
                    {
                        actionContext.ModelState.Remove(item.Key);
                        continue;
                    }
                    msg += "\n" + item.Key;
                    foreach (var err in item.Value.Errors)
                    {
                        msg += "\n" + err.ErrorMessage;
                        if (err.Exception != null) msg += "exception: " + err.Exception.Message;
                    }
                }
                if (actionContext.ModelState.Count > 0)
                {
                    actionContext.Result = new BadRequestObjectResult(ServiceResult<string>.Instance.ErrorResult(ServiceResultCode.InvalidModel, msg));
                }                
                return;
            }
        }
    }
}
