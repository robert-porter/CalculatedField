using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniLanguage
{
    class Compiler : Visitor
    {
        public List<Instruction> Instructions { get; set; }
        
        public Compiler()
        {
            Instructions = new List<Instruction>();
        }

        public override void Visit(BinaryExpression binaryExpression)
        {
            binaryExpression.Left.Accept(this);
            binaryExpression.Right.Accept(this);
            switch (binaryExpression.Op) {  
                case BinaryExpression.Operator.Add: 
                    Instructions.Add(new AddInstruction());
                    break;
                case BinaryExpression.Operator.Less:
                    Instructions.Add(new LessInstruction());
                    break;
            }
        }

        public override void Visit(Number number)
        {
            double value = double.Parse(number.Value);
            Instructions.Add(new LoadNumberInstruction(new NumberValue(value)));
        }

        public override void Visit(Identifier identifier)
        {
            Instructions.Add(new LoadVariableInstruction(identifier.Name));
        }

        public override void Visit(IfStatement ifStatement)
        {
            JumpOnFalseInstruction jumpOnFalseInstruction = new JumpOnFalseInstruction();
            JumpInstruction jumpPastElseInstruction = null;

            ifStatement.Condition.Accept(this);
            Instructions.Add(jumpOnFalseInstruction);
            ifStatement.Consequent.Accept(this);
            if (ifStatement.Alternate != null)
            {
                jumpPastElseInstruction = new JumpInstruction();
                Instructions.Add(jumpPastElseInstruction);
            }
            jumpOnFalseInstruction.JumpLocation = Instructions.Count;
            if (ifStatement.Alternate != null)
            {
                ifStatement.Alternate.Accept(this);
                jumpPastElseInstruction.JumpLocation = Instructions.Count;
            }
            
        }

        public override void Visit(BlockStatement blockStatement)
        {
            foreach(Statement statement in blockStatement.Statements)
            {
                statement.Accept(this);
            }
        }

        public override void Visit(ExpressionStatement expessionStatement)
        {
            expessionStatement.Expression.Accept(this);
            Instructions.Add(new PopInstruction());
        }

        public override void Visit(AssignmentStatement assignmentStatement)
        {
            if(assignmentStatement.Right != null)
            {
                assignmentStatement.Right.Accept(this);
                Instructions.Add(new StoreVariableInstruction(assignmentStatement.Left.Name));
            }
        }

        public override void Visit(VarDeclarationStatement varDeclStatement)
        {
            NewVariableInstruction newVariableInstuction = new NewVariableInstruction(varDeclStatement.Identifier);
            Instructions.Add(newVariableInstuction);

            if (varDeclStatement.InitialValue != null)
            {
                varDeclStatement.InitialValue.Accept(this);
                StoreVariableInstruction storeVariableInstruction = new StoreVariableInstruction(varDeclStatement.Identifier);
                Instructions.Add(storeVariableInstruction);
            }
        }
    }
}
