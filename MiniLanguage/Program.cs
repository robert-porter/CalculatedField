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

var q = f(1);

function f(x) { 
    var z = 34;
    if(x < 2) 
        return z;
    else 
        return 1;
}

"
                );


            lexer.Lex();

            Parser parser = new Parser(lexer.Tokens);

            ProgramNode node = parser.ParseProgram();
            ScopeChecker scopeChecker = new ScopeChecker();
            node.Accept(scopeChecker);
            Compiler compiler = new Compiler();
            compiler.Compile(node);
            VirtualMachine machine = new VirtualMachine();
            machine.Run(compiler.Instructions, compiler.Constants, compiler.StartAddress);

            Console.WriteLine(machine.GetVar(0).DoubleVal);

            System.Console.ReadKey();
        }
    }
}

