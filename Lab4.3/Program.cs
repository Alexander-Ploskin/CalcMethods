using MyMathLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab4._3
{
    internal class Program
    {
        struct IntegralCalculationResult
        {
            public CalculationMethod method;
            public double exactValue;
            public double result;
            public double absoluteError;
            public double relativeError;
            public double theoreticalError;
        }

        const double DefaultLeftBorder = 1;
        const double DefaultRightBorder = 6;
        const int DefaultNumberOfPartitions = 5000;
        const int DefaultPartitionsCountFactor = 10;

        const string StringF = "f(x) = Exp(x)";
        static readonly Func<double, double> F = x => Math.Exp(x);
        static readonly Func<double, double> FAntiderivative = x => Math.Exp(x);
        const string StringF_0 = "f_0(x) = 4";
        static readonly Func<double, double> F_0 = x => 4;
        static readonly Func<double, double> F_0Antiderivative = x => 4 * x;
        const string StringF_1 = "f_1(x) = 4 * x";
        static readonly Func<double, double> F_1 = x => 4 * x;
        static readonly Func<double, double> F_1Antiderivative = x => 2 * Math.Pow(x, 2);
        const string StringF_2 = "f_2(x) = 4 * x^2 + 16 * x";
        static readonly Func<double, double> F_2 = x => 4 * Math.Pow(x, 2) + 16 * x;
        static readonly Func<double, double> F_2Antiderivative = x => (4d / 3d) * Math.Pow(x, 3) + 8 * Math.Pow(x, 2);
        const string StringF_3 = "f_3(x) = 4 * x^3 + 16 * x^2 + 64 * x";
        static readonly Func<double, double> F_3 = x => 4 * Math.Pow(x, 3) + 16 * Math.Pow(x, 2) + 64 * x;
        static readonly Func<double, double> F_3Antiderivative = x => Math.Pow(x, 4) + (16d / 3d) * Math.Pow(x, 3) + 32 * Math.Pow(x, 2);

        static void Main(string[] args)
        {
            CUIHelpers.CUIHelpers.Introduce("4.3", "Integration problem solving with composite quadrature formulas", new string[] {
                StringF,
                "p(x) = 1",
                StringF_0,
                StringF_1,
                StringF_2,
                StringF_3,
                $"Default value for the left border: {DefaultLeftBorder}",
                $"Default value for the right border: {DefaultRightBorder}",
                $"Default value for the number of partitions: {DefaultNumberOfPartitions}"
            });

            var leftBorder = CUIHelpers.CUIHelpers.EnterParameter(i => double.Parse(i), x => double.IsFinite(x),
            (ex, res) => Console.WriteLine("The value is not correct"), DefaultLeftBorder, "Enter the value for the left border");
            var rightBorder = CUIHelpers.CUIHelpers.EnterParameter(i => double.Parse(i), x => double.IsFinite(x) && x >= leftBorder,
                (ex, res) => Console.WriteLine("The value is not correct"), DefaultRightBorder, "Enter the value for the right border");
            var partitionsCount = CUIHelpers.CUIHelpers.EnterParameter(i => int.Parse(i), x => x > 0,
                (ex, res) => Console.WriteLine("The value is not correct"), DefaultNumberOfPartitions, "Enter the number of partitions");

            Console.WriteLine($"Step: {(rightBorder - leftBorder) / partitionsCount}");
            Console.WriteLine();

            var polynoms = new[] {
                (StringF_0, F_0, F_0Antiderivative),
                (StringF_1, F_1, F_1Antiderivative),
                (StringF_2, F_2, F_2Antiderivative),
                (StringF_3, F_3, F_3Antiderivative),
            };

            foreach (var func in polynoms)
            {
                var result = CalcIntegral(leftBorder, rightBorder, partitionsCount, func.Item2, func.Item3);
                PrintResults(result, func.Item1, false);
            }

            var funcResult = CalcIntegral(leftBorder, rightBorder, partitionsCount, F, FAntiderivative);
            PrintResults(funcResult, StringF, true);

            Console.WriteLine($"Default value for l: {DefaultPartitionsCountFactor}");
            var partitionsCountFactor = CUIHelpers.CUIHelpers.EnterParameter(i => int.Parse(i), x => x > 0,
                (ex, res) => Console.WriteLine("The value is not correct"), DefaultPartitionsCountFactor, "Enter l:");
            Console.WriteLine($"h: {(rightBorder - leftBorder) / partitionsCount}");
            partitionsCount *= partitionsCountFactor;
            Console.WriteLine($"h / l: {(rightBorder - leftBorder) / partitionsCount}");
            Console.WriteLine();

            var newResults = CalcIntegral(leftBorder, rightBorder, partitionsCount, F, FAntiderivative);
            PrintResults(newResults, StringF, false);

            var refinedResults = RefineResults(funcResult, newResults, partitionsCountFactor);
            Console.WriteLine("Refined results:");
            PrintResults(refinedResults, StringF, false);
        }

        static List<IntegralCalculationResult> CalcIntegral(double a, double b, int partitionsCount, Func<double, double> func, Func<double, double> antiderivative)
        {
            var exactValue = antiderivative(b) - antiderivative(a);

            var methods = Enum.GetValues(typeof(CalculationMethod)) as CalculationMethod[];
            var results = new List<IntegralCalculationResult>();
            for (int i = 0; i < methods.Length; ++i)
            {
                var resultValue = CompositeQuadratureFormulas.CalcIntegral(methods[i], a, b, partitionsCount, func);
                var absoluteError = Math.Abs(exactValue - resultValue);
                var relativeError = exactValue != 0 ? absoluteError / Math.Abs(exactValue) : -1;
                var h = (b - a) / partitionsCount;
                var theoreticalError = CompositeQuadratureFormulas.CalcTheoreticalErrorForGrowingFunction(methods[i], a, b, h, func);
                results.Add(new IntegralCalculationResult
                {
                    method = methods[i],
                    result = resultValue,
                    exactValue = exactValue,
                    absoluteError = absoluteError,
                    relativeError = relativeError,
                    theoreticalError = theoreticalError
                });
            }
            return results;
        }

        static List<IntegralCalculationResult> RefineResults(List<IntegralCalculationResult> oldResults, List<IntegralCalculationResult> newResults, int partitionsCountFactor)
        {
            var refinedResults = new List<IntegralCalculationResult>();
            for (int i = 0; i < oldResults.Count; ++i)
            {
                var oldResult = oldResults[i];
                var newResult = newResults[i];

                var refinedResultValue = CompositeQuadratureFormulas.Refine(oldResult.method.GetAccuracyDegree(), oldResult.result, newResult.result, partitionsCountFactor);
                var absoluteError = Math.Abs(oldResult.exactValue - refinedResultValue);
                var relativeError = oldResult.exactValue != 0 ? absoluteError / oldResult.exactValue : -1;
                refinedResults.Add(new IntegralCalculationResult
                {
                    method = oldResult.method,
                    exactValue = oldResult.exactValue,
                    result = refinedResultValue,
                    absoluteError = absoluteError,
                    relativeError = relativeError
                });
            }
            return refinedResults;
        }

        static void PrintResults(List<IntegralCalculationResult> results, string stringFunc, bool printTheoreticalError)
        {
            var resultTableHeader = new List<string> { "Method", "Result", "Absolute error", "Relative error" };
            if (printTheoreticalError)
            {
                resultTableHeader.Add("Theoretical error");
            }
            var resultTable = new object[5, resultTableHeader.Count];
            
            for(int i = 0; i < results.Count; ++i)
            {
                var result = results[i];
                resultTable[i, 0] = result.method.ToString();
                resultTable[i, 1] = result.result;
                resultTable[i, 2] = result.absoluteError;
                resultTable[i, 3] = result.relativeError;
                if(printTheoreticalError)
                {
                    resultTable[i, 4] = result.theoreticalError;
                }
            }

            Console.WriteLine($"Results for {stringFunc}");
            Console.WriteLine($"Exact value: {results[0].exactValue}");
            CUIHelpers.CUIHelpers.PrintTable(resultTableHeader.ToArray(), resultTable);
            Console.WriteLine();
        }
    }
}
