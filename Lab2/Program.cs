using System;
using System.Collections.Generic;
using System.Text;

namespace Lab2
{
    public class Program
    {
        Func<double, double> defaultFunction = x => Math.Exp(-x) - x * x / 2;
        (double a, double b) defaultSector = (0, 1);
        int defaultSourceTableSize = 15;
        int defaultPolynomDegree = 7;
        double defaultInterpolationPoint = 0.65;

        void Main(string[] args)
        {
            CUIHelpers.CUIHelpers.Introduce(2, "Algebraic interpolation problem",
                new[] { "f(x) = exp(-x) - x^2/2", "[a, b] = [0, 1]", "m = 15", "n = 7", "x = 0,65" });

            Console.WriteLine("Enter source table size or enter nothing to use default value:");
            var sourceTableSize = CUIHelpers.CUIHelpers.EnterParameter(i => int.Parse(i), x => x > 0,
                    (ex, res) => Console.WriteLine("Enter correct value"), defaultSourceTableSize);
            Console.WriteLine("Enter correct degree for interpolation polynom or enter nothing to use default value\n" + 
                $"Remind that it should be less than source table size of {sourceTableSize}:");
            var polynomDegree = CUIHelpers.CUIHelpers.EnterParameter(i => int.Parse(i), x => x > 0,
                (ex, res) => Console.WriteLine("Enter correct value"), defaultPolynomDegree);
            Console.WriteLine("Enter interpolation point:");
            var interpolationPoint = CUIHelpers.CUIHelpers.EnterParameter(i => double.Parse(i), x => true,
                (ex, res) => Console.WriteLine("Enter correct value"), defaultInterpolationPoint);

        }
    }
}
