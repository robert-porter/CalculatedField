using System.Collections.Generic;

namespace CalculatedField
{
    class CodeGenerator 
    {
        List<Instruction> Instructions { get; set; }

        public CodeGenerator()
        {
            Instructions = new List<Instruction>();
        }

        public List<Instruction> GenerateProgram(BlockExpression program)
        {
            Generate(program);
            return Instructions;
        }

        void Generate(Expression expression)
        {
            switch(expression)
            {
                case LiteralExpression e:
                    GenerateLiteralExpression(e);
                    break;
                case BlockExpression e:
                    GenerateBlockExpression(e);
                    break;
                case IfExpression e:
                    GenerateIfExpression(e);
                    break;
                case BinaryExpression e:
                    GenerateBinaryExpression(e);
                    break;
                case UnaryExpression e:
                    GenerateUnaryExpression(e);
                    break;
                case AssignmentExpression e:
                    GenerateAssignmentExpression(e);
                    break;
                case FunctionExpression e:
                    GenerateFunctionCallExpression(e);
                    break;
                case IdentifierExpression e:
                    GenerateIdentifierExpression(e);
                    break;
                case FieldExpression e:
                    GenerateFieldExpression(e);
                    break;
            }
        }

        public void GenerateBlockExpression(BlockExpression block)
        {
            if (block.Expressions.Count == 0)
            {
                Instructions.Add(Instruction.PushConstant);
                Instructions.Add(0); // constant 0 is null
                return;
            }

            for(var i = 0; i < block.Expressions.Count - 1; i++)
            {
                Generate(block.Expressions[i]);
                Instructions.Add(Instruction.Pop);
            }

            Generate(block.Expressions[block.Expressions.Count - 1]);
        }

        public void GenerateUnaryExpression(UnaryExpression expression)
        {
            Generate(expression.Right);

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
         
        public void GenerateBinaryExpression(BinaryExpression expression)
        {
            Generate(expression.Left);
            Generate(expression.Right);

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

        public void GenerateLiteralExpression(LiteralExpression expression)
        {
            Instructions.Add(Instruction.PushConstant);
            Instructions.Add((Instruction)expression.Location);
        }

        public void GenerateIdentifierExpression(IdentifierExpression identifier)
        {
            Instructions.Add(Instruction.PushVariable);
            Instructions.Add((Instruction) identifier.Location);
        }

        public void GenerateFieldExpression(FieldExpression expression)
        {
            Instructions.Add(Instruction.PushField);
            Instructions.Add((Instruction)expression.Location);
        }

        public void GenerateIfExpression(IfExpression expression)
        {
            int jumpPastElseArgLocation = 0;
            int jumpOnFalseArgLocation = 0;

            Generate(expression.Condition);

            Instructions.Add(Instruction.JumpOnFalse);
            jumpOnFalseArgLocation = Instructions.Count;
            Instructions.Add((Instruction)0); // placeholder for jump on false argument
            
            Generate(expression.ThenExpression);
            if (expression.ElseExpression != null)
            {
                Instructions.Add(Instruction.Jump);
                jumpPastElseArgLocation = Instructions.Count;
                Instructions.Add((Instruction)0); // placeholder for jump argument
            }
            Instructions[jumpOnFalseArgLocation] = (Instruction) Instructions.Count;
            if (expression.ElseExpression != null)
            {
                Generate(expression.ElseExpression);
                Instructions[jumpPastElseArgLocation] = (Instruction) Instructions.Count;
            }
            
        }

        public void GenerateAssignmentExpression(AssignmentExpression assignment)
        {           
            Generate(assignment.Right);
            Instructions.Add(Instruction.Store);
            Instructions.Add((Instruction)assignment.Location);
            Instructions.Add(Instruction.PushVariable);
            Instructions.Add((Instruction)assignment.Location);
        }

        public void GenerateFunctionCallExpression(FunctionExpression call)
        {
            foreach (Expression argument in call.Arguments)
            {
                Generate(argument); // this leaves all of the expression results on the stack
            }
            Instructions.Add(Instruction.Call);
            Instructions.Add((Instruction)call.Location); 
        }
    }
}
