using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlFormatter.Net.Models
{
    public record Token
    {
        public TokenType Type { get; init; }
        public string Value { get; init; }
        public string? PrecedingWitespace { get; set; }

        public Token(TokenType type, string value)
        {
            Type = type;
            Value = value;
        }
    }

    public record TokenWithKey : Token
    {
        public Func<string, string> KeyParser { get; init; }

        public TokenWithKey(Token token, Func<string, string> keyParser) : base(token.Type, token.Value)
        {
            KeyParser = keyParser;
        }

    }
}
