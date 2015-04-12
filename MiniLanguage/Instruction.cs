using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MiniLanguage
{
    class Instruction
    {
        public virtual void Execute(VirtualMachine machine, ref int ip)
        {
        }

    }

    class PopInstruction : Instruction
    {
        public override void Execute(VirtualMachine machine, ref int ip)
        {
            machine.Pop();
        }
    }

    class AddInstruction : Instruction
    {
        public override void Execute(VirtualMachine machine, ref int ip)
        {
            NumberValue right = machine.Pop() as NumberValue;
            NumberValue left = machine.Pop() as NumberValue;

            machine.Push(new NumberValue(left.DoubleVal + right.DoubleVal));
        }
    }
    class SubtractInstruction : Instruction
    {
        public override void Execute(VirtualMachine machine, ref int ip)
        {
            NumberValue right = machine.Pop() as NumberValue;
            NumberValue left = machine.Pop() as NumberValue;

            machine.Push(new NumberValue(left.DoubleVal - right.DoubleVal));
        }
    }
    class MultiplyInstruction : Instruction
    {
        public override void Execute(VirtualMachine machine, ref int ip)
        {
            NumberValue right = machine.Pop() as NumberValue;
            NumberValue left = machine.Pop() as NumberValue;

            machine.Push(new NumberValue(left.DoubleVal * right.DoubleVal));
        }
    }
    class DivideInstruction : Instruction
    {
        public override void Execute(VirtualMachine machine, ref int ip)
        {
            NumberValue right = machine.Pop() as NumberValue;
            NumberValue left = machine.Pop() as NumberValue;

            machine.Push(new NumberValue(left.DoubleVal / right.DoubleVal));
        }
    }

    class LessInstruction : Instruction
    {
        public override void Execute(VirtualMachine machine, ref int ip)
        {
            NumberValue right = machine.Pop() as NumberValue;
            NumberValue left = machine.Pop() as NumberValue;


            machine.Push(new BoolValue(left.DoubleVal < right.DoubleVal));
        }
    }

    class LoadNumberInstruction : Instruction
    {
        public LoadNumberInstruction(NumberValue number)
        {
            Number = number;
        }
        public NumberValue Number;

        public override void Execute(VirtualMachine machine, ref int ip)
        {
            machine.Push(Number);
        }
    }

    class LoadVariableInstruction : Instruction
    {
        public String Identifier;
        public LoadVariableInstruction(String identifier)
        {
            Identifier = identifier;
        }
        public override void Execute(VirtualMachine machine, ref int ip)
        {
            machine.Push(machine.GetVar(Identifier));
        }

    }

    class StoreVariableInstruction : Instruction
    {
        String Identifier;
        public StoreVariableInstruction(String identifier)
        {
            Identifier = identifier;
        }

        public override void Execute(VirtualMachine machine, ref int ip)
        {
            machine.SetVar(Identifier, machine.Pop());
        }
    }

    class NewVariableInstruction : Instruction
    {
        String Identifier;

        public NewVariableInstruction(String identifier)
        {
            Identifier = identifier;
        }
        public override void Execute(VirtualMachine machine, ref int ip)
        {
            machine.AddVar(Identifier);
        }

    }
    class JumpOnFalseInstruction : Instruction
    {
        public int JumpLocation { get; set; }
        public override void Execute(VirtualMachine machine, ref int ip)
        {
            BoolValue bVal = machine.Pop() as BoolValue;
            // -1 since the instruction pointer will be incremented after the instruction is executed.
            if (!bVal.BoolVal)
                ip = (int)JumpLocation - 1; 
        }
    }

    class CallInstruction : Instruction
    {
        public String Name;
        public int Location;
        public List<String> ArgumentIdentifiers;

        public override void Execute(VirtualMachine machine, ref int ip)
        {
            machine.PushReturnAddress(ip);
            machine.PushScope();

            ip = Location;
        }
    }

    class ReturnInstruction : Instruction
    {
        
        public override void Execute(VirtualMachine machine, ref int ip)
        {
            int address = machine.PopReturnAddress();
            machine.PopScope();
            ip = address;
        }
    }

    class JumpInstruction : Instruction
    {
        public int JumpLocation { get; set; }

        public override void Execute(VirtualMachine machine, ref int ip)
        {
            // -1 since the instruction pointer will be incremented after the instruction is executed.
            ip = JumpLocation - 1;
        }
    }


}
