using System;
using System.Linq;

namespace Lab5._1
{
    internal class Program
    {
        const int DefaultNumberOfNodes = 2;
        const double DefaultLeftBorder = 0;
        const double DefaultRightBorder = 1;

        const string StringF = "f(x) = sin(x)";
        static readonly Func<double, double> F = x => Math.Sin(x);
        const string StringP = "p(x) = exp(-x)";
        const string StringFP = "F(x) = sin(x) * exp(-x)";
        static readonly Func<double, double> P = x => Math.Exp(-x);
        static readonly Func<double, double> FP_Antiderivative = x => -0.5 * Math.Exp(-x) * (Math.Sin(x) + Math.Cos(x));

        static void Main(string[] args)
        {
            CUIHelpers.CUIHelpers.Introduce("5.1", "Integration problem solving with quadrature formulas of 2N-1 accuracy degree", new string[] {
                StringF,
                StringP,
                $"Default value for the left border: {DefaultLeftBorder}",
                $"Default value for the right border: {DefaultRightBorder}",
                $"Default value for the number of nodes: {DefaultNumberOfNodes}"
            });

            var leftBorder = CUIHelpers.CUIHelpers.EnterParameter(i => double.Parse(i), x => double.IsFinite(x),
                (ex, res) => Console.WriteLine("The value is not correct"), DefaultLeftBorder, "Enter the value for the left border");
            var rightBorder = CUIHelpers.CUIHelpers.EnterParameter(i => double.Parse(i), x => double.IsFinite(x) && x >= leftBorder,
                (ex, res) => Console.WriteLine("The value is not correct"), DefaultRightBorder, "Enter the value for the right border");
            var numberOfNodes = CUIHelpers.CUIHelpers.EnterParameter(i => int.Parse(i), x => x >= 0,
                (ex, res) => Console.WriteLine("The value is not correct"), DefaultNumberOfNodes, "Enter the value for the number of nodes");

            Enumerable.Range(0, numberOfNodes * 2).ToList()
                .ForEach(d => CalcIntegral(
                                    leftBorder, 
                                    rightBorder, 
                                    numberOfNodes,
                                    $"p_{d}(x) = x^{d}",
                                    x => 1,
                                    x => Math.Pow(x, d),
                                    x => Math.Pow(x, d + 1) / (d + 1)));

            CalcIntegral(leftBorder, rightBorder, numberOfNodes, StringFP, F, P, FP_Antiderivative);

        }

        static void CalcIntegral(double leftBorder, double rightBorder, int numberOfNodes, string stringFunction, Func<double, double> function, Func<double, double> weight, Func<double, double> antiderivative)
        {
            Console.WriteLine($"Results for {stringFunction}");
            var exactValue = antiderivative(rightBorder) - antiderivative(leftBorder);

            var moments = MyMathLib.Integration.CalculateMoments(leftBorder, rightBorder, numberOfNodes * 2, weight);
            CUIHelpers.CUIHelpers.PrintNumeredParameters(moments, 'M', "Moments:");

            var nodes = MyMathLib.Integration.FindNodes(moments);
            CUIHelpers.CUIHelpers.PrintNumeredParameters(nodes, 'x', "Nodes:");

            var coefficients = MyMathLib.Integration.CalculateCoefficients(nodes, moments).ToList();
            CUIHelpers.CUIHelpers.PrintNumeredParameters(coefficients, 'A', "Coefficients:");

            var calculatedValue = MyMathLib.Integration.CalcIntegral(nodes, coefficients, function);
            Console.WriteLine($"Exact value: {exactValue}");
            Console.WriteLine($"Calculated value: {calculatedValue}");
            Console.WriteLine($"Error: {Math.Abs(exactValue - calculatedValue)}");

            Console.WriteLine();
        }
    }
}
