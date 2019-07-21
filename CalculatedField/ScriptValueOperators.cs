namespace CalculatedField
{
    partial class ScriptValue
    {
        public static ScriptValue operator -(ScriptValue right)
        {
            switch(right.Type)
            {
                case ScriptType.Decimal:
                    return new ScriptValue(-right.DecimalValue);
                case ScriptType.Integer:
                    return new ScriptValue(-right.IntegerValue);
                default:
                    return right;
            }
        }
       
        public static ScriptValue operator +(ScriptValue left, ScriptValue right)
        {
            if (left.Type == ScriptType.Integer && right.Type == ScriptType.Integer)
                return new ScriptValue(left.IntegerValue + right.IntegerValue);
            if (left.Type == ScriptType.Decimal && right.Type == ScriptType.Integer)
                return new ScriptValue(left.DecimalValue + right.IntegerValue);
            if (left.Type == ScriptType.Integer && right.Type == ScriptType.Decimal)
                return new ScriptValue(left.IntegerValue + right.DecimalValue);
            if (left.Type == ScriptType.Decimal && right.Type == ScriptType.Decimal)
                return new ScriptValue(left.DecimalValue + right.DecimalValue);
            if (left.Type == ScriptType.String && right.Type == ScriptType.String)
                return new ScriptValue(left.StringValue + right.StringValue);
            if (left.Type == ScriptType.String && right.Type == ScriptType.Null)
                return new ScriptValue(left.StringValue);
            if (left.Type == ScriptType.Null && right.Type == ScriptType.String)
                return new ScriptValue(right.StringValue);
            return new ScriptValue();
        }

        public static ScriptValue operator -(ScriptValue left, ScriptValue right)
        {
            if (left.Type == ScriptType.Integer && right.Type == ScriptType.Integer) 
                return new ScriptValue(left.IntegerValue - right.IntegerValue);
            if (left.Type == ScriptType.Decimal && right.Type == ScriptType.Integer)
                return new ScriptValue(left.DecimalValue - right.IntegerValue);
            if (left.Type == ScriptType.Integer && right.Type == ScriptType.Decimal)
                return new ScriptValue(left.IntegerValue - right.DecimalValue);
            if (left.Type == ScriptType.Decimal && right.Type == ScriptType.Decimal)
                return new ScriptValue(left.DecimalValue - right.DecimalValue);
            return new ScriptValue();
        }

        public static ScriptValue operator *(ScriptValue left, ScriptValue right)
        {
            if (left.Type == ScriptType.Integer && right.Type == ScriptType.Integer)
                return new ScriptValue(left.IntegerValue * right.IntegerValue);
            if (left.Type == ScriptType.Decimal && right.Type == ScriptType.Integer)
                return new ScriptValue(left.DecimalValue * right.IntegerValue);
            if (left.Type == ScriptType.Integer && right.Type == ScriptType.Decimal)
                return new ScriptValue(left.IntegerValue * right.DecimalValue);
            if (left.Type == ScriptType.Decimal && right.Type == ScriptType.Decimal)
                return new ScriptValue(left.DecimalValue * right.DecimalValue);
            return new ScriptValue();
        }

        public static ScriptValue operator /(ScriptValue left, ScriptValue right)
        {
            if (left.Type == ScriptType.Integer && right.Type == ScriptType.Integer) // convert integers to decimal with division
                return new ScriptValue(left.IntegerValue / (decimal) right.IntegerValue);
            if (left.Type == ScriptType.Decimal && right.Type == ScriptType.Integer)
                return new ScriptValue(left.DecimalValue / right.IntegerValue);
            if (left.Type == ScriptType.Integer && right.Type == ScriptType.Decimal)
                return new ScriptValue(left.IntegerValue / right.DecimalValue);
            if (left.Type == ScriptType.Decimal && right.Type == ScriptType.Decimal)
                return new ScriptValue(left.DecimalValue / right.DecimalValue);
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
            if (left.Type == ScriptType.Integer && right.Type == ScriptType.Integer) 
                return new ScriptValue(left.IntegerValue < right.IntegerValue);
            if (left.Type == ScriptType.Decimal && right.Type == ScriptType.Integer)
                return new ScriptValue(left.DecimalValue < right.IntegerValue);
            if (left.Type == ScriptType.Integer && right.Type == ScriptType.Decimal)
                return new ScriptValue(left.IntegerValue < right.DecimalValue);
            if (left.Type == ScriptType.Decimal && right.Type == ScriptType.Decimal)
                return new ScriptValue(left.DecimalValue < right.DecimalValue);
            return new ScriptValue();
        }

        public static ScriptValue operator >(ScriptValue left, ScriptValue right)
        {
            if (left.Type == ScriptType.Integer && right.Type == ScriptType.Integer) 
                return new ScriptValue(left.IntegerValue > right.IntegerValue);
            if (left.Type == ScriptType.Decimal && right.Type == ScriptType.Integer)
                return new ScriptValue(left.DecimalValue > right.IntegerValue);
            if (left.Type == ScriptType.Integer && right.Type == ScriptType.Decimal)
                return new ScriptValue(left.IntegerValue > right.DecimalValue);
            if (left.Type == ScriptType.Decimal && right.Type == ScriptType.Decimal)
                return new ScriptValue(left.DecimalValue > right.DecimalValue);
            return new ScriptValue();
        }

        public static ScriptValue operator <=(ScriptValue left, ScriptValue right)
        {
            if (left.Type == ScriptType.Integer && right.Type == ScriptType.Integer) 
                return new ScriptValue(left.IntegerValue <= right.IntegerValue);
            if (left.Type == ScriptType.Decimal && right.Type == ScriptType.Integer)
                return new ScriptValue(left.DecimalValue <= right.IntegerValue);
            if (left.Type == ScriptType.Integer && right.Type == ScriptType.Decimal)
                return new ScriptValue(left.IntegerValue <= right.DecimalValue);
            if (left.Type == ScriptType.Decimal && right.Type == ScriptType.Decimal)
                return new ScriptValue(left.DecimalValue <= right.DecimalValue);
            return new ScriptValue();
        }

        public static ScriptValue operator >=(ScriptValue left, ScriptValue right)
        {
            if (left.Type == ScriptType.Integer && right.Type == ScriptType.Integer)
                return new ScriptValue(left.IntegerValue >= right.IntegerValue);
            if (left.Type == ScriptType.Decimal && right.Type == ScriptType.Integer)
                return new ScriptValue(left.DecimalValue >= right.IntegerValue);
            if (left.Type == ScriptType.Integer && right.Type == ScriptType.Decimal)
                return new ScriptValue(left.IntegerValue >= right.DecimalValue);
            if (left.Type == ScriptType.Decimal && right.Type == ScriptType.Decimal)
                return new ScriptValue(left.DecimalValue >= right.DecimalValue);
            return new ScriptValue();
        }

        public static ScriptValue operator ==(ScriptValue left, ScriptValue right)
        {
            if (left.Type != right.Type) return new ScriptValue(false);
            switch(left.Type)
            {
                case ScriptType.Bool: return new ScriptValue(left.BoolValue == right.BoolValue);
                case ScriptType.DateTime: return new ScriptValue(left.DateTimeValue == right.DateTimeValue);
                case ScriptType.Decimal: return new ScriptValue(left.DecimalValue == right.DecimalValue);
                case ScriptType.String: return new ScriptValue(left.StringValue == right.StringValue);
                case ScriptType.Error: return new ScriptValue(true);
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
