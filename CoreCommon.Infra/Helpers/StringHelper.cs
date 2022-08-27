using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Linq;

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

        public static string GetFirstNotEmpty(params string[] texts)
        {
            foreach (var item in texts)
            {
                if (!string.IsNullOrWhiteSpace(item)) return item;
            }
            return null;
        }

        public static string FirstCharToUpper(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            return input[0].ToString().ToUpper() + input.Substring(1);
        }

        public static string TitleCase(string txt)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(txt ?? "");
        }

        public static string SentenceCase(string txt)
        {
            var endChars = "?!.:,;";
            var regex = new Regex($@"(.*?)([{endChars}]|($))", RegexOptions.ExplicitCapture);
            var result = regex.Replace((txt ?? "").ToLower(), s =>
            {
                return " " + FirstCharToUpper(s.Value?.Trim());
            }).Trim();

            if (!endChars.Contains(result.LastOrDefault()))
            {
                result += ".";
            }

            return result;
        }
    }
}
