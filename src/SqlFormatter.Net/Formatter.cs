using SqlFormatter.Net.Models;
using SqlFormatter.Net.Utils;
using System.Text.RegularExpressions;

namespace SqlFormatter.Net
{
    public abstract class Formatter
    {
        private int _linesBetweenQueries;
        private bool _uppercase;
        private Indentation _indentation = default!;
        private readonly InlineBlock _inlineBlock = new();
        private Params _params = default!;
        private List<Token> _tokens = new();
        protected Token? _previousReservedToken;
        private int _index;

        protected abstract Tokenizer Tokenizer();

        public string Format(string query, FormatOptions options = new())
        {
            SetOptions(options);

            _tokens = Tokenizer().Tokenize(query);

            string formattedQuery = GetFormattedQueryFromTokens();

            return formattedQuery.Trim();
        }

        private void SetOptions(FormatOptions options)
        {
            _linesBetweenQueries = options.LinesBetweenQueries;
            _uppercase = options.Uppercase;
            _indentation = new Indentation(options.Indent);
            _params = new Params(options.Params);
        }

        private string GetFormattedQueryFromTokens()
        {
            string formattedQuery = "";

            for (int i = 0; i < _tokens.Count; i++)
            {
                _index = i;
                Token token = TokenOverride(_tokens[i]);

                formattedQuery = token.Type switch
                {
                    TokenType.LineComment => FormatLineComment(token, formattedQuery),
                    TokenType.BlockComment => FormatBlockComment(token, formattedQuery),
                    TokenType.ReservedTopLevel => FormatTopLevelReservedWord(token, formattedQuery),
                    TokenType.ReservedTopLevelNoIndent => FormatTopLevelReservedWordNoIndent(token, formattedQuery),
                    TokenType.ReservedNewline => FormatNewlineReservedWord(token, formattedQuery),
                    TokenType.Reserved => FormatWithSpaces(token, formattedQuery, setPreviousToken: true),
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
            return formattedQuery.Trim();
        }

        protected virtual Token TokenOverride(Token token) => token;

        private string FormatQuerySeparator(Token token, string query)
        {
            _indentation.ResetIndentation();

            int times = _linesBetweenQueries == 0 ? 1 : _linesBetweenQueries;

            return query.TrimSpaceEnd() + ToUpperIfNeeded(token) + Environment.NewLine.Repeat(times);
        }

        private string FormatComma(Token token, string query)
        {
            query = query.TrimSpaceEnd() + ToUpperIfNeeded(token) + " ";

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

                if (behindToken is not { Type: TokenType.OpenParen or TokenType.LineComment or TokenType.Operator })
                {
                    query = query.TrimSpaceEnd();
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
            return query.TrimSpaceEnd() + ToUpperIfNeeded(token);
        }

        private string FormatWithSpaces(Token token, string query, bool setPreviousToken = false)
        {
            if (setPreviousToken)
            {
                SetPreviousToken(token);
            }

            return query + ToUpperIfNeeded(token) + " ";
        }

        protected Token? TokenLookBehind(int n = 1) => _tokens.ElementAtOrDefault(_index - n);
        protected Token? TokenLookAhead(int n = 1) => _tokens.ElementAtOrDefault(_index + n);

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
                (token.Type == TokenType.Reserved ||
                token.Type == TokenType.ReservedTopLevel ||
                token.Type == TokenType.ReservedTopLevelNoIndent ||
                token.Type == TokenType.ReservedNewline ||
                token.Type == TokenType.OpenParen ||
                token.Type == TokenType.CloseParen))
            {
                return token.Value.ToUpper();
            }
            return token.Value;
        }

        private string AddNewLine(string query)
        {
            query = query.TrimSpaceEnd();

            if (!query.EndsWith("\n"))
            {
                query += Environment.NewLine;
            }

            return query + _indentation.GetIndent();
        }
    }
}
