using System.Collections.Generic;

namespace CalculatedField
{
    class Compiler 
    {
        List<Instruction> Instructions { get; set; }
        List<ScriptValue> Constants;
        List<string> VariableLocations;
        List<string> FunctionLocations;

        public Compiler()
        {
            Instructions = new List<Instruction>();
            FunctionLocations = new List<string>();
            VariableLocations = new List<string>();
            Constants = new List<ScriptValue>();
        }

        public CompiledScript Compile(BlockExpression program)
        {
            CompileBlockExpression(program);
            return new CompiledScript
            {
                Constants = Constants,
                Instructions = Instructions, 
                Variables = VariableLocations,
                Functions = FunctionLocations
            };
        }

        void Compile(Expression expression)
        {
            switch(expression)
            {
                case IntegerLiteralExpression e:
                    CompileIntegerLiteralExpression(e);
                    break;
                case DecimalLiteralExpression e:
                    CompileDecimalLiteralExpression(e);
                    break;
                case BoolLiteralExpression e:
                    CompileBoolLiteralExpression(e);
                    break;
                case NullLiteralExpression e:
                    CompileNullLiteralExpression(e);
                    break;
                case DateTimeLiteralExpression e:
                    CompileDateTimeLiteralExpression(e);
                    break;
                case BlockExpression e:
                    CompileBlockExpression(e);
                    break;
                case IfExpression e:
                    CompileIfExpression(e);
                    break;
                case BinaryExpression e:
                    CompileBinaryExpression(e);
                    break;
                case UnaryExpression e:
                    CompileUnaryExpression(e);
                    break;
                case AssignmentExpression e:
                    CompileAssignmentExpression(e);
                    break;
                case FunctionCallExpression e:
                    CompileFunctionCallExpression(e);
                    break;
                case IdentifierExpression e:
                    CompileIdentifierExpression(e);
                    break;                 
            }
        }

        public void CompileBlockExpression(BlockExpression program)
        {
            foreach(var expression in program.Expressions)
            {
                Compile(expression);
            }
        } 

        public void CompileUnaryExpression(UnaryExpression expression)
        {
            Compile(expression.Right);

            switch(expression.Operator)
            {
                case UnaryOperator.Plus: // do nothing
                    break;
                case UnaryOperator.Minus:
                    Instructions.Add(Instruction.Negate);
                    break;
                case UnaryOperator.Not:
                    Instructions.Add(Instruction.Not);
                    break;
            }
        }
         
        public void CompileBinaryExpression(BinaryExpression expression)
        {
            Compile(expression.Left);
            Compile(expression.Right);

            switch (expression.Operator) {  
                case BinaryOperator.Add:
                    Instructions.Add(Instruction.Add);
                    break;
                case BinaryOperator.Subtract:
                    Instructions.Add(Instruction.Subtract);
                    break;
                case BinaryOperator.Multiply:
                    Instructions.Add(Instruction.Multiply);
                    break;
                case BinaryOperator.Divide:
                    Instructions.Add(Instruction.Divide);
                    break;
                case BinaryOperator.Less:
                    Instructions.Add(Instruction.Less);
                    break;
                case BinaryOperator.LessOrEqual:
                    Instructions.Add(Instruction.LessOrEqual);
                    break;
                case BinaryOperator.Greater:
                    Instructions.Add(Instruction.Greater);
                    break;
                case BinaryOperator.GreaterOrEqual:
                    Instructions.Add(Instruction.GreaterOrEqual);
                    break;
                case BinaryOperator.CompareEqual:
                    Instructions.Add(Instruction.Equal);
                    break;
                case BinaryOperator.CompareNotEqual:
                    Instructions.Add(Instruction.NotEqual);
                    break;
                case BinaryOperator.And:
                    Instructions.Add(Instruction.And);
                    break;
                case BinaryOperator.Or:
                    Instructions.Add(Instruction.Or);
                    break;

            }
        }

        public void CompileNullLiteralExpression(NullLiteralExpression literal)
        {
            Constants.Add(new ScriptValue());
            Instructions.Add(Instruction.PushConstant);
            Instructions.Add((Instruction)Constants.Count - 1);
        }

