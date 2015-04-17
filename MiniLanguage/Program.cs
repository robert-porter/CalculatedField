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
    var y[20];
    var i = 0;
    while(i < 5)
    {
        y[i] = i;
        i = i + 1;
    }

    return y[3];
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

