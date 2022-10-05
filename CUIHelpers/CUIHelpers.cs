using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleTables;

namespace CUIHelpers
{
    public static class CUIHelpers
    {
        public static void PrintTable(string[] columns, object[,] rows)
        {
            if(columns.Length != rows.GetLength(1))
            {
                throw new ArgumentException();
            }
            var table = new ConsoleTable(columns);
            for(int i = 0; i < rows.GetLength(0); ++i)
            {
                table.AddRow(rows[i, 0], rows[i, 1], rows[i, 2], rows[i, 3], rows[i, 4], rows[i, 5]);
            }
            table.Write();
        }

        public static T EnterParameter<T>(Func<string, T> convert,
                                          Predicate<T> predicate,
                                          Action<string, string> notifyFaiure,
                                          T defaultValue,
                                          string getMessage = null)
        {
            return EnterParameter(convert, predicate, notifyFaiure, true, defaultValue, getMessage);
        }

        public static T EnterParameter<T>(Func<string, T> convert,
                                          Predicate<T> predicate,
                                          Action<string, string> notifyFaiure,
                                          string getMessage = null)
        {
            return EnterParameter(convert, predicate, notifyFaiure, false, default(T), getMessage);
        }

        static T EnterParameter<T>(Func<string, T> convert,
                                   Predicate<T> predicate,
                                   Action<string, string> notifyFaiure,
                                   bool useDefaultValue,
                                   T defaultValue = default(T),
                                   string getMessage = null)
        {
            if(getMessage != null)
            {
                Console.WriteLine(getMessage);
            }

            T result;
            while(true)
            {
                try
                {
                    var input = Console.ReadLine();
                    if (useDefaultValue && string.IsNullOrWhiteSpace(input) && predicate(defaultValue))
                    {
                        return defaultValue;
                    } 
                    else if (string.IsNullOrWhiteSpace(input))
                    {
                        notifyFaiure(null, defaultValue.ToString());
                        continue;
                    }
                    result = convert(input);
                    if(!predicate(result))
                    {
                        notifyFaiure(null, input);
                        continue;
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    notifyFaiure(ex.Message, null);
                }
            }
        }

        public static T EnterParameter<T>(Func<string, T> convert, Predicate<T> predicate)
        {
            return EnterParameter<T>(convert, predicate, (ex, res) => Console.WriteLine("Enter correct parameter"));
        }

        static IEnumerable<string> Introduction(string number, string problem, string[] defaultParameters, int variant = 13)
        {
            yield return $"Labaratory work {number}";
            yield return problem;
            yield return $"Variant {variant}";
            yield return $"Default parameters:\n{defaultParameters.Aggregate((res, x) => res + $"\n{x}")}";

        }

        public static void Introduce(string number, string problem, string[] defaultParameters, int variant = 13)
        {
            foreach(var message in Introduction(number, problem, defaultParameters, variant))
            {
                Console.WriteLine(message);
                Console.WriteLine();
            }
        }

        public static void PrintSourceTable(List<(double x, double fx)> sourceTable)
        {
            Console.WriteLine();
            for(int i = 0; i < sourceTable.Count; ++i)
            {
                Console.WriteLine($"X_{i} = {sourceTable[i].x}, F(X_{i}) = {sourceTable[i].fx}");
            }
            Console.WriteLine();
        }

        public static void PrintReversedSourceTable(List<(double x, double fx)> sourceTable)
        {
            Console.WriteLine();
            for (int i = 0; i < sourceTable.Count; ++i)
            {
                Console.WriteLine($"F(X_{i}) = {sourceTable[i].x}, F^(-1)(F(X_{i})) = {sourceTable[i].fx}");
            }
            Console.WriteLine();
        }
    }
}
