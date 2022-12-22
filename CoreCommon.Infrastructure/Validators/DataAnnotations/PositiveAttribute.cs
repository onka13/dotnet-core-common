using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace CoreCommon.Infrastructure.Validators.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PositiveAttribute : DataTypeAttribute
    {
        public PositiveAttribute()
            : base("positive")
        {
        }

        public override string FormatErrorMessage(string name)
        {
            if (ErrorMessage == null && ErrorMessageResourceName == null)
            {
                ErrorMessage = "The field {0} must be a positive number";
            }

            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name);
        }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }

            return double.TryParse(Convert.ToString(value), out double valueAsDouble) && valueAsDouble > 0;
        }
    }
}
