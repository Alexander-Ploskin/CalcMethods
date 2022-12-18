using MyMathLib;
using System;
using System.Collections.Generic;

namespace Lab5._3
{
    internal class Program
    {
        const int DefaultNumberOfPartitions = 1;
        const int DefaultNummberOfNodes = 3;
        const double DefaultLeftBorder = 0;
        const double DefaultRightBorder = 1;
        static readonly Func<double, double> F = x => Math.Sin(x);
        static readonly Func<double, double> Weight = x => Math.Exp(-x);
        const string StringF = "f(x) = sin(x)";
        const string StringWeight = "p(x) = exp(-x)";

        static void Main(string[] args)
        {
            CUIHelpers.CUIHelpers.Introduce("5.3", "Integration problem solving with composite Gaussian quadrature", new string[] {
                StringF,
                StringWeight,
                $"Default value for the left border: {DefaultLeftBorder}",
                $"Default value for the right border: {DefaultRightBorder}",
                $"Default number of nodes: {DefaultNummberOfNodes}",
                $"Default number of partitions: {DefaultNumberOfPartitions}"
            });

            Iterate();
            while(true)
            {
                Console.WriteLine("Print 1 to continue with new parameters, print any onther to exit");
                var choice = Console.ReadLine();
                if (!int.TryParse(choice, out var intChoice) || intChoice != 1)
                {
                    return;
                }
                Iterate();
            }
        }

        static void Iterate()
        {
            var leftBorder = CUIHelpers.CUIHelpers.EnterParameter(i => double.Parse(i), x => double.IsFinite(x),
               (ex, res) => Console.WriteLine("The value is not correct"), DefaultLeftBorder, "Enter the value for the left border");
            var rightBorder = CUIHelpers.CUIHelpers.EnterParameter(i => double.Parse(i), x => double.IsFinite(x) && x >= leftBorder,
                (ex, res) => Console.WriteLine("The value is not correct"), DefaultRightBorder, "Enter the value for the right border");
            var numberOfNodes = CUIHelpers.CUIHelpers.EnterParameter(i => int.Parse(i), x => x > 0,
               (ex, res) => Console.WriteLine("The value is not correct"), DefaultNummberOfNodes, "Enter the number of nodes");
            var numberOfPartitions = CUIHelpers.CUIHelpers.EnterParameter(i => int.Parse(i), x => x > 0,
                (ex, res) => Console.WriteLine("The value is not correct"), DefaultNumberOfPartitions, "Enter the number of partitions");

            var exactIntegral = Integration.CalcIntegralExactly(leftBorder, rightBorder, x => F(x) * Weight(x));
            var compoundGaussFormula = new CompoundGaussFormula(leftBorder, rightBorder, numberOfNodes, numberOfPartitions, F, Weight);
            var calculatedIntegral = compoundGaussFormula.CalculateIntegral();
            var absoluteError = Math.Abs(calculatedIntegral - exactIntegral);
            Console.WriteLine($"N = {numberOfNodes}, m = {numberOfPartitions}, f(x) = {StringF}, p(x) = {StringWeight}");
            PrintNodesAndCoefeficients(compoundGaussFormula.Nodes, compoundGaussFormula.Coefficients);
            Console.WriteLine($"Exact integral: {exactIntegral}");
            Console.WriteLine($"Calculated integral: {calculatedIntegral}");
            Console.WriteLine($"Absolute error: {absoluteError}");
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
