using System;
using System.Collections.Generic;
using System.Reflection;

namespace CalculatedField
{
    class Resolver 
    {   
        public Type Resolve(Syntax expression, Symbols symbols)
        {
            switch (expression)
            {
                case LiteralExpression e:
                    return ResolveLiteralExpression(e, symbols);
                case BinaryExpression e:
                    return ResolveBinaryExpression(e, symbols);
                case UnaryExpression e:
                    return ResolveUnaryExpression(e, symbols);
                case FunctionExpression e:
                    return ResolveFunctionCallExpression(e, symbols);
                case FieldExpression e:
                    return ResolveFieldExpression(e, symbols);
                default:
                    return null;
            }
        }

        Type ResolveLiteralExpression(LiteralExpression  e, Symbols symbols)
        {
            return e.Type;
        }

        Type ResolveBinaryExpression(BinaryExpression e, Symbols symbols)
        {
            var left = Resolve(e.Left, symbols);
            var right = Resolve(e.Right, symbols);
            Type type = null;
            switch (e.Operator)
            {
                case BinaryOperator.Add:
                    type = ResolveAdd(e, left, right);
                    break;
                case BinaryOperator.Subtract:
                    type = CheckSubtract(left, right);
                    break;
                case BinaryOperator.Multiply:
                    type = CheckMultiply(left, right);
                    break;
                case BinaryOperator.Divide:
                    type = CheckDivide(left, right);
                    break;
                case BinaryOperator.DivideAndTruncate:
                    type = CheckDivideThenTruncate(left, right);
                    break;
                case BinaryOperator.CompareNotEqual:
                case BinaryOperator.CompareEqual:
                    type = CheckCompareEqual(left, right);
                    break;
                case BinaryOperator.Greater:
                case BinaryOperator.GreaterOrEqual:
                case BinaryOperator.Less:
                case BinaryOperator.LessOrEqual:
                    type = CheckCompareOrder(left, right);
                    break;
                case BinaryOperator.And:
                    type = CheckAndOr(left, right);
                    break;
                case BinaryOperator.Or:
                    type = CheckAndOr(left, right);
                    break;
            }
            e.Type = type;
            if (type == null)
                throw new ScriptError(e.Token.Column, e.Token.Line, $"Operator {e.Token.Contents} is not defined on {left} and {right}");
            else return type;
        }

        Type ResolveUnaryExpression(UnaryExpression e, Symbols symbols)
        {
            var right = Resolve(e.Right, symbols);
            Type type = null;
            switch (e.Operator)
            {
                case UnaryOperator.Not:
                    type = CheckNot(right);
                    break;
                case UnaryOperator.Plus:
                    type = CheckUnaryPlus(right);
                    break;
                case UnaryOperator.Minus:
                    type = CheckUnaryMinus(right);
                    break;
            }
            e.Type = type;
            if (type == null)
                throw new ScriptError(e.Token.Column, e.Token.Line, $"Operator {e.Token.Contents} is not defined on {right}");
            return type;
        }

        Type ResolveFieldExpression(FieldExpression e, Symbols symbols)
        {
            Field field = symbols.GetField(e.Name);
            symbols.AddScriptField(field);
            e.Type = field.Type;
            return field.Type;
        }

        Type ResolveFunctionCallExpression(FunctionExpression e, Symbols symbols)
        {
            var arguments = new List<Type>();
            foreach(var argument in e.Arguments)
            {
                var type = Resolve(argument, symbols);
                arguments.Add(type);
            }
            var function = symbols.GetFunction(e.Name, arguments);
            if (function == null)
            {
                throw new ScriptError(e.Token.Column, e.Token.Line, $"Function not found {e.Name}({string.Join(", ", arguments)})");
            }
            else
            {
                e.Method = function;
            }
            e.Type = function.ReturnType;
            return function.ReturnType;
        }

        public static Type CheckUnaryPlus(Type right)
        {

            if (right == typeof(decimal?) || right == typeof(decimal))
                return typeof(decimal?);
            return null;
        }

        public static Type CheckUnaryMinus(Type right)
        {

            if (right == typeof(decimal?) || right == typeof(decimal))
                return typeof(decimal?);
            return null;
        }

