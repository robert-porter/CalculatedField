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

function g(z) { return z * 2; }

function f(x) 
{
  var p = 2;
 if(x < 2) 
   return 0;
else 
  return 1 + f(x-1); 
}

var q = f(10);
}
"
                );


            lexer.Lex();

            Parser parser = new Parser(lexer.Tokens);

            SyntaxTree node = parser.ParseProgram();
            ScopeChecker scopeChecker = new ScopeChecker();
            node.Accept(scopeChecker);
            Compiler compiler = new Compiler();
            compiler.Compile(node);
            VirtualMachine machine = new VirtualMachine();
            machine.Run(compiler.Instructions, compiler.StartAddress);

            Console.WriteLine((machine.GetVar("q") as NumberValue).DoubleVal);

            System.Console.ReadKey();
        }
    }
}
