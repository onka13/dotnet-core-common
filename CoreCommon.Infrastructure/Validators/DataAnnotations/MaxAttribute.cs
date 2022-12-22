using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace CoreCommon.Infrastructure.Validators.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class MaxAttribute : DataTypeAttribute
    {
        private readonly double max;

        public MaxAttribute(double max)
            : base("max")
        {
            this.max = max;
        }

        public object Max
        {
            get { return max; }
        }

        public override string FormatErrorMessage(string name)
        {
            if (ErrorMessage == null && ErrorMessageResourceName == null)
            {
                ErrorMessage = "The field {0} must be less than or equal to {1}";
            }

            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, max);
        }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }

            return double.TryParse(Convert.ToString(value), out double valueAsDouble) && valueAsDouble <= max;
        }
    }
}
