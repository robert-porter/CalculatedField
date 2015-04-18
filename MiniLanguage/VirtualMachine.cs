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
        Stack<int> FrameOffsets;
        List<Value> Variables;


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
            ReturnAddresses.Push(address+1); // +1 since address is at the call instruction
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

            if (location < 0)
                Variables[-location - 1] = value;
            else 
                Variables[location + FrameOffsets.Peek()] = value;
        }
        public Value GetVar(int location)
        {
            if (location < 0)
                return Variables[-location - 1];
            else
                return Variables[location + FrameOffsets.Peek()];
        }
        public void AddVar()
        {
            Variables.Add(new Value()) ;
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
                    case Instruction.Over:
                        {
                            Value top = Pop();
                            Value next = Pop();
                            Push(next);
                            Push(top);
                            Push(next);
                            break;
                        }
                    case Instruction.Tuck:
                        {
                            Value top = Pop();
                            Value next = Pop();
                            Push(top);
                            Push(next);
                            Push(top);
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
                    case Instruction.Equal: 
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
                            Push(new Value(left.AsBool() && right.AsBool()));
                            break;
                        }
                    case Instruction.Or:
                        {
                            Value right = Pop();
                            Value left = Pop();
                            Push(new Value(left.AsBool() || right.AsBool()));
                            break;
                        }
                    case Instruction.Not:
                        {
                            Value value = Pop();
                            Push(new Value(!value.AsBool()));
                            break;
                        }
                    case Instruction.PushTrue:
                        {
                            Push(new Value(true));
                            break;
                        }
                    case Instruction.PushFalse:
                        {
                            Push(new Value(false));
                            break;
                        }
                    case Instruction.PushNumber:
                        {
                            int location = (int)instructions[++ip];
                            Value number = Constants[location];
                            Push(number);
                            break;
                        }
                    case Instruction.PushVariable:
                        {
                            int location = (int)instructions[++ip];
                            Push(GetVar(location));
                            break;
                        }
                    case Instruction.PushOffsetVariable:
                        {
                            int location = (int)instructions[++ip];
                            int offset = (int) Pop().DoubleVal;
                            Push(GetVar(location + offset));
                            break;
                        }
                    case Instruction.PushReference:
                        {
                            int referenceLocation = (int)instructions[++ip];
                            Value reference = GetVar(referenceLocation);
                            Value value = GetVar(reference.PointerVal);
                            Push(value);
                            break;
                        }
                    case Instruction.StoreVariable:
                        {
                            int location = (int)instructions[++ip];
                            SetVar(location, Pop());
                            break;
                        }
                    case Instruction.StoreReference:
                        {
                            int referenceLocation = (int)instructions[++ip];
                            Value reference = GetVar(referenceLocation);
                            SetVar(reference.PointerVal, Pop());
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
                            AddVar();
                            break;
                        }
                    case Instruction.NewReference:
                        {
                            AddVar();
                            int location = (int)instructions[++ip];
                            Variables[Variables.Count - 1] = new Value(location);
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
                            Value value = Pop();
                            if (!value.AsBool())
                                ip = (int)jumpLocation;
                            continue;
                        }
                    case Instruction.Call:
                        {
                            int jumpLocation = (int)instructions[++ip];
                            PushReturnAddress(ip);
                            ip = jumpLocation;
                            continue;
                        }
                    case Instruction.Return:
                        {
                            int address = PopReturnAddress();
                            ip = address;
                            continue;
                        }
                    case Instruction.Jump:
                        {
                            int jumpLocation = (int)instructions[++ip];
                            ip = jumpLocation;
                            continue;
                        }
                }
                ip++;
            }
        }
    }
}
