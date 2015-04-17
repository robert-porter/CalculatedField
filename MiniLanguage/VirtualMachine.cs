using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniLanguage
{
    class VirtualMachine
    {
        Stack<Value> Stack;
        Stack<int> ReturnAddresses;
        List<Value> Variables;
        Stack<int> FrameOffsets;

        public VirtualMachine()
        {
            Stack = new Stack<Value>();
            ReturnAddresses = new Stack<int>();

            FrameOffsets = new Stack<int>();
            Variables = new List<Value>();
            FrameOffsets.Push(0);
        }

        public void Push(Value value)
        {
            Stack.Push(value);
        }

        public Value Pop()
        {
            return Stack.Pop();
        }

        public Value Peek()
        {
            return Stack.Peek();
        }

        public void PushReturnAddress(int address)
        {
            ReturnAddresses.Push(address);
            FrameOffsets.Push(Variables.Count);
        }

        public int PopReturnAddress()
        {
            int frameOffset = FrameOffsets.Pop();
            Variables.RemoveRange(frameOffset, Variables.Count - frameOffset);
            return ReturnAddresses.Pop();
        }

        public void SetVar(int location, Value value)
        {
            int frameOffset = FrameOffsets.Peek();
            Variables[location + frameOffset] = value;
        }
        public Value GetVar(int location)
        {
            return Variables[location + FrameOffsets.Peek()];
        }
        public void AddVar(Value value)
        {
            Variables.Add(value);
        }
        public void AddArray(int size)
        {
            for (int i = 0; i < size; i++) // change this...
                Variables.Add(new Value());
        }

        public void Run(List<Instruction> instructions, List<Value> Constants, int startAddress)
        {
            int ip = startAddress;

            while (ip < instructions.Count)
            {
                Instruction instruction = instructions[ip];
                switch (instruction)
                {
                    case Instruction.Pop:
                        {
                            Pop();
                            break;
                        }
                    case Instruction.Dup:
                        {
                            Value value = Peek();
                            Push(value);
                            break;
                        }
                    case Instruction.Add:
                        {
                            Value right = Pop(); ;
                            Value left = Pop();
                            Push(new Value(left.DoubleVal + right.DoubleVal));
                            break;
                        }
                    case Instruction.Subtract:
                        {
                            Value right = Pop();
                            Value left = Pop();
                            Push(new Value(left.DoubleVal - right.DoubleVal));
                            break;
                        }

                    case Instruction.Multiply:
                        {
                            Value right = Pop();
                            Value left = Pop();
                            Push(new Value(left.DoubleVal * right.DoubleVal));
                            break;
                        }
                    case Instruction.Divide:
                        {
                            Value right = Pop();
                            Value left = Pop();
                            Push(new Value(left.DoubleVal / right.DoubleVal));
                            break;
                        }
                    case Instruction.Negate:
                        {
                            Value value = Pop();
                            Push(new Value(-value.DoubleVal));
                            break;
                        }
                    case Instruction.Less:
                        {
                            Value right = Pop();
                            Value left = Pop();
                            Push(new Value(left.DoubleVal < right.DoubleVal));
                            break;
                        }
                    case Instruction.LessOrEqual:
                        {
                            Value right = Pop();
                            Value left = Pop();
                            Push(new Value(left.DoubleVal <= right.DoubleVal));
                            break;
                        }
                    case Instruction.Greater:
                        {
                            Value right = Pop();
                            Value left = Pop();
                            Push(new Value(left.DoubleVal > right.DoubleVal));
                            break;
                        }
                    case Instruction.GreaterOrEqual:
                        {
                            Value right = Pop();
                            Value left = Pop();
                            Push(new Value(left.DoubleVal >= right.DoubleVal));
                            break;
                        }
                    case Instruction.DoubleEqual: 
                        {
                            Value right = Pop();
                            Value left = Pop();
                            Push(new Value(left.DoubleVal == right.DoubleVal));
                            break;
                        }
                    case Instruction.NotEqual:
                        {
                            Value right = Pop();
                            Value left = Pop();
                            Push(new Value(left.DoubleVal != right.DoubleVal));
                            break;
                        }
                    case Instruction.And:
                        {
                            Value right = Pop();
                            Value left = Pop();
                            Push(new Value(left.BoolVal && right.BoolVal));
                            break;
                        }
                    case Instruction.Or:
                        {
                            Value right = Pop();
                            Value left = Pop();
                            Push(new Value(left.BoolVal || right.BoolVal));
                            break;
                        }
                    case Instruction.Not:
                        {
                            Value value = Pop();
                            Push(new Value(!value.BoolVal));
                            break;
                        }
                    case Instruction.LoadTrue:
                        {
                            Push(new Value(true));
                            break;
                        }
                    case Instruction.LoadFalse:
                        {
                            Push(new Value(false));
                            break;
                        }
                    case Instruction.LoadNumber:
                        {
                            int location = (int)instructions[++ip];
                            Value number = Constants[location];
                            Push(number);
                            break;
                        }
                    case Instruction.LoadVariable:
                        {
                            int location = (int)instructions[++ip];
                            Push(GetVar(location));
                            break;
                        }
                    case Instruction.LoadOffsetVariable:
                        {
                            int location = (int)instructions[++ip];
                            int offset = (int) Pop().DoubleVal;
                            Push(GetVar(location + offset));
                            break;
                        }
                    case Instruction.StoreVariable:
                        {
                            int location = (int)instructions[++ip];
                            SetVar(location, Pop());
                            break;
                        }
                    case Instruction.StoreOffsetVariable:
                        {
                            int location = (int)instructions[++ip];
                            int offset = (int)Pop().DoubleVal;
                            SetVar(location + offset, Pop());
                            break;
                        }
                    case Instruction.NewVariable:
                        {
                            //int location = (int)instructions[++ip];
                            AddVar(new Value());
                            break;
                        }
                    case Instruction.NewArray:
                        {
                            int size = (int)instructions[++ip];
                            AddArray(size);
                            break;
                        }
                    case Instruction.JumpOnFalse:
                        {
                            int jumpLocation = (int)instructions[++ip];
                            Value bVal = Pop();
                            // -1 since the instruction pointer will be incremented after the instruction is executed.
                            if (!bVal.BoolVal)
                                ip = (int)jumpLocation - 1;
                            break;
                        }
                    case Instruction.Call:
                        {
                            int jumpLocation = (int)instructions[++ip];
                            PushReturnAddress(ip);
                            // -1 since the instruction pointer will be incremented after the instruction is executed.
                            ip = jumpLocation - 1;
                            break;
                        }
                    case Instruction.Return:
                        {
                            int address = PopReturnAddress();
                            ip = address;
                            break;
                        }
                    case Instruction.Jump:
                        {
                            int jumpLocation = (int)instructions[++ip];
                            // -1 since the instruction pointer will be incremented after the instruction is executed.
                            ip = jumpLocation - 1;
                            break;
                        }
                }
                ip++;
            }
        }
    }
}
