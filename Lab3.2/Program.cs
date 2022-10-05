using MyMathLib;
using System;
using System.Linq;

namespace Lab3._2
{
    class Program
    {
        static readonly Func<double, double> DefaultFunction = x => Math.Exp(6 * x);
        static readonly Func<double, double> DefaultFunctionFirstDerivative = x => 6 * Math.Exp(6 * x);
        static readonly Func<double, double> DefaultFunctionSecondDerivative = x => 36 * Math.Exp(6 * x);

        const double DefaultStartPoint = 0;
        const double DefaultStep = 0.05;
        const int DefaultNumberOfValues = 10;

        static void Main(string[] args)
        {
            CUIHelpers.CUIHelpers.Introduce("3.2", "Numerical differentiation problem", new string[] {
                "Given function: e^(6 * x)",
                "Real first derivative for the given function: 6 * e^(6 * x)",
                "Real second derivative for the given function: 36 * e^(6 * x)",
                $"Default value for the start point: {DefaultStartPoint}",
                $"Default value for the step between values: {DefaultStep}",
                $"Default number of values: {DefaultNumberOfValues}"
            });

            var startPoint = CUIHelpers.CUIHelpers.EnterParameter(i => double.Parse(i), x => double.IsFinite(x),
                (ex, res) => Console.WriteLine("The value is not correct"), DefaultStartPoint, "Enter the value for the start point");
            var step = CUIHelpers.CUIHelpers.EnterParameter(i => double.Parse(i), x => double.IsFinite(x),
                (ex, res) => Console.WriteLine("The value is not correct"), DefaultStep, "Enter the step for the table");
            var numberOfValues = CUIHelpers.CUIHelpers.EnterParameter(i => int.Parse(i), x => x > 3,
                (ex, res) => Console.WriteLine("The value is not correct"), DefaultNumberOfValues, "Enter the value for the start point");

            var sourceTable = NumericalDifferentiation.GetSourceTable(startPoint, step, numberOfValues, DefaultFunction);
            Console.WriteLine("Source table:");
            CUIHelpers.CUIHelpers.PrintSourceTable(sourceTable);

            var firstDerivatives = NumericalDifferentiation.FindDerivatives(sourceTable);
            var secondDerivatives = NumericalDifferentiation.FindSecondDerivatives(sourceTable);

            var resultTableHeader = new string[] { "X_i", "F(X_i)", "Computed F'(X_i)", "Absolute error for F'(X_i)", "Computed F''(X_i)", "Absolute error for F''(X_i)" };
            var resultTable = new object[numberOfValues, resultTableHeader.Length];

            for(int i = 0; i < numberOfValues; ++i)
            {
                resultTable[i, 0] = sourceTable[i].Item1;
                resultTable[i, 1] = sourceTable[i].Item2;
                resultTable[i, 2] = firstDerivatives[i].Item2;
                resultTable[i, 3] = Math.Abs(DefaultFunctionFirstDerivative(sourceTable[i].Item1) - firstDerivatives[i].Item2);
                resultTable[i, 4] = secondDerivatives[i].Item2;
                resultTable[i, 5] = secondDerivatives[i].Item2.HasValue
                    ? (double?)Math.Abs(DefaultFunctionSecondDerivative(sourceTable[i].Item1) - secondDerivatives[i].Item2.Value)
                    : null;
            }

            CUIHelpers.CUIHelpers.PrintTable(resultTableHeader, resultTable);
        }
    }
}
