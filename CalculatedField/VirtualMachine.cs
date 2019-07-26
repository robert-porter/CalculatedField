using System.Collections.Generic;
using System.Linq;

namespace CalculatedField
{
    static class VirtualMachine
    {
        public static ScriptValue Run(CompiledScript compiledScript, List<ScriptValue> fieldValues)
        {
            var ip = 0;
            var stack = new Stack<ScriptValue>();
            var variables = new List<ScriptValue>();
            for (var i = 0; i < compiledScript.NumVariables; i++) {
                variables.Add(new ScriptValue());
            }
            var instructions = compiledScript.Instructions;
            var constants = compiledScript.Constants;
            var functions = compiledScript.Functions;
            var fields = fieldValues;

            while (ip < instructions.Count)
            {
                Instruction instruction = instructions[ip];
                switch (instruction)
                {
                    case Instruction.Add:
                        {
                            var right = Pop(); 
                            var left = Pop();
                            Push(left + right);
                            break;
                        }
                    case Instruction.Subtract:
                        {
                            var right = Pop();
                            var left = Pop();
                            Push(left - right);
                            break;
                        }
                    case Instruction.Multiply:
                        {
                            var right = Pop();
                            var left = Pop();
                            Push(left * right);
                            break;
                        }
                    case Instruction.Divide:
                        {
                            var right = Pop();
                            var left = Pop();
                            Push(left / right);
                            break;
                        }
                    case Instruction.DivideAndTruncate:
                        {
                            var right = Pop();
                            var left = Pop();
                            Push(left % right);
                            break;
                        }
                    case Instruction.Negate:
                        {
                            var value = Pop();
                            Push(-value);
                            break;
                        }
                    case Instruction.Less:
                        {
                            var right = Pop();
                            var left = Pop();
                            Push(left < right);
                            break;
                        }
                    case Instruction.LessOrEqual:
                        {
                            var right = Pop();
                            var left = Pop();
                            Push(left <= right);
                            break;
                        }
                    case Instruction.Greater:
                        {
                            var right = Pop();
                            var left = Pop();
                            Push(left > right);
                            break;
                        }
                    case Instruction.GreaterOrEqual:
                        {
                            var right = Pop();
                            var left = Pop();
                            Push(left >= right);
                            break;
                        }
                    case Instruction.Equal: 
                        {
                            var right = Pop();
                            var left = Pop();
                            Push(left == right);
                            break;
                        }
                    case Instruction.NotEqual:
                        {
                            var right = Pop();
                            var left = Pop();
                            Push(left != right);
                            break;
                        }
                    case Instruction.And:
                        {
                            var right = Pop();
                            var left = Pop();
                            Push(left & right);
                            break;
                        }
                    case Instruction.Or:
                        {
                            var right = Pop();
                            var left = Pop();
                            Push(left | right);
                            break;
                        }
                    case Instruction.Not:
                        {
                            var value = Pop();
                            Push(!value);
                            break;
                        }
                    case Instruction.Pop:
                        {
                            Pop();
                            break;
                        }
                    case Instruction.PushConstant:
                        {
                            int location = ReadNextInstruction();
                            var value = constants[location];
                            Push(value);
                            break;
                        }
                    case Instruction.PushVariable:
                        {
                            int location = ReadNextInstruction();
                            var value = variables[location];
                            Push(value);
                            break;
                        }
                    case Instruction.PushField:
                        {
                            int location = ReadNextInstruction();
                            var value = fields[location];
                            Push(value);
                            break;
                        }
                    case Instruction.Store:
                        {
                            int location = ReadNextInstruction();
                            var value = Pop();
                            variables[location] = value;
                            break;
                        }
                    case Instruction.JumpOnFalse:
                        {
                            int jumpLocation = ReadNextInstruction();
                            var value = Pop();
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

        }
    }
}
