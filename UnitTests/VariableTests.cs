using CalculatedField;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    [TestClass]
    public class VariableTests
    {
        [TestMethod]
        public void TestVariables()
        {
            var engine = new Engine();
            var field = new Field();
            field.Type = ScriptType.Decimal;
            ScriptValue value;
            CompiledScript compiledScript;

            field.Script = @"
x=3
y = 2.0
x+y";
            compiledScript = engine.Compile(field, new List<Field>());
            value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(value.Value, 5M);

            field.Script = @"
x = 2
y = 3
z = x + y
z + 2.0
";
            compiledScript = engine.Compile(field, new List<Field>());
            value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(value.Value, 7M);

        }
    }
}
