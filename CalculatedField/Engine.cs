using System;
using System.Collections.Generic;

namespace CalculatedField
{
    public class Engine
    {
        public Engine()
        {
        }

        public object CalculateValue(string script)
        {
            var fields = new List<Field>();
            var record = new Dictionary<string, object>();
            var calculate = Compile(script, fields);

            if (calculate != null)
            {
                var value = calculate(record);
                return value;
            }
            return null;
        }

        public List<ScriptError> GetErrors(string script, List<Field> fields)
        {
            List<ScriptError> errors = new List<ScriptError>();
            var tokenizer = new Tokenizer();
            tokenizer.CreateTokenDefinitions();
            var (tokens, tokenizerErrors) = tokenizer.Tokenize(script);
            errors.AddRange(tokenizerErrors);
            Parser parser = new Parser(tokens);
            var (expression, parserErrors) = parser.Parse();
            errors.AddRange(parserErrors);
            if (expression != null)
            {
                var resolver = new Resolver(fields);
                var resolverErrors = resolver.ResolveScript(expression);
                errors.AddRange(resolverErrors);
                return errors;
            }
            return errors;
        }

        public Func<Dictionary<string, object>, object> Compile(string script, List<Field> fields)
        {
            List<ScriptError> errors = new List<ScriptError>();
            var tokenizer = new Tokenizer();
            tokenizer.CreateTokenDefinitions();
            var (tokens, tokenizerErrors) = tokenizer.Tokenize(script);
            errors.AddRange(tokenizerErrors);
            Parser parser = new Parser(tokens);
            var (expression, parserErrors) = parser.Parse();
            errors.AddRange(parserErrors);
            if(expression != null)
            {
                var resolver = new Resolver(fields);
                var resolverErrors = resolver.ResolveScript(expression);
                errors.AddRange(resolverErrors);
                if(errors.Count == 0)
                {
                    var codeGenerator = new LambdaGenerator();
                    return codeGenerator.GenerateProgram(expression, fields);
                }
            }
            return null;
        }
    }
}
