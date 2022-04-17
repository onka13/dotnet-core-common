using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace CoreCommon.Application.WebBase.Components
{
    public class ModelBindingConvention : IActionModelConvention
    {
        public void Apply(ActionModel action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var isFromForm = action.Parameters.Any(x => x.ParameterInfo.ParameterType == typeof(IFormFile) ||
            x.ParameterInfo.ParameterType == typeof(List<IFormFile>));
            if (isFromForm)
            {
                return;
            }

            /*
            foreach (var parameter in action.Parameters)
            {
                var paramType = parameter.ParameterInfo.ParameterType;
                var isSimpleType = paramType.IsPrimitive
                                    || paramType.IsEnum
                                    || paramType == typeof(string)
                                    || paramType == typeof(DateTime)
                                    || paramType == typeof(decimal);

                if (!isSimpleType)
                {
                    parameter.BindingInfo = parameter.BindingInfo ?? new BindingInfo();
                    parameter.BindingInfo.BindingSource = BindingSource.Body;
                }
            }
            */
        }
    }
}