        public static Type ResolveAdd(BinaryExpression e, Type left, Type right)
        {
            Type type = null;
            if (left == typeof(decimal?) || left == typeof(decimal) &&
                right == typeof(decimal?) || right == typeof(decimal))
                return typeof(decimal?);
            if (left == typeof(DateTime?) || left == typeof(DateTime) &&
                right == typeof(TimeSpan?) || right == typeof(TimeSpan))
                return typeof(DateTime?);
            if (left == typeof(TimeSpan?) || left == typeof(TimeSpan) &&
                right == typeof(TimeSpan?) || right == typeof(TimeSpan))
                return typeof(TimeSpan?);
            if (left == typeof(string) && right == typeof(string))
            {
                var method = typeof(LibString).GetMethod("concat");
                e.Method = method;
                return typeof(string);
            }
            else
            {
                /*
                var methods = left.GetMethods();
                var addition = Array.Find(methods, method =>
                {
                    var parameters = method.GetParameters();
                    if (method.Name == "op_Addition" && parameters.Length == 2 &&
                        parameters[0].ParameterType == left && parameters[1].ParameterType == right)
                        return true;
                    return false;
                });
                if (addition != null)
                {
                    //e.Method = addition;
                    return addition.ReturnType;
                }
                methods = right.GetMethods();
                addition = Array.Find(methods, method =>
                {
                    var parameters = method.GetParameters();
                    if (method.Name == "op_Addition" && parameters.Length == 2 &&
                        parameters[0].ParameterType == left && parameters[1].ParameterType == right)
                        return true;
                    return false;
                });
                if (addition != null)
                {
                    //e.Method = addition;
                    return addition.ReturnType;
                } */
            }
            return type;
        }

        public static Type CheckSubtract(Type left, Type right)
        {

            if (left == typeof(decimal?) || left == typeof(decimal) &&
                right == typeof(decimal?) || right == typeof(decimal))
                return typeof(decimal?);
            if (left == typeof(TimeSpan?) || left == typeof(TimeSpan) &&
                right == typeof(TimeSpan?) || right == typeof(TimeSpan))
                return typeof(TimeSpan?);
            if (left == typeof(DateTime?) || left == typeof(DateTime) &&
                right == typeof(DateTime?) || right == typeof(DateTime))
                return typeof(TimeSpan?);
            return null;
        }

        public static Type CheckMultiply(Type left, Type right)
        {

            if (left == typeof(decimal?) || left == typeof(decimal) &&
                right == typeof(decimal?) || right == typeof(decimal))
                return typeof(decimal?);
            return null;
        }

        public static Type CheckDivide(Type left, Type right)
        {

            if (left == typeof(decimal?) || left == typeof(decimal) &&
                right == typeof(decimal?) || right == typeof(decimal))
                return typeof(decimal?);

            return null;
        }

        public static Type CheckDivideThenTruncate(Type left, Type right)
        {

            if (left == typeof(decimal?) || left == typeof(decimal) &&
                right == typeof(decimal?) || right == typeof(decimal))
                return typeof(decimal?);
            return null;
        }

        public static Type CheckNot(Type right)
        {
            if (right == typeof(bool?) || right == typeof(bool))
                return typeof(bool?);
            return null;
        }

        public static Type CheckAndOr(Type left, Type right)
        {
            if (left == typeof(bool?) || left == typeof(bool) && right == typeof(bool?) || right == typeof(bool))
                return typeof(bool?);
            return null;
        }

        public static Type CheckCompareOrder(Type left, Type right)
        {
            if (left == typeof(decimal?) || left == typeof(decimal) &&
                right == typeof(decimal?) || right == typeof(decimal))
                return typeof(bool?);
            if (left == typeof(string) && right == typeof(string))
                return typeof(bool?);
            if (left == typeof(TimeSpan?) || left == typeof(TimeSpan) &&
                right == typeof(TimeSpan?) || right == typeof(TimeSpan))
                return typeof(bool?);
            if (left == typeof(DateTime?) || left == typeof(DateTime) &&
                right == typeof(DateTime?) || right == typeof(DateTime))
                return typeof(bool?);
            return null;
        }

        public static Type CheckCompareEqual(Type left, Type right)
        {
            if (left == typeof(bool?) && right == typeof(bool?))
                return typeof(bool?);
            if (left == typeof(decimal?) || left == typeof(decimal) &&
                right == typeof(decimal?) || right == typeof(decimal))
                return typeof(bool?);
            if (left == typeof(string) && right == typeof(string))
                return typeof(bool?);
            if (left == typeof(TimeSpan?) || left == typeof(TimeSpan) &&
                right == typeof(TimeSpan?) || right == typeof(TimeSpan))
                return typeof(bool?);
            if (left == typeof(DateTime?) || left == typeof(DateTime) &&
                right == typeof(DateTime?) || right == typeof(DateTime))
                return typeof(bool?);
            return null;
        }

        public MethodInfo GetBinaryOperatorMethod(Type left, Type right)
        {
            return null;
            /*
== op_Equality
!= op_Inequality
>  op_GreaterThan
<  op_LessThan
>= op_GreaterThanOrEqual
<= op_LessThanOrEqual
+  op_Addition
-  op_Subtraction
/  op_Division
%  op_Modulus
*  op_Multiply
-  op_UnaryNegation
+  op_UnaryPlus
!  op_LogicalNot

            */
        }

    }
}
