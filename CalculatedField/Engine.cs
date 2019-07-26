using System;
using System.Collections.Generic;
using System.Linq;

namespace CalculatedField
{
    public class Engine
    {
        public void Calculate(List<Field> fields, List<Dictionary<Guid, object>> records)
        {
            var compiledScripts = fields
                .Where(field => !string.IsNullOrWhiteSpace(field.Script))
                .Select(field => Compile(field, fields))
                .Where(compiledScript => compiledScript != null);

            //sort 

            foreach (var compiledScript in compiledScripts)
            {
                foreach (var record in records)
                {
                    Calculate(compiledScript, record);
                }
            }
        }

        public void Calculate(CompiledScript compiledScript, Dictionary<Guid, object> record)
        {
            if (compiledScript != null)
            {
                List<ScriptValue> fieldValues = compiledScript.Fields.Select(field => new ScriptValue(record[field.FieldId])).ToList();
                var value = VirtualMachine.Run(compiledScript, fieldValues);
                if (value.Type != ScriptType.Null)
                    record[compiledScript.Field.FieldId] = value.Value;
            }
        }

        public ScriptValue CalculateValue(CompiledScript compiledScript, Dictionary<Guid, object> record)
        {
            if (compiledScript != null)
            {
                List<ScriptValue> fieldValues = compiledScript.Fields.Select(field => new ScriptValue(record[field.FieldId])).ToList();
                var value = VirtualMachine.Run(compiledScript, fieldValues);
                return value;
            }
            return new ScriptValue();
        }

        public ScriptValue CalculateValue(string script)
        {
            var fields = new List<Field>();
            var record = new Dictionary<Guid, object>();
            return CalculateValue(script, fields, record);
        }

        public ScriptValue CalculateValue(string script, List<Field> fields, Dictionary<Guid, object> record)
        {
            var compiledScript = Compile(script, fields);
            if (compiledScript != null)
            {
                List<ScriptValue> fieldValues = compiledScript.Fields.Select(field => new ScriptValue(record[field.FieldId])).ToList();
                var value = VirtualMachine.Run(compiledScript, fieldValues);
                return value;
            }
            return new ScriptValue();
        }

        public CompiledScript Compile(string script, List<Field> fields)
        {
            var tokenizer = new Tokenizer();
            tokenizer.CreateTokenDefinitions();
            var tokenizerResult = tokenizer.Tokenize(script);
            var tokens = Tokenizer.FixNewlines(tokenizerResult.ToList());
            Parser parser = new Parser();
            var tokenStream = new TokenStream(tokens);
            var expression = parser.Parse(tokenStream);
            Symbols symbols = new Symbols(null, fields);
            var resolver = new Resolver();
            resolver.ResolveProgram(expression, symbols);
            var codeGenerator = new CodeGenerator();
            var instructions = codeGenerator.GenerateProgram(expression);
            var compiledScript = new CompiledScript
            {
                Functions = symbols.GetScriptFunctions(),
                Constants = symbols.GetScriptConstants(),
                Fields = symbols.GetScriptFields(),
                NumVariables = symbols.ScriptVariablesCount,
                Instructions = instructions,
                Field = null
            };

            return compiledScript;
        }
    

        public CompiledScript Compile(Field field, List<Field> fields)
        {
            var tokenizer = new Tokenizer();
            tokenizer.CreateTokenDefinitions();
            var tokenizerResult = tokenizer.Tokenize(field.Script);
            var tokens = Tokenizer.FixNewlines(tokenizerResult.ToList());
            Parser parser = new Parser();
            var tokenStream = new TokenStream(tokens.ToList());
            var expression = parser.Parse(tokenStream);
            Symbols symbols = new Symbols(field, fields);
            var resolver = new Resolver();
            resolver.ResolveProgram(expression, symbols);
            var codeGenerator = new CodeGenerator();
            var instructions = codeGenerator.GenerateProgram(expression);
            var compiledScript = new CompiledScript
            {
                Functions = symbols.GetScriptFunctions(),
                Constants = symbols.GetScriptConstants(),
                Fields = symbols.GetScriptFields(),
                NumVariables = symbols.ScriptVariablesCount,
                Instructions = instructions,
                Field = field
            };

            return compiledScript;
        }
    }
}
