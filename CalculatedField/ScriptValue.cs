using System;

namespace CalculatedField
{
    public partial class ScriptValue
    {
        public ScriptValue()
        {
            Type = ScriptType.Null;
            Value = null;
        }

        public ScriptValue(ScriptType type, object value)
        {
            if (value == null)
            {
                Type = ScriptType.Null;
                Value = null;
            }
            else
            {
                Type = type;
                Value = value;
            }
        }

        public ScriptValue(object value)
        {
            if (value == null)
            {
                Type = ScriptType.Null;
                Value = null;
                return;
            }

            Type = CSharpTypeToScriptType(value.GetType());

            switch (Type)
            {
                case ScriptType.Number:
                    Value = Convert.ToDecimal(value);
                    return;
                case ScriptType.String:
                    Value = Convert.ToString(value);
                    return;
                case ScriptType.Boolean:
                    Value = Convert.ToBoolean(value);
                    return;
                case ScriptType.DateTime:
                    Value = Convert.ToDateTime(value);
                    return;
                case ScriptType.TimeSpan:
                    Value = (TimeSpan) value;
                    return;
                default:
                    Type = ScriptType.Null;
                    Value = null;
                    return;
            }         
        }

        public ScriptValue(decimal value)
        {
            Type = ScriptType.Number;
            Value = value;
        }

        public ScriptValue(bool value)
        {
            Type = ScriptType.Boolean;
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

        public ScriptValue(TimeSpan value)
        {
            Type = ScriptType.TimeSpan;
            Value = value;
        }

        public static ScriptValue Parse(string stringValue, ScriptType type)
        {
            switch(type)
            {
                case ScriptType.Number:
                    return new ScriptValue(type, decimal.Parse(stringValue));
                case ScriptType.Boolean:
                    return new ScriptValue(type, bool.Parse(stringValue));
                case ScriptType.DateTime:
                    return new ScriptValue(type, DateTime.Parse(stringValue));
                case ScriptType.String:
                    return new ScriptValue(type, stringValue);
                case ScriptType.TimeSpan:
                    return new ScriptValue(type, TimeSpan.Parse(stringValue));
                default:
                    return new ScriptValue();
            }
        }

        public static ScriptType CSharpTypeToScriptType(Type type)
        {
            if (type == typeof(decimal?) | type == typeof(decimal) || type == typeof(long?) || type == typeof(long) || type == typeof(int?) || type == typeof(int))
                return ScriptType.Number;
            if (type == typeof(string))
                return ScriptType.String;
            if (type == typeof(bool?) || type == typeof(bool))
                return ScriptType.Boolean;
            if (type == typeof(DateTime?) || type == typeof(DateTime))
                return ScriptType.DateTime;
            return ScriptType.Null;
        }

        public object Value { get; set; }
        public ScriptType Type { get; set; }

        public decimal NumberValue => (decimal)Value;
        public string StringValue => (string)Value;
        public DateTime DateTimeValue => (DateTime)Value;
        public TimeSpan TimeSpanValue => (TimeSpan)Value;
        public bool BoolValue => (bool)Value;
    }
}
