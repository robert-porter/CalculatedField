using System;
using System.Collections.Generic;
using System.Linq;

namespace CalculatedField
{

    static class ScriptFunctions
    {
        public static List<Function> Functions;

        static ScriptFunctions()
        {
            Functions = new List<Function>();
            AddAll();
        }
        
        static public IEnumerable<Function> GetByName(string name)
        {
            return Functions.Where(function => function.Name == name);
        }

        static void AddAll()
        {
            var type = typeof(ScriptMath);
            var methodInfos = type.GetMethods();
            foreach (var methodInfo in methodInfos)
            {
                Functions.Add(new Function(type, methodInfo));
            }

            type = typeof(ScriptString);
            methodInfos = type.GetMethods();
            foreach (var methodInfo in methodInfos)
            {
                Functions.Add(new Function(type, methodInfo));
            }
        }

    }

    static class ScriptString
    {
        public static long length(string s)
        {
            if (s == null) return 0;
            return s.Length;
        }

        public static string concat(string a, string b)
        {
            if (a == null && b == null) return null;
            if (a == null) return b;
            if (b == null) return a;
            return a + b;
        }

        public static bool contains(string a, string b)
        {
            if (a == null || b == null) return false;
            return a.Contains(b);
        }
    }

    static class ScriptDateTime
    {
        public static DateTime? date(string s)
        {
            if (DateTime.TryParse(s, out var dt))
                return dt;
            return null;
        }

    }

    static class ScriptMath
    {
        public static decimal? abs(decimal? x)
        {
            if (x == null) return null;
            return Math.Abs(x.Value);
        }

        public static long? abs(long? x)
        {
            if (x == null) return null;
            return Math.Abs(x.Value);
        }

        public static decimal? ceil(decimal? x)
        {
            if (x == null) return null;
            return Math.Ceiling(x.Value);
        }

        public static decimal? exp(decimal? x)
        {
            if (x == null) return null;
            return (decimal)Math.Exp((double)x);
        }

        public static decimal? exp(long? x)
        {
            if (x == null) return null;
            return (decimal)Math.Exp((double)x);
        }

        public static decimal? floor(decimal? x)
        {
            if (x == null) return null;
            return Math.Floor(x.Value);
        }

        public static decimal? log(decimal? x, decimal? b)
        {
            if (x == null || b == null) return null;
            return (decimal)Math.Log((double)x, (double)b);
        }

        public static decimal? log(long? x, decimal? b)
        {
            if (x == null || b == null) return null;
            return (decimal)Math.Log(x.Value, (double)b);
        }

        public static decimal? log(decimal? x, long? b)
        {
            if (x == null || b == null) return null;
            return (decimal)Math.Log((double)x, b.Value);
        }

        public static decimal? log(long? x, long? b)
        {
            if (x == null || b == null) return null;
            return (decimal)Math.Log(x.Value, b.Value);
        }

        public static decimal? log(decimal? x)
        {
            if (x == null) return null;
            return (decimal)Math.Log((double)x);
        }

        public static decimal? log(long? x)
        {
            if (x == null) return null;
            return (decimal)Math.Log(x.Value);
        }

        public static decimal? log10(decimal? x)
        {
            if (x == null) return null;
            return (decimal)Math.Log10((double)x);
        }

        public static decimal? log10(long? x)
        {
            if (x == null) return null;
            return (decimal)Math.Log10(x.Value);
        }

        public static decimal? sqrt(decimal? x)
        {
            if (x == null) return null;
            return (decimal)Math.Sqrt((double)x);
        }

        public static decimal? sqrt(long? x)
        {
            if (x == null) return null;
            return (decimal)Math.Sqrt(x.Value);
        }

        public static long? max(long? x, long? y)
        {
            if (x == null || y == null) return null;
            return Math.Max(x.Value, y.Value);
        }

        public static decimal? max(decimal? x, decimal? y)
        {
            if (x == null) return y;
            if (y == null) return x;
            return Math.Max(x.Value, y.Value);
        }

        public static decimal? max(decimal? x, long? y)
        {
            if (x == null || y == null) return null;
            return Math.Max(x.Value, y.Value);
        }

        public static decimal? max(long? x, decimal? y)
        {
            if (x == null || y == null) return null;
            return Math.Max(x.Value, y.Value);
        }

        public static long? min(long? x, long? y)
        {
            if (x == null || y == null) return null;
            return Math.Min(x.Value, y.Value);
        }

        public static decimal? min(decimal? x, decimal? y)
        {
            if (x == null || y == null) return null;
            return Math.Min(x.Value, y.Value);
        }

        public static decimal? min(decimal? x, long? y)
        {
            if (x == null || y == null) return null;
            return Math.Min(x.Value, y.Value);
        }

        public static decimal? min(long? x, decimal? y)
        {
            if (x == null || y == null) return null;
            return Math.Min(x.Value, y.Value);
        }

        public static long? sign(decimal? x)
        {
            if (x == null) return null;
            return Math.Sign(x.Value);
        }

        public static long? sign(long? x)
        {
            if (x == null) return null;
            return Math.Sign(x.Value);
        }

        public static decimal? trunc(decimal? x)
        {
            if (x == null) return null;
            return Math.Truncate(x.Value);
        }

        public static decimal? round(decimal? x)
        {
            if (x == null) return null;
            return Math.Round(x.Value);
        }
    }
    
}
