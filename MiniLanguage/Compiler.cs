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
        
        // a list of pairs of function names and call argument indices.
        // after all functions are compiled, go back and fill in all the call instruction with the 
        // funciton lcocations.
        public List<Tuple<String, int>> CallArguments;
        public List<Instruction> Instructions { get; set; }
        public List<Value> Constants;
        public int StartAddress;

        public int NextVariableLocation;
        Environment VariableLocations;

        public void PushScope()
        {
            Environment newEnv = new Environment();
            newEnv.Parent = VariableLocations;
            VariableLocations = newEnv;
        }
        public void PopScope()
        {
            VariableLocations = VariableLocations.Parent;
        }

        public Compiler()
        {
            Instructions = new List<Instruction>();
            FunctionLocations = new Dictionary<string, int>();
            Constants = new List<Value>();
            Constants.Add(new Value(0.0f));
            VariableLocations = new Environment();
            CallArguments = new List<Tuple<string, int>>();
            PushScope();
        }


        public void Compile(ProgramNode program)
        {
            program.Accept(this);


            // convert function names to function locations.
            foreach (Tuple<String, int> callArg in CallArguments)
            {
                Instructions[callArg.Item2] = (Instruction) FunctionLocations[callArg.Item1];
            }
           
        }

        public override void Visit(ProgramNode program)
        {
            foreach (FunctionDeclarationStatement funcDecl in program.FunctionDeclarations)
            {
                funcDecl.Accept(this);
            }

            foreach (VarDeclarationStatement varDecl in program.VariableDeclarations)
            {
                varDecl.Accept(this);
            }

        } 

        public override void Visit(UnaryExpression unaryExpression)
        {
            unaryExpression.Argument.Accept(this);

            switch(unaryExpression.Op)
            {
                case UnaryExpression.Operator.Plus: // do nothing
                    break;
                case UnaryExpression.Operator.Minus:
                    Instructions.Add(Instruction.Negate);
                    break;
                case UnaryExpression.Operator.Not:
                    Instructions.Add(Instruction.Not);
                    break;
            }
        }
        
        public override void Visit(WhileStatement whileStatement)
        {
            int conditionLocation = Instructions.Count;
            int jumpArgLocation = 0;
            whileStatement.Condition.Accept(this);  // leaves the result on the stack
            Instructions.Add(Instruction.JumpOnFalse);
            // keep track of argument location, it will be set after the body is compiled
            jumpArgLocation = Instructions.Count; 
            Instructions.Add((Instruction)0); // just allocating the spot

            whileStatement.Body.Accept(this);
            Instructions.Add(Instruction.Jump);
            Instructions.Add((Instruction)conditionLocation);
            Instructions[jumpArgLocation] = (Instruction) Instructions.Count;
        }

  
        public override void Visit(BinaryExpression binaryExpression)
        {
            binaryExpression.Left.Accept(this);
            binaryExpression.Right.Accept(this);
            switch (binaryExpression.Op) {  
                case BinaryExpression.Operator.Add:
                    Instructions.Add(Instruction.Add);
                    break;
                case BinaryExpression.Operator.Subtract:
                    Instructions.Add(Instruction.Subtract);
                    break;
                case BinaryExpression.Operator.Multiply:
                    Instructions.Add(Instruction.Multiply);
                    break;
                case BinaryExpression.Operator.Divide:
                    Instructions.Add(Instruction.Divide);
                    break;
                case BinaryExpression.Operator.Less:
                    Instructions.Add(Instruction.Less);
                    break;
                case BinaryExpression.Operator.LessOrEqual:
                    Instructions.Add(Instruction.LessOrEqual);
                    break;
                case BinaryExpression.Operator.Greater:
                    Instructions.Add(Instruction.Greater);
                    break;
                case BinaryExpression.Operator.GreaterOrEqual:
                    Instructions.Add(Instruction.GreaterOrEqual);
                    break;
                case BinaryExpression.Operator.DoubleEqual:
                    Instructions.Add(Instruction.DoubleEqual);
                    break;
                case BinaryExpression.Operator.NotEqual:
                    Instructions.Add(Instruction.NotEqual);
                    break;
                case BinaryExpression.Operator.And:
                    Instructions.Add(Instruction.And);
                    break;
                case BinaryExpression.Operator.Or:
                    Instructions.Add(Instruction.Or);
                    break;

            }
        }

        public override void Visit(NumberExpression number)
        {
            double value = double.Parse(number.Value);
            Constants.Add(new Value(value));
            Instructions.Add(Instruction.LoadNumber);
            Instructions.Add((Instruction)Constants.Count - 1);

        }

        public override void Visit(BoolExpression boolExpression)
        {
            if (boolExpression.Value)
                Instructions.Add(Instruction.LoadTrue);
            else
                Instructions.Add(Instruction.LoadFalse);

        }

        public override void Visit(IdentifierExpression identifier)
        {
            Instructions.Add(Instruction.LoadVariable);
            Instructions.Add((Instruction)VariableLocations.GetLocation(identifier.Name));
        }

        public override void Visit(ArrayIndexExpression arrayIndexExpression)
        {
            arrayIndexExpression.IndexExpression.Accept(this);
            Instructions.Add(Instruction.LoadOffsetVariable);
            Instructions.Add((Instruction)VariableLocations.GetLocation(arrayIndexExpression.Name));
        }

        public override void Visit(IfStatement ifStatement)
        {
            int jumpPastElseArgLocation = 0;
            int jumpOnFalseArgLocation = 0;

            ifStatement.Condition.Accept(this);
            Instructions.Add(Instruction.JumpOnFalse);
            jumpOnFalseArgLocation = Instructions.Count;
            Instructions.Add((Instruction)0); // placeholder for jump on false argument
            
            ifStatement.Consequent.Accept(this);
            if (ifStatement.Alternate != null)
            {
                Instructions.Add(Instruction.Jump);
                jumpPastElseArgLocation = Instructions.Count;
                Instructions.Add((Instruction)0); // placeholder for jump argument
            }
            Instructions[jumpOnFalseArgLocation] = (Instruction) Instructions.Count;
            if (ifStatement.Alternate != null)
            {
                ifStatement.Alternate.Accept(this);
                Instructions[jumpPastElseArgLocation] = (Instruction) Instructions.Count;
            }
            
        }

        public override void Visit(BlockStatement blockStatement)
        {
            PushScope();
            foreach(Statement statement in blockStatement.Statements)
            {
                statement.Accept(this);
            }
            PopScope();
        }

        public override void Visit(ExpressionStatement expressionStatement)
        {
            expressionStatement.Expression.Accept(this);
            Instructions.Add(Instruction.Pop);
        }

        public override void Visit(AssignmentExpression assignmentStatement)
        {
            if(assignmentStatement.Right != null)
            {
                assignmentStatement.Right.Accept(this);


                if (assignmentStatement.Left is ArrayIndexExpression)
                {
                    // this is probably not the most effecient way to do this...
                    ArrayIndexExpression arrayIndexExpression = assignmentStatement.Left as ArrayIndexExpression;
                    arrayIndexExpression.IndexExpression.Accept(this);
                    Instructions.Add(Instruction.Dup); // duplicate this since it's needed to put the result back on the stack
                    Instructions.Add(Instruction.StoreOffsetVariable);
                    Instructions.Add((Instruction)VariableLocations.GetLocation(assignmentStatement.Left.Name));
                    Instructions.Add(Instruction.LoadOffsetVariable);
                    Instructions.Add((Instruction)VariableLocations.GetLocation(assignmentStatement.Left.Name));
                    
                }
                else
                {
                    Instructions.Add(Instruction.StoreVariable);
                    Instructions.Add((Instruction)VariableLocations.GetLocation(assignmentStatement.Left.Name));
                    Instructions.Add(Instruction.LoadVariable);
                    Instructions.Add((Instruction)VariableLocations.GetLocation(assignmentStatement.Left.Name));
                }
            }
        }

        public override void Visit(VarDeclarationStatement varDeclStatement)
        {
            VariableLocations.AddLocation(varDeclStatement.Identifier, NextVariableLocation);

            if (varDeclStatement.IsArray)
            {
                Instructions.Add(Instruction.NewArray);
                Instructions.Add((Instruction)varDeclStatement.ArraySize);
                NextVariableLocation += varDeclStatement.ArraySize;
            }
            else
            {
                Instructions.Add(Instruction.NewVariable);
                if (varDeclStatement.InitialValue != null)
                {
                    varDeclStatement.InitialValue.Accept(this);

                    Instructions.Add(Instruction.StoreVariable);
                    Instructions.Add((Instruction)NextVariableLocation);
                }
                NextVariableLocation++;
            }
        }

        public override void Visit(FunctionDeclarationStatement funcDeclStatement)
        {
            PushScope();
            NextVariableLocation = 0;

            FunctionLocations.Add(funcDeclStatement.Name, Instructions.Count);

            for (int i = funcDeclStatement.Arguments.Count - 1; i >= 0; i-- )
            {
                Instructions.Add(Instruction.NewVariable);
                Instructions.Add(Instruction.StoreVariable);
                Instructions.Add((Instruction)NextVariableLocation);
                VariableLocations.AddLocation(funcDeclStatement.Arguments[i].Name, NextVariableLocation);
                NextVariableLocation++;
            }
            funcDeclStatement.Body.Accept(this);

            // return 0
            Instructions.Add(Instruction.LoadNumber);
            Instructions.Add((Instruction)0);
            Instructions.Add(Instruction.Return);

            NextVariableLocation = 0;
            PopScope();
            
            // since we first generate code for all the global functions, the gloabl code starts after the last global function.
            StartAddress = Instructions.Count;
        }

        public override void Visit(ReturnStatement returnStatement)
        {
            // this will leave the result on the stack.
            returnStatement.Expression.Accept(this);
            Instructions.Add(Instruction.Return);
        }

        public override void Visit(FunctionCallExpression funcCallExpression)
        {
            String name = funcCallExpression.Identifier.Name;


            // minus 1 since we will increment the pointer after the call expression is executed.
            foreach (Expression argument in funcCallExpression.Arguments)
            {
                argument.Accept(this); // this leaves all of the expression results on the stack
            }
            Instructions.Add(Instruction.Call);
            Instructions.Add((Instruction)0); // placeholder for the function location
            CallArguments.Add(new Tuple<string, int>(name, Instructions.Count - 1));

            
        }
    }
}
