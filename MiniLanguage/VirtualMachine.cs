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
        Environment Env;

        public VirtualMachine()
        {
            Stack = new Stack<Value>();
            Env = new Environment();
            ReturnAddresses = new Stack<int>();
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
        }

        public int PopReturnAddress()
        {
            return ReturnAddresses.Pop();
        }
        public void SetVar(String identifier, Value value)
        {
            Env.SetVar(identifier, value);
        }
        public Value GetVar(String identifier)
        {
            return Env.GetVar(identifier);
        }

        public void AddVar(String identifier, Value value)
        {
            Env.AddVar(identifier, value);
        }
        public void AddVar(String identifier)
        {
            Env.AddVar(identifier);
        }

        public void PushScope()
        {
            Environment newEnv = new Environment();
            newEnv.Parent = Env;
            Env = newEnv;
        }
        public void PopScope()
        {
            Env = Env.Parent;
        }
        public void Run(List<Instruction> instructions, int startAddress)
        {
            int ip = startAddress;

            while (ip < instructions.Count)
            {
                Instruction instruction = instructions[ip];
                instruction.Execute(this, ref ip);
                ip++;
            }
        }
    }
}
