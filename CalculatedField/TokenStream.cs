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
        public int Index
        {
            get;
            protected set;
        }

        public TokenStream(List<Token> tokens)
        {
            Tokens = tokens;
            Index = 0;
        }

        public Token Read()
        {
            if (Index >= Tokens.Count - 1)
                throw new ScriptError(Index, string.Format("Unexpected end of file"));
            return Tokens[Index++];
        }

        public bool Match(params TokenType[] types)
        {
            if (Index >= Tokens.Count)
                throw new ScriptError(Index, "Unexpected end of file");
            foreach (var type in types)
                if (Tokens[Index].Type == type)
                    return true;
            return false;
        }


        public Token Expect(params TokenType[] types)
        {
            if (Index >= Tokens.Count)
                throw new ScriptError(Index, "Unexpected end of file");

            foreach (var type in types)
            {
                if (Tokens[Index].Type == type)
                {
                    var token = Tokens[Index];
                    Index++;
                    return token;
                }
            }
            var typesString = string.Join(", ", types);
            throw new ScriptError(Index, $"Expected ({typesString}) found {Tokens[Index].Contents}. ");
        }
    }
}
