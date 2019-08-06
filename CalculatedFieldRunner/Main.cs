using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace CalculatedField
{
    static class ProgramMain
    {
        static void Main()
        {
            long? x = 3;
            object o = x;
            var y = (decimal?)(long?)o;
            var engine = new Engine();
            while (true)
            { 
                var line = "";
                try
                {
                    line = Console.ReadLine();
                    var errors = engine.GetErrors(line, new List<Field>());
                    if (errors.Count > 0)
                    {
                        foreach (var error in errors)
                        {
                            var (start, end) = error.Range;
                            var lineStart = line.Substring(0, start);
                            var lineError = line.Substring(start, end - start);
                            var lineEnd = line.Substring(end);
                            Console.Write(lineStart);
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(lineError);
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine(lineEnd);
                            Console.WriteLine(error.Message);
                        }

                    }
                    else
                    {
                        var value = engine.CalculateValue(line);
                        Console.WriteLine(value ?? "null");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    
                }

            }
            
        }


        static void TestPerformance()
        {
            var engine = new Engine();
            var calculate = engine.Compile("2 + 2", new List<Field>());

            Stopwatch sw = new Stopwatch();

            sw.Start();
            for (int i = 0; i < 1000000; i++)
            {
                Func<int> x = () => 2 + 2;
                x();
            }
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);

            sw.Reset();

            sw.Start();
            for (int i = 0; i < 1000000; i++)
            {
                calculate(null);
            }
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);

            var method = GetMethod();
            sw.Start();
            for (int i = 0; i < 1000000; i++)
            {
                method?.Invoke(null, null);
            }
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);


            Console.ReadKey();

        }


        public static MethodInfo GetMethod()
        {
            // check that all required properties have been set

            // get source
            var source = GetSource();

            // create compiled class
            var classInstance = GetClassInstance(source);
            if (classInstance == null) return null;

            var method = classInstance?.GetType()?.GetMethod("theFunction");
            return method;
        }



        static string GetSource()
        {
            var source = new StringBuilder("namespace CalculatedFields {");
            source.AppendLine("using System;");
            source.AppendLine("using static System.Math;");
            source.AppendLine("public class CalculatedField {");
            source.AppendLine("public object theFunction() { return 2 + 2; }");
            source.AppendLine("}");
            source.AppendLine("}");
            return source.ToString();
        }

        private static object GetClassInstance(string source)
        {
            var provider = CodeDomProvider.CreateProvider("CSharp");
            var compilerResults = provider.CompileAssemblyFromSource(GetCompilerParameters(), source);
            try
            {
                var classInstance = compilerResults.CompiledAssembly.CreateInstance("CalculatedFields.CalculatedField");
                return classInstance;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static CompilerParameters GetCompilerParameters()
        {
            var compilerParameters = new CompilerParameters
            {
                GenerateExecutable = false,
                GenerateInMemory = true
            };
            compilerParameters.ReferencedAssemblies.Add("System.dll");
            return compilerParameters;
        }



    }
}
