using MyMathLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab3
{
    class Program
    {
        static readonly Func<double, double> DefaultFunction = x => Math.Log(1 + x) - Math.Exp(x);
        static (double a, double b) DefaultSector = (1, 10);
        const int DefaultSourceTableSize = 100;
        const int DefaultPolynomDegree = 7;
        const double DefaultFunctionValue = -5;
        const double BisectionMethodAccuracy = 1.0e-12;

        static void Main(string[] args)
        {
            CUIHelpers.CUIHelpers.Introduce("3.1", "Reverse interpolation problem",
                new[] { "f(x) = ln(1 + x) - exp(x)", $"[a, b] = [{DefaultSector.a}, {DefaultSector.b}]",
                    $"m = {DefaultSourceTableSize}", $"n = {DefaultPolynomDegree}", $"x = {DefaultFunctionValue}",
                     $"Accuracy for the bisection method = {BisectionMethodAccuracy}"});

            Console.WriteLine("Enter parameters or enter nothing to use default values\n");
            var sourceTableSize = CUIHelpers.CUIHelpers.EnterParameter(i => int.Parse(i), x => x > 0,
                    (ex, res) => Console.WriteLine("Enter a correct value for the source table size"), DefaultSourceTableSize, "Enter the source table size:");
            var leftBorder = CUIHelpers.CUIHelpers.EnterParameter(i => double.Parse(i), x => double.IsFinite(x),
                (ex, res) => Console.WriteLine("Enter a correct value for left border"), DefaultSector.a, "Enter the value for left border of the interpolation sector");
            var rightBorder = CUIHelpers.CUIHelpers.EnterParameter(i => double.Parse(i), x => double.IsFinite(x) && x > leftBorder,
                (ex, res) => Console.WriteLine("Enter a correct value for right border"), DefaultSector.b, "Enter the value for right border of the interpolation sector");
            var sourceTable = AlgebraicInterpolaion.GetSourceTable(sourceTableSize, (leftBorder, rightBorder), DefaultFunction);
            Console.WriteLine("Source table:");
            CUIHelpers.CUIHelpers.PrintSourceTable(sourceTable);

            var shouldContinue = 1;

            while (shouldContinue == 1)
            {
                var polynomDegree = CUIHelpers.CUIHelpers.EnterParameter(i => int.Parse(i), x => x > 0 && x < sourceTableSize,
                    (ex, res) => Console.WriteLine("Enter a correct value for polynom degree"), DefaultPolynomDegree,
                    "Enter a correct degree for the interpolation polynom or enter nothing to use the default value\n" +
                        $"Remind that it should be less than source table size of {sourceTableSize}:");
                var functionValue = CUIHelpers.CUIHelpers.EnterParameter(i => double.Parse(i), x => double.IsFinite(x),
                    (ex, res) => Console.WriteLine("Enter a correct value for the function"), DefaultFunctionValue,
                    "Enter the function value to find the interpolation point:");


                GetPointFromValue(sourceTable, functionValue, polynomDegree, DefaultFunction);

                Console.WriteLine();
                Console.WriteLine("To continue with a new point enter 1, either enter 0");
                shouldContinue = CUIHelpers.CUIHelpers.EnterParameter(i => int.Parse(i), x => x == 1 || x == 0,
                    (ex, res) => Console.WriteLine("Enter 0 or 1"));
                Console.WriteLine();
            }
        }

        static void GetPointFromValue(List<(double, double)> sourceTable, double functionValue, int polynomDegree, Func<double, double> function)
        {
            var reversedSourceTable = sourceTable.Select(x => (x.Item2, x.Item1)).ToList();
            reversedSourceTable.Sort((first, second) => Math.Sign(Math.Abs(functionValue - first.Item1) - Math.Abs(functionValue - second.Item1)));
            Console.WriteLine();
            Console.WriteLine("Reversed and sorted table:");
            CUIHelpers.CUIHelpers.PrintReversedSourceTable(reversedSourceTable);

            var leftBorder = CUIHelpers.CUIHelpers.EnterParameter(i => double.Parse(i), x => double.IsFinite(x),
               (ex, res) => Console.WriteLine("The value is not correct"), getMessage: "Enter the value for the left border for the bisection method");
            var rightBorder = CUIHelpers.CUIHelpers.EnterParameter(i => double.Parse(i), x => double.IsFinite(x) && x > leftBorder,
                (ex, res) => Console.WriteLine("The value is not correct"), getMessage: "Enter the value for the right border for the bisection method");

            var lagrangeResult = AlgebraicInterpolaion.GetInterpolationPolynome(reversedSourceTable, polynomDegree, functionValue, AlgebraicInterpolaion.Method.Lagrange);
            var getPolynome = new Func<double, double>(x => AlgebraicInterpolaion.GetInterpolationPolynome(sourceTable, polynomDegree, x, AlgebraicInterpolaion.Method.Lagrange));
            var bisectionResult = NonLinearEquations.FindRoot(x => getPolynome(x) - functionValue, (leftBorder, rightBorder), BisectionMethodAccuracy, NonLinearEquations.Method.BisectionMetod);

            PrintMethodResult("Lagrange method", lagrangeResult, functionValue, function);
            Console.WriteLine();
            PrintMethodResult("Bisection method", bisectionResult.Root, functionValue, function);
        }

        static void PrintMethodResult(string methodName, double methodResult, double functionValue, Func<double, double> function)
        {
            Console.WriteLine($"The {methodName} result = {methodResult}");
            Console.WriteLine($"The point where the function has the specified value = {functionValue}");
            Console.WriteLine($"The absolute value for residual = {Math.Abs(functionValue - function(methodResult))}");
        }
    }
}
