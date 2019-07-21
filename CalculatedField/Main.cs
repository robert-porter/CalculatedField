using System;

namespace CalculatedField
{
    static class ProgramMain
    {
        static void Main(string[] args)
        {
            FileWatcher watcher = new FileWatcher("..\\..\\..\\", "test.script");
            watcher.StartWatch();

            Console.WriteLine("Press any key to continue...");
            System.Console.Read();
        }

        public static string func(int x, string y)
        {
            return x.ToString() + y;
        }
    }
}
