using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculatedField
{
    static class TypeChecker
    {
        public static bool CheckAddition(ScriptType left, ScriptType right, out ScriptType scriptType)
        {
            scriptType = ScriptType.Null;
            if (left == ScriptType.Number && right == ScriptType.Number) scriptType = ScriptType.Number;
            if (left == ScriptType.DateTime && right == ScriptType.TimeSpan) scriptType = ScriptType.DateTime;
            if (left == ScriptType.TimeSpan && right == ScriptType.DateTime) scriptType = ScriptType.DateTime;
            if (left == ScriptType.TimeSpan && right == ScriptType.TimeSpan) scriptType = ScriptType.TimeSpan;
            if (left == ScriptType.String && right == ScriptType.String) scriptType = ScriptType.String;
            return scriptType != ScriptType.Null;
        }

        public static bool CheckSubtraction(ScriptType left, ScriptType right, out ScriptType scriptType)
        {
            scriptType = ScriptType.Null;
            if (left == ScriptType.Number && right == ScriptType.Number) scriptType = ScriptType.Number;
            if (left == ScriptType.TimeSpan && right == ScriptType.TimeSpan) scriptType = ScriptType.TimeSpan;
            if (left == ScriptType.DateTime && right == ScriptType.DateTime) scriptType = ScriptType.TimeSpan;
            return scriptType != ScriptType.Null;
        }

        public static bool CheckMultiplication(ScriptType left, ScriptType right, out ScriptType scriptType)
        {
            scriptType = ScriptType.Null;
            if (left == ScriptType.Number && right == ScriptType.Number) scriptType = ScriptType.Number;
            if (left == ScriptType.TimeSpan && right == ScriptType.Number) scriptType = ScriptType.TimeSpan;
            if (left == ScriptType.Number && right == ScriptType.TimeSpan) scriptType = ScriptType.TimeSpan;
            return scriptType != ScriptType.Null;
        }

        public static bool CheckDivision(ScriptType left, ScriptType right, out ScriptType scriptType)
        {
            scriptType = ScriptType.Null;
            if (left == ScriptType.Number && right == ScriptType.Number) scriptType = ScriptType.Number;
            if (left == ScriptType.TimeSpan && right == ScriptType.Number) scriptType = ScriptType.TimeSpan;
            return scriptType != ScriptType.Null;
        }
        public static bool CheckDivisionAndTruncate(ScriptType left, ScriptType right, out ScriptType scriptType)
        {
            scriptType = ScriptType.Null;
            if (left == ScriptType.Number && right == ScriptType.Number) scriptType = ScriptType.Number;
            return scriptType != ScriptType.Null;
        }

        public static bool CheckCompareEqual(ScriptType left, ScriptType right, out ScriptType scriptType)
        {
            scriptType = ScriptType.Bool;
            if (left != right)
            {
                scriptType = ScriptType.Null;
                return false;
            }
            return true;
        }

        public static bool CheckCompareOrder(ScriptType left, ScriptType right, out ScriptType scriptType)
        {
            scriptType = ScriptType.Null;
            if (left == ScriptType.Number && right == ScriptType.Number) scriptType = ScriptType.Bool;
            if (left == ScriptType.String && right == ScriptType.String) scriptType = ScriptType.Bool;
            if (left == ScriptType.TimeSpan && right == ScriptType.TimeSpan) scriptType = ScriptType.Bool;
            if (left == ScriptType.DateTime && right == ScriptType.DateTime) scriptType = ScriptType.Bool;
            return scriptType != ScriptType.Null;
        }

        public static bool CheckAndOr(ScriptType left, ScriptType right, out ScriptType scriptType)
        {
            if (left == ScriptType.Bool && right == ScriptType.Bool)
            {
                scriptType = ScriptType.Bool;
                return true;
            }
            else
            {
                scriptType = ScriptType.Null;
                return false;
            }
        }

        public static bool CheckNot(ScriptType right, out ScriptType scriptType)
        {
            if (right == ScriptType.Bool)
            {
                scriptType = ScriptType.Bool;
                return true;
            }
            else
            {
                scriptType = ScriptType.Null;
                return false;
            }
        }

        public static bool CheckPlusMinus(ScriptType right, out ScriptType scriptType)
        {

            if (right == ScriptType.Number)
            {
                scriptType = right;
                return true;
            }
            else
            {
                scriptType = ScriptType.Null;
                return false;
            }
        }

        public static bool CheckIfBranches(ScriptType thenType, ScriptType elseType, out ScriptType scriptType)
        {
            scriptType = ScriptType.Null;
            if (thenType == elseType) scriptType = thenType;
            return scriptType != ScriptType.Null;
        }

        public static bool CheckAssignment(ScriptType variableType, ScriptType rightType, out ScriptType scriptType)
        {
            scriptType = ScriptType.Null;
            if (variableType == rightType) scriptType = variableType;
            return scriptType != ScriptType.Null;
        }

    }

}
