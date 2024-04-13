using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DA_Assets.Shared.Extensions
{
    public static class StringExtensions
    {
        public static float GetSizeMB(this string myString)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(myString);

            using (MemoryStream stream = new MemoryStream(byteArray))
            {
                long sizeInBytes = stream.Length;
                float sizeInMegabytes = (float)sizeInBytes / (1024 * 1024);

                sizeInMegabytes = (float)Math.Floor(sizeInMegabytes * 100) / 100;
                return sizeInMegabytes;
            }
        }

        public static string AddWithNewLine(this object source)
        {
            string result = "";
            result += Environment.NewLine;
            result += source;
            return result;
        }

        /// <summary>
        /// https://stackoverflow.com/a/46095771
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public static string ToPascalCase(this string original)
        {
            Regex invalidCharsRgx = new Regex("[^_a-zA-Z0-9]");
            Regex whiteSpace = new Regex(@"(?<=\s)");
            Regex startsWithLowerCaseChar = new Regex("^[a-z]");
            Regex firstCharFollowedByUpperCasesOnly = new Regex("(?<=[A-Z])[A-Z0-9]+$");
            Regex lowerCaseNextToNumber = new Regex("(?<=[0-9])[a-z]");
            Regex upperCaseInside = new Regex("(?<=[A-Z])[A-Z]+?((?=[A-Z][a-z])|(?=[0-9]))");

            // replace white spaces with undescore, then replace all invalid chars with empty string
            var pascalCase = invalidCharsRgx.Replace(whiteSpace.Replace(original, "_"), string.Empty)
                // split by underscores
                .Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries)
                // set first letter to uppercase
                .Select(w => startsWithLowerCaseChar.Replace(w, m => m.Value.ToUpper()))
                // replace second and all following upper case letters to lower if there is no next lower (ABC -> Abc)
                .Select(w => firstCharFollowedByUpperCasesOnly.Replace(w, m => m.Value.ToLower()))
                // set upper case the first lower case following a number (Ab9cd -> Ab9Cd)
                .Select(w => lowerCaseNextToNumber.Replace(w, m => m.Value.ToUpper()))
                // lower second and next upper case letters except the last if it follows by any lower (ABcDEf -> AbcDef)
                .Select(w => upperCaseInside.Replace(w, m => m.Value.ToLower()));

            return string.Concat(pascalCase);
        }

        public static string ToPascalSnakeCase(this string original)
        {
            string normalized = Regex.Replace(original, "[^-_a-zA-Z0-9]", " ");

            normalized = Regex.Replace(normalized, "([a-z])([A-Z])", "$1 $2");
            normalized = Regex.Replace(normalized, "([0-9])([a-zA-Z])", "$1 $2");
            normalized = Regex.Replace(normalized, "([a-zA-Z])([0-9])", "$1 $2");
            normalized = normalized.Replace("-", " ").Replace("_", " ");

            normalized = Regex.Replace(normalized, @"\s+", " ").Trim();

            string[] words = normalized
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(word => CultureInfo.InvariantCulture.TextInfo.ToTitleCase(word.ToLowerInvariant()))
                .ToArray();

            return string.Join("_", words);
        }

        public static bool IsEmpty(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// Truncates a string to a specific character only if the string really needs to be truncated. Does not cause an exception.
        /// https://stackoverflow.com/a/2776689
        /// </summary>
        public static string SubstringSafe(this string value, int maxLength)
        {
            return value?.Length > maxLength ? value.Substring(0, maxLength) : value;
        }
        /// <summary>
        /// Removes all HTML tags from string.
        /// <para><see href="https://stackoverflow.com/a/18154046"/></para>
        /// </summary>
        public static string RemoveHTML(this string text)
        {
            return Regex.Replace(text, "<.*?>", string.Empty);
        }
        /// <summary>
        /// Removing string between two strings.
        /// <para><see href="https://stackoverflow.com/q/51891661"/></para>
        /// </summary>
        public static string RemoveBetween(this string text, string startTag, string endTag)
        {
            Regex regex = new Regex(string.Format("{0}(.*?){1}", Regex.Escape(startTag), Regex.Escape(endTag)), RegexOptions.RightToLeft);
            string result = regex.Replace(text, startTag + endTag);
            return result;
        }
        /// <summary>
        /// Get part of string between two strings.
        /// <para><see href="https://stackoverflow.com/a/17252672"/></para>
        /// </summary>
        public static string GetBetween(this string text, string startTag, string endTag)
        {
            int pFrom = text.IndexOf(startTag) + startTag.Length;
            int pTo = text.LastIndexOf(endTag);
            string result = text.Substring(pFrom, pTo - pFrom);
            return result;
        }
        /// <summary>
        /// Simplified syntax for splitting string by string
        /// </summary>
        public static string[] SplitByString(this string text, string separator)
        {
            return text.Split(new string[] { separator }, StringSplitOptions.None);
        }
    }
}