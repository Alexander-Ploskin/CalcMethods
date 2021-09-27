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

        static void Main(string[] args)
        {
            foreach(var message in Introduce())
            {
                Console.WriteLine(message);
            }
            Console.WriteLine("Enter amount of chunks to detach roots");
            var chunksCount = CUIHelpers.CUIHelpers.EnterParameter(i => int.Parse(i), x => x > 0);

            var func = new Func<double, double>(x => 8 * Math.Cos(x) - x - 6);
            var derivative = new Func<double, double>(x => -8 * Math.Sin(x) - 1);
            var section = (-10, 2);
            var accuracy = 1.0e-12;

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
