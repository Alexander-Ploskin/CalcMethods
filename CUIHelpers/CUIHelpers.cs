using System;
using System.Collections.Generic;
using System.Linq;

namespace CUIHelpers
{
    public static class CUIHelpers
    {
        public static T EnterParameter<T>(Func<string, T> convert,
                                          Predicate<T> predicate,
                                          Action<string, string> notifyFaiure,
                                          T defaultValue = default(T))
        {
            var result = default(T);
            while(true)
            {
                try
                {
                    var input = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(input) && predicate(defaultValue))
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

        static IEnumerable<string> Introduction(int number, string problem, string[] defaultParameters, int variant = 5)
        {
            yield return $"Labaratory work {number}";
            yield return problem;
            yield return $"Variant {variant}";
            yield return $"Default parameters:\n{defaultParameters.Aggregate((res, x) => res + $"\n{x}")}";

        }

        public static void Introduce(int number, string problem, string[] defaultParameters, int variant = 5)
        {
            foreach(var message in Introduction(number, problem, defaultParameters, variant))
            {
                Console.WriteLine(message);
                Console.WriteLine();
            }
        }
    }
}
