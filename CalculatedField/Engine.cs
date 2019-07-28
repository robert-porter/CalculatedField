using System;
using System.Collections.Generic;
using System.Linq;

namespace CalculatedField
{
    public class Engine
    {
        public object CalculateValue(CompiledScript compiledScript, Dictionary<Guid, object> record)
        {
            if (compiledScript != null)
            {
                List<object> fieldValues = compiledScript.FieldsUsed.Select(field => record[field.FieldId]).ToList();
                var value = compiledScript.Calculate();
                return value;
            }
            return null;
        }

        public object CalculateValue(string script)
        {
            var fields = new List<Field>();
            var record = new Dictionary<Guid, object>();
            var compiledScript = Compile(script, fields);
            if (compiledScript != null)
            {
                var value = compiledScript.Calculate();
                return value;
            }
            return null;
        }

        public CompiledScript Compile(string script, List<Field> fields)
        {
            var tokenizer = new Tokenizer();
            tokenizer.CreateTokenDefinitions();
            var tokens = tokenizer.Tokenize(script).ToList();
            var tokenStream = new TokenStream(tokens);
            Parser parser = new Parser();
            var expression = parser.Parse(tokenStream);
            var symbols = new Symbols(fields);
            var resolver = new Resolver();
            resolver.Resolve(expression, symbols);
            var codeGenerator = new CSharpCodeGenerator();
            var function = codeGenerator.GenerateProgram(expression, symbols);

            var compiledScript = new CompiledScript
            {
                Calculate = function, 
            };

            return compiledScript;
        }
    }
}
