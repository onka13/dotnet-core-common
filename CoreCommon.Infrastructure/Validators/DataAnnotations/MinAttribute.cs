using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace CoreCommon.Infrastructure.Validators.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class MinAttribute : DataTypeAttribute
    {
        private readonly double min;

        public MinAttribute(double min)
            : base("min")
        {
            this.min = min;
        }

        public override string FormatErrorMessage(string name)
        {
            if (ErrorMessage == null && ErrorMessageResourceName == null)
            {
                ErrorMessage = "The field {0} must be greater than or equal to {1}";
            }

            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, min);
        }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }

            return double.TryParse(Convert.ToString(value), out double valueAsDouble) && valueAsDouble >= min;
        }
    }
}
