using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace CoreCommon.Infrastructure.Validators
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations?view=net-6.0
    /// </summary>
    public static class ValidatorHelper
    {
        public static List<ValidationResult> IsValid<T>(this T obj, string prefix = "")
        {
            var results = new List<ValidationResult>();

            if (!Validator.TryValidateObject(obj, new ValidationContext(obj), results, true))
            {
                if (!string.IsNullOrEmpty(prefix))
                {
                    results.ForEach(x => x.ErrorMessage = $"{prefix}.{x.ErrorMessage}");
                }

                return results;
            }

            return null;
        }
    }
}
