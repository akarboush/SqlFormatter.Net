using SqlFormatter.Net.Models;
using System.Text.RegularExpressions;

namespace SqlFormatter.Net.Utils
{
    public static class RegexUtils
    {
        private static readonly Regex _escapeRegex = new(@"[.*+?^${}()|[\]\\]", RegexOptions.CultureInvariant | RegexOptions.Compiled);
        private static readonly Regex _equalizeWhitespaceRegex = new(@"\s+", RegexOptions.CultureInvariant | RegexOptions.Compiled);

        public static string ExcapeSpecialCharacters(string s)
        {
            return _escapeRegex.Replace(s, @"\$&");
        }

        public static string ExcapeParen(string paren)
        {
            if (paren.Length == 1)
            {
                return ExcapeSpecialCharacters(paren);
            }
            return @$"\b{paren}\b";
        }

        public static string EqualizeWhitespace(string s)
        {
            return _equalizeWhitespaceRegex.Replace(s, " ");
        }


    }
}
