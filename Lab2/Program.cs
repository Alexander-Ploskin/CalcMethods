using System;
using System.Collections.Generic;
using System.Linq;
using MyMathLib;

namespace Lab2
{
    public class Program
    {
        static Func<double, double> defaultFunction = x => Math.Exp(-x) - x * x / 2;
        static (double a, double b) defaultSector = (0, 1);
        static int defaultSourceTableSize = 15;
        static int defaultPolynomDegree = 7;
        static double defaultInterpolationPoint = 0.65;

        static List<(double, double)> GetSourceTable(int sourceTableSize, (double a, double b) sector, Func<double, double> function)
        {
            if(sector.b <= sector.a)
            {
                throw new ArgumentException("Invalid sector");
            }
            var distance = (sector.b - sector.a) / (sourceTableSize + 1);
            return Enumerable.Range(0, sourceTableSize).Select(i =>
                {
                    var point = sector.a + i * distance;
                    return (point, function(point));
                }).ToList();
        }

        static void PrintSourceTable(List<(double x, double fx)> sourceTable)
        {
            Console.WriteLine();
            for(int i = 0; i < sourceTable.Count; ++i)
            {
                Console.WriteLine($"X_{i} = {sourceTable[i].x}, F(X_{i}) = {sourceTable[i].fx}");
            }
            Console.WriteLine();
        }

        static void PrintMethodResult(string methodName, double methodResult, double functionValue)
        {
            Console.WriteLine($"The {methodName} result = {methodResult}");
            Console.WriteLine($"The actual function value in the interpolation point = {functionValue}");
            Console.WriteLine($"The Absolute value for residual = {Math.Abs(functionValue - methodResult)}");
        }

        static void GetValueInPoint(List<(double, double)> sourceTable, double interpolationPoint, int polynomDegree) {
            sourceTable.Sort((first, second) => Math.Sign(Math.Abs(interpolationPoint - first.Item1) - Math.Abs(interpolationPoint - second.Item1)));
            Console.WriteLine();
            Console.WriteLine("Sorted table:");
            PrintSourceTable(sourceTable);

            var lagrangeResult = AlgebraicInterpolaion.GetInterpolationPolynome(sourceTable, polynomDegree, interpolationPoint, AlgebraicInterpolaion.Method.Lagrange);
            var newthonResult = AlgebraicInterpolaion.GetInterpolationPolynome(sourceTable, polynomDegree, interpolationPoint, AlgebraicInterpolaion.Method.Newton);
            var actualResult = defaultFunction(interpolationPoint);

            PrintMethodResult("Lagrange method", lagrangeResult, actualResult);
            Console.WriteLine();
            PrintMethodResult("Newthon method", newthonResult, actualResult);
        }

        static void Main(string[] args)
        {
            CUIHelpers.CUIHelpers.Introduce(2, "Algebraic interpolation problem",
                new[] { "f(x) = exp(-x) - x^2/2", "[a, b] = [0, 1]", "m = 15", "n = 7", "x = 0,65" });

            Console.WriteLine("Enter parameters or enter nothing to use default values\n");
            Console.WriteLine("Enter source table size:");
            var sourceTableSize = CUIHelpers.CUIHelpers.EnterParameter(i => int.Parse(i), x => x > 0,
                    (ex, res) => Console.WriteLine("Enter correct value for source table size"), defaultSourceTableSize);
            Console.WriteLine("Enter left border for interpolation sector");
            var leftBorder = CUIHelpers.CUIHelpers.EnterParameter(i => double.Parse(i), x => double.IsFinite(x),
                (ex, res) => Console.WriteLine("Enter correct value for left border"), defaultSector.a);
            Console.WriteLine("Enter right border for interpolation sector");
            var rightBorder = CUIHelpers.CUIHelpers.EnterParameter(i => double.Parse(i), x => double.IsFinite(x) && x > leftBorder,
                (ex, res) => Console.WriteLine("Enter correct value for right border"), defaultSector.b);
            var sourceTable = GetSourceTable(sourceTableSize, (leftBorder, rightBorder), defaultFunction);
            Console.WriteLine("Source table:");
            PrintSourceTable(sourceTable);

            var shouldContinue = 1;

            while(shouldContinue == 1)
            {
                Console.WriteLine("Enter correct degree for interpolation polynom or enter nothing to use default value\n" +
                $"Remind that it should be less than source table size of {sourceTableSize}:");
                var polynomDegree = CUIHelpers.CUIHelpers.EnterParameter(i => int.Parse(i), x => x > 0 && x < sourceTableSize,
                    (ex, res) => Console.WriteLine("Enter correct value for polynom degree"), defaultPolynomDegree);
                Console.WriteLine("Enter interpolation point:");
                var interpolationPoint = CUIHelpers.CUIHelpers.EnterParameter(i => double.Parse(i), x => double.IsFinite(x),
                    (ex, res) => Console.WriteLine("Enter correct value for interpolaion point"), defaultInterpolationPoint);

                GetValueInPoint(sourceTable, interpolationPoint, polynomDegree);

                Console.WriteLine();
                Console.WriteLine("To continue work with new point enter 1, to close enter 0");
                shouldContinue = CUIHelpers.CUIHelpers.EnterParameter(i => int.Parse(i), x => x == 1 || x == 0,
                    (ex, res) => Console.WriteLine("Enter 0 or 1"));
                Console.WriteLine();
            }
        }
    }
}
