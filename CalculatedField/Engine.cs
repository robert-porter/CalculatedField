using System.Collections.Generic;
using System.Linq;

namespace CalculatedField
{
    class Engine
    {
        public Engine(string script)
        {
            Script = script;
            CompilerErrors = new List<ScriptError>();
        }

        public bool Compile()
        {
            CompilerErrors.Clear();
            var lexer = new Lexer(Script);
            CompilerErrors.AddRange(lexer.Errors);
            Parser parser = new Parser(lexer.Tokens);
            var expression = parser.Parse();
            CompilerErrors.AddRange(parser.Errors);
            var codeGenerator = new Compiler();
            var compiledScript = codeGenerator.Compile(expression);
            Machine = new VirtualMachine(compiledScript);
            return CompilerErrors.Count() == 0;
        }

        public ScriptValue Run(Dictionary<string, ScriptValue> record)
        {
            if (Machine == null)
                Compile();
            return Machine.Run();            
        }

        VirtualMachine Machine;
        public List<ScriptError> CompilerErrors { get; private set; }
        //List<ScriptError> RuntimeErrors;
        string Script;
    }
}
