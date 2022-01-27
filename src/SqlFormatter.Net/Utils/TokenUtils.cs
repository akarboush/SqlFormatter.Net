using SqlFormatter.Net.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqlFormatter.Net.Utils
{
    public static class TokenUtils
    {
        public static bool IsToken(TokenType type, string pattern, Token token)
        {
            return token.Type == type &&
                Regex.IsMatch(token.Value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        }

        public static bool IsAnd(this Token token) => IsToken(TokenType.ReservedNewline, "^AND$", token);
        public static bool isBetween(this Token token) => IsToken(TokenType.Reserved, "^BETWEEN", token);
        public static bool isLimit(this Token token) => IsToken(TokenType.ReservedTopLevel, "^LIMIT", token);
        public static bool isSet(this Token token) => IsToken(TokenType.ReservedTopLevel, "^SET", token);
        public static bool isBy(this Token token) => IsToken(TokenType.Reserved, "^BY", token);
        public static bool isWindow(this Token token) => IsToken(TokenType.ReservedTopLevel, "^WINDOW", token);
        public static bool isEnd(this Token token) => IsToken(TokenType.CloseParen, "^END", token);
    }
}
