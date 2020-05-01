using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CoreCommon.Infra.Helpers
{
    public class StringHelper
    {
        public static int ParseInt(string txt, int defaultValue = 0)
        {
            if (txt != null) txt = txt.Trim();
            return int.TryParse(txt, out int result) ? result : defaultValue;
        }

        public static string GetCapitals(string txt)
        {
            return Regex.Replace(txt, @"[^A-Z]+", "");
        }

        public static string Trim(string s, string trimmer)
        {
            if (string.IsNullOrEmpty(s) || string.IsNullOrEmpty(trimmer) || !s.EndsWith(trimmer, StringComparison.OrdinalIgnoreCase))
                return s;
            return s.Substring(0, s.Length - trimmer.Length);
        }
    }
}
