using System.Collections.Generic;

namespace CalculatedField
{
    class Resolver 
    {   
        public void ResolveProgram(ScriptExpression script, Symbols symbols)
        {
            ScriptType type;
            if (script.Expressions.Count == 0)
                type = ScriptType.Null;
            else
            {
                for (var i = 0; i < script.Expressions.Count - 1; i++)
                    Resolve(script.Expressions[i], symbols);
                type = Resolve(script.Expressions[script.Expressions.Count - 1], symbols);
            }
            var field = symbols.GetField();
            if (field != null)
            {
                if (type != field.Type)
                    throw new ScriptError(0, 0, $"The script calculates a {type} but the field is a {field.Type}");
            }
        }
        public ScriptType Resolve(Syntax expression, Symbols symbols)
        {
            switch (expression)
            {
                case LiteralExpression e:
                    return ResolveLiteralExpression(e, symbols);
                case BinaryExpression e:
                    return ResolveBinaryExpression(e, symbols);
                case UnaryExpression e:
                    return ResolveUnaryExpression(e, symbols);
                case AssignmentStatement e:
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

        ScriptType ResolveBinaryExpression(BinaryExpression e, Symbols symbols)
        {
            var left = Resolve(e.Left, symbols);
            var right = Resolve(e.Right, symbols);
            ScriptType type = ScriptType.Null;
            switch (e.Operator)
            {
                case BinaryOperator.Add:
                    if (!TypeChecker.CheckAddition(left, right, out type))
                        throw new ScriptError(e.Token.Column, e.Token.Line, $"Can't add {left} and {right}");
                    else return type;
                case BinaryOperator.Subtract:
                    if (!TypeChecker.CheckSubtraction(left, right, out type))
                        throw new ScriptError(e.Token.Column, e.Token.Line, $"Can't subtract {left} and {right}");
                    else return type;
                case BinaryOperator.Multiply:
                    if (!TypeChecker.CheckMultiplication(left, right, out type))
                        throw new ScriptError(e.Token.Column, e.Token.Line, $"Can't multiply {left} and {right}");
                    else return type;
                case BinaryOperator.Divide:
                    if (!TypeChecker.CheckDivision(left, right, out type))
                        throw new ScriptError(e.Token.Column, e.Token.Line, $"Can't divide {left} and {right}");
                    else return type;
                case BinaryOperator.DivideAndTruncate:
                    if (!TypeChecker.CheckDivisionAndTruncate(left, right, out type))
                        throw new ScriptError(e.Token.Column, e.Token.Line, $"Can't divide and truncate {left} and {right}");
                    else return type;
                case BinaryOperator.CompareNotEqual:
                case BinaryOperator.CompareEqual:
                    if (!TypeChecker.CheckCompareEqual(left, right, out type))
                        throw new ScriptError(e.Token.Column, e.Token.Line, $"Can't compare {left} and {right}");
                    else return type;
                case BinaryOperator.Greater:
                case BinaryOperator.GreaterOrEqual:
                case BinaryOperator.Less:
                case BinaryOperator.LessOrEqual:
                    if (!TypeChecker.CheckCompareOrder(left, right, out type))
                        throw new ScriptError(e.Token.Column, e.Token.Line, $"Can't compare {left} and {right}");
                    else return type;
                case BinaryOperator.And:
                    if (!TypeChecker.CheckAndOr(left, right, out type))
                        throw new ScriptError(e.Token.Column, e.Token.Line, $"Can't and {left} and {right}");
                    else return type;
                case BinaryOperator.Or:
                    if (!TypeChecker.CheckAndOr(left, right, out type))
                        throw new ScriptError(e.Token.Column, e.Token.Line, $"Can't or {left} and {right}");
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
                        throw new ScriptError(e.Token.Column, e.Token.Line, $"Can't not {right}");
                    return type;
                case UnaryOperator.Plus:
                    if (!TypeChecker.CheckNot(right, out type))
                        throw new ScriptError(e.Token.Column, e.Token.Line, $"Can't positive {right}");
                    return type;
                case UnaryOperator.Minus:
                    if (!TypeChecker.CheckNot(right, out type))
                        throw new ScriptError(e.Token.Column, e.Token.Line, $"Type negative {right}");
                    return type;
                default:
                    throw new ScriptError(0, 0, "Internal compiler error");
            }
        }

        ScriptType ResolveAssignmentExpression(AssignmentStatement e, Symbols symbols)
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
                if (!TypeChecker.CheckAssignment(variable.Type, rightType, out var type))
                    throw new ScriptError(e.Token.Column, e.Token.Line, $"{e.Name} was previously assigned as {variable.Type} but is being assigned as {rightType}");
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
                    throw new ScriptError(e.Token.Column, e.Token.Line, $"Can't use undefined field ({e.Name})");
                }
            }
            else
            {
                field = symbols.GetScriptFields()[location];
            }

            e.Location = location;
            return field.Type;

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
                    throw new ScriptError(e.Token.Column, e.Token.Line, $"Function not found {e.Name}({string.Join(", ", argumentTypes)})");
                }
            }
            e.Location = location;
            var function = symbols.GetScriptFunction(location);
            return function.ReturnType;
        }
    }
}
