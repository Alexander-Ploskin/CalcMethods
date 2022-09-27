using System;
using System.Collections.Generic;
using System.Linq;
using MyMathLib;

namespace Lab2
{
    public class Program
    {
        static Func<double, double> defaultFunction = x => Math.Log(1 + x) - Math.Exp(x);
        static (double a, double b) defaultSector = (1, 10);
        const int DefaultSourceTableSize = 15;
        const int DefaultPolynomDegree = 7;
        const double defaultInterpolationPoint = 5.25;

        static List<(double, double)> GetSourceTable(int sourceTableSize, (double a, double b) sector, Func<double, double> function)
        {
            if(sector.b <= sector.a)
            {
                throw new ArgumentException("Invalid sector");
            }
            var distance = (sector.b - sector.a) / sourceTableSize;
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
            Console.WriteLine($"The absolute value for residual = {Math.Abs(functionValue - methodResult)}");
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
                new[] { "f(x) ln(1 + x) - exp(x)", "[a, b] = [1, 10]", "m = 15", "n = 7", "x = 5,25" });

            Console.WriteLine("Enter parameters or enter nothing to use default values\n");
            Console.WriteLine("Enter the source table size:");
            var sourceTableSize = CUIHelpers.CUIHelpers.EnterParameter(i => int.Parse(i), x => x > 0,
                    (ex, res) => Console.WriteLine("Enter a correct value for the source table size"), DefaultSourceTableSize);
            Console.WriteLine("Enter the value for left border of the interpolation sector");
            var leftBorder = CUIHelpers.CUIHelpers.EnterParameter(i => double.Parse(i), x => double.IsFinite(x),
                (ex, res) => Console.WriteLine("Enter a correct value for left border"), defaultSector.a);
            Console.WriteLine("Enter the value for right border of the interpolation sector");
            var rightBorder = CUIHelpers.CUIHelpers.EnterParameter(i => double.Parse(i), x => double.IsFinite(x) && x > leftBorder,
                (ex, res) => Console.WriteLine("Enter a correct value for right border"), defaultSector.b);
            var sourceTable = GetSourceTable(sourceTableSize, (leftBorder, rightBorder), defaultFunction);
            Console.WriteLine("Source table:");
            PrintSourceTable(sourceTable);

            var shouldContinue = 1;

            while(shouldContinue == 1)
            {
                Console.WriteLine("Enter a correct degree for the interpolation polynom or enter nothing to use the default value\n" +
                $"Remind that it should be less than source table size of {sourceTableSize}:");
                var polynomDegree = CUIHelpers.CUIHelpers.EnterParameter(i => int.Parse(i), x => x > 0 && x < sourceTableSize,
                    (ex, res) => Console.WriteLine("Enter a correct value for polynom degree"), DefaultPolynomDegree);
                Console.WriteLine("Enter the interpolation point:");
                var interpolationPoint = CUIHelpers.CUIHelpers.EnterParameter(i => double.Parse(i), x => double.IsFinite(x),
                    (ex, res) => Console.WriteLine("Enter a correct value for interpolaion point"), defaultInterpolationPoint);

                GetValueInPoint(sourceTable, interpolationPoint, polynomDegree);

                Console.WriteLine();
                Console.WriteLine("To continue with a new point enter 1, either enter 0");
                shouldContinue = CUIHelpers.CUIHelpers.EnterParameter(i => int.Parse(i), x => x == 1 || x == 0,
                    (ex, res) => Console.WriteLine("Enter 0 or 1"));
                Console.WriteLine();
            }
        }
    }
}
