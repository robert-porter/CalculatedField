using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculatedField
{
    class TokenStream
    {
        List<Token> Tokens;
        int Index;

        public TokenStream(List<Token> tokens)
        {
            Tokens = tokens;
            Index = 0;
        }

        public int Line => Tokens[Index].Line;
        public int Column => Tokens[Index].Column;

        public Token Current()
        {
            return Tokens[Index];
        }

        public bool EOF()
        {
            return Index >= Tokens.Count || Tokens[Index].Type == TokenType.EOF;
        }

        public TokenType PeekToken()
        {
            if (Index >= Tokens.Count)
                throw new ScriptError(Tokens[Index].Column, Tokens[Index].Line, string.Format("Unexpected end of file"));
            return Tokens[Index].Type;
        }

        public string ReadContents()
        {
            if (Index >= Tokens.Count - 1)
                throw new ScriptError(Tokens[Index].Column, Tokens[Index].Line, string.Format("Unexpected end of file"));
            return Tokens[Index++].Contents;
        }

        public bool MatchAndRead(TokenType type)
        {
            if (Index >= Tokens.Count)
                throw new ScriptError(Tokens[Index].Column, Tokens[Index].Line, string.Format("Unexpected end of file"));
            if (Tokens[Index].Type == type)
            {
                Index++;
                return true;
            }
            return false;
        }

        public bool Match(TokenType type, int ahead = 0)
        {
            if (Index + ahead >= Tokens.Count)
                throw new ScriptError(Tokens[Index].Column, Tokens[Index].Line, string.Format("Unexpected end of file"));
            return Tokens[Index + ahead].Type == type;
        }

        public bool Expect(TokenType token)
        {
            if (MatchAndRead(token))
                return true;
            throw new ScriptError(Tokens[Index].Column, Tokens[Index].Line, string.Format("Expected {0}, found {1}", token, Tokens[Index].Type));
        }
    }
}
