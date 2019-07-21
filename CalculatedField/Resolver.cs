using System.Collections.Generic;

namespace CalculatedField
{    
    class Variable
    {
        public ScriptType Type;
        public string Name;
    }

    class Resolver 
    {
        List<Function> EnvironmentFunctions;
        List<Variable> EnvironmentVariables;
        List<Variable> Variables;
        List<Function> Functions;

        public Resolver(List<Function> functions, List<Variable> variables)
        {
            EnvironmentFunctions = functions;
            EnvironmentVariables = variables;
            Variables = new List<Variable>();
            Functions = new List<Function>();
        }

        ScriptType Resolve(Expression expression)
        {
            switch (expression)
            {
                case IntegerLiteralExpression e:
                    return ScriptType.Integer;
                case DecimalLiteralExpression e:
                    return ScriptType.Decimal;
                case BoolLiteralExpression e:
                    return ScriptType.Bool;
                case NullLiteralExpression e:
                    return ScriptType.Null;
                case DateTimeLiteralExpression e:
                    return ScriptType.DateTime;
                case BlockExpression e:
                    return ResolveBlockExpression(e);
                case IfExpression e:
                    return ResolveIfExpression(e);
                case BinaryExpression e:
                    return ResolveBinaryExpression(e);
                case UnaryExpression e:
                    return ResolveUnaryExpression(e);
                case AssignmentExpression e:
                    return ResolveAssignmentExpression(e);
                case FunctionCallExpression e:
                    return ResolveFunctionCallExpression(e);
                case IdentifierExpression e:
                    return ResolveIdentifierExpression(e);
                default:
                    return ScriptType.Error;
            }

        }

        ScriptType ResolveBlockExpression(BlockExpression e)
        {
            if (e.Expressions.Count == 0)
                return ScriptType.Null;

            for (var i = 0; i < e.Expressions.Count - 1; i++)
                Resolve(e.Expressions[i]);

            return Resolve(e.Expressions[e.Expressions.Count - 1]);
        }

        ScriptType ResolveBinaryExpression(BinaryExpression e)
        {
            var left = Resolve(e.Left);
            var right = Resolve(e.Right);
            return left;
        }

        ScriptType ResolveUnaryExpression(UnaryExpression e)
        {
            var right = Resolve(e.Right);
            if(e.Operator == UnaryOperator.Not)
            {
                if (right == ScriptType.Bool) return ScriptType.Bool;
                return ScriptType.Error;
            }

            if (right == ScriptType.Integer || right == ScriptType.Decimal)
                return right;
            return ScriptType.Error;
        }

        ScriptType ResolveAssignmentExpression(AssignmentExpression e)
        {
            var rightType = Resolve(e.Right);
            return rightType;
            /*
            if (Variables.TryGetValue(e.Left.Name, out var type))
            {
                if (type != rightType)
                {
                    // warn or error
                }
                return rightType;

            }
            else
            {
                Variables.Add(e.Left.Name, rightType);
                return rightType;
            }*/
        }

        ScriptType ResolveIdentifierExpression(IdentifierExpression e)
        {
            return ScriptType.Bool;

        }

        ScriptType ResolveIfExpression(IfExpression e)
        {
            var conditionType = Resolve(e.Condition);
            if(conditionType != ScriptType.Bool)
            {
                // warn or error
            }
            var thenType = Resolve(e.ThenExpression);
            var elseType = ScriptType.Null;
            if (e.ElseExpression != null)
            {
                elseType = Resolve(e.ElseExpression);
            }
            if (thenType == ScriptType.Decimal && elseType == ScriptType.Integer) return ScriptType.Decimal;
            if (thenType == ScriptType.Integer && elseType == ScriptType.Decimal) return ScriptType.Decimal;
            if (thenType == elseType) return thenType;
            return ScriptType.Error;
        }

        ScriptType ResolveFunctionCallExpression(FunctionCallExpression e)
        {
            var argumentTypes = new List<ScriptType>();
            foreach(var argument in e.Arguments)
            {
                var type = Resolve(argument);
                argumentTypes.Add(type);
            }

            Function match = null;
            foreach (var function in EnvironmentFunctions)
            {
                if(function.Name == e.Name && function.ArgumentTypes.Count == argumentTypes.Count)
                {
                    bool argumentsMatch = true;
                    for(int i = 0; i < argumentTypes.Count; i++)
                    {
                        if (argumentTypes[i] != function.ArgumentTypes[i])
                            argumentsMatch = false;
                    }
                    if(argumentsMatch)
                    {
                        match = function;
                        //Functions.Add(e, function);
                        break;
                    }
                }
            }
            if(match == null)
            {
                // no function found
                return ScriptType.Error;
            }

            return match.ReturnType;

        }


    }
}
