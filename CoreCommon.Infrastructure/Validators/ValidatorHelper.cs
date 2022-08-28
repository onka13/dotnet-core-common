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

        public static List<ValidationResult> IsValid<T>(this List<T> items, [CallerArgumentExpression("items")] string itemsName = "")
        {
            var results = new List<ValidationResult>();
            if (items != null)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    var result = items[i].IsValid($"{itemsName}[{i}]");
                    if (result != null)
                    {
                        results.AddRange(result);
                    }
                }
            }

            return results;
        }
    }
}
