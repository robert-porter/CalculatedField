using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CalculatedField
{


    class LambdaGenerator
    {
        ParameterExpression Record;
        List<Field> EntityFields;

        public Func<Dictionary<string, object>, object> GenerateProgram(Syntax script, List<Field> entityFields)
        {
            EntityFields = entityFields;
            Record = Expression.Parameter(typeof(Dictionary<string, object>));
            var expression = Generate(script);
            expression = Expression.Convert(expression, typeof(object));
            var parameters = new ParameterExpression[] { Record };
            var lambda = Expression.Lambda<Func<Dictionary<string, object>, object>>(expression, parameters).Compile();
            return (records) => lambda(CopyRecord(records));
        }

        static Dictionary<string, object> CopyRecord(Dictionary<string, object> records)
        {
            var results = new Dictionary<string, object>();
            foreach(var record in records)
            {
                foreach (var kvp in records)
                {
                    var type = kvp.Value.GetType();
                    if (type == typeof(int?))
                    {
                        results[kvp.Key] = (decimal?)(int?)kvp.Value;
                    }
                    else if (type == typeof(int))
                    {
                        results[kvp.Key] = (decimal?)(int)kvp.Value;
                    }
                    else if (type == typeof(long?))
                    {
                        results[kvp.Key] = (decimal?)(long?)kvp.Value;
                    }
                    else if (type == typeof(long))
                    {
                        results[kvp.Key] = (decimal?)(long?)kvp.Value;
                    }
                    else if (type == typeof(decimal))
                    {
                        results[kvp.Key] = (decimal?)(decimal)kvp.Value;
                    }
                    else
                    {
                        results[kvp.Key] = kvp.Value;
                    }
                }
            }
            return results;
        }

        Expression Generate(Syntax expression)
        {
            switch (expression)
            {
                case LiteralExpression e:
                    return GenerateLiteralExpression(e);
                case BinaryExpression e:
                    return GenerateBinaryExpression(e);
                case UnaryExpression e:
                    return GenerateUnaryExpression(e);
                case FunctionExpression e:
                    return GenerateFunctionCallExpression(e);
                case FieldExpression e:
                    return GenerateFieldExpression(e);
                case IdentifierExpression e:
                    return GenerateConstantExpression(e);
                default:
                    return null;
            }
        }

        public Expression GenerateUnaryExpression(UnaryExpression expression)
        {
            var right = Generate(expression.Right);

            switch (expression.Token.Type)
            {
                case TokenType.Plus: // do nothing
                    return right; //return Expression.UnaryPlus(right);
                case TokenType.Minus:
                    return Expression.Negate(right);
                case TokenType.Not:
                    return Expression.Not(right);
                default:
                    return null;
            }
        }
       
        public Expression GenerateBinaryExpression(BinaryExpression expression)
        {
            var left = Generate(expression.Left);
            var right = Generate(expression.Right);

            switch (expression.Token.Type)
            {
                case TokenType.Plus:
                    if (expression.Left.Type == ScriptType.String && expression.Right.Type == ScriptType.String)
                        return GenerateStringAddExpression(left, right);
                    else
                        return Expression.Add(left, right);
                case TokenType.Minus:
                    return Expression.Subtract(left, right);
                case TokenType.Multiply:
                    return Expression.Multiply(left, right);
                case TokenType.Divide:
                    return Expression.Divide(left, right);
                case TokenType.LessThen:
                    if(expression.Left.Type == ScriptType.String && expression.Right.Type == ScriptType.String)
                        return GenerateStringLessExpression(left, right);
                    else
                        return Expression.LessThan(left, right);
                case TokenType.LessThenOrEqual:
                    if (expression.Left.Type == ScriptType.String && expression.Right.Type == ScriptType.String)
                        return GenerateStringLessOrEqualExpression(left, right);
                        else
                        return Expression.LessThanOrEqual(left, right);                   
                case TokenType.GreaterThen:
                    if (expression.Left.Type == ScriptType.String && expression.Right.Type == ScriptType.String)
                        return GenerateStringGreaterExpression(left, right);
                    else
                        return Expression.GreaterThan(left, right);
                case TokenType.GreaterThenOrEqual:
                    if (expression.Left.Type == ScriptType.String && expression.Right.Type == ScriptType.String)
                        return GenerateStringGreaterOrEqualExpression(left, right);
                    else
                        return Expression.GreaterThanOrEqual(left, right);
                case TokenType.Equal:
                    return Expression.Equal(left, right);
                case TokenType.NotEqual:
                    return Expression.NotEqual(left, right);
                case TokenType.And:
                    return Expression.And(left, right);
                case TokenType.Or:
                    return Expression.Or(left, right);
                default:
                    return null;
            }
        }


        Expression GenerateStringLessExpression(Expression left, Expression right)
        {
            var methodInfo = typeof(string).GetMethod("Compare", new Type[] { typeof(string), typeof(string)});
            return Expression.LessThan(Expression.Call(null, methodInfo, new Expression[] { left, right }), Expression.Constant(0));
        }

        Expression GenerateStringLessOrEqualExpression(Expression left, Expression right)
        {
            var methodInfo = typeof(string).GetMethod("Compare", new Type[] { typeof(string), typeof(string) });
            return Expression.LessThanOrEqual(Expression.Call(null, methodInfo, new Expression[] { left, right }), Expression.Constant(0));
        }

        Expression GenerateStringGreaterExpression(Expression left, Expression right)
        {
            var methodInfo = typeof(string).GetMethod("Compare", new Type[] { typeof(string), typeof(string) });
            return Expression.GreaterThan(Expression.Call(null, methodInfo, new Expression[] { left, right }), Expression.Constant(0));
        }

        Expression GenerateStringGreaterOrEqualExpression(Expression left, Expression right)
        {
            var methodInfo = typeof(string).GetMethod("Compare", new Type[] { typeof(string), typeof(string) });
            return Expression.GreaterThanOrEqual(Expression.Call(null, methodInfo, new Expression[] { left, right }), Expression.Constant(0));
        }

        Expression GenerateStringAddExpression(Expression left, Expression right)
        {
            var methodInfo = typeof(Lib).GetMethod("concat");
            return Expression.Call(null, methodInfo, new Expression[] { left, right });
        }

        public Expression GenerateLiteralExpression(LiteralExpression expression)
        {
            var constant = Expression.Constant(expression.Value);
            if (expression.Type == ScriptType.Null)
            {
                return constant;
            }
            else
            {
                return Expression.Convert(constant, TypeHelper.ScriptToSystemType(expression.Type));
            }
        }

        public Expression GenerateConstantExpression(IdentifierExpression expression)
        {
            var property = Expression.Property(null, expression.Property);
            return Expression.Convert(property, TypeHelper.ScriptToSystemType(expression.Type));
        }

        Type GetNullableType(Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type; 
            if (type.IsValueType)
                return typeof(Nullable<>).MakeGenericType(type);
            else
                return type;
        }


        public Expression GenerateFieldExpression(FieldExpression expression)
        {
            // record.ContainsKey(FieldId) ? record[FieldId] : null
            var field = EntityFields.Find(f => f.Name == expression.Name);
            var scriptType = TypeHelper.ScriptToSystemType(TypeHelper.SystemToScriptType(field.Type));
            var keyConstant = Expression.Constant(field.FieldId);
            var access = Expression.Property(Record, "Item", keyConstant);
            return Expression.Convert(access, scriptType);
            //var containsKeyMethod = typeof(Dictionary<string, object>).GetMethod("ContainsKey");
            //var containsKey = Expression.Call(Record, containsKeyMethod, new Expression[] { keyConstant });
            //var condition = Expression.Condition(containsKey, access, Expression.Convert(Expression.Constant(null), type));

            //return Expression.Convert(condition, type);
            return Expression.Convert(access, scriptType);
        }

        public Expression GenerateFunctionCallExpression(FunctionExpression call)
        {
            var arguments = new List<Expression>();
            foreach (Syntax argument in call.Arguments)
            {
                arguments.Add(Generate(argument));
            }
            if (call.Name == "cases" || call.Name == "ifs")
            {
                arguments = arguments.Select(arg => (Expression) Expression.Convert(arg, typeof(object))).ToList();
                var argument = Expression.NewArrayInit(typeof(object), arguments);
                return Expression.Call(null, call.Function.Method, argument);
            }
            else
            {
                return Expression.Call(null, call.Function.Method, arguments);
            }
        }
    }
}
