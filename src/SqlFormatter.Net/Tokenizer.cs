using SqlFormatter.Net.Models;
using SqlFormatter.Net.Utils;
using System.Text.RegularExpressions;

namespace SqlFormatter.Net
{
    public class Tokenizer
    {
        private readonly Regex _numberRegex;
        private readonly Regex _operatorRegex;
        private readonly Regex _blockCommentRegex;
        private readonly Regex _lineCommentRegex;
        private readonly Regex _reservedTopLevelRegex;
        private readonly Regex _reservedTopLevelNoIndentRegex;
        private readonly Regex _reservedNewLineRegex;
        private readonly Regex _reservedPlainRegex;
        private readonly Regex _wordRegex;
        private readonly Regex _stringRegex;
        private readonly Regex _openParenRegex;
        private readonly Regex _closeParenRegex;
        private readonly Regex? _indexedPlaceholderRegex;
        private readonly Regex? _identNamedPlceholderRegex;
        private readonly Regex? _stringNamedPlceholderRegex;

        public Tokenizer(
            string[] reservedWords,
            string[] reservedTopLevelWords,
            string[] reservedNewlineWords,
            string[] reservedTopLevelWordsNoIndent,
            string[] stringTypes,
            string[] openParens,
            string[] closeParens,
            char[] indexedPlaceholderTypes,
            char[] namedPlaceholderTypes,
            string[] lineCommentTypes,
            string[] specialWordChars,
            string[] operators
            )
        {
            _numberRegex = RegexFactory.CreateNumberRegex();
            _operatorRegex = RegexFactory.CreateOperatorRegex(new[] { "<>", ">=", "<=" }.Concat(operators).ToArray());
            _blockCommentRegex = RegexFactory.CreateBlockCommentRegex();
            _lineCommentRegex = RegexFactory.CreateLineCommentRegex(lineCommentTypes);
            _reservedTopLevelRegex = RegexFactory.CreateReservedWordRegex(reservedTopLevelWords);
            _reservedTopLevelNoIndentRegex = RegexFactory.CreateReservedWordRegex(reservedTopLevelWordsNoIndent);
            _reservedNewLineRegex = RegexFactory.CreateReservedWordRegex(reservedNewlineWords);
            _reservedPlainRegex = RegexFactory.CreateReservedWordRegex(reservedWords);
            _wordRegex = RegexFactory.CreateWordRegex(specialWordChars);
            _stringRegex = RegexFactory.CreateStringRegex(stringTypes);
            _openParenRegex = RegexFactory.CreateParenRegex(openParens);
            _closeParenRegex = RegexFactory.CreateParenRegex(closeParens);
            _indexedPlaceholderRegex = RegexFactory.CreatePlaceholderRegex(indexedPlaceholderTypes, "[0-9]*");
            _identNamedPlceholderRegex = RegexFactory.CreatePlaceholderRegex(namedPlaceholderTypes, "[a-zA-Z0-9._$]+");
            _stringNamedPlceholderRegex = RegexFactory.CreatePlaceholderRegex(namedPlaceholderTypes, _stringRegex.ToString());
        }


        internal List<Token> Tokenize(string input)
        {
            List<Token> tokens = new();
            Token? token = null;

            while (input.Length != 0)
            {
                string precedingWitespace = GetPrecedingWitespace(input);
                input = input[precedingWitespace.Length..];

                if (input.Length != 0)
                {
                    token = getNextToken(input, previousToken: token);

                    input = input[token.Value.Length..];

                    token.PrecedingWitespace = precedingWitespace;

                    tokens.Add(token);
                }
            }
            return tokens;
        }

        private string GetPrecedingWitespace(string input)
        {
            int n = 0;
            for (int i = 0; i < input.Length; i++)
            {
                n = i;
                char c = input[i];
                if (!char.IsWhiteSpace(c))
                    break;
            }
            return input[..n];
        }

        private Token getNextToken(string input, Token? previousToken)
        {
            return
                GetCommentToken(input) ??
                GetStringtToken(input) ??
                GetOpenParenToken(input) ??
                GetCloseParenToken(input) ??
                GetPlaceholderToken(input) ??
                GetNumberToken(input) ??
                GetReservedWordToken(input, previousToken) ??
                GetWordToken(input) ??
                GetOperatorToken(input)!;
        }

        private Token? GetCommentToken(string input) => GetLineCommentToken(input) ?? GetBlockCommentToken(input);

