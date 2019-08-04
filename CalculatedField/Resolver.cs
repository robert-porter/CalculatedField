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
        readonly Dictionary<(TokenType, ScriptType, ScriptType), ScriptType> BinaryOperators;
        readonly Dictionary<(TokenType, ScriptType), ScriptType> UnaryOperators;

        public Resolver(List<Field> entityFields)
        {
            EntityFields = entityFields;

            BinaryOperators = new Dictionary<(TokenType, ScriptType, ScriptType), ScriptType>()
            {
                [(TokenType.Plus, ScriptType.Null, ScriptType.Null)] = ScriptType.Null,
                [(TokenType.Plus, ScriptType.Number, ScriptType.Number)] = ScriptType.Number,
                [(TokenType.Plus, ScriptType.Null, ScriptType.Number)] = ScriptType.Number,
                [(TokenType.Plus, ScriptType.Number, ScriptType.Null)] = ScriptType.Number,
                [(TokenType.Plus, ScriptType.String, ScriptType.String)] = ScriptType.String,
                [(TokenType.Plus, ScriptType.Null, ScriptType.String)] = ScriptType.String,
                [(TokenType.Plus, ScriptType.String, ScriptType.Null)] = ScriptType.String,
                [(TokenType.Plus, ScriptType.TimeSpan, ScriptType.TimeSpan)] = ScriptType.TimeSpan,
                [(TokenType.Plus, ScriptType.Null, ScriptType.TimeSpan)] = ScriptType.TimeSpan,
                [(TokenType.Plus, ScriptType.TimeSpan, ScriptType.Null)] = ScriptType.TimeSpan,
                [(TokenType.Plus, ScriptType.DateTime, ScriptType.TimeSpan)] = ScriptType.DateTime,
                [(TokenType.Plus, ScriptType.DateTime, ScriptType.Null)] = ScriptType.DateTime,

                [(TokenType.Minus, ScriptType.Null, ScriptType.Null)] = ScriptType.Null,
                [(TokenType.Minus, ScriptType.Number, ScriptType.Number)] = ScriptType.Number,
                [(TokenType.Minus, ScriptType.Null, ScriptType.Number)] = ScriptType.Number,
                [(TokenType.Minus, ScriptType.Number, ScriptType.Null)] = ScriptType.Number,
                [(TokenType.Minus, ScriptType.TimeSpan, ScriptType.TimeSpan)] = ScriptType.TimeSpan,
                [(TokenType.Minus, ScriptType.Null, ScriptType.TimeSpan)] = ScriptType.TimeSpan,
                [(TokenType.Minus, ScriptType.TimeSpan, ScriptType.Null)] = ScriptType.TimeSpan,
                [(TokenType.Minus, ScriptType.DateTime, ScriptType.DateTime)] = ScriptType.TimeSpan,
                [(TokenType.Minus, ScriptType.Null, ScriptType.DateTime)] = ScriptType.TimeSpan,
                [(TokenType.Minus, ScriptType.DateTime, ScriptType.Null)] = ScriptType.TimeSpan,

                [(TokenType.Multiply, ScriptType.Null, ScriptType.Null)] = ScriptType.Null,
                [(TokenType.Multiply, ScriptType.Number, ScriptType.Number)] = ScriptType.Number,
                [(TokenType.Multiply, ScriptType.Null, ScriptType.Number)] = ScriptType.Number,
                [(TokenType.Multiply, ScriptType.Number, ScriptType.Null)] = ScriptType.Number,

                [(TokenType.Divide, ScriptType.Null, ScriptType.Null)] = ScriptType.Null,
                [(TokenType.Divide, ScriptType.Number, ScriptType.Number)] = ScriptType.Number,
                [(TokenType.Divide, ScriptType.Null, ScriptType.Number)] = ScriptType.Number,
                [(TokenType.Divide, ScriptType.Number, ScriptType.Null)] = ScriptType.Number,

                [(TokenType.And, ScriptType.Null, ScriptType.Null)] = ScriptType.Null,
                [(TokenType.And, ScriptType.Boolean, ScriptType.Boolean)] = ScriptType.Boolean,
                [(TokenType.And, ScriptType.Null, ScriptType.Boolean)] = ScriptType.Boolean,
                [(TokenType.And, ScriptType.Boolean, ScriptType.Null)] = ScriptType.Boolean,

                [(TokenType.Or, ScriptType.Null, ScriptType.Null)] = ScriptType.Null,
                [(TokenType.Or, ScriptType.Boolean, ScriptType.Boolean)] = ScriptType.Boolean,
                [(TokenType.Or, ScriptType.Null, ScriptType.Boolean)] = ScriptType.Boolean,
                [(TokenType.Or, ScriptType.Boolean, ScriptType.Null)] = ScriptType.Boolean,

                [(TokenType.Equal, ScriptType.Null, ScriptType.Null)] = ScriptType.Null,
                [(TokenType.Equal, ScriptType.Number, ScriptType.Number)] = ScriptType.Boolean,
                [(TokenType.Equal, ScriptType.Null, ScriptType.Number)] = ScriptType.Boolean,
                [(TokenType.Equal, ScriptType.Number, ScriptType.Null)] = ScriptType.Boolean,
                [(TokenType.Equal, ScriptType.Boolean, ScriptType.Boolean)] = ScriptType.Boolean,
                [(TokenType.Equal, ScriptType.Null, ScriptType.Boolean)] = ScriptType.Boolean,
                [(TokenType.Equal, ScriptType.Boolean, ScriptType.Null)] = ScriptType.Boolean,
                [(TokenType.Equal, ScriptType.String, ScriptType.String)] = ScriptType.Boolean,
                [(TokenType.Equal, ScriptType.Null, ScriptType.String)] = ScriptType.Boolean,
                [(TokenType.Equal, ScriptType.String, ScriptType.Null)] = ScriptType.Boolean,
                [(TokenType.Equal, ScriptType.DateTime, ScriptType.DateTime)] = ScriptType.Boolean,
                [(TokenType.Equal, ScriptType.Null, ScriptType.DateTime)] = ScriptType.Boolean,
                [(TokenType.Equal, ScriptType.DateTime, ScriptType.Null)] = ScriptType.Boolean,
                [(TokenType.Equal, ScriptType.TimeSpan, ScriptType.TimeSpan)] = ScriptType.Boolean,
                [(TokenType.Equal, ScriptType.Null, ScriptType.TimeSpan)] = ScriptType.Boolean,
                [(TokenType.Equal, ScriptType.TimeSpan, ScriptType.Null)] = ScriptType.Boolean,

                [(TokenType.NotEqual, ScriptType.Null, ScriptType.Null)] = ScriptType.Null,
                [(TokenType.NotEqual, ScriptType.Number, ScriptType.Number)] = ScriptType.Boolean,
                [(TokenType.NotEqual, ScriptType.Null, ScriptType.Number)] = ScriptType.Boolean,
                [(TokenType.NotEqual, ScriptType.Number, ScriptType.Null)] = ScriptType.Boolean,
                [(TokenType.NotEqual, ScriptType.Boolean, ScriptType.Boolean)] = ScriptType.Boolean,
                [(TokenType.NotEqual, ScriptType.Null, ScriptType.Boolean)] = ScriptType.Boolean,
                [(TokenType.NotEqual, ScriptType.Boolean, ScriptType.Null)] = ScriptType.Boolean,
                [(TokenType.NotEqual, ScriptType.String, ScriptType.String)] = ScriptType.Boolean,
                [(TokenType.NotEqual, ScriptType.Null, ScriptType.String)] = ScriptType.Boolean,
                [(TokenType.NotEqual, ScriptType.String, ScriptType.Null)] = ScriptType.Boolean,
                [(TokenType.NotEqual, ScriptType.DateTime, ScriptType.DateTime)] = ScriptType.Boolean,
                [(TokenType.NotEqual, ScriptType.Null, ScriptType.DateTime)] = ScriptType.Boolean,
                [(TokenType.NotEqual, ScriptType.DateTime, ScriptType.Null)] = ScriptType.Boolean,
                [(TokenType.NotEqual, ScriptType.TimeSpan, ScriptType.TimeSpan)] = ScriptType.Boolean,
                [(TokenType.NotEqual, ScriptType.Null, ScriptType.TimeSpan)] = ScriptType.Boolean,
                [(TokenType.NotEqual, ScriptType.TimeSpan, ScriptType.Null)] = ScriptType.Boolean,

                [(TokenType.GreaterThen, ScriptType.Null, ScriptType.Null)] = ScriptType.Null,
                [(TokenType.GreaterThen, ScriptType.Number, ScriptType.Number)] = ScriptType.Boolean,
                [(TokenType.GreaterThen, ScriptType.Null, ScriptType.Number)] = ScriptType.Boolean,
                [(TokenType.GreaterThen, ScriptType.Number, ScriptType.Null)] = ScriptType.Boolean,
                [(TokenType.GreaterThen, ScriptType.String, ScriptType.String)] = ScriptType.Boolean,
                [(TokenType.GreaterThen, ScriptType.Null, ScriptType.String)] = ScriptType.Boolean,
                [(TokenType.GreaterThen, ScriptType.String, ScriptType.Null)] = ScriptType.Boolean,
                [(TokenType.GreaterThen, ScriptType.DateTime, ScriptType.DateTime)] = ScriptType.Boolean,
                [(TokenType.GreaterThen, ScriptType.Null, ScriptType.DateTime)] = ScriptType.Boolean,
                [(TokenType.GreaterThen, ScriptType.DateTime, ScriptType.Null)] = ScriptType.Boolean,
                [(TokenType.GreaterThen, ScriptType.TimeSpan, ScriptType.TimeSpan)] = ScriptType.Boolean,
                [(TokenType.GreaterThen, ScriptType.Null, ScriptType.TimeSpan)] = ScriptType.Boolean,
                [(TokenType.GreaterThen, ScriptType.TimeSpan, ScriptType.Null)] = ScriptType.Boolean,

                [(TokenType.GreaterThenOrEqual, ScriptType.Null, ScriptType.Null)] = ScriptType.Null,
                [(TokenType.GreaterThenOrEqual, ScriptType.Number, ScriptType.Number)] = ScriptType.Boolean,
                [(TokenType.GreaterThenOrEqual, ScriptType.Null, ScriptType.Number)] = ScriptType.Boolean,
                [(TokenType.GreaterThenOrEqual, ScriptType.Number, ScriptType.Null)] = ScriptType.Boolean,
                [(TokenType.GreaterThenOrEqual, ScriptType.String, ScriptType.String)] = ScriptType.Boolean,
                [(TokenType.GreaterThenOrEqual, ScriptType.Null, ScriptType.String)] = ScriptType.Boolean,
                [(TokenType.GreaterThenOrEqual, ScriptType.String, ScriptType.Null)] = ScriptType.Boolean,
                [(TokenType.GreaterThenOrEqual, ScriptType.DateTime, ScriptType.DateTime)] = ScriptType.Boolean,
                [(TokenType.GreaterThenOrEqual, ScriptType.Null, ScriptType.DateTime)] = ScriptType.Boolean,
                [(TokenType.GreaterThenOrEqual, ScriptType.DateTime, ScriptType.Null)] = ScriptType.Boolean,
                [(TokenType.GreaterThenOrEqual, ScriptType.TimeSpan, ScriptType.TimeSpan)] = ScriptType.Boolean,
                [(TokenType.GreaterThenOrEqual, ScriptType.Null, ScriptType.TimeSpan)] = ScriptType.Boolean,
                [(TokenType.GreaterThenOrEqual, ScriptType.TimeSpan, ScriptType.Null)] = ScriptType.Boolean,

                [(TokenType.LessThen, ScriptType.Null, ScriptType.Null)] = ScriptType.Null,
                [(TokenType.LessThen, ScriptType.Number, ScriptType.Number)] = ScriptType.Boolean,
                [(TokenType.LessThen, ScriptType.Null, ScriptType.Number)] = ScriptType.Boolean,
                [(TokenType.LessThen, ScriptType.Number, ScriptType.Null)] = ScriptType.Boolean,
                [(TokenType.LessThen, ScriptType.String, ScriptType.String)] = ScriptType.Boolean,
                [(TokenType.LessThen, ScriptType.Null, ScriptType.String)] = ScriptType.Boolean,
                [(TokenType.LessThen, ScriptType.String, ScriptType.Null)] = ScriptType.Boolean,
                [(TokenType.LessThen, ScriptType.DateTime, ScriptType.DateTime)] = ScriptType.Boolean,
                [(TokenType.LessThen, ScriptType.Null, ScriptType.DateTime)] = ScriptType.Boolean,
                [(TokenType.LessThen, ScriptType.DateTime, ScriptType.Null)] = ScriptType.Boolean,
                [(TokenType.LessThen, ScriptType.TimeSpan, ScriptType.TimeSpan)] = ScriptType.Boolean,
                [(TokenType.LessThen, ScriptType.Null, ScriptType.TimeSpan)] = ScriptType.Boolean,
                [(TokenType.LessThen, ScriptType.TimeSpan, ScriptType.Null)] = ScriptType.Boolean,

                [(TokenType.LessThenOrEqual, ScriptType.Null, ScriptType.Null)] = ScriptType.Null,
                [(TokenType.LessThenOrEqual, ScriptType.Number, ScriptType.Number)] = ScriptType.Boolean,
                [(TokenType.LessThenOrEqual, ScriptType.Null, ScriptType.Number)] = ScriptType.Boolean,
                [(TokenType.LessThenOrEqual, ScriptType.Number, ScriptType.Null)] = ScriptType.Boolean,
                [(TokenType.LessThenOrEqual, ScriptType.String, ScriptType.String)] = ScriptType.Boolean,
                [(TokenType.LessThenOrEqual, ScriptType.Null, ScriptType.String)] = ScriptType.Boolean,
                [(TokenType.LessThenOrEqual, ScriptType.String, ScriptType.Null)] = ScriptType.Boolean,
                [(TokenType.LessThenOrEqual, ScriptType.DateTime, ScriptType.DateTime)] = ScriptType.Boolean,
                [(TokenType.LessThenOrEqual, ScriptType.Null, ScriptType.DateTime)] = ScriptType.Boolean,
                [(TokenType.LessThenOrEqual, ScriptType.DateTime, ScriptType.Null)] = ScriptType.Boolean,
                [(TokenType.LessThenOrEqual, ScriptType.TimeSpan, ScriptType.TimeSpan)] = ScriptType.Boolean,
                [(TokenType.LessThenOrEqual, ScriptType.Null, ScriptType.TimeSpan)] = ScriptType.Boolean,
                [(TokenType.LessThenOrEqual, ScriptType.TimeSpan, ScriptType.Null)] = ScriptType.Boolean,

            };


            UnaryOperators = new Dictionary<(TokenType, ScriptType), ScriptType>()
            {
                [(TokenType.Plus, ScriptType.Null)] = ScriptType.Null,
                [(TokenType.Plus, ScriptType.Number)] = ScriptType.Null,

                [(TokenType.Minus, ScriptType.Null)] = ScriptType.Null,
                [(TokenType.Minus, ScriptType.Number)] = ScriptType.Number,

                [(TokenType.Not, ScriptType.Null)] = ScriptType.Null,
                [(TokenType.Not, ScriptType.Boolean)] = ScriptType.Boolean,
            };
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
                e.Type = TypeHelper.SystemToScriptType(constant.PropertyType);
                e.Property = constant;
            }
            else
            {
                e.Type = ScriptType.Unknown;
                Errors.Add(ScriptError.UnresolvedIdentifier(e.Token.Index, e.Name));
            }
        }

        void ResolveBinaryExpression(BinaryExpression e)
        {
            Resolve(e.Left);
            Resolve(e.Right);
            var tuple = (e.Token.Type, e.Left.Type, e.Right.Type);
            if (BinaryOperators.TryGetValue(tuple, out var type))
            {
                e.Type = type;
                if (e.Left.Type == ScriptType.Null)
                    e.Left.Type = e.Right.Type;
                if (e.Right.Type == ScriptType.Null)
                    e.Right.Type = e.Left.Type;
            }
            else
            {
                e.Type = ScriptType.Unknown;
                Errors.Add(ScriptError.UnresolvedOperator(e.Token.Index, e.Token.Contents, e.Left.Type, e.Right.Type));
            }
        }

        void ResolveUnaryExpression(UnaryExpression e)
        {
            Resolve(e.Right);
            var tuple = (e.Token.Type, e.Right.Type);
            if (UnaryOperators.TryGetValue(tuple, out var type))
            {
                e.Type = type;                
            }
            else 
            {
                e.Type = ScriptType.Unknown;
                Errors.Add(ScriptError.UnresolvedOperator(e.Token.Index, e.Token.Contents, e.Right.Type));
            }
        }

        void ResolveFieldExpression(FieldExpression e)
        {
            Field field = EntityFields.Find(f => f.Name == e.Name);
            if (field == null)
            {
                Errors.Add(ScriptError.UnresolvedField(e.Token.Index, e.Name));
            }
            else
            {
                e.Type = ScriptType.Unknown;
                e.Type = TypeHelper.SystemToScriptType(field.Type);
            }
        }

        void ResolveFunctionCallExpression(FunctionExpression e)
        {
            var argumentTypes = new List<ScriptType>();
            foreach(var argument in e.Arguments)
            {
                Resolve(argument);
                argumentTypes.Add(argument.Type);
            }
            var function = GetFunction(e.Name, argumentTypes);
            if (function == null)
            {
                e.Type = ScriptType.Unknown;
                Errors.Add(ScriptError.UnresolvedFunction(e.Token.Index, e.Name, argumentTypes));
            }
            else
            {
                e.Function = function;
                e.Type = function.ReturnType;
            }
        }
     
        public PropertyInfo GetConstant(string name)
        {
            return Runtime.Constants.Find(constant => constant.Name == name);
        }

        public Function GetFunction(string searchName, List<ScriptType> searchArguments)
        {
            if(searchName == "ifs")
            {
                var function = TryResolveIfs(searchArguments);
                if (function != null) return function;
            }
            if(searchName == "cases")
            {
                var function = ResolveCases(searchArguments);
                if (function != null) return function;
            }

            var argumentTypes = searchArguments.Select(TypeHelper.ScriptToSystemType);
            var method = Runtime.Functions.FirstOrDefault(function =>
                function.Name == searchName &&
                argumentTypes.SequenceEqual(function.GetParameters().Select(param => param.ParameterType))
            );

            if (method == null) return null;

            return new Function
            {
                Name = method.Name,
                ArgumentTypes = method.GetParameters().Select(param => TypeHelper.SystemToScriptType(param.ParameterType)).ToList(),
                ReturnType = TypeHelper.SystemToScriptType(method.ReturnType), 
                Method = method
            };
        }

        Function TryResolveIfs(List<ScriptType> arguments)
        {

            if (arguments.Count < 3)
            {
                return null;
            }
            bool hasDefault = arguments.Count % 2 == 1;
            var numIfBranches = arguments.Count / 2;
            var resultsType = arguments[1];
            for (var branch = 0; branch < numIfBranches; branch++)
            {
                if (arguments[branch * 2] != ScriptType.Null && arguments[branch * 2] != ScriptType.Boolean) return null;
                if (arguments[branch * 2 + 1] != ScriptType.Null && arguments[branch * 2 + 1] != resultsType) return null;
            }

            if (hasDefault)
            {
                if (arguments[arguments.Count - 1] != ScriptType.Null && arguments[arguments.Count - 1] != resultsType) return null;
            }

            var argumentTypes = new List<ScriptType>();
            for (var branch = 0; branch < numIfBranches; branch++)
            {
                argumentTypes.Add(ScriptType.Boolean);
                argumentTypes.Add(resultsType);
            }

            if(hasDefault)
            {
                argumentTypes.Add(resultsType);
            }
            
            return  new Function
            {
                Name = "ifs",
                ReturnType = resultsType,
                ArgumentTypes = argumentTypes,
                Method = typeof(LibStandard).GetMethod("ifs")
            };
        }

        Function ResolveCases(List<ScriptType> arguments)
        {
            if(arguments.Count < 3) return null;
            bool hasDefault = arguments.Count % 2 == 0;
            var numCases = (arguments.Count - 1) / 2;
            var resultsType = arguments[2];
            var casesType = arguments[0];
            for (var kase = 0; kase < numCases; kase++)
            {
                if(casesType == ScriptType.Null && arguments[1 + kase * 2] != ScriptType.Null && arguments[1 + kase * 2] != ScriptType.Unknown)
                {
                    casesType = arguments[1 + kase * 2];
                }

                if (resultsType == ScriptType.Null && arguments[2 + kase * 2] != ScriptType.Null && arguments[ + kase * 2] != ScriptType.Unknown)
                {
                    resultsType = arguments[1 + kase * 2];
                }
                if (arguments[1 + kase * 2] != ScriptType.Null && arguments[1 + kase * 2] != casesType) return null;
                if (arguments[2 + kase * 2] != ScriptType.Null && arguments[2 + kase * 2] != resultsType) return null;
            }

            if (hasDefault)
            {
                if (arguments[arguments.Count - 1] != ScriptType.Null && arguments[arguments.Count - 1] != resultsType) return null;
            }

            var argumentTypes = new List<ScriptType>();
            argumentTypes.Add(casesType);
            for (var kase = 0; kase < numCases; kase++)
            {
                argumentTypes.Add(casesType);
                argumentTypes.Add(resultsType);
            }

            if (hasDefault)
            {
                argumentTypes.Add(resultsType);
            }

            return new Function
            {
                Name = "cases",
                ReturnType = resultsType,
                ArgumentTypes = argumentTypes,
                Method = typeof(LibStandard).GetMethod("cases")
            };
        }
    }
}
