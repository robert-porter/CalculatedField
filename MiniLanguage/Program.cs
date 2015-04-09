using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniLanguage
{
    class Program
    {
        static void Main(string[] args)
        {
            Lexer lexer = new Lexer(
            @"
{

var x = 41;

if(x < 5) 
{
    x = 1+2;
}
else 
{
 x = 7;
}

}
            "
                );

            lexer.Lex();

            Parser p = new Parser(lexer.Tokens);

            Node n = p.ParseProgram();
            Compiler c = new Compiler();
            n.Accept(c);

            VirtualMachine m = new VirtualMachine();
            m.Run(c.Instructions);

            Console.WriteLine((m.GetVar("x") as NumberValue).DoubleVal);

                System.Console.ReadKey();
        }
    }
}
