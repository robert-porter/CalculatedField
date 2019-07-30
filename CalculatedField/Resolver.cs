using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CalculatedField
{
    class Resolver
    {
        public List<Field> EntityFields;
        List<ScriptError> Errors;

        public Resolver(List<Field> entityFields)
        {
            EntityFields = entityFields;
        }

        public List<ScriptError> ResolveScript(Syntax syntax)
        {
            Errors = new List<ScriptError>();
            Resolve(syntax);
            return Errors;
        }

        public void Resolve(Syntax expression)
        {
            switch (expression)
            {
                case BinaryExpression e:
                    ResolveBinaryExpression(e);
                    break;
                case UnaryExpression e:
                    ResolveUnaryExpression(e);
                    break;
                case FunctionExpression e:
                    ResolveFunctionCallExpression(e);
                    break;
                case FieldExpression e:
                    ResolveFieldExpression(e);
                    break;
                case IdentifierExpression e:
                    ResolveIdentifierExpression(e);
                    break;
            }
        }

        void ResolveIdentifierExpression(IdentifierExpression e)
        {
            var constant = GetConstant(e.Name);
            if(constant != null)
            {
                e.Type = constant.PropertyType;
                e.Property = constant;
            }
            else
            {
                Errors.Add(new ScriptError(e.Token.Index, $"Constant {e.Name} is not defined"));
            }
        }

        void ResolveBinaryExpression(BinaryExpression e)
        {
            Resolve(e.Left);
            Resolve(e.Right);
            Type type = null;
            switch (e.Token.Type)
            {
                case TokenType.Plus:
                    type = ResolveAdd(e, e.Left.Type, e.Right.Type);
                    break;
                case TokenType.Minus:
                    type = CheckSubtract(e.Left.Type, e.Right.Type);
                    break;
                case TokenType.Multiply:
                    type = CheckMultiply(e.Left.Type, e.Right.Type);
                    break;
                case TokenType.Divide:
                    type = CheckDivide(e.Left.Type, e.Right.Type);
                    break;
                case TokenType.Equal:
                case TokenType.NotEqual:
                    type = CheckCompareEqual(e.Left.Type, e.Right.Type);
                    break;
                case TokenType.GreaterThan:
                case TokenType.GreaterThenOrEqual:
                case TokenType.LessThen:
                case TokenType.LessThanOrEqual:
                    type = CheckCompareOrder(e.Left.Type, e.Right.Type);
                    break;
                case TokenType.And:
                    type = CheckAndOr(e.Left.Type, e.Right.Type);
                    break;
                case TokenType.Or:
                    type = CheckAndOr(e.Left.Type, e.Right.Type);
                    break;
            }
            e.Type = type;
            if (type == null)
            {
                var leftName = GetFiendlyTypeName(e.Left.Type);
                var rightName = GetFiendlyTypeName(e.Right.Type);
                Errors.Add(new ScriptError(e.Token.Index, $"Operator {e.Token.Contents} is not defined on {leftName} and {rightName}"));
            }
        }

        void ResolveUnaryExpression(UnaryExpression e)
        {
            Resolve(e.Right);
            Type type = null;
            switch (e.Token.Type)
            {
                case TokenType.Not:
                    type = CheckNot(e.Right.Type);
                    break;
                case TokenType.Plus:
                    type = CheckUnaryPlus(e.Right.Type);
                    break;
                case TokenType.Minus:
                    type = CheckUnaryMinus(e.Right.Type);
                    break;
            }
            e.Type = type;
            if (type == null)
            {
                var typeName = GetFiendlyTypeName(e.Right.Type);
                Errors.Add(new ScriptError(e.Token.Index, $"Operator {e.Token.Contents} is not defined on {typeName}"));
            }
        }

        void ResolveFieldExpression(FieldExpression e)
        {
            Field field = EntityFields.Find(f => f.Name == e.Name);
            if (field == null)
            {
                Errors.Add(new ScriptError(e.Token.Index, $"Field {e.Name} is not defined"));
            }
            else
            {
                e.Type = field.Type;
            }
        }

        void ResolveFunctionCallExpression(FunctionExpression e)
        {
            var argumentTypes = new List<Type>();
            foreach(var argument in e.Arguments)
            {
                Resolve(argument);
                argumentTypes.Add(argument.Type);
            }
            var function = GetFunction(e.Name, argumentTypes);
            if (function == null)
            {
                var fiendlyTypes = argumentTypes.Select(type => GetFiendlyTypeName(type));
                Errors.Add(new ScriptError(e.Token.Index, $"Function {e.Name}({string.Join(", ", fiendlyTypes)}) is not defined"));
            }
            else
            {
                e.Method = function;
                e.Type = function.ReturnType;
            }
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
            if ((left == typeof(decimal?) || left == typeof(decimal)) &&
                (right == typeof(decimal?) || right == typeof(decimal)))
                return typeof(decimal?);
            if ((left == typeof(DateTime?) || left == typeof(DateTime)) &&
                (right == typeof(TimeSpan?) || right == typeof(TimeSpan)))
                return typeof(DateTime?);
            if ((left == typeof(TimeSpan?) || left == typeof(TimeSpan)) &&
                (right == typeof(TimeSpan?) || right == typeof(TimeSpan)))
                return typeof(TimeSpan?);
            if ((left == typeof(string) && right == typeof(string)))
                return typeof(string);
            
            return type;
        }

        public static Type CheckSubtract(Type left, Type right)
        {

            if ((left == typeof(decimal?) || left == typeof(decimal)) &&
                (right == typeof(decimal?) || right == typeof(decimal)))
                return typeof(decimal?);
            if ((left == typeof(TimeSpan?) || left == typeof(TimeSpan)) &&
                (right == typeof(TimeSpan?) || right == typeof(TimeSpan)))
                return typeof(TimeSpan?);
            if ((left == typeof(DateTime?) || left == typeof(DateTime)) &&
                (right == typeof(DateTime?) || right == typeof(DateTime)))
                return typeof(TimeSpan?);
            return null;
        }

        public static Type CheckMultiply(Type left, Type right)
        {

            if ((left == typeof(decimal?) || left == typeof(decimal)) &&
                (right == typeof(decimal?) || right == typeof(decimal)))
                return typeof(decimal?);
            return null;
        }

        public static Type CheckDivide(Type left, Type right)
        {

            if ((left == typeof(decimal?) || left == typeof(decimal)) &&
                (right == typeof(decimal?) || right == typeof(decimal)))
                return typeof(decimal?);

            return null;
        }

        public static Type CheckDivideThenTruncate(Type left, Type right)
        {

            if ((left == typeof(decimal?) || left == typeof(decimal)) &&
                (right == typeof(decimal?) || right == typeof(decimal)))
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
            if ((left == typeof(bool?) || left == typeof(bool)) && 
                (right == typeof(bool?) || right == typeof(bool)))
                return typeof(bool?);
            return null;
        }

        public static Type CheckCompareOrder(Type left, Type right)
        {
            if ((left == typeof(decimal?) || left == typeof(decimal)) &&
                (right == typeof(decimal?) || right == typeof(decimal)))
                return typeof(bool?);
            if (left == typeof(string) && right == typeof(string))
                return typeof(bool?);
            if ((left == typeof(TimeSpan?) || left == typeof(TimeSpan)) &&
                (right == typeof(TimeSpan?) || right == typeof(TimeSpan)))
                return typeof(bool?);
            if ((left == typeof(DateTime?) || left == typeof(DateTime)) &&
                (right == typeof(DateTime?) || right == typeof(DateTime)))
                return typeof(bool?);
            return null;
        }

        public static Type CheckCompareEqual(Type left, Type right)
        {
            if ((left == typeof(bool?) || left == typeof(bool)) && 
                (right == typeof(bool?) || right == typeof(bool)))
                return typeof(bool?);
            if ((left == typeof(decimal?) || left == typeof(decimal)) &&
                (right == typeof(decimal?) || right == typeof(decimal)))
                return typeof(bool?);
            if (left == typeof(string) && right == typeof(string))
                return typeof(bool?);
            if ((left == typeof(TimeSpan?) || left == typeof(TimeSpan)) &&
                (right == typeof(TimeSpan?) || right == typeof(TimeSpan)))
                return typeof(bool?);
            if ((left == typeof(DateTime?) || left == typeof(DateTime)) &&
                (right == typeof(DateTime?) || right == typeof(DateTime)))
                return typeof(bool?);
            return null;
        }

        string GetFiendlyTypeName(Type type)
        {
            if (type == typeof(bool?) || type == typeof(bool))
                return "Boolean";
            if (type == typeof(decimal?) || type == typeof(decimal))
                return "Number";
            if (type == typeof(string))
                return "String";
            if (type == typeof(TimeSpan?) || type == typeof(TimeSpan)) 
                return "TimeSpan";
            if (type == typeof(DateTime?) || type == typeof(DateTime))
                return "DateTime";
            return "Undefined";

        }

        public PropertyInfo GetConstant(string name)
        {
            return Runtime.Constants.Find(constant => constant.Name == name);
        }

        public MethodInfo GetFunction(string searchName, List<Type> searchArguments)
        {
            return Runtime.Functions.FirstOrDefault(function =>
                function.Name == searchName &&
                searchArguments.SequenceEqual(function.GetParameters().Select(param => param.ParameterType))
            );
        }
    }
}
