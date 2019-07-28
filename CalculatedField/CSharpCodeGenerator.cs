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

        public CSharpCodeGenerator()
        {
        }

        public Func<object> GenerateProgram(Syntax script, Symbols symbols)
        {
            var expression = Generate(script, symbols);
            expression = Expression.Convert(expression, typeof(object));
            return Expression.Lambda<Func<object>>(expression).Compile();
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
                default:
                    throw new ScriptError(0, 0, "Internal compiler error");
            }
        }

        public Expression GenerateUnaryExpression(UnaryExpression expression, Symbols symbols)
        {
            var right = Generate(expression.Right, symbols);

            switch (expression.Operator)
            {
                case UnaryOperator.Plus: // do nothing
                    return right; //return Expression.UnaryPlus(right);
                case UnaryOperator.Minus:
                    return Expression.Negate(right);
                case UnaryOperator.Not:
                    return Expression.Not(right);
                default:
                    throw new ScriptError(0, 0, "Internal compiler error");
            }
        }

        public Expression GenerateBinaryExpression(BinaryExpression expression, Symbols symbols)
        {
            var left = Generate(expression.Left, symbols);
            var right = Generate(expression.Right, symbols);

            switch (expression.Operator)
            {
                case BinaryOperator.Add:
                    if (expression.Method != null)
                    {
                        return Expression.Call(null, expression.Method, new Expression[] { left, right });
                    }
                    else
                    {
                        return Expression.Add(left, right);
                    }
                case BinaryOperator.Subtract:
                    return Expression.Subtract(left, right);
                case BinaryOperator.Multiply:
                    return Expression.Multiply(left, right);
                case BinaryOperator.Divide:
                    return Expression.Divide(left, right);
                case BinaryOperator.Less:
                    if(expression.Left.Type == typeof(string) && expression.Right.Type == typeof(string))
                        return GenerateStringLessExpression(left, right);
                    else
                        return Expression.LessThan(left, right);
                case BinaryOperator.LessOrEqual:
                    if (expression.Left.Type == typeof(string) && expression.Right.Type == typeof(string))
                        return GenerateStringLessOrEqualExpression(left, right);
                        else
                        return Expression.LessThanOrEqual(left, right);                   
                case BinaryOperator.Greater:
                    if (expression.Left.Type == typeof(string) && expression.Right.Type == typeof(string))
                        return GenerateStringGreaterExpression(left, right);
                    else
                        return Expression.GreaterThan(left, right);
                case BinaryOperator.GreaterOrEqual:
                    if (expression.Left.Type == typeof(string) && expression.Right.Type == typeof(string))
                        return GenerateStringGreaterOrEqualExpression(left, right);
                    else
                        return Expression.GreaterThanOrEqual(left, right);
                case BinaryOperator.CompareEqual:
                    return Expression.Equal(left, right);
                case BinaryOperator.CompareNotEqual:
                    return Expression.NotEqual(left, right);
                case BinaryOperator.And:
                    return Expression.And(left, right);
                case BinaryOperator.Or:
                    return Expression.Or(left, right);
                default:
                    throw new ScriptError(0, 0, "Internal compiler error");
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

        public Expression GenerateLiteralExpression(LiteralExpression expression, Symbols symbols)
        {
            var constant = Expression.Constant(expression.Value);
            return Expression.Convert(constant, expression.Type);
        }

        public Expression GenerateFieldExpression(FieldExpression expression, Symbols symbols)
        {
            var field = symbols.GetField(expression.Name);
            return Expression.Variable(field.Type, field.Name);
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