        public void CompileDecimalLiteralExpression(DecimalLiteralExpression literal)
        {
            Constants.Add(new ScriptValue(literal.DecimalValue));
            Instructions.Add(Instruction.PushConstant);
            Instructions.Add((Instruction)Constants.Count - 1);
        }

        public void CompileBoolLiteralExpression(BoolLiteralExpression literal)
        {
            Constants.Add(new ScriptValue(literal.BoolValue));
            Instructions.Add(Instruction.PushConstant);
            Instructions.Add((Instruction)Constants.Count - 1);
        }

        public void CompileStringLiteralExpression(StringLiteralExpression literal)
        {
            Constants.Add(new ScriptValue(literal.Value));
            Instructions.Add(Instruction.PushConstant);
            Instructions.Add((Instruction)Constants.Count - 1);
        }

        public void CompileIntegerLiteralExpression(IntegerLiteralExpression expression)
        {
            Constants.Add(new ScriptValue(expression.IntegerValue));
            Instructions.Add(Instruction.PushConstant);
            Instructions.Add((Instruction)Constants.Count - 1);
        }

        public void CompileDateTimeLiteralExpression(DateTimeLiteralExpression expression)
        {
            Constants.Add(new ScriptValue(expression.DateTimeValue));
            Instructions.Add(Instruction.PushConstant);
            Instructions.Add((Instruction)Constants.Count - 1);
        }

        public void CompileIdentifierExpression(IdentifierExpression identifier)
        {
            Instructions.Add(Instruction.PushVariable);
            int location = VariableLocations.Count;
            if (VariableLocations.Contains(identifier.Name)) {
                location = VariableLocations.IndexOf(identifier.Name);
            }
            else
            {
                VariableLocations.Add(identifier.Name);
            }
            Instructions.Add((Instruction)location);
        }

        public void CompileIfExpression(IfExpression expression)
        {
            int jumpPastElseArgLocation = 0;
            int jumpOnFalseArgLocation = 0;

            Compile(expression.Condition);

            Instructions.Add(Instruction.JumpOnFalse);
            jumpOnFalseArgLocation = Instructions.Count;
            Instructions.Add((Instruction)0); // placeholder for jump on false argument
            
            Compile(expression.ThenExpression);
            if (expression.ElseExpression != null)
            {
                Instructions.Add(Instruction.Jump);
                jumpPastElseArgLocation = Instructions.Count;
                Instructions.Add((Instruction)0); // placeholder for jump argument
            }
            Instructions[jumpOnFalseArgLocation] = (Instruction) Instructions.Count;
            if (expression.ElseExpression != null)
            {
                Compile(expression.ElseExpression);
                Instructions[jumpPastElseArgLocation] = (Instruction) Instructions.Count;
            }
            
        }

        public void CompileAssignmentExpression(AssignmentExpression assignment)
        {
            int location = VariableLocations.Count;
            if (VariableLocations.Contains(assignment.Left.Name))
            {
                location = VariableLocations.IndexOf(assignment.Left.Name);
            }
            else
            {
                VariableLocations.Add(assignment.Left.Name);
            }
            Compile(assignment.Right);
            Instructions.Add(Instruction.Store);
            Instructions.Add((Instruction)location);
            Instructions.Add(Instruction.PushVariable);
            Instructions.Add((Instruction)location);
        }

        public void CompileFunctionCallExpression(FunctionCallExpression call)
        {
            foreach (Expression argument in call.Arguments)
            {
                Compile(argument); // this leaves all of the expression results on the stack
            }
            Instructions.Add(Instruction.Call);
            int location = FunctionLocations.Count;
            if (FunctionLocations.Contains(call.Name))
            {
                location = FunctionLocations.IndexOf(call.Name);
            }
            else
            {
                FunctionLocations.Add(call.Name);
            }
            Instructions.Add((Instruction)FunctionLocations.Count - 1); // placeholder for the function location
        }
    }
}
