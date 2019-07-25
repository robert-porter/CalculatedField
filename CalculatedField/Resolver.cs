using System.Collections.Generic;

namespace CalculatedField
{
    class Resolver 
    {   
        public void ResolveProgram(Expression expression, Symbols symbols)
        {
            var type = Resolve(expression, symbols);
            if (type != symbols.GetField().Type)
                throw new ScriptError(0, 0, "Type error");
        }
        public ScriptType Resolve(Expression expression, Symbols symbols)
        {
            switch (expression)
            {
                case LiteralExpression e:
                    return ResolveLiteralExpression(e, symbols);
                case BlockExpression e:
                    return ResolveBlockExpression(e, symbols);
                case IfExpression e:
                    return ResolveIfExpression(e, symbols);
                case BinaryExpression e:
                    return ResolveBinaryExpression(e, symbols);
                case UnaryExpression e:
                    return ResolveUnaryExpression(e, symbols);
                case AssignmentExpression e:
                    return ResolveAssignmentExpression(e, symbols);
                case FunctionExpression e:
                    return ResolveFunctionCallExpression(e, symbols);
                case IdentifierExpression e:
                    return ResolveIdentifierExpression(e, symbols);
                case FieldExpression e:
                    return ResolveFieldExpression(e, symbols);
                default:
                    throw new ScriptError(0, 0, "Type error");
            }
        }

        ScriptType ResolveLiteralExpression(LiteralExpression  e, Symbols symbols)
        {
            var value = e.ScriptValue;
            var location = symbols.GetConstantLocation(value);
            if (location == -1)
                location = symbols.AddConstant(value);
            e.Location = location;
            return e.Type;
        }

        ScriptType ResolveBlockExpression(BlockExpression e, Symbols symbols)
        {
            if (e.Expressions.Count == 0)
                return ScriptType.Null;
            for (var i = 0; i < e.Expressions.Count - 1; i++)
                Resolve(e.Expressions[i], symbols);
            return Resolve(e.Expressions[e.Expressions.Count - 1], symbols);
        }

        ScriptType ResolveBinaryExpression(BinaryExpression e, Symbols symbols)
        {
            var left = Resolve(e.Left, symbols);
            var right = Resolve(e.Right, symbols);
            switch (e.Operator)
            {
                case BinaryOperator.Add:
                    if (left == ScriptType.Integer && right == ScriptType.Integer) return ScriptType.Integer;
                    if (left == ScriptType.Decimal && right == ScriptType.Integer) return ScriptType.Decimal;
                    if (left == ScriptType.Integer && right == ScriptType.Decimal) return ScriptType.Decimal;
                    if (left == ScriptType.Decimal && right == ScriptType.Decimal) return ScriptType.Decimal;
                    if (left == ScriptType.DateTime && right == ScriptType.TimeSpan) return ScriptType.DateTime;
                    if (left == ScriptType.TimeSpan && right == ScriptType.DateTime) return ScriptType.DateTime;
                    if (left == ScriptType.TimeSpan && right == ScriptType.TimeSpan) return ScriptType.TimeSpan;
                    if (left == ScriptType.String && right == ScriptType.String) return ScriptType.String;
                    throw new ScriptError(0, 0, "Type error");
                case BinaryOperator.Subtract:
                    if (left == ScriptType.Integer && right == ScriptType.Integer) return ScriptType.Integer;
                    if (left == ScriptType.Decimal && right == ScriptType.Integer) return ScriptType.Decimal;
                    if (left == ScriptType.Integer && right == ScriptType.Decimal) return ScriptType.Decimal;
                    if (left == ScriptType.Decimal && right == ScriptType.Decimal) return ScriptType.Decimal;
                    if (left == ScriptType.TimeSpan && right == ScriptType.TimeSpan) return ScriptType.TimeSpan;
                    if (left == ScriptType.DateTime && right == ScriptType.DateTime) return ScriptType.TimeSpan;
                    throw new ScriptError(0, 0, "Type error");
                case BinaryOperator.Multiply:
                    if (left == ScriptType.Integer && right == ScriptType.Integer) return ScriptType.Integer;
                    if (left == ScriptType.Decimal && right == ScriptType.Integer) return ScriptType.Decimal;
                    if (left == ScriptType.Integer && right == ScriptType.Decimal) return ScriptType.Decimal;
                    if (left == ScriptType.Decimal && right == ScriptType.Decimal) return ScriptType.Decimal;
                    if (left == ScriptType.TimeSpan && right == ScriptType.Decimal) return ScriptType.TimeSpan;
                    if (left == ScriptType.Decimal && right == ScriptType.TimeSpan) return ScriptType.TimeSpan;
                    if (left == ScriptType.TimeSpan && right == ScriptType.Integer) return ScriptType.TimeSpan;
                    if (left == ScriptType.Integer && right == ScriptType.TimeSpan) return ScriptType.TimeSpan;
                    throw new ScriptError(0, 0, "Type error");
                case BinaryOperator.Divide:
                    if (left == ScriptType.Integer && right == ScriptType.Integer) return ScriptType.Decimal;
                    if (left == ScriptType.Decimal && right == ScriptType.Integer) return ScriptType.Decimal;
                    if (left == ScriptType.Integer && right == ScriptType.Decimal) return ScriptType.Decimal;
                    if (left == ScriptType.Decimal && right == ScriptType.Decimal) return ScriptType.Decimal;
                    if (left == ScriptType.TimeSpan && right == ScriptType.Decimal) return ScriptType.TimeSpan;
                    if (left == ScriptType.TimeSpan && right == ScriptType.Integer) return ScriptType.TimeSpan;
                    throw new ScriptError(0, 0, "Type error");
                case BinaryOperator.CompareNotEqual:
                case BinaryOperator.CompareEqual:
                    if (left == ScriptType.Decimal && right == ScriptType.Integer)
                        ;//warn;
                    if (left == ScriptType.Integer && right == ScriptType.Decimal)
                        ;//warn
                    if(left != right)
                    {
                        throw new ScriptError(0, 0, "Type error");
                    }
                    return ScriptType.Bool;
                case BinaryOperator.Greater:
                case BinaryOperator.GreaterOrEqual:
                case BinaryOperator.Less:
                case BinaryOperator.LessOrEqual:
                    if (left == ScriptType.Integer && right == ScriptType.Integer) return ScriptType.Bool;
                    if (left == ScriptType.Decimal && right == ScriptType.Decimal) return ScriptType.Bool;
                    if (left == ScriptType.Decimal && right == ScriptType.Integer) return ScriptType.Bool; // warn
                    if (left == ScriptType.Integer && right == ScriptType.Decimal) return ScriptType.Bool; // warn
                    if (left == ScriptType.String && right == ScriptType.String) return ScriptType.Bool;
                    if (left == ScriptType.TimeSpan && right == ScriptType.TimeSpan) return ScriptType.Bool;
                    if (left == ScriptType.DateTime && right == ScriptType.DateTime) return ScriptType.Bool;
                    throw new ScriptError(0, 0, "Type error");
                case BinaryOperator.And:
                case BinaryOperator.Or:
                    if (left == ScriptType.Bool && right == ScriptType.Bool) return ScriptType.Bool;
                    throw new ScriptError(0, 0, "Type error");
            }
            return left;
        }

