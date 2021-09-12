using MyMathLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab1
{
    public class Program
    {
        static IEnumerable<string> Introduce()
        {
            yield return "Labaratory work 1";
            yield return "Numerical methods for non-linear equations";
            yield return "Equation is 10 * cos(x) - 0,1 * x ^ 2 = 0";

        }

        static int EnterIntegerParameter(Predicate<int> predicate)
        {
            var result = 0;
            while(!int.TryParse(Console.ReadLine(), out result) || !predicate(result))
            {
                Console.WriteLine("Enter correct value");
            }
            return result;
        }

        static void Main(string[] args)
        {
            foreach(var message in Introduce())
            {
                Console.WriteLine(message);
            }
            Console.WriteLine("Enter amount of chunks to detach roots");
            var chunksCount = EnterIntegerParameter(x => x > 0);

            var func = new Func<double, double>(x => 10 * Math.Cos(x) - 0.1 * x * x);
            var derivative = new Func<double, double>(x => -10 * Math.Sin(x) - 0.2 * x);
            var section = (-8, 2);
            var accuracy = 1.0e-5;

            var splittedSection = NonLinearEquations.Split(func, section, chunksCount).ToList();
            Console.WriteLine();
            Console.WriteLine($"Found {splittedSection.Count} sections where function changes its sign");
            splittedSection.ForEach(s => Console.WriteLine($"[{s.a}; {s.b}]"));

            foreach (var method in Enum.GetValues(typeof(NonLinearEquations.Method)).Cast<NonLinearEquations.Method>())
            {
                Console.WriteLine();
                Console.WriteLine(method.ToString() + "\n");

                splittedSection.Select(s => NonLinearEquations.FindRoot(func, s, accuracy, method, derivative))
                    .ToList().ForEach(result => {
                    Console.WriteLine();
                    Console.WriteLine($"Initial approximation is {result.InitialApproximation}");
                    Console.WriteLine($"Length of last sector is {result.LastSectorLength}");
                    Console.WriteLine($"Approximated root is {result.Root}");
                    Console.WriteLine($"Absolute value for residual is {result.Residual}");
                    Console.WriteLine($"Root was found in {result.StepsCount} steps");
                });
            }
        }
    }
}
