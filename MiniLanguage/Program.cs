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

function f(x, y, z) 
{
    if(x < 5) 
    {
        x = 200;
    }
    else 
    {
        x = 7;
    }  
    return x;
}

var q = f(1, 2, 3);

}
            "
                );

            lexer.Lex();

            Parser p = new Parser(lexer.Tokens);

            Node n = p.ParseProgram();
            Compiler c = new Compiler();
            c.Compile(n);

            VirtualMachine m = new VirtualMachine();
            m.Run(c.Instructions, c.StartAddress);

            Console.WriteLine((m.GetVar("q") as NumberValue).DoubleVal);

            System.Console.ReadKey();
        }
    }
}
