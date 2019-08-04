using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculatedField
{

    public static class TypeHelper
    {

        public static bool IsNumberType(Type toCheck) =>
             toCheck == typeof(decimal?) ||
             toCheck == typeof(decimal) ||
             toCheck == typeof(double?) ||
             toCheck == typeof(double?) ||
             toCheck == typeof(float?) ||
             toCheck == typeof(float?) ||
             toCheck == typeof(long?) ||
             toCheck == typeof(long) ||
             toCheck == typeof(int?) ||
             toCheck == typeof(int);

        public static bool IsStringType(Type toCheck) => toCheck == typeof(string);
        public static bool IsBoolType(Type toCheck) => toCheck == typeof(bool) || toCheck == typeof(bool?);
        public static bool IsDateTimeType(Type toCheck) => toCheck == typeof(DateTime) || toCheck == typeof(DateTime?);
        public static bool IsTimeSpanType(Type toCheck) => toCheck == typeof(TimeSpan) || toCheck == typeof(TimeSpan?);

        public static ScriptType SystemToScriptType(Type type)
        {
            if (IsNumberType(type))
                return ScriptType.Number;
            if (IsStringType(type))
                return ScriptType.String;
            if (IsBoolType(type))
                return ScriptType.Boolean;
            if (IsDateTimeType(type))
                return ScriptType.DateTime;
            if (IsTimeSpanType(type))
                return ScriptType.TimeSpan;
            return default;
        }

        public static Type ScriptToSystemType(ScriptType type)
        {
            if (type == ScriptType.Number)
                return typeof(decimal?);
            if (type == ScriptType.String)
                return typeof(string);
            if (type == ScriptType.Boolean)
                return typeof(bool?);
            if (type == ScriptType.DateTime)
                return typeof(DateTime?);
            if (type == ScriptType.TimeSpan)
                return typeof(TimeSpan?);
            else return null;
        }
    }



}
