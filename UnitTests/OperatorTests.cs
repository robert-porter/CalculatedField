using System;
using System.Collections.Generic;
using CalculatedField;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class OperatorTests
    {
        [TestMethod]
        public void TestArithmeticOperators()
        {
            var engine = new Engine();
            var field = new Field();
            field.Type = ScriptType.Integer;
            field.Script = "2 + 2";
            var compiledScript = engine.Compile(field, new List<Field>());
            var value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(value.Value, 4L);

            field.Script = "2 - 2";
            compiledScript = engine.Compile(field, new List<Field>());
            value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(value.Value, 0L);

            field.Script = "2 * 2";
            compiledScript = engine.Compile(field, new List<Field>());
            value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(value.Value, 4L);

            field.Script = "1 / 2";
            field.Type = ScriptType.Decimal;
            compiledScript = engine.Compile(field, new List<Field>());
            value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(value.Value, 0.5m);

            field.Script = "2.0 + 2.0";
            compiledScript = engine.Compile(field, new List<Field>());
            value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(value.Value, 4.0m);

            field.Script = "2.0 - 2.0";
            compiledScript = engine.Compile(field, new List<Field>());
            value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(value.Value, 0m);

            field.Script = "2.0 * 2.0";
            compiledScript = engine.Compile(field, new List<Field>());
            value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(value.Value, 4m);

            field.Script = "1.0 / 2.0";
            field.Type = ScriptType.Decimal;
            compiledScript = engine.Compile(field, new List<Field>());
            value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(value.Value, 0.5m);

            field.Script = "2 + 2.0";
            compiledScript = engine.Compile(field, new List<Field>());
            value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(value.Value, 4.0m);

            field.Script = "2 - 2.0";
            compiledScript = engine.Compile(field, new List<Field>());
            value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(value.Value, 0m);

            field.Script = "2 * 2.0";
            compiledScript = engine.Compile(field, new List<Field>());
            value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(value.Value, 4m);

            field.Script = "1 / 2.0";
            field.Type = ScriptType.Decimal;
            compiledScript = engine.Compile(field, new List<Field>());
            value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(value.Value, 0.5m);

            field.Script = "2.0 + 2";
            compiledScript = engine.Compile(field, new List<Field>());
            value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(value.Value, 4.0m);

            field.Script = "2.0 - 2";
            compiledScript = engine.Compile(field, new List<Field>());
            value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(value.Value, 0m);

            field.Script = "2.0 * 2";
            compiledScript = engine.Compile(field, new List<Field>());
            value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(value.Value, 4m);

            field.Script = "1.0 / 2";
            field.Type = ScriptType.Decimal;
            compiledScript = engine.Compile(field, new List<Field>());
            value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(value.Value, 0.5m);
        }

        [TestMethod]
        public void TestStringOperators()
        {
            var engine = new Engine();
            var field = new Field();
            field.Type = ScriptType.String;
            field.Script = "\"ab\" + \"cd\"";
            var compiledScript = engine.Compile(field, new List<Field>());
            var value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(value.Value, "abcd");

            field.Script = "'ab' + 'cd'";
            compiledScript = engine.Compile(field, new List<Field>());
            value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(value.Value, "abcd");
           
        }

        [TestMethod]
        public void TestComparisonOperators()
        {
            var engine = new Engine();
            var field = new Field();
            field.Type = ScriptType.Bool;
            field.Script = "\"ab\" < \"cd\"";
            var compiledScript = engine.Compile(field, new List<Field>());
            var value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(value.Value, true);

            field.Script = "\"ab\" > \"cd\"";
            compiledScript = engine.Compile(field, new List<Field>());
            value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(value.Value, false);

            field.Script = "\"ab\" > \"ab\"";
            compiledScript = engine.Compile(field, new List<Field>());
            value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(value.Value, false);

            field.Script = "\"ab\" >= \"cd\"";
            compiledScript = engine.Compile(field, new List<Field>());
            value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(value.Value, false);

            field.Script = "\"ab\" <= \"cd\"";
            compiledScript = engine.Compile(field, new List<Field>());
            value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(value.Value, true);

            field.Script = "\"ab\" <= \"ab\"";
            compiledScript = engine.Compile(field, new List<Field>());
            value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(value.Value, true);

            field.Script = "\"ab\" == \"cd\"";
            compiledScript = engine.Compile(field, new List<Field>());
            value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(value.Value, false);

            field.Script = "\"ab\" == \"ab\"";
            compiledScript = engine.Compile(field, new List<Field>());
            value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(value.Value, true);

            field.Script = "\"ab\" <> \"ab\"";
            compiledScript = engine.Compile(field, new List<Field>());
            value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(value.Value, false);

            field.Script = "\"ab\" <> \"cd\"";
            compiledScript = engine.Compile(field, new List<Field>());
            value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(value.Value, true);
        }

        [TestMethod]
        public void TestBoolOperators()
        {
            var engine = new Engine();
            var field = new Field();
            field.Type = ScriptType.Bool;
            field.Script = "true and true";
            var compiledScript = engine.Compile(field, new List<Field>());
            var value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(value.Value, true);

            field.Script = "true and false";
            compiledScript = engine.Compile(field, new List<Field>());
            value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(value.Value, false);

            field.Script = "false and true";
            compiledScript = engine.Compile(field, new List<Field>());
            value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(value.Value, false);

            field.Script = "false and false";
            compiledScript = engine.Compile(field, new List<Field>());
            value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(value.Value, false);

            field.Script = "true or true";
            compiledScript = engine.Compile(field, new List<Field>());
            value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(value.Value, true);

            field.Script = "true or false";
            compiledScript = engine.Compile(field, new List<Field>());
            value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(value.Value, true);

            field.Script = "false or true";
            compiledScript = engine.Compile(field, new List<Field>());
            value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(value.Value, true);

            field.Script = "false or false";
            compiledScript = engine.Compile(field, new List<Field>());
            value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(value.Value, false);

            field.Script = "not false";
            compiledScript = engine.Compile(field, new List<Field>());
            value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(value.Value, true);

            field.Script = "not true";
            compiledScript = engine.Compile(field, new List<Field>());
            value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(value.Value, false);

        }
    }
}
