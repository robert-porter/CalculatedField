namespace CalculatedField
{
    public partial class ScriptValue
    {
        public static ScriptValue operator -(ScriptValue right)
        {
            switch(right.Type)
            {
                case ScriptType.Number:
                    return new ScriptValue(-right.NumberValue);
                default:
                    return right;
            }
        }
       
        public static ScriptValue operator +(ScriptValue left, ScriptValue right)
        {
            if (left.Type == ScriptType.Number && right.Type == ScriptType.Number)
                return new ScriptValue(left.NumberValue + right.NumberValue);
            if (left.Type == ScriptType.String && right.Type == ScriptType.String)
                return new ScriptValue(left.StringValue + right.StringValue);
            if (left.Type == ScriptType.String && right.Type == ScriptType.Null)
                return new ScriptValue(left.StringValue);
            if (left.Type == ScriptType.Null && right.Type == ScriptType.String)
                return new ScriptValue(right.StringValue);
            if (left.Type == ScriptType.TimeSpan && right.Type == ScriptType.TimeSpan)
                return new ScriptValue(left.TimeSpanValue + right.TimeSpanValue);
            if (left.Type == ScriptType.DateTime && right.Type == ScriptType.TimeSpan)
                return new ScriptValue(left.DateTimeValue + right.TimeSpanValue);
            return new ScriptValue();
        }

        public static ScriptValue operator -(ScriptValue left, ScriptValue right)
        {
            if (left.Type == ScriptType.Number && right.Type == ScriptType.Number)
                return new ScriptValue(left.NumberValue - right.NumberValue);
            if (left.Type == ScriptType.DateTime && right.Type == ScriptType.DateTime)
                return new ScriptValue(left.DateTimeValue - right.DateTimeValue);
            if (left.Type == ScriptType.TimeSpan && right.Type == ScriptType.TimeSpan)
                return new ScriptValue(left.TimeSpanValue - right.TimeSpanValue);
            if (left.Type == ScriptType.DateTime && right.Type == ScriptType.TimeSpan)
                return new ScriptValue(left.DateTimeValue - right.TimeSpanValue);
            return new ScriptValue();
        }

        public static ScriptValue operator *(ScriptValue left, ScriptValue right)
        {
            if (left.Type == ScriptType.Number && right.Type == ScriptType.Number)
                return new ScriptValue(left.NumberValue * right.NumberValue);
            return new ScriptValue();
        }

        public static ScriptValue operator /(ScriptValue left, ScriptValue right)
        {
            if (left.Type == ScriptType.Number && right.Type == ScriptType.Number)
                return new ScriptValue(left.NumberValue / right.NumberValue);
            return new ScriptValue();
        }

        // divide and truncate
        public static ScriptValue operator %(ScriptValue left, ScriptValue right)
        {
            if (left.Type == ScriptType.Number && right.Type == ScriptType.Number)
                return new ScriptValue((long)left.NumberValue / (long)right.NumberValue);
            return new ScriptValue();
        }

        public static ScriptValue operator !(ScriptValue right)
        {
            if(right.Type == ScriptType.Bool)
                return new ScriptValue(!right.BoolValue);
            return new ScriptValue();
        }

        // logical and
        public static ScriptValue operator &(ScriptValue left, ScriptValue right)
        {
            if (left.Type == ScriptType.Bool && right.Type == ScriptType.Bool)
                return new ScriptValue(left.BoolValue && right.BoolValue);
            return new ScriptValue();

        }

        //logcal or
        public static ScriptValue operator |(ScriptValue left, ScriptValue right)
        {
            if (left.Type == ScriptType.Bool && right.Type == ScriptType.Bool)
                return new ScriptValue(left.BoolValue || right.BoolValue);
            return new ScriptValue();
        }

        public static ScriptValue operator <(ScriptValue left, ScriptValue right)
        {
            if (left.Type == ScriptType.Number && right.Type == ScriptType.Number)
                return new ScriptValue(left.NumberValue < right.NumberValue);
            if (left.Type == ScriptType.String && right.Type == ScriptType.String)
                return new ScriptValue(left.StringValue.CompareTo(right.StringValue) < 0);
            return new ScriptValue();
        }

        public static ScriptValue operator >(ScriptValue left, ScriptValue right)
        {
            if (left.Type == ScriptType.Number && right.Type == ScriptType.Number)
                return new ScriptValue(left.NumberValue > right.NumberValue);
            if (left.Type == ScriptType.String && right.Type == ScriptType.String)
                return new ScriptValue(left.StringValue.CompareTo(right.StringValue) > 0);
            return new ScriptValue();
        }

        public static ScriptValue operator <=(ScriptValue left, ScriptValue right)
        {
            if (left.Type == ScriptType.Number && right.Type == ScriptType.Number)
                return new ScriptValue(left.NumberValue <= right.NumberValue);
            if (left.Type == ScriptType.String && right.Type == ScriptType.String)
                return new ScriptValue(left.StringValue.CompareTo(right.StringValue) <= 0);
            return new ScriptValue();
        }

        public static ScriptValue operator >=(ScriptValue left, ScriptValue right)
        {
            if (left.Type == ScriptType.Number && right.Type == ScriptType.Number)
                return new ScriptValue(left.NumberValue >= right.NumberValue);
            if (left.Type == ScriptType.String && right.Type == ScriptType.String)
                return new ScriptValue(left.StringValue.CompareTo(right.StringValue) >= 0);
            return new ScriptValue();
        }

        public static ScriptValue operator ==(ScriptValue left, ScriptValue right)
        {
            if (left.Type != right.Type) return new ScriptValue(false);
            switch(left.Type)
            {
                case ScriptType.Bool: return new ScriptValue(left.BoolValue == right.BoolValue);
                case ScriptType.DateTime: return new ScriptValue(left.DateTimeValue == right.DateTimeValue);
                case ScriptType.Number: return new ScriptValue(left.NumberValue == right.NumberValue);
                case ScriptType.String: return new ScriptValue(left.StringValue == right.StringValue);
                case ScriptType.TimeSpan: return new ScriptValue(left.TimeSpanValue == right.TimeSpanValue);
                case ScriptType.Null: return new ScriptValue(true);
            }
            return new ScriptValue();
        }

        public static ScriptValue operator !=(ScriptValue left, ScriptValue right)
        {
            return !(left == right);
        }
    }
}
