using SqlFormatter.Net.Models;
using SqlFormatter.Net.Utils;
using System.Text.RegularExpressions;

namespace SqlFormatter.Net
{
    public abstract class Formatter
    {
        private readonly int _linesBetweenQueries;
        private readonly bool _uppercase;
        private readonly Indentation _indentation;
        private readonly InlineBlock _inlineBlock;
        private readonly Params _params;
        private List<Token> _tokens = new();
        private Token? _previousReservedToken;
        private int _index;

        protected Formatter(FormatOptions? formatOptions)
        {
            formatOptions ??= new FormatOptions();

            _linesBetweenQueries = formatOptions.LinesBetweenQueries;
            _uppercase = formatOptions.Uppercase;
            _indentation = new Indentation(formatOptions.Indent);
            _inlineBlock = new InlineBlock();
            _params = new Params(formatOptions.Params);
        }

        public abstract Tokenizer Tokenizer();

        public string Format(string query)
        {
            _tokens = Tokenizer().Tokenize(query);

            string formattedQuery = GetFormattedQueryFromTokens();

            return formattedQuery.Trim();
        }

        private string GetFormattedQueryFromTokens()
        {
            string formattedQuery = "";

            for (int i = 0; i < _tokens.Count; i++)
            {
                _index = i;
                Token token = _tokens[i];

                formattedQuery = token.Type switch
                {
                    TokenType.LineComment => FormatLineComment(token, formattedQuery),
                    TokenType.BlockComment => FormatBlockComment(token, formattedQuery),
                    TokenType.ReservedTopLevel => FormatTopLevelReservedWord(token, formattedQuery),
                    TokenType.ReservedTopLevelNoIndent => FormatTopLevelReservedWordNoIndent(token, formattedQuery),
                    TokenType.ReservedNewline => FormatNewlineReservedWord(token, formattedQuery),
                    TokenType.Reserved => FormatWithSpaces(token, formattedQuery),
                    TokenType.OpenParen => FormatOpeningParentheses(token, formattedQuery),
                    TokenType.CloseParen => FormatClosingParentheses(token, formattedQuery),
                    TokenType.Placeholder => FormatPlaceholder(token, formattedQuery),

                    _ => token.Value switch
                    {
                        "," => FormatComma(token, formattedQuery),
                        ":" => FormatWithSpaceAfter(token, formattedQuery),
                        "." => FormatWithoutSpaces(token, formattedQuery),
                        ";" => FormatQuerySeparator(token, formattedQuery),
                        _ => FormatWithSpaces(token, formattedQuery)
                    }
                };
            }
            return formattedQuery;
        }

        private string FormatQuerySeparator(Token token, string query)
        {
            _indentation.ResetIndentation();

            int times = _linesBetweenQueries == 0 ? 1 : _linesBetweenQueries;

            return query.TrimEnd() + ToUpperIfNeeded(token) + new string('\n', times);
        }

        private string FormatComma(Token token, string query)
        {
            query = query.TrimEnd() + ToUpperIfNeeded(token) + " ";

            if (_inlineBlock.IsActive() || (_previousReservedToken != null && _previousReservedToken.isLimit()))
            {
                return query;
            }
            return AddNewLine(query);
        }

        private string FormatPlaceholder(Token token, string query)
        {
            return query + _params.Get((TokenWithKey)token) + " ";
        }

        private string FormatClosingParentheses(Token token, string query)
        {
            if (_inlineBlock.IsActive())
            {
                _inlineBlock.End();
                return FormatWithSpaceAfter(token, query);
            }
            _indentation.DecreaseBlockLevel();

            return FormatWithSpaces(token, AddNewLine(query));
        }

        private string FormatOpeningParentheses(Token token, string query)
        {
            if (token.PrecedingWitespace is { Length: 0 })
            {
                Token? behindToken = TokenLookBehind();

                if (behindToken is not { Type: TokenType.OpenParen | TokenType.LineComment | TokenType.Operator })
                {
                    query = query.TrimEnd();
                }
            }

            query += ToUpperIfNeeded(token);

            _inlineBlock.BeginIfPossible(_tokens, _index);

            if (!_inlineBlock.IsActive())
            {
                _indentation.IncreaseBlockLevel();
                query = AddNewLine(query);
            }

            return query;
        }

        private string FormatNewlineReservedWord(Token token, string query)
        {
            if (token.IsAnd())
            {
                Token? t = TokenLookBehind(2);

                if (t != null && t.isBetween())
                {
                    return FormatWithSpaces(token, query);
                }
            }

            SetPreviousToken(token);

            return AddNewLine(query) + RegexUtils.EqualizeWhitespace(ToUpperIfNeeded(token)) + " ";
        }

        private string FormatWithSpaceAfter(Token token, string query)
        {
            return FormatWithoutSpaces(token, query) + " ";
        }

        private string FormatWithoutSpaces(Token token, string query)
        {
            return query.TrimEnd(' ') + ToUpperIfNeeded(token);
        }

        private string FormatWithSpaces(Token token, string query)
        {
            SetPreviousToken(token);

            return query + ToUpperIfNeeded(token) + " ";
        }

        private Token? TokenLookBehind(int n = 1) => _tokens.ElementAtOrDefault(_index - n);
        private Token? TokenLookAhead(int n = 1) => _tokens.ElementAtOrDefault(_index + n);

        private string FormatTopLevelReservedWordNoIndent(Token token, string query)
        {
            _indentation.DecreaseTopLevel();

            query = AddNewLine(query) + RegexUtils.EqualizeWhitespace(ToUpperIfNeeded(token));

            SetPreviousToken(token);

            return AddNewLine(query);
        }

        private string FormatTopLevelReservedWord(Token token, string query)
        {
            _indentation.DecreaseTopLevel();

            query = AddNewLine(query);

            _indentation.IncreaseTopLevel();

            query += RegexUtils.EqualizeWhitespace(ToUpperIfNeeded(token));

            SetPreviousToken(token);

            return AddNewLine(query);
        }

        private void SetPreviousToken(Token token) => _previousReservedToken = token;
        private string FormatLineComment(Token token, string query)
        {
            return AddNewLine(query + ToUpperIfNeeded(token));
        }

        private string FormatBlockComment(Token token, string query)
        {
            return AddNewLine(AddNewLine(query) + IndentComment(token.Value));
        }

        private string IndentComment(string comment)
        {
            var pattern = @"\n[ \t]*";

            return Regex.Replace(comment, pattern, $"\n{_indentation.GetIndent()} ", RegexOptions.CultureInvariant);
        }

        private string ToUpperIfNeeded(Token token)
        {
            if (_uppercase &&
                token.Type == TokenType.Reserved ||
                token.Type == TokenType.ReservedTopLevel ||
                token.Type == TokenType.ReservedTopLevelNoIndent ||
                token.Type == TokenType.ReservedNewline ||
                token.Type == TokenType.OpenParen ||
                token.Type == TokenType.CloseParen)
            {
                return token.Value.ToUpper();
            }
            return token.Value;
        }

        private string AddNewLine(string query)
        {
            query = query.TrimEnd(' ');

            if (!query.EndsWith("\n"))
            {
                query += "\n";
            }

            return query + _indentation.GetIndent();
        }


    }
}
