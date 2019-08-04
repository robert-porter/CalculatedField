using System;
using System.Linq.Expressions;
using System.Reflection;

namespace CalculatedField
{
    static class ProgramMain
    {
        static void Main()
        {
            Engine engine = new Engine();
            while (true)
            {
                try
                {
                    var line = Console.ReadLine();
                    var value = engine.CalculateValue(line);
                    Console.WriteLine(value ?? "null");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }
        }

    }
}
