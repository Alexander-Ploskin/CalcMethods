using System;
using System.Collections.Generic;

namespace Lab4._3
{
    internal class Program
    {
        const double DefaultLeftBorder = 1.5;
        const double DefaultRightBorder = 2.5;
        const int DefaultNumberOfPartitions = 10;

        const string StringF = "f(x) = Ln(x) * Exp(x)";
        static readonly Func<double, double> F = x => Math.Log(x) * Math.Exp(x);
        const string StringF_0 = "f_0(x) = 4";
        static readonly Func<double, double> F_0 = x => 4;
        const string StringF_1 = "f_1(x) = 4 * x";
        static readonly Func<double, double> F_1 = x => 4 * x;
        const string StringF_2 = "f_2(x) = 4 * x^2 + 16 * x";
        static readonly Func<double, double> F_2 = x => 4 * Math.Pow(x, 2) + 16 * x;
        const string StringF_3 = "f_3(x) = 4 * x^3 + 16 * x^2 + 64 * x";
        static readonly Func<double, double> F_3 = x => 4 * Math.Pow(x, 3) + 16 * Math.Pow(x, 2) + 64 * Math.Pow(x, 3);

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
            });

            var leftBorder = CUIHelpers.CUIHelpers.EnterParameter(i => double.Parse(i), x => double.IsFinite(x),
            (ex, res) => Console.WriteLine("The value is not correct"), DefaultLeftBorder, "Enter the value for the left border");
            var rightBorder = CUIHelpers.CUIHelpers.EnterParameter(i => double.Parse(i), x => double.IsFinite(x) && x >= leftBorder,
                (ex, res) => Console.WriteLine("The value is not correct"), DefaultRightBorder, "Enter the value for the right border");
            var partitionsCount = CUIHelpers.CUIHelpers.EnterParameter(i => int.Parse(i), x => x > 0,
                (ex, res) => Console.WriteLine("The value is not correct"), DefaultNumberOfPartitions, "Enter the number of partitions");
            Console.WriteLine($"Step: {(rightBorder - leftBorder) / partitionsCount}");
            Console.WriteLine();

            CalcIntegral(leftBorder, rightBorder, partitionsCount, StringF, true, F);
            CalcIntegral(leftBorder, rightBorder, partitionsCount, StringF_0, false, F_0);
            CalcIntegral(leftBorder, rightBorder, partitionsCount, StringF_1, false, F_1);
            CalcIntegral(leftBorder, rightBorder, partitionsCount, StringF_2, false, F_2);
            CalcIntegral(leftBorder, rightBorder, partitionsCount, StringF_3, false,F_3);
        }

        static void CalcIntegral(double a, double b, int partitionsCount, string stringFunc, bool printTheoreticalError, Func<double, double> func)
        {
            Console.WriteLine($"Results for {stringFunc}");

            var exactValue = MyMathLib.Integration.CalcIntegralExactly(a, b, func);

            var resultTableHeader = new List<string> { "Method", "Result", "Absolute error", "Relative error" };
            if (printTheoreticalError)
            {
                resultTableHeader.Add("Theoretical error");
            }
            var resultTable = new object[5, resultTableHeader.Count];

            resultTable[0, 0] = "Left rectangle";
            resultTable[1, 0] = "Right rectangle";
            resultTable[2, 0] = "Center rectangle";
            resultTable[3, 0] = "Trapezoid";
            resultTable[4, 0] = "Simpson";

            var leftRect = MyMathLib.CompositeQuadratureFormulas.LeftRectangle(a, b, partitionsCount, func);
            var rightRect = MyMathLib.CompositeQuadratureFormulas.RightRectangle(a, b, partitionsCount, func);
            var centerRect = MyMathLib.CompositeQuadratureFormulas.CenterRectangle(a, b, partitionsCount, func);
            var trapezoid = MyMathLib.CompositeQuadratureFormulas.Trapezoid(a, b, partitionsCount, func);
            var simpson = MyMathLib.CompositeQuadratureFormulas.Simpson(a, b, partitionsCount, func);
            
            resultTable[0, 1] = leftRect;
            resultTable[1, 1] = rightRect;
            resultTable[2, 1] = centerRect;
            resultTable[3, 1] = trapezoid;
            resultTable[4, 1] = simpson;

            var leftRectAbsError = Math.Abs(exactValue - leftRect);
            var rightRectAbsError = Math.Abs(exactValue - rightRect);
            var centerRectAbsError = Math.Abs(exactValue - centerRect);
            var trapezoidAbsError = Math.Abs(exactValue - trapezoid);
            var simpsonAbsError = Math.Abs(exactValue - simpson);

            resultTable[0, 2] = leftRectAbsError;
            resultTable[1, 2] = rightRectAbsError;
            resultTable[2, 2] = centerRectAbsError;
            resultTable[3, 2] = trapezoidAbsError;
            resultTable[4, 2] = simpsonAbsError;

            var leftRectRelativeError = exactValue != 0 ? leftRectAbsError / Math.Abs(exactValue) : -1;
            var rightRectRelativeError = exactValue != 0 ? rightRectAbsError / Math.Abs(exactValue) : -1;
            var centerRectRelativeError = exactValue != 0 ? centerRectAbsError / Math.Abs(exactValue) : -1;
            var trapezoidRelativeError = exactValue != 0 ? trapezoidAbsError / Math.Abs(exactValue) : -1;
            var simpsonRelativeError = exactValue != 0 ? simpsonAbsError / Math.Abs(exactValue) : -1;

            resultTable[0, 3] = leftRectRelativeError;
            resultTable[1, 3] = rightRectRelativeError;
            resultTable[2, 3] = centerRectRelativeError;
            resultTable[3, 3] = trapezoidRelativeError;
            resultTable[4, 3] = simpsonRelativeError;

            var leftRectTheoreticalError = (1d / 2d) * Math.Abs(func(b)) * (b - a) * ((b - a) / partitionsCount);
            var rightRectTheoreticalError = (1d / 2d) * Math.Abs(func(b)) * (b - a) * ((b - a) / partitionsCount);
            var centerRectTheoreticalError = (1d / 24d) * Math.Abs(Math.Pow(func(b), 2)) * (b - a) * Math.Pow((b - a) / partitionsCount, 2);
            var trapezoidTheoreticalError = (1d / 12d) * Math.Abs(Math.Pow(func(b), 2)) * (b - a) * Math.Pow((b - a) / partitionsCount, 2);
            var simpsonTheoreticalError = (1d / 2880d) * Math.Abs(Math.Pow(func(b), 4)) * (b - a) * Math.Pow((b - a) / partitionsCount, 4);

            if(printTheoreticalError)
            {
                resultTable[0, 4] = leftRectTheoreticalError;
                resultTable[1, 4] = rightRectTheoreticalError;
                resultTable[2, 4] = centerRectTheoreticalError;
                resultTable[3, 4] = trapezoidTheoreticalError;
                resultTable[4, 4] = simpsonTheoreticalError;
            }


            Console.WriteLine($"Exact value: {exactValue}");

            CUIHelpers.CUIHelpers.PrintTable(resultTableHeader.ToArray(), resultTable);

            Console.WriteLine();
        }
    }
}