        ScriptType ResolveUnaryExpression(UnaryExpression e, Symbols symbols)
        {
            var right = Resolve(e.Right, symbols);
            if(e.Operator == UnaryOperator.Not)
            {
                if (right == ScriptType.Bool)
                {
                    return ScriptType.Bool;
                }
                throw new ScriptError(0, 0, "Type error");
            }

            if (right == ScriptType.Integer || right == ScriptType.Decimal)
            {
                return right;
            }
            throw new ScriptError(0, 0, "Type error");
        }

        ScriptType ResolveAssignmentExpression(AssignmentExpression e, Symbols symbols)
        {
            var rightType = Resolve(e.Right, symbols);
            var variable = symbols.GetVariable(e.Name);
            if(variable == null)
            {
                int location = symbols.AddVariable(e.Name, rightType);
                e.Location = location;
                return rightType;
            }
            else
            { 
                e.Location = variable.Location;
                return CoerceTypes(variable.Type, rightType);
            }
        }

        ScriptType ResolveIdentifierExpression(IdentifierExpression e, Symbols symbols)
        {
            var variable = symbols.GetVariable(e.Name);
            if(variable == null)
            {
                throw new ScriptError(0, 0, $"Can't use unassigned variable ({e.Name})");
            }
            else
            {
                e.Location = variable.Location;
                return variable.Type;
            }
        }

        ScriptType ResolveFieldExpression(FieldExpression e, Symbols symbols)
        {
            int location = symbols.GetScriptFieldLocation(e.Name);
            Field field = null;
            if (location == -1)
            {
                field = symbols.GetFields().Find(f => f.Name == e.Name);
                if (field != null)
                {
                    location = symbols.AddScriptField(field);
                }
                else
                {
                    throw new ScriptError(0, 0, $"Field is not defined ({e.Name})");
                }
            }
            else
            {
                field = symbols.GetScriptFields()[location];
            }

            e.Location = location;
            return field.Type;

        }

        ScriptType ResolveIfExpression(IfExpression e, Symbols symbols)
        {
            var conditionType = Resolve(e.Condition, symbols);
            if(conditionType != ScriptType.Bool)
            {
                throw new ScriptError(0, 0, "Conditional must be a boolean value or expression");
            }
            var thenType = Resolve(e.ThenExpression, symbols);
            var elseType = ScriptType.Null;
            if (e.ElseExpression != null)
            {
                elseType = Resolve(e.ElseExpression, symbols);
            }
            return CoerceTypes(thenType, elseType);
        }

        ScriptType ResolveFunctionCallExpression(FunctionExpression e, Symbols symbols)
        {
            var argumentTypes = new List<ScriptType>();
            foreach(var argument in e.Arguments)
            {
                var type = Resolve(argument, symbols);
                argumentTypes.Add(type);
            }
            var location = symbols.GetFunctionLocation(e.Name, argumentTypes);
            if(location == -1)
            {
                location = symbols.AddFunction(e.Name, argumentTypes);
                if(location == -1)
                {
                    // error
                    throw new ScriptError(0, 0, "Function not found");
                }
            }
            e.Location = location;
            var function = symbols.GetScriptFunction(location);
            return function.ReturnType;
        }

        public static ScriptType CoerceTypes(ScriptType a, ScriptType b)
        {
            if (a == ScriptType.Decimal && b == ScriptType.Integer) return ScriptType.Decimal;
            if (a == ScriptType.Integer && b == ScriptType.Decimal) return ScriptType.Decimal;
            if (a == b) return a;
            throw new ScriptError(0, 0, "Type error");
        }

    }
}
