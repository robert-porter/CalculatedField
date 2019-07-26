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

        public Token Peek()
        {
            if (Index >= Tokens.Count)
                throw new ScriptError(Tokens[Index].Column, Tokens[Index].Line, string.Format("Unexpected end of file"));
            return Tokens[Index];
        }

        public TokenType PeekType()
        {
            if (Index >= Tokens.Count)
                throw new ScriptError(Tokens[Index].Column, Tokens[Index].Line, string.Format("Unexpected end of file"));
            return Tokens[Index].Type;
        }

        public Token Read()
        {
            if (Index >= Tokens.Count - 1)
                throw new ScriptError(Tokens[Index].Column, Tokens[Index].Line, string.Format("Unexpected end of file"));
            return Tokens[Index++];
        }

        public Token MatchAndRead(TokenType type)
        {
            if (Index >= Tokens.Count)
                throw new ScriptError(Tokens[Index].Column, Tokens[Index].Line, string.Format("Unexpected end of file"));
            if (Tokens[Index].Type == type)
            {
                var token = Tokens[Index];
                Index++;
                return token;
            }
            return null;
        }

        public bool Match(TokenType type, int ahead = 0)
        {
            if (Index + ahead >= Tokens.Count)
                throw new ScriptError(Tokens[Index].Column, Tokens[Index].Line, string.Format("Unexpected end of file"));
            return Tokens[Index + ahead].Type == type;
        }

        public Token Expect(TokenType type)
        {
            if (Index >= Tokens.Count)
                throw new ScriptError(Tokens[Index].Column, Tokens[Index].Line, string.Format("Unexpected end of file"));
            if(Tokens[Index].Type == type)
            {
                var token = Tokens[Index];
                Index++;
                return token;
            }
            if(Tokens[Index].Type == TokenType.Equal)
            {
                throw new ScriptError(Tokens[Index].Column, Tokens[Index].Line, "An assignemnt statement must be on the left hand side of a line");
            }
            throw new ScriptError(Tokens[Index].Column, Tokens[Index].Line, $"Unexpected token {Tokens[Index].Contents}");
        }
    }
}
