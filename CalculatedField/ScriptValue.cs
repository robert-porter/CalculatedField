using System;

namespace CalculatedField
{
    partial class ScriptValue
    {

        public ScriptValue()
        {
            Type = ScriptType.Null;
        }

        public ScriptValue(ScriptType type, object value)
        {
            if (value == null)
            {
                Type = ScriptType.Null;
            }
            else
            {
                Type = type;
                Value = value;
            }
        }

        public ScriptValue(decimal value)
        {
            Type = ScriptType.Decimal;
            Value = value;
        }

        public ScriptValue(long value)
        {
            Type = ScriptType.Integer;
            Value = value;
        }

        public ScriptValue(bool value)
        {
            Type = ScriptType.Bool;
            Value = value;
        }

        public ScriptValue(string value)
        {
            Type = ScriptType.String;
            Value = value;
        }

        public ScriptValue(DateTime value)
        {
            Type = ScriptType.DateTime;
            Value = value;
        }

        public object Value { get; set; }
        public ScriptType Type { get; set; }

        public decimal DecimalValue => (decimal) Value;
        public string StringValue => (string)Value;
        public DateTime DateTimeValue => (DateTime)Value;
        public long IntegerValue => (long)Value;
        public bool BoolValue => (bool)Value;
    }
}
