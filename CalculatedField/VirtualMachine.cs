using System.Collections.Generic;
using System.Linq;

namespace CalculatedField
{
    class VirtualMachine
    {
        CompiledScript CompiledScript;
        public VirtualMachine(CompiledScript compiledScript)
        {
            CompiledScript = compiledScript;
        }

        public ScriptValue Run()
        {
            var ip = 0;
            var stack = new Stack<ScriptValue>();
            var variables = new List<ScriptValue>();
            for (var i = 0; i < CompiledScript.Variables.Count; i++) {
                variables.Add(new ScriptValue());
            }
            var instructions = CompiledScript.Instructions;
            var constants = CompiledScript.Constants;
            var functions = new List<Function>();
            for(var i =0; i < CompiledScript.Functions.Count; i++)
            {
                var function = ScriptFunctions.GetByName(CompiledScript.Functions[i]).FirstOrDefault();
                functions.Add(function);
            }

            while (ip < instructions.Count)
            {
                Instruction instruction = instructions[ip];
                switch (instruction)
                {
                    case Instruction.Add:
                        {
                            ScriptValue right = Pop(); 
                            ScriptValue left = Pop();
                            Push(left + right);
                            break;
                        }
                    case Instruction.Subtract:
                        {
                            ScriptValue right = Pop();
                            ScriptValue left = Pop();
                            Push(left - right);
                            break;
                        }

                    case Instruction.Multiply:
                        {
                            ScriptValue right = Pop();
                            ScriptValue left = Pop();
                            Push(left * right);
                            break;
                        }
                    case Instruction.Divide:
                        {
                            ScriptValue right = Pop();
                            ScriptValue left = Pop();
                            Push(left / right);
                            break;
                        }
                    case Instruction.Negate:
                        {
                            ScriptValue value = Pop();
                            Push(-value);
                            break;
                        }
                    case Instruction.Less:
                        {
                            ScriptValue right = Pop();
                            ScriptValue left = Pop();
                            Push(left < right);
                            break;
                        }
                    case Instruction.LessOrEqual:
                        {
                            ScriptValue right = Pop();
                            ScriptValue left = Pop();
                            Push(left <= right);
                            break;
                        }
                    case Instruction.Greater:
                        {
                            ScriptValue right = Pop();
                            ScriptValue left = Pop();
                            Push(left > right);
                            break;
                        }
                    case Instruction.GreaterOrEqual:
                        {
                            ScriptValue right = Pop();
                            ScriptValue left = Pop();
                            Push(left >= right);
                            break;
                        }
                    case Instruction.Equal: 
                        {
                            ScriptValue right = Pop();
                            ScriptValue left = Pop();
                            Push(left == right);
                            break;
                        }
                    case Instruction.NotEqual:
                        {
                            ScriptValue right = Pop();
                            ScriptValue left = Pop();
                            Push(left != right);
                            break;
                        }
                    case Instruction.And:
                        {
                            ScriptValue right = Pop();
                            ScriptValue left = Pop();
                            Push(left & right);
                            break;
                        }
                    case Instruction.Or:
                        {
                            ScriptValue right = Pop();
                            ScriptValue left = Pop();
                            Push(left & right);
                            break;
                        }
                    case Instruction.Not:
                        {
                            ScriptValue value = Pop();
                            Push(!value);
                            break;
                        }
                    case Instruction.PushConstant:
                        {
                            int location = ReadNextInstruction();
                            ScriptValue value = constants[location];
                            Push(value);
                            break;
                        }
                    case Instruction.PushVariable:
                        {
                            int location = ReadNextInstruction();
                            var value = Read(location);
                            Push(value);
                            break;
                        }
                    case Instruction.Store:
                        {
                            int location = ReadNextInstruction();
                            var value = Pop();
                            Store(location, value);
                            break;
                        }
                    case Instruction.JumpOnFalse:
                        {
                            int jumpLocation = ReadNextInstruction();
                            ScriptValue value = Pop();
                            if (!value.BoolValue)
                                ip = (int)jumpLocation;
                            continue;
                        }
                    case Instruction.Jump:
                        {
                            int jumpLocation = ReadNextInstruction();
                            ip = jumpLocation;
                            continue;
                        }
                    case Instruction.Call:
                        {
                            int location = ReadNextInstruction();
                            var function = functions[location];
                            var argumentCount = function.ArgumentTypes.Count;
                            var arguments = new ScriptValue[argumentCount];
                            for (var i = 0; i < argumentCount; i++)
                            {
                                arguments[i] = Pop();
                            }
                            var value = function.Call(arguments);
                            Push(value);
                            break;
                        }
                }

                ip++;
            }

            return stack.Peek();


            void Push(ScriptValue value)
            {
                stack.Push(value);
            }

            ScriptValue Pop()
            {
                return stack.Pop();
            }

            int ReadNextInstruction()
            {
                return (int)instructions[++ip];
            }

            void Store(int location, ScriptValue value)
            {
                variables[location] = value;
            }

            ScriptValue Read(int location)
            {
                return variables[location];
            }


        }
    }
}
