using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CalculatedField
{
    class CSharpCodeGenerator
    {
        ParameterExpression Parameter;

        public Func<Dictionary<Guid, object>, object> GenerateProgram(Syntax script, Symbols symbols)
        {
            Parameter = Expression.Parameter(typeof(Dictionary<Guid, object>));
            var expression = Generate(script, symbols);
            expression = Expression.Convert(expression, typeof(object));
            var parameters = new ParameterExpression[] { Parameter };
            return Expression.Lambda<Func<Dictionary<Guid, object>, object>>(expression, parameters).Compile();
        }

        Expression Generate(Syntax expression, Symbols symbols)
        {
            switch (expression)
            {
                case LiteralExpression e:
                    return GenerateLiteralExpression(e, symbols);
                case BinaryExpression e:
                    return GenerateBinaryExpression(e, symbols);
                case UnaryExpression e:
                    return GenerateUnaryExpression(e, symbols);
                case FunctionExpression e:
                    return GenerateFunctionCallExpression(e, symbols);
                case FieldExpression e:
                    return GenerateFieldExpression(e, symbols);
                case IdentifierExpression e:
                    return GenerateConstantExpression(e, symbols);
                default:
                    return null;
            }
        }

        public Expression GenerateUnaryExpression(UnaryExpression expression, Symbols symbols)
        {
            var right = Generate(expression.Right, symbols);

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

        public Expression GenerateBinaryExpression(BinaryExpression expression, Symbols symbols)
        {
            var left = Generate(expression.Left, symbols);
            var right = Generate(expression.Right, symbols);

            switch (expression.Token.Type)
            {
                case TokenType.Plus:
                    if (expression.Left.Type == typeof(string) && expression.Right.Type == typeof(string))
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
                    if(expression.Left.Type == typeof(string) && expression.Right.Type == typeof(string))
                        return GenerateStringLessExpression(left, right);
                    else
                        return Expression.LessThan(left, right);
                case TokenType.LessThanOrEqual:
                    if (expression.Left.Type == typeof(string) && expression.Right.Type == typeof(string))
                        return GenerateStringLessOrEqualExpression(left, right);
                        else
                        return Expression.LessThanOrEqual(left, right);                   
                case TokenType.GreaterThan:
                    if (expression.Left.Type == typeof(string) && expression.Right.Type == typeof(string))
                        return GenerateStringGreaterExpression(left, right);
                    else
                        return Expression.GreaterThan(left, right);
                case TokenType.GreaterThenOrEqual:
                    if (expression.Left.Type == typeof(string) && expression.Right.Type == typeof(string))
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
            var methodInfo = typeof(LibString).GetMethod("concat");
            return Expression.Call(null, methodInfo, new Expression[] { left, right });
        }

        public Expression GenerateLiteralExpression(LiteralExpression expression, Symbols symbols)
        {
            var constant = Expression.Constant(expression.Value);
            return Expression.Convert(constant, expression.Type);
        }

        public Expression GenerateConstantExpression(IdentifierExpression expression, Symbols symbols)
        {
            var property = Expression.Property(null, expression.Property);
            return Expression.Convert(property, expression.Type);
        }

        public Expression GenerateFieldExpression(FieldExpression expression, Symbols symbols)
        {
            var field = symbols.GetField(expression.Name);
            var access = Expression.Property(Parameter, "Item", Expression.Constant(field.FieldId));
            return Expression.Convert(access, field.Type);
        }

        public Expression GenerateFunctionCallExpression(FunctionExpression call, Symbols symbols)
        {
            var arguments = new List<Expression>();
            foreach (Syntax argument in call.Arguments)
            {
                arguments.Add(Generate(argument, symbols));
            }
            return Expression.Call(null, call.Method, arguments);
        }
    }
}
