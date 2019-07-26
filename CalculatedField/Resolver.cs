using System.Collections.Generic;

namespace CalculatedField
{
    class Resolver 
    {   
        public void ResolveProgram(Expression expression, Symbols symbols)
        {
            var type = Resolve(expression, symbols);
            var field = symbols.GetField();
            if (field != null)
            {
                if (type != field.Type)
                    throw new ScriptError(0, 0, $"The script calculates a {type} but the field is a {field.Type}");
            }
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
                    throw new ScriptError(0, 0, "Internal compiler error");
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
            ScriptType type = ScriptType.Null;
            switch (e.Operator)
            {
                case BinaryOperator.Add:
                    if (!TypeChecker.CheckAddition(left, right, out type))
                        throw new ScriptError(e.Token.Column, e.Token.Line, "Type error");
                    else return type;
                case BinaryOperator.Subtract:
                    if (!TypeChecker.CheckSubtraction(left, right, out type))
                        throw new ScriptError(e.Token.Column, e.Token.Line, "Type error");
                    else return type;
                case BinaryOperator.Multiply:
                    if (!TypeChecker.CheckMultiplication(left, right, out type))
                        throw new ScriptError(e.Token.Column, e.Token.Line, "Type error");
                    else return type;
                case BinaryOperator.Divide:
                    if (!TypeChecker.CheckDivision(left, right, out type))
                        throw new ScriptError(e.Token.Column, e.Token.Line, "Type error");
                    else return type;
                case BinaryOperator.DivideAndTruncate:
                    if (!TypeChecker.CheckDivisionAndTruncate(left, right, out type))
                        throw new ScriptError(e.Token.Column, e.Token.Line, "Type error");
                    else return type;
                case BinaryOperator.CompareNotEqual:
                case BinaryOperator.CompareEqual:
                    if (!TypeChecker.CheckCompareEqual(left, right, out type))
                        throw new ScriptError(e.Token.Column, e.Token.Line, "Type error");
                    else return type;
                case BinaryOperator.Greater:
                case BinaryOperator.GreaterOrEqual:
                case BinaryOperator.Less:
                case BinaryOperator.LessOrEqual:
                    if (!TypeChecker.CheckCompareOrder(left, right, out type))
                        throw new ScriptError(e.Token.Column, e.Token.Line, "Type error");
                    else return type;
                case BinaryOperator.And:
                case BinaryOperator.Or:
                    if (!TypeChecker.CheckAndOr(left, right, out type))
                        throw new ScriptError(e.Token.Column, e.Token.Line, "Type error");
                    else return type;
                default:
                    throw new ScriptError(e.Token.Column, e.Token.Line, "Internal compiler error");
            }
        }

        ScriptType ResolveUnaryExpression(UnaryExpression e, Symbols symbols)
        {
            var right = Resolve(e.Right, symbols);
            var type = ScriptType.Null;
            switch (e.Operator)
            {
                case UnaryOperator.Not:
                    if (!TypeChecker.CheckNot(right, out type))
                        throw new ScriptError(e.Token.Column, e.Token.Line, "Type error");
                    return type;
                case UnaryOperator.Plus:
                case UnaryOperator.Minus:
                    if (!TypeChecker.CheckNot(right, out type))
                        throw new ScriptError(e.Token.Column, e.Token.Line, "Type error");
                    return type;
                default:
                    throw new ScriptError(0, 0, "Internal compiler error");
            }
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
                if (TypeChecker.CheckAssignment(variable.Type, rightType, out var type))
                    throw new ScriptError(e.Token.Column, e.Token.Line, "Type error");
                return type;
            }
        }

        ScriptType ResolveIdentifierExpression(IdentifierExpression e, Symbols symbols)
        {
            var variable = symbols.GetVariable(e.Name);
            if(variable == null)
            {
                throw new ScriptError(e.Token.Column, e.Token.Line, $"Can't use unassigned variable ({e.Name})");
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
                    throw new ScriptError(e.Token.Column, e.Token.Line, $"Field is not defined ({e.Name})");
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
                throw new ScriptError(e.Token.Column, e.Token.Line, "Conditional must be a boolean value or expression");
            }
            var thenType = Resolve(e.ThenExpression, symbols);
            var elseType = Resolve(e.ElseExpression, symbols);
            // only an error if used in an expression
            TypeChecker.CheckIfBranches(thenType, elseType, out var type);
            return type;

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
                    throw new ScriptError(e.Token.Column, e.Token.Line, "Function not found");
                }
            }
            e.Location = location;
            var function = symbols.GetScriptFunction(location);
            return function.ReturnType;
        }
    }
}
