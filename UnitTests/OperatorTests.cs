using System;
using System.Collections.Generic;
using CalculatedField;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class OperatorTests
    {
        Engine engine = new Engine();


        [TestMethod]
        public void TestIntegerAddtion()
        {
            var engine = new Engine();
            var value = engine.CalculateValue("2 + 2");
            Assert.AreEqual(4m, value.Value);
        }

        [TestMethod]
        public void TestIntegerSubtraction()
        {
            var value = engine.CalculateValue("2 - 2");
            Assert.AreEqual(0m, value.Value);
        }

        [TestMethod]
        public void TestIntegerMultiplication()
        {
            var engine = new Engine();
            var value = engine.CalculateValue("2 * 2");
            Assert.AreEqual(4m, value.Value);
        }

        [TestMethod]
        public void TestIntegerDivision()
        {
            var engine = new Engine();
            var value = engine.CalculateValue("1 / 2");
            Assert.AreEqual(0.5m, value.Value);
        }

        [TestMethod]
        public void TestDecimalAddtion()
        {
            var engine = new Engine();
            var value = engine.CalculateValue("2.0 + 2.0");
            Assert.AreEqual(4m, value.Value);
        }

        [TestMethod]
        public void TestDecimalSubtraction()
        {
            var value = engine.CalculateValue("2.0 - 2.0");
            Assert.AreEqual(0m, value.Value);
        }

        [TestMethod]
        public void TestDecimalMultiplication()
        {
            var engine = new Engine();
            var value = engine.CalculateValue("2.0 * 2.0");
            Assert.AreEqual(4m, value.Value);
        }

        [TestMethod]
        public void TestDecimalDivision()
        {
            var engine = new Engine();
            var value = engine.CalculateValue("1.0 / 2.0");
            Assert.AreEqual(0.5m, value.Value);
        }
      
        [TestMethod]
        public void TestStringAddition()
        {
            var value = engine.CalculateValue("\"ab\" + \"cd\"");
            Assert.AreEqual("abcd", value.Value);

            value = engine.CalculateValue("'ab' + 'cd'");
            Assert.AreEqual("abcd", value.Value);       
        }

        [TestMethod]
        public void TestStringLess()
        {
            var value = engine.CalculateValue("\"ab\" < \"cd\"");
            Assert.AreEqual(true, value.Value);

            value = engine.CalculateValue("\"cd\" < \"ab\"");
            Assert.AreEqual(false, value.Value);

            value = engine.CalculateValue("\"ab\" < \"ab\"");
            Assert.AreEqual(false, value.Value);
        }

        [TestMethod]
        public void TestStringLessOrEqual()
        {
            var value = engine.CalculateValue("\"ab\" <= \"cd\"");
            Assert.AreEqual(true, value.Value);

            value = engine.CalculateValue("\"cd\" <= \"ab\"");
            Assert.AreEqual(false, value.Value);

            value = engine.CalculateValue("\"ab\" <= \"ab\"");
            Assert.AreEqual(true, value.Value);
        }

        [TestMethod]
        public void TestStringGreater()
        {
            var value = engine.CalculateValue("\"ab\" > \"cd\"");
            Assert.AreEqual(false, value.Value);

            value = engine.CalculateValue("\"ab\" > \"ab\"");
            Assert.AreEqual(false, value.Value);
        }

        [TestMethod]
        public void TestStringGreaterOrEqual()
        {
            var value = engine.CalculateValue("\"ab\" >= \"cd\"");
            Assert.AreEqual(false, value.Value);
        }

        [TestMethod]
        public void TestStringEqual()
        {
            var value = engine.CalculateValue("\"ab\" == \"cd\"");
            Assert.AreEqual(false, value.Value);

            value = engine.CalculateValue("\"ab\" == \"ab\"");
            Assert.AreEqual(true, value.Value);
        }

        [TestMethod]
        public void TestStringNotEqual()
        {
            var value = engine.CalculateValue("\"ab\" <> \"ab\"");
            Assert.AreEqual(false, value.Value);

            value = engine.CalculateValue("\"ab\" <> \"cd\"");
            Assert.AreEqual(true, value.Value);
        }

        [TestMethod]
        public void TestDateSubtraction()
        {
            var value = engine.CalculateValue("#1/1/2000# - #11/1/200#");
            var expected = DateTime.Parse("1/1/2000") - DateTime.Parse("11/1/200");            
            Assert.AreEqual(expected, value.Value);
        }

        [TestMethod]
        public void TestBoolOperators()
        {
            var engine = new Engine();
            var field = new Field();
            field.Type = ScriptType.Boolean;
            field.Script = "true and true";
            var compiledScript = engine.Compile(field, new List<Field>());
            var value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(true, value.Value);

            field.Script = "true and false";
            compiledScript = engine.Compile(field, new List<Field>());
            value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(false, value.Value);

            field.Script = "false and true";
            compiledScript = engine.Compile(field, new List<Field>());
            value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(false, value.Value);

            field.Script = "false and false";
            compiledScript = engine.Compile(field, new List<Field>());
            value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(false, value.Value);

            field.Script = "true or true";
            compiledScript = engine.Compile(field, new List<Field>());
            value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(true, value.Value);

            field.Script = "true or false";
            compiledScript = engine.Compile(field, new List<Field>());
            value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(true, value.Value);

            field.Script = "false or true";
            compiledScript = engine.Compile(field, new List<Field>());
            value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(true, value.Value);

            field.Script = "false or false";
            compiledScript = engine.Compile(field, new List<Field>());
            value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(false, value.Value);

            field.Script = "not false";
            compiledScript = engine.Compile(field, new List<Field>());
            value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(true, value.Value);

            field.Script = "not true";
            compiledScript = engine.Compile(field, new List<Field>());
            value = engine.CalculateValue(compiledScript, new Dictionary<Guid, object>());
            Assert.AreEqual(false, value.Value);

        }
    }
}
