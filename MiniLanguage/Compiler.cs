using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniLanguage
{
    class Compiler : Visitor
    {

        public Dictionary<String, int> FunctionLocations;
        public List<Instruction> Instructions { get; set; }
        public int StartAddress;
        
        public Compiler()
        {
            Instructions = new List<Instruction>();
            FunctionLocations = new Dictionary<string, int>();
        }

        public void Compile(Node root)
        {
            root.Accept(this);
           
        }

        public override void Visit(UnaryExpression unaryExpression)
        {
            throw new NotImplementedException();
        }
        
        public override void Visit(WhileStatement whileStatement)
        {
            throw new NotImplementedException();
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

        public override void Visit(NumberExpression number)
        {
            double value = double.Parse(number.Value);
            Instructions.Add(new LoadNumberInstruction(new NumberValue(value)));
        }

        public override void Visit(IdentifierExpression identifier)
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

        public override void Visit(FunctionDeclarationStatement funcDeclStatement)
        {

            // Does not work for nested functions.
            // currently need all function definitions at top of file, they cannot be between statements 
            // this will be changed later...
            FunctionLocations.Add(funcDeclStatement.Name, Instructions.Count);

            for (int i = funcDeclStatement.Arguments.Count - 1; i >= 0; i-- )
            {
                Instructions.Add(new NewVariableInstruction(funcDeclStatement.Arguments[i].Name));
                Instructions.Add(new StoreVariableInstruction(funcDeclStatement.Arguments[i].Name));
            }
            funcDeclStatement.Body.Accept(this);

            // if there is no manual return.
            Instructions.Add(new LoadNumberInstruction(new NumberValue(0))); 
            Instructions.Add(new ReturnInstruction());

            StartAddress = Instructions.Count;
        }

        public override void Visit(ReturnStatement returnStatement)
        {
            // this will leave the result on the stack.
            returnStatement.Expression.Accept(this);
            Instructions.Add(new ReturnInstruction());
        }

        public override void Visit(FunctionCallExpression funcCallExpression)
        {
            String name = funcCallExpression.Identifier.Name;
            


            if (FunctionLocations.ContainsKey(name))
            {
                CallInstruction callInstruction = new CallInstruction();
                callInstruction.Name = name;
                // minus 1 since we will increment the pointer after the call expression is executed.
                callInstruction.Location = FunctionLocations[name] - 1; 
                foreach (Expression argument in funcCallExpression.Arguments)
                {
                    argument.Accept(this); // this leaves all of the expression results on the stack
                }
                Instructions.Add(callInstruction);

            }
            else
            {
                throw new Exception("Calling undeclared function.");
            }
            
        }
    }
}
