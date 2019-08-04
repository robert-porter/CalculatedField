using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CalculatedField
{

    static class Runtime
    {
        public static List<MethodInfo> Functions { get; private set; }
        public static List<PropertyInfo> Constants { get; set; }

        static Runtime()
        {
            Functions = new List<MethodInfo>();
            Constants = new List<PropertyInfo>();
            addAll(typeof(LibMath));
            addAll(typeof(LibString));
            addAll(typeof(LibDateTime));
            addAll(typeof(LibConversion));
            addAll(typeof(LibTimeSpan));
            addAll(typeof(LibStandard));

            void addAll(Type type)
            {
                var methodInfos = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly)
                    .Where(methodInfo => !methodInfo.IsSpecialName);
                foreach (var methodInfo in methodInfos)
                {
                    Functions.Add(methodInfo);
                }
                var propertyInfos = type.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly);
                foreach(var propertyInfo in propertyInfos)
                {
                    Constants.Add(propertyInfo);
                }
            }
        }
    }

#pragma warning disable IDE1006

    static class LibStandard
    {
        public static bool? @if(bool? condition, bool ?valueOnTrue, bool? valueOnFalse)
        {
            if (condition == null) return null;
            if(condition.Value) return valueOnTrue;
            else return valueOnFalse;
        }

        public static decimal? @if(bool? condition, decimal? valueOnTrue, decimal? valueOnFalse)
        {
            if (condition == null) return null;
            if (condition.Value) return valueOnTrue;
            else return valueOnFalse;
        }

        public static string @if(bool? condition, string valueOnTrue, string valueOnFalse)
        {
            if (condition == null) return null;
            if (condition.Value) return valueOnTrue;
            else return valueOnFalse;
        }

        public static DateTime? @if(bool? condition, DateTime? valueOnTrue, DateTime? valueOnFalse)
        {
            if (condition == null) return null;
            if (condition.Value) return valueOnTrue;
            else return valueOnFalse;
        }

        public static TimeSpan? @if(bool? condition, TimeSpan? valueOnTrue, TimeSpan? valueOnFalse)
        {
            if (condition == null) return null;
            if (condition.Value) return valueOnTrue;
            else return valueOnFalse;
        }

        public static object ifs(object[] args)
        {            
            for(var i = 0; i < args.Length - 1; i+=2)
            {
                if (args[i]?.Equals(true) == true) return args[i + 1];
            }
            if (args.Length % 2 == 0) return null;
            else return args[args.Length - 1];
        }

        public static object cases(object[] args)
        {
            var value = args[0];
            for (var i = 1; i < args.Length - 1; i += 2)
            {
                if (args[i] == null && value == null) return args[i + 1];
                if (args[i]?.Equals(value) == true) return args[i + 1];
            }
            if (args.Length % 2 == 1) return null;
            else return args[args.Length - 1];
        }

    }

    static class LibConversion
    {

        public static decimal? parseNumber(string s)
        {
            if (s == null) return null;
            if (decimal.TryParse(s, out var result))
                return result;
            return null;
        }

        public static bool? parseBool(string s)
        {
            if (s == null) return null;
            if (bool.TryParse(s, out var result))
                return result;
            return null;
        }

        public static DateTime? parseDate(string s)
        {
            if (s == null) return null;
            if (DateTime.TryParse(s, out var result))
                return result;
            return null;
        }

        public static string toString(long? x) => x?.ToString();
        public static string toString(decimal? x) => x?.ToString();
        public static string toString(bool? x) => x?.ToString();
        public static string toString(DateTime? x) => x?.ToString();
        public static string toString(TimeSpan? x) => x?.ToString();

    }

    static class LibString
    {
        public static decimal? length(string s)
        {
            if (s == null) return 0;
            return s.Length;
        }

        public static string @char(string s, decimal? index)
        {
            if (index == null) return null;
            index--;
            if (s == null || index == 0) return null;
            if (index < 0 || index >= s.Length) return null;
            return s?.Substring((int)index, 1);
        }

        public static string concat(string a, string b)
        {
            if (a == null && b == null) return null;
            if (a == null) return b;
            if (b == null) return a;
            return string.Concat(a, b);
        }

        public static bool contains(string a, string b)
        {
            if (string.IsNullOrEmpty(b)) return true;
            if (a == null) return false;
            return a.Contains(b);
        }

        public static bool? endsWith(string s, string value)
        {
            if (string.IsNullOrEmpty(value)) return true;
            if (string.IsNullOrEmpty(s)) return false;
            return s.EndsWith(value);
        }

        public static bool startsWith(string s, string value)
        {
            if (string.IsNullOrEmpty(value)) return true;
            if (string.IsNullOrEmpty(s)) return false;
            return s.StartsWith(value);
        }

        public static decimal? indexOf(string s, string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return -1;
            if (s == null) return -1;
            var result = s.IndexOf(value);
            if (result == -1) return null;
            return result + 1;
        }

        public static decimal? lastIndexOf(string s, string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return -1;
            if (s == null) return -1;
            var result = s.LastIndexOf(value);
            if (result == -1) return null;
            return result + 1;
        }

        public static string insert(string s, decimal? index, string value)
        {
            if (index == null) return null;
            if (string.IsNullOrEmpty(value)) return s;
            index--;
            if (s == null) s = "";
            if (index < 0) index = 0;
            if (index >= s.Length) index = s.Length;
            return s.Insert((int)index, value);
        }

        public static string substring(string s, decimal? startIndex, decimal? length)
        {
            if (s == null || startIndex == null || length == null) return null;
            startIndex--;
            if (startIndex < 0 || startIndex >= s.Length || length < 0) return null;
            if (startIndex >= s.Length) return null;
            if (startIndex + length >= s.Length) length = s.Length - startIndex;
            return s.Substring((int)startIndex, (int)length);
        }

        public static string substring(string s, decimal? startIndex)
        {
            if (s == null || startIndex == null) return null;
            startIndex--;
            if (startIndex < 0 || startIndex >= s.Length) return null;
              return s.Substring((int)startIndex);
        }

        public static string remove(string s, decimal? startIndex, decimal? count)
        {
            if (s == null || startIndex == null || count == null)
                return null;
            startIndex--;
            return s.Remove((int)startIndex, (int)count);
        }

        public static string replace(string s, string oldValue, string newValue)
        {
            if (string.IsNullOrEmpty(oldValue)) return null;
            return s?.Replace(oldValue, newValue ?? "");
        }

        public static string padLeft(string s, decimal? width)
        {
            if (width == null) return s;
            return s.PadLeft((int)width.Value);
        }

        public static string padRight(string s, decimal? width)
        {
            if (width == null) return s;
            return s.PadRight((int)width.Value);
        }

        public static string trim(string s) => s?.Trim();
        public static string trimStart(string s) => s?.TrimStart();
        public static string trimEnd(string s) => s?.TrimEnd();
        public static string toUpper(string s) => s?.ToUpper();   
        public static string toLower(string s) => s?.ToLower();
    }

    static class LibTimeSpan
    {

        public static decimal? days(TimeSpan? ts) => (decimal)ts?.Days;
        public static decimal? hours(TimeSpan? ts) => (decimal)ts?.Hours;
        public static decimal? minutes(TimeSpan? ts) => (decimal)ts?.Minutes;
        public static decimal? seconds(TimeSpan? ts) => (decimal)ts?.Seconds;

        public static decimal? totalDays(TimeSpan? ts) => (decimal?)ts?.TotalDays;
        public static decimal? totalHours(TimeSpan? ts) => (decimal)ts?.TotalHours;
        public static decimal? totalMinutes(TimeSpan? ts) => (decimal?)ts?.TotalMinutes;
        public static decimal? totalSeconds(TimeSpan? ts) => (decimal?)ts?.TotalSeconds;

        public static TimeSpan? timeSpanFromSeconds(decimal? seconds)
        {
            if (seconds == null) return null;
            return TimeSpan.FromSeconds((double)seconds);
        }

        public static TimeSpan? timeSpanFromMinutes(decimal? minutes)
        {
            if (minutes == null) return null;
            return TimeSpan.FromMinutes((double)minutes);
        }

        public static TimeSpan? timeSpanFromHours(decimal? hours)
        {
            if (hours == null) return null;
            return TimeSpan.FromHours((double)hours);
        }

        public static TimeSpan? timeSpanFromDays(decimal? days)
        {
            if (days == null) return null;
            return TimeSpan.FromDays((double)days);
        }

    }

    static class LibDateTime
    {
        public static decimal? second(DateTime? dt) => dt?.Second;
        public static decimal? minute(DateTime? dt) => dt?.Minute;
        public static decimal? hour(DateTime? dt) => dt?.Hour;

        public static decimal? day(DateTime? dt) => dt?.Day;
        public static decimal? month(DateTime? dt) => dt?.Month;
        public static decimal? year(DateTime? dt) => dt?.Year;

        public static DateTime? date(decimal? year, decimal? month, decimal? day)
        {
            if (year == null || month == null || day == null) return null;
            return new DateTime((int) year, (int) month, (int) day);
        }

        public static DateTime? time(decimal? hour, decimal? minute, decimal? second)
        {
            if (hour == null || minute == null || second == null) return null;
            return new DateTime(0, 0, 0, (int)hour, (int)minute, (int)second);
        }


        public static DateTime? Today => DateTime.Today;
        public static DateTime? Yesterday => DateTime.Today.AddDays(-1);
        public static DateTime? Tomorrow => DateTime.Today.AddDays(1);
        public static DateTime? StartOfWeek => DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
        public static DateTime? EndOfWeek => DateTime.Today.AddDays(6 - (int)DateTime.Today.DayOfWeek);
        public static DateTime? StartOfMonth => new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        public static DateTime? EndOfMonth => new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(1).AddDays(-1);
        public static DateTime? StartOfYear => new DateTime(DateTime.Today.Year, 1, 1);
        public static DateTime? EndOfYear => new DateTime(DateTime.Today.Year, 12, 31);
        public static DateTime? StartOfQuarter => new DateTime(DateTime.Today.Year, DateTime.Today.Month - (DateTime.Today.Month % 3) + 1, 1);
        public static DateTime? EndOfQuarter => new DateTime(DateTime.Today.Year, DateTime.Today.Month - (DateTime.Today.Month % 3) + 1, 1).AddMonths(1).AddDays(-1);
        public static DateTime? StartOfLastWeek => DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek - 7);
        public static DateTime? EndOfLastWeek => DateTime.Today.AddDays((int)DateTime.Today.DayOfWeek - 1);
        public static DateTime? StartOfLastMonth => new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);
        public static DateTime? EndOfLastMonth => new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddDays(-1);
        public static DateTime? StartOfLastQuarter => new DateTime(DateTime.Today.Year, DateTime.Today.Month - (DateTime.Today.Month % 3) + 1, 1).AddMonths(-3);
        public static DateTime? EndOfLastQuarter => new DateTime(DateTime.Today.Year, DateTime.Today.Month - (DateTime.Today.Month % 3) + 1, 1).AddMonths(-2).AddDays(-1);
        public static DateTime? StartOfLastYear => new DateTime(DateTime.Today.Year - 1, 1, 1);
        public static DateTime? EndOfLastYear => new DateTime(DateTime.Today.Year - 1, 12, 31);
        public static DateTime? StartOfNextWeek => DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 7);
        public static DateTime? EndOfNextWeek => DateTime.Today.AddDays((int)DateTime.Today.DayOfWeek + 6);
        public static DateTime? StartOfNextMonth => new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(1);
        public static DateTime? EndOfNextMonth => new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(2).AddDays(-1);
        public static DateTime? StartOfNextQuarter => new DateTime(DateTime.Today.Year, DateTime.Today.Month - (DateTime.Today.Month % 3) + 1, 1).AddMonths(3);
        public static DateTime? EndOfNextQuarter => new DateTime(DateTime.Today.Year, DateTime.Today.Month - (DateTime.Today.Month % 3) + 1, 1).AddMonths(5).AddDays(-1);
        public static DateTime? StartOfNextYear => new DateTime(DateTime.Today.Year + 1, 1, 1);
        public static DateTime? EndOfNextYear => new DateTime(DateTime.Today.Year + 1, 12, 31);
    }

    static class LibMath
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

        public static decimal? pow(decimal? a,  decimal? b)
        {
            if (a == null || b == null) return null;
            return (decimal) Math.Pow((double)a.Value, (double)b.Value);
        }

        public static decimal? pow(decimal? a, long? b)
        {
            if (a == null || b == null) return null;
            return (decimal)Math.Pow((double)a.Value, b.Value);
        }

        public static decimal? pow(long? a, decimal? b)
        {
            if (a == null || b == null) return null;
            return (decimal)Math.Pow(a.Value, (double)b.Value);
        }

        public static decimal? pow(long? a, long? b)
        {
            if (a == null || b == null) return null;
            return (decimal) Math.Pow(a.Value, b.Value);
        }
    }
    
}
