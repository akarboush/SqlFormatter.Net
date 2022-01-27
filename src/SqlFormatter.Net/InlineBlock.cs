using SqlFormatter.Net.Models;

namespace SqlFormatter.Net
{
    internal class InlineBlock
    {
        internal const short InlineMaxLenght = 50;
        private int level = 0;

        public void BeginIfPossible(List<Token> tokens, int index)
        {
            if (level == 0 && IsInlineBlock(tokens, index))
            {
                level = 1;
            }
            else if (level > 0)
            {
                level++;
            }
            else
            {
                level = 0;
            }
        }

        public void End() => level--;
        public bool IsActive() => level > 0;

        private bool IsInlineBlock(List<Token> tokens, int index)
        {
            int lenght = 0;
            int level = 0;

            for (int i = index; i < tokens.Count; i++)
            {
                Token token = tokens[i];
                lenght += token.Value.Length;

                if (lenght > InlineMaxLenght) return false;

                if (token.Type == TokenType.OpenParen)
                {
                    level++;
                }
                else if (token.Type == TokenType.CloseParen)
                {
                    level--;
                    if (level == 0) return true;
                }

                if (IsForbiddenToken(token)) return false;
            }
            return true;
        }

        private bool IsForbiddenToken(Token token)
        {
            return 
                token.Type == TokenType.ReservedTopLevel ||
                token.Type == TokenType.ReservedNewline ||
                token.Type == TokenType.BlockComment ||
                //TODO check what is Comment
                //token.Type == TokenType.Comment ||
                token.Value == ";";
        }
    }
}
