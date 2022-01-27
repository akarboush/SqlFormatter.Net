using SqlFormatter.Net.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqlFormatter.Net
{
    internal class RegexFactory
    {
        internal const string WhiteSpaceRegexPattern = @"^(\s+)";
        internal const string NumberRegexPattern = @"^((-\s*)?[0-9]+(\.[0-9]+)?([eE]-?[0-9]+(\.[0-9]+)?)?|0x[0-9a-fA-F]+|0b[01]+)\b";
        internal const string BlockCommentRegexPattern = @"^(\/\*[\s\S]*?(?:\*\/|$))";

        private const RegexOptions DefaultOption = RegexOptions.CultureInvariant | RegexOptions.Compiled;

        public static Regex Create(string pattern, RegexOptions options) => new Regex(pattern, options);

        public static Regex Default(string pattern) => Create(pattern, DefaultOption);

        public static Regex CreateOperatorRegex(params string[] operators)
        {
            operators.SortByLengthDesc();

            operators.ExcapeSpecialRegexCharacters();

            string pattern = $"^({string.Join('|', operators)}|.)";

            return Default(pattern);
        }

        public static Regex CreateWhiteSpaceRegex() => Default(WhiteSpaceRegexPattern);

        public static Regex CreateNumberRegex() => Default(NumberRegexPattern);

        public static Regex CreateBlockCommentRegex() => Default(BlockCommentRegexPattern);

        public static Regex CreateLineCommentRegex(string[] lineCommentTypes)
        {
            lineCommentTypes.ExcapeSpecialRegexCharacters();

            string pattern = @$"^((?:{string.Join('|', lineCommentTypes)}).*?)(?:\r\n|\r|\n|$)";

            return Default(pattern);
        }

        internal static Regex CreateReservedWordRegex(string[] reservedWords)
        {
            if (reservedWords.Length == 0)
            {
                return Default(@"^\b$");
            }
            reservedWords.SortByLengthDesc();

            var reservedWordsPattern = Regex.Replace(string.Join('|', reservedWords), " ", @"\s+", RegexOptions.CultureInvariant);

            return Create(@$"^({reservedWordsPattern})\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        internal static Regex CreateWordRegex(string[] specialWordChars)
        {
            // https://docs.microsoft.com/en-us/dotnet/standard/base-types/character-classes-in-regular-expressions#supported-unicode-general-categories
            var pattern = @"^([\w" + string.Join("", specialWordChars) + "]+)";
            //var pattern = @"^([\p{L}\p{M}\p{N}\p{Pc}\p{C}" + string.Join("", specialWordChars) + "]+)";

            return Default(pattern);
        }

        internal static Regex CreateStringRegex(string[] stringTypes)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append("^(");

            for (int i = 0; i < stringTypes.Length; i++)
            {
                if (i != 0 && i  != stringTypes.Length)
                {
                    stringBuilder.Append('|');
                }
                _ = stringTypes[i] switch
                {
                    "``" => stringBuilder.Append(@"((`[^`]*($|`))+)"),
                    "{}" => stringBuilder.Append(@"((\\{[^\\}]*($|\\}))+)"),
                    "[]" => stringBuilder.Append(@"((\\[[^\\]]*($|\\]))(\\][^\\]]*($|\\]))*)"),
                    "\"\"" => stringBuilder.Append(@"((""[^""\\\\]*(?:\\\\.[^""\\\\]*)*(""|$))+)"),
                    "''" => stringBuilder.Append(@"(('[^'\\\\]*(?:\\\\.[^'\\\\]*)*('|$))+)"),
                    "N''" => stringBuilder.Append(@"((N'[^'\\\\]*(?:\\\\.[^'\\\\]*)*('|$))+)"),
                    "U&''" => stringBuilder.Append(@"((U&'[^'\\\\]*(?:\\\\.[^'\\\\]*)*('|$))+)"),
                    "U&\"\"" => stringBuilder.Append(@"((U&""[^""\\\\]*(?:\\\\.[^""\\\\]*)*(""|$))+)"),
                    "$$" => stringBuilder.Append(@"((?<tag>\\$\\w*\\$)[\\s\\S]*?(?:\\k<tag>|$))"),
                    _ => throw new NotImplementedException(),
                };
            }
            stringBuilder.Append(")");

            return Default(stringBuilder.ToString());
        }

        internal static Regex CreateParenRegex(string[] parens)
        {
            parens.ExcapeParen();

            var pattern = $@"^({string.Join('|', parens)})";

            return Create(pattern, DefaultOption | RegexOptions.IgnoreCase);
        }

        internal static Regex? CreatePlaceholderRegex(char[] placeholderTypes, string endPattern)
        {
            if (placeholderTypes.Length == 0)
            {
                return null;
            }
            var stringArray = placeholderTypes.Select(x => x.ToString()).ToArray();

            stringArray.ExcapeSpecialRegexCharacters();

            var pattern = @$"^((?:{string.Join('|', stringArray)})(?:{endPattern}))";

            return Default(pattern);
        }


    }
}
