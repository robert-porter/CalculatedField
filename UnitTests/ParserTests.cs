using CalculatedField;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;


namespace UnitTests
{
    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void TestComplexExpressions()
        {
            var engine = new Engine();
            var field = new Field();
            field.Type = ScriptType.Decimal;
            ScriptValue value;
            CompiledScript compiledScript;

            field.Script = "(2.0) + (17*2-30) * (5)+2 - (8/2)*4";
            compiledScript = engine.Compile(field, new List<Field>());
            value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(value.Value, 8m);

            field.Script = "(( ((2.0)) + 4))*((5))";
            compiledScript = engine.Compile(field, new List<Field>());
            value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(value.Value, 30m);

        }

        [TestMethod]
        public void TestIf()
        {
            var engine = new Engine();
            var field = new Field();
            field.Type = ScriptType.Decimal;
            ScriptValue value;
            CompiledScript compiledScript;

            field.Script = "if true or false then 1.0 else 2.0 end";
            compiledScript = engine.Compile(field, new List<Field>());
            value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(value.Value, 1m);

            field.Script = @"

1 + if true or false then 1.0 else

2.0 

end";
            compiledScript = engine.Compile(field, new List<Field>());
            value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(value.Value, 2m);

            field.Script = @"
if true or false then 
1.0 
else

2.0 

end";
            compiledScript = engine.Compile(field, new List<Field>());
            value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(value.Value, 1m);

            field.Script = @"
if true or false then 1.0 
else 2.0 end";
            compiledScript = engine.Compile(field, new List<Field>());
            value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(value.Value, 1m);

            field.Script = "if false then null end";
            field.Type = ScriptType.Null;
            compiledScript = engine.Compile(field, new List<Field>());
            value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(value.Value, null);
        }
    }
}
