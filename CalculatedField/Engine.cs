using System;
using System.Collections.Generic;
using System.Linq;

namespace CalculatedField
{
    public class Engine
    {
        public object CalculateValue(string script)
        {
            var fields = new List<Field>();
            var record = new Dictionary<Guid, object>();

            /*
            Field field = new Field
            {
                Name = "a",
                Type = typeof(decimal?),
                FieldId = Guid.NewGuid(),

            };
            fields.Add(field);
            record.Add(field.FieldId, 10m); */
            var calculate = Compile(script, fields);
            if (calculate != null)
            {
                var value = calculate(record);
                return value;
            }
            return null;
        }

        public Func<Dictionary<Guid, object>, object> Compile(string script, List<Field> fields)
        {
            var tokenizer = new Tokenizer();
            tokenizer.CreateTokenDefinitions();
            var (tokens, tokenizerErrors) = tokenizer.Tokenize(script);
            var tokenStream = new TokenStream(tokens);
            Parser parser = new Parser();
            var (expression, parserErrors) = parser.Parse(tokenStream);
            var symbols = new Symbols(fields);
            var resolver = new Resolver();
            resolver.Resolve(expression, symbols);
            var codeGenerator = new CSharpCodeGenerator();
            return codeGenerator.GenerateProgram(expression, symbols);
        }
    }
}