        private Token? GetLineCommentToken(string input) => GetTokenOnFirstMatchOrDefualt(input, TokenType.LineComment, _lineCommentRegex);

        private Token? GetBlockCommentToken(string input) => GetTokenOnFirstMatchOrDefualt(input, TokenType.BlockComment, _blockCommentRegex);

        private Token? GetStringtToken(string input) => GetTokenOnFirstMatchOrDefualt(input, TokenType.String, _stringRegex);

        private Token? GetOpenParenToken(string input) => GetTokenOnFirstMatchOrDefualt(input, TokenType.OpenParen, _openParenRegex);

        private Token? GetCloseParenToken(string input) => GetTokenOnFirstMatchOrDefualt(input, TokenType.CloseParen, _closeParenRegex);

        private Token? GetPlaceholderToken(string input)
        {
            return GetIdentNamedPlaceholderToken(input) ??
                GetStringNamedPlaceholderToken(input) ??
                GetIndexedPlaceholderToken(input);
        }

        private Token? GetIdentNamedPlaceholderToken(string input)
        {
            if (_identNamedPlceholderRegex is null)
            {
                return null;
            }
            return GetPlaceholderTokenWithKey(input, _identNamedPlceholderRegex, x => x.Substring(1));
        }

        // TODO: Check 
        private Token? GetStringNamedPlaceholderToken(string input)
        {
            if (_stringNamedPlceholderRegex is null)
            {
                return null;
            }

            var keyParser = (string x) =>
            {
                var key = x.Substring(2, -1);
                var quoteChar = x.Substring(-1);
                var pattern = RegexUtils.ExcapeSpecialCharacters(@$"\\{quoteChar}");
                var escapedPlaceholderKey = Regex.Replace(key, pattern, quoteChar, RegexOptions.CultureInvariant);

                return escapedPlaceholderKey;
            };

            return GetPlaceholderTokenWithKey(input, _stringNamedPlceholderRegex, keyParser);
        }
        private Token? GetIndexedPlaceholderToken(string input)
        {
            if (_indexedPlaceholderRegex is null)
            {
                return null;
            }
            return GetPlaceholderTokenWithKey(input, _indexedPlaceholderRegex, x => x.Substring(1));
        }

        private Token? GetPlaceholderTokenWithKey(string input, Regex regex, Func<string, string> keyParser)
        {
            var token = GetTokenOnFirstMatchOrDefualt(input, TokenType.Placeholder, regex);

            if (token != null)
            {
                token = new TokenWithKey(token, keyParser);
            }
            return token;
        }

        private Token? GetNumberToken(string input) => GetTokenOnFirstMatchOrDefualt(input, TokenType.Number, _numberRegex);

        private Token? GetReservedWordToken(string input, Token? previousToken)
        {
            // A reserved word cannot be preceded by a "."
            // this makes it so in "mytable.from", "from" is not considered a reserved word
            if (previousToken is Token { Value: "." })
            {
                return null;
            }

            return GetTopLevelReservedToken(input) ??
                GetNewlineReservedToken(input) ??
                GetTopLevelReservedTokenNoIndent(input) ??
                GetPlainReservedToken(input);
        }

        private Token? GetTopLevelReservedToken(string input) => GetTokenOnFirstMatchOrDefualt(input, TokenType.ReservedTopLevel, _reservedTopLevelRegex);

        private Token? GetNewlineReservedToken(string input) => GetTokenOnFirstMatchOrDefualt(input, TokenType.ReservedNewline, _reservedNewLineRegex);

        private Token? GetTopLevelReservedTokenNoIndent(string input) => GetTokenOnFirstMatchOrDefualt(input, TokenType.ReservedTopLevelNoIndent, _reservedTopLevelNoIndentRegex);

        private Token? GetPlainReservedToken(string input) => GetTokenOnFirstMatchOrDefualt(input, TokenType.Reserved, _reservedPlainRegex);

        private Token? GetWordToken(string input) => GetTokenOnFirstMatchOrDefualt(input, TokenType.Word, _wordRegex);

        private Token? GetOperatorToken(string input) => GetTokenOnFirstMatchOrDefualt(input, TokenType.Operator, _operatorRegex);


        private Token? GetTokenOnFirstMatchOrDefualt(string input, TokenType tokenType, Regex regex)
        {
            var matche = regex.Match(input);

            return matche.Success ? new Token(tokenType, matche.Value) : null;
        }
    }
}
