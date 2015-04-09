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
        Environment Environment;

        public VirtualMachine()
        {
            Stack = new Stack<Value>();
            Environment = new Environment();
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
        public void SetVar(String identifier, Value value)
        {
            Environment.SetVar(identifier, value);
        }
        public Value GetVar(String identifier)
        {
            return Environment.GetVar(identifier);
        }

        public void AddVar(String identifier)
        {
            Environment.AddVar(identifier);
        }

        public void Run(List<Instruction> instructions)
        {
            int ip = 0;

            while (ip < instructions.Count)
            {
                Instruction instruction = instructions[ip];
                instruction.Execute(this, ref ip);
                ip++;
            }
        }
    }
}
