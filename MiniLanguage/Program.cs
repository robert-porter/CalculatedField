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

function f(x) { 
    var b = true;
    if(b && false) {
        return 1; 
    }

    if(b || true)
        return 10;

    return 5;
}
var x=f(1+34);
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

