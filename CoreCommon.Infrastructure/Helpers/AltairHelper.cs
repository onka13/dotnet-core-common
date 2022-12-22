using System;
using System.Globalization;

namespace CoreCommon.Infrastructure.Helpers
{
    public static class AltairHelper
    {
        public static bool ParseBoolean(string value)
        {
            return value == "X" || value == "x" ? true : false;
        }

        public static double ConvertToDouble(string value)
        {
            return Convert.ToDouble(value, new CultureInfo("en-GB"));
        }

        public static string ConvertDurationUnitFromAltairTOClarizen(string altairUnit)
        {
            // TODO: Add all the cases
            switch (altairUnit)
            {
                case "H":
                    return "Hours";
                default:
                    return string.Empty;
            }
        }
    }
}
