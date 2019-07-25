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
                //TODO: assert value's type
                Type = type;
                Value = value;
            }
        }

        public ScriptValue(object value)
        {
            var type = value.GetType();
            if (value == null)
            {
                Type = ScriptType.Null;
                Value = null;
            }
            else if (type == typeof(long?) || type == typeof(long) || type == typeof(int?) || type == typeof(int))
            {
                Type = ScriptType.Integer;
                Value = Convert.ToInt64(value);
            }
            else if (type == typeof(decimal?) | type == typeof(decimal))
            {
                Type = ScriptType.Decimal;
                Value = Convert.ToDecimal(value);
            }
            else if (type == typeof(string))
            {
                Type = ScriptType.String;
                Value = Convert.ToString(value);
            }
            else if (type == typeof(bool?) || type == typeof(bool))
            {
                Type = ScriptType.Bool;
                Value = Convert.ToBoolean(value);
            }
            else if (type == typeof(DateTime?) || type == typeof(DateTime))
            {
                Type = ScriptType.DateTime;
                value = Convert.ToDateTime(value);
            }
            else
            {
                Type = ScriptType.Null;
                value = null;
            }

            if(value == null)
            {
                Type = ScriptType.Null;
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
            Value = (long) value;
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

        public ScriptValue(TimeSpan value)
        {
            Type = ScriptType.TimeSpan;
            Value = value;
        }

        public static ScriptValue Parse(string stringValue, ScriptType type)
        {
            switch(type)
            {
                case ScriptType.Decimal:
                    return new ScriptValue(type, decimal.Parse(stringValue));
                case ScriptType.Integer:
                    return new ScriptValue(type, long.Parse(stringValue));
                case ScriptType.Bool:
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
            if (type == typeof(long?) || type == typeof(long) || type == typeof(int?) || type == typeof(int))
                return ScriptType.Integer;
            if (type == typeof(decimal?) | type == typeof(decimal))
                return ScriptType.Decimal;
            if (type == typeof(string))
                return ScriptType.String;
            if (type == typeof(bool?) || type == typeof(bool))
                return ScriptType.Bool;
            if (type == typeof(DateTime?) || type == typeof(DateTime))
                return ScriptType.DateTime;
            return ScriptType.Null;
        }

        public object Value { get; set; }
        public ScriptType Type { get; set; }

        public decimal DecimalValue => (decimal)Value;
        public string StringValue => (string)Value;
        public DateTime DateTimeValue => (DateTime)Value;
        public TimeSpan TimeSpanValue => (TimeSpan)Value;
        public long IntegerValue => (long)Value;
        public bool BoolValue => (bool)Value;
    }
}
