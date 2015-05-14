using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniLanguage
{
    class ProgramMain
    {
        static void Main(string[] args)
        {
            String program = @"

function f(x : int, y) { 
    var z : int = 20;
    return x+z;
}
var q = 10;
var v = 2*(3+3)*5 + 2 * 3;
";

            try
            {
                Lexer lexer = new Lexer(program);


                lexer.Lex();

                Parser parser = new Parser(lexer.Tokens);

                ProgramNode node = parser.ParseProgram();
                TypeChecker scopeChecker = new TypeChecker();
                node.Accept(scopeChecker);
                Compiler compiler = new Compiler();
                compiler.Compile(node);
                VirtualMachine machine = new VirtualMachine();
                machine.Run(compiler.Instructions, compiler.Constants, compiler.StartAddress);

                Console.WriteLine(machine.GetVar(1).DoubleVal);
            }
            catch (SyntaxError syntaxError)
            {
                Console.WriteLine(syntaxError.Message);
            }
            Console.WriteLine("Press any key to continue...");
            System.Console.ReadKey();
        }
    }
}
