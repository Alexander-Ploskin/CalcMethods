using MyMathLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab5._2
{
    internal class Program
    {
        const double DefaultLeftBorder = 0;
        const double DefaultRightBorder = 1;
        const string StringF = "f(x) = sqrt(x) * exp(x^2)";
        const string StringChebyshevGaussF = "f(x) = ln(2 + x) / (1 + x^3)";
        static readonly int[] NodeCounts = { 4, 5, 6, 7 };
        static readonly Func<double, double> F = x => Math.Sqrt(x) * Math.Exp(Math.Pow(x, 2));
        static readonly Func<double, double> ChebyshevGaussF = x => Math.Log(2 + x) / (1 + Math.Pow(x, 3));

        static void Main(string[] args)
        {
            CUIHelpers.CUIHelpers.Introduce("5.2", "Integration problem solving with Gaussian quadrature and Chebyshev-Gauss quadrature", new string[] {
                StringF,
                $"Default value for the left border: {DefaultLeftBorder}",
                $"Default value for the right border: {DefaultRightBorder}",
            });

            var leftBorder = CUIHelpers.CUIHelpers.EnterParameter(i => double.Parse(i), x => double.IsFinite(x),
               (ex, res) => Console.WriteLine("The value is not correct"), DefaultLeftBorder, "Enter the value for the left border");
            var rightBorder = CUIHelpers.CUIHelpers.EnterParameter(i => double.Parse(i), x => double.IsFinite(x) && x >= leftBorder,
                (ex, res) => Console.WriteLine("The value is not correct"), DefaultRightBorder, "Enter the value for the right border");

            Console.WriteLine("Gaussian nodes and coefficients when N = 1..8");
            Console.WriteLine();

            var polynomeLeftBorder = GaussianQuadrature.DefaultLeftBorder;
            var polynomeRightBorder = GaussianQuadrature.DefaultRightBorder;

            for (int i = 1; i < 9; ++i)
            {
                var gaussianFormula = CreateGaussianFormula(i, polynomeLeftBorder, polynomeRightBorder);

                if (NodeCounts.Contains(i))
                {
                    var degree = 2 * i - 1;
                    var polynomeString = $"f(x) = x^{degree} + x^{degree - 1}";
                    var polynomeFunc = new Func<double, double>(x => Math.Pow(x, degree) + Math.Pow(x, degree - 1));
                    var antiderivative = new Func<double, double>(x => Math.Pow(x, degree + 1) / (degree + 1) + Math.Pow(x, degree) / degree);
                    var exactValue = antiderivative(polynomeRightBorder) - antiderivative(polynomeLeftBorder);
                    Console.WriteLine($"Results for {polynomeString}, a = {polynomeLeftBorder}, b = {polynomeRightBorder}");
                    CalcIntegralWithGaussianFormula(gaussianFormula, exactValue, polynomeFunc);
                    Console.WriteLine();
                }
            }

            Console.WriteLine($"Results for f(x) = {StringF}, p(x) = 1 from {leftBorder} to {rightBorder} given by Gauissan formula");
            Console.WriteLine();

            foreach (var nodeCount in NodeCounts)
            {
                var exactValue = Integration.CalcIntegralExactly(leftBorder, rightBorder, F);
                var formula = CreateGaussianFormula(nodeCount, leftBorder, rightBorder);
                CalcIntegralWithGaussianFormula(formula, exactValue, F);
                Console.WriteLine();
            }

            var mahlerNodeCounts = CUIHelpers.CUIHelpers.EnterParameters<int>(i => int.Parse(i), x => x > 0,
                (ex, res) => Console.WriteLine("The value is not correct"), NodeCounts.ToList(), "Enter node counts to test Chebyshev-Gauss formula:").ToList();

            Console.WriteLine($"Results for f(x) = {StringChebyshevGaussF}, p(x) = {ChebyshevGaussQuadrature.StringWeight}" +
                $"from {ChebyshevGaussQuadrature.LeftBorder} to {ChebyshevGaussQuadrature.RightBorder} given by Chebyshev-Gauss formula:");
            Console.WriteLine();

            foreach (var nodeCount in NodeCounts)
            {
                var f = new Func<double, double>(x => ChebyshevGaussF(x) * ChebyshevGaussQuadrature.Weight(x));
                var exactValue = Integration.CalcIntegralExactly(ChebyshevGaussQuadrature.LeftBorder, ChebyshevGaussQuadrature.RightBorder, f);
                var formula = CreateChebyshevGaussFormula(nodeCount, ChebyshevGaussF);
                CalcIntegralWithChebyshevGaussFormula(formula, exactValue);
                Console.WriteLine();
            }
        }

        static void CalcIntegralWithGaussianFormula(
            GaussianQuadrature gaussianFormula,
            double exactValue,
            Func<double, double> func)
        {
            var calculatedValue = gaussianFormula.CalcIntegral(func);
            var absoluteError = Math.Abs(exactValue - calculatedValue);
            Console.WriteLine($"Exact value: {exactValue}");
            Console.WriteLine($"Calculated value: {calculatedValue}");
            Console.WriteLine($"Absolute error: {absoluteError}");
            Console.WriteLine();
        }

        static void CalcIntegralWithChebyshevGaussFormula(
            ChebyshevGaussQuadrature mahlerFormula,
            double exactValue)
        {
            var calculatedValue = mahlerFormula.CalcIntegral();
            var absoluteError = Math.Abs(exactValue - calculatedValue);
            Console.WriteLine($"Exact value: {exactValue}");
            Console.WriteLine($"Calculated value: {calculatedValue}");
            Console.WriteLine($"Absolute error: {absoluteError}");
            Console.WriteLine();
        }

        static GaussianQuadrature CreateGaussianFormula(int nodesCount, double leftBorder, double rightBorder)
        {
            Console.WriteLine($"N = {nodesCount}");
            var gaussianFormula = new GaussianQuadrature(nodesCount, leftBorder, rightBorder);
            PrintNodesAndCoefeficients(gaussianFormula.Nodes, gaussianFormula.Coefficients);
            return gaussianFormula;
        }

        static ChebyshevGaussQuadrature CreateChebyshevGaussFormula(int nodesCount, Func<double, double> function)
        {
            Console.WriteLine($"N = {nodesCount}");
            var mahlerFormula = new ChebyshevGaussQuadrature(nodesCount, function);
            PrintNodesAndCoefeficients(mahlerFormula.Nodes, mahlerFormula.Coefficients);
            return mahlerFormula;
        }

        static void PrintNodesAndCoefeficients(List<double> nodes, List<double> coefficients)
        {
            var header = new string[] { "Node", "Coefficient" };
            var table = new object[nodes.Count, 2];
            for (int j = 0; j < nodes.Count; ++j)
            {
                table[j, 0] = nodes[j];
                table[j, 1] = coefficients[j];
            }

            CUIHelpers.CUIHelpers.PrintTable(header, table);
        }
    }
}
